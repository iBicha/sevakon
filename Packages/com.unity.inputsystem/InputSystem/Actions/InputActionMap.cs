using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Experimental.Input.Utilities;

////TODO: add public InputActionManager that supports various device allocation strategies (one stack
////      per device, multiple devices per stack, etc.); should also resolve the problem of having
////      two bindings stack on top of each other and making the one on top suppress the one below

namespace UnityEngine.Experimental.Input
{
    /// <summary>
    /// A mapping of <see cref="InputBinding">input bindings</see> to <see cref="InputAction">
    /// input actions</see>.
    /// </summary>
    /// <remarks>
    /// Also stores data for actions. All actions have to have an associated
    /// action map. "Lose" actions constructed without a map will internally
    /// create their own map to hold their data.
    ///
    /// A common usage pattern for action maps is to use them to group action
    /// "contexts". So one map could hold "menu" actions, for example, whereas
    /// another set holds "gameplay" actions. This kind of splitting can be
    /// made arbitrarily complex. Like, you could have separate "driving" and
    /// "walking" action maps, for example, that you enable and disable depending
    /// on whether the player is walking or driving around.
    /// </remarks>
    [Serializable]
    public class InputActionMap : ICloneable, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Name of the action map.
        /// </summary>
        public string name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// If the action map is part of an asset, this refers to the asset. Otherwise it is <c>null</c>.
        /// </summary>
        public InputActionAsset asset
        {
            get { return m_Asset; }
        }

        /// <summary>
        /// A stable, unique identifier for the map.
        /// </summary>
        /// <remarks>
        /// This can be used instead of the name to refer to the action map. Doing so allows referring to the
        /// map such that renaming it does not break references.
        /// </remarks>
        public Guid id
        {
            get
            {
                if (m_Guid == Guid.Empty)
                {
                    if (m_Id == null)
                    {
                        m_Guid = Guid.NewGuid();
                        m_Id = m_Guid.ToString();
                    }
                    else
                    {
                        m_Guid = new Guid(m_Id);
                    }
                }
                return m_Guid;
            }
        }

        internal Guid idDontGenerate
        {
            get
            {
                if (m_Guid == Guid.Empty && !string.IsNullOrEmpty(m_Id))
                    m_Guid = new Guid(m_Id);
                return m_Guid;
            }
        }

        /// <summary>
        /// Whether any action in the map is currently enabled.
        /// </summary>
        public bool enabled
        {
            get { return m_EnabledActionsCount > 0; }
        }

        /// <summary>
        /// List of actions contained in the map.
        /// </summary>
        /// <remarks>
        /// Actions are owned by their map. The same action cannot appear in multiple maps.
        ///
        /// Does not allocate. Note that values returned by the property become invalid if
        /// the setup of actions in a set is changed.
        /// </remarks>
        public ReadOnlyArray<InputAction> actions
        {
            get { return new ReadOnlyArray<InputAction>(m_Actions); }
        }

        //what about explicitly grouping bindings into named sets?

        /// <summary>
        /// List of bindings contained in the map.
        /// </summary>
        /// <remarks>
        /// <see cref="InputBinding">InputBindings</see> are owned by action maps and not by individual
        /// actions. The bindings in a map can form a tree and conceptually, this array represents a depth-first
        /// traversal of the tree.
        ///
        /// Bindings that trigger actions refer to the action by name.
        /// </remarks>
        public ReadOnlyArray<InputBinding> bindings
        {
            get { return new ReadOnlyArray<InputBinding>(m_Bindings); }
        }

        public ReadOnlyArray<InputControl> controls
        {
            get { throw new NotImplementedException(); }
        }

        public ReadOnlyArray<InputDevice> devices
        {
            get { throw new NotImplementedException(); }
        }

        public InputBinding? bindingMask
        {
            get
            {
                if (m_BindingMask.isEmpty)
                    return null;
                return m_BindingMask;
            }
        }

        ////REVIEW: should this operate by binding path or by action name?
        public InputAction this[string actionNameOrId]
        {
            get
            {
                if (string.IsNullOrEmpty(actionNameOrId))
                    throw new ArgumentNullException("actionNameOrId");
                return GetAction(actionNameOrId);
            }
        }

        /// <summary>
        /// Add or remove a callback that is triggered when an action in the map changes its <see cref="InputActionPhase">
        /// phase</see>.
        /// </summary>
        /// <seealso cref="InputAction.started"/>
        /// <seealso cref="InputAction.performed"/>
        /// <seealso cref="InputAction.cancelled"/>
        public event Action<InputAction.CallbackContext> actionTriggered
        {
            add { m_ActionCallbacks.AppendWithCapacity(value); }
            remove { m_ActionCallbacks.RemoveByMovingTailWithCapacity(value); } // Changes callback ordering.
        }

        public InputActionMap(string name = null, InputActionMap extend = null)
        {
            m_Name = name;

            if (extend != null)
                throw new NotImplementedException();
        }

        internal int TryGetActionIndex(string nameOrId)
        {
            ////REVIEW: have transient lookup table? worth optimizing this?
            ////   Ideally, this should at least be an InternedString comparison but due to serialization,
            ////   that's quite tricky.

            if (string.IsNullOrEmpty(nameOrId))
                throw new ArgumentNullException("nameOrId");

            if (m_Actions == null)
                return InputActionMapState.kInvalidIndex;
            var actionCount = m_Actions.Length;

            var isReferenceById = nameOrId[0] == '{';
            if (isReferenceById)
            {
                var id = new Guid(nameOrId);
                for (var i = 0; i < actionCount; ++i)
                    if (m_Actions[i].idDontGenerate == id)
                        return i;
            }
            else
            {
                for (var i = 0; i < actionCount; ++i)
                    if (string.Compare(m_Actions[i].m_Name, nameOrId, StringComparison.InvariantCultureIgnoreCase) == 0)
                        return i;
            }

            return InputActionMapState.kInvalidIndex;
        }

        internal int TryGetActionIndex(Guid id)
        {
            if (m_Actions == null)
                return InputActionMapState.kInvalidIndex;
            var actionCount = m_Actions.Length;
            for (var i = 0; i < actionCount; ++i)
                if (m_Actions[i].idDontGenerate == id)
                    return i;

            return InputActionMapState.kInvalidIndex;
        }

        public InputAction TryGetAction(string name)
        {
            var index = TryGetActionIndex(name);
            if (index == -1)
                return null;
            return m_Actions[index];
        }

        public InputAction TryGetAction(Guid id)
        {
            var index = TryGetActionIndex(id);
            if (index == -1)
                return null;
            return m_Actions[index];
        }

        public InputAction GetAction(string name)
        {
            var action = TryGetAction(name);
            if (action == null)
                throw new KeyNotFoundException(string.Format("Could not find action '{0}' in map '{1}'", name,
                    this.name));
            return action;
        }

        public InputAction GetAction(Guid id)
        {
            var action = TryGetAction(id);
            if (action == null)
                throw new KeyNotFoundException(string.Format("Could not find action with ID '{0}' in map '{1}'", id,
                    name));
            return action;
        }

        public void SetBindingMask(InputBinding bindingMask)
        {
            if (bindingMask == m_BindingMask)
                return;

            m_BindingMask = bindingMask;

            if (m_State != null)
                ResolveBindings();
        }

        public void SetBindingMask(string bindingGroups)
        {
            if (string.IsNullOrEmpty(bindingGroups))
                bindingGroups = null;
            SetBindingMask(new InputBinding {groups = bindingGroups});
        }

        public void ClearBindingMask()
        {
            SetBindingMask(new InputBinding());
        }

        /// <summary>
        /// Enable all the actions in the map.
        /// </summary>
        public void Enable()
        {
            if (m_Actions == null || m_EnabledActionsCount == m_Actions.Length)
                return;

            ResolveBindingsIfNecessary();
            m_State.EnableAllActions(this);
            m_EnabledActionsCount = m_Actions.Length;
        }

        /// <summary>
        /// Disable all the actions in the map.
        /// </summary>
        public void Disable()
        {
            if (!enabled)
                return;

            m_State.DisableAllActions(this);
            m_EnabledActionsCount = 0;
        }

        ////REVIEW: right now the Clone() methods aren't overridable; do we want that?
        public InputActionMap Clone()
        {
            // Internal action sets from singleton actions should not be visible outside of
            // them. Cloning them is not allowed.
            if (m_SingletonAction != null)
                throw new InvalidOperationException(
                    string.Format("Cloning internal map of singleton action '{0}' is not allowed", m_SingletonAction));

            var clone = new InputActionMap
            {
                m_Name = m_Name,
            };

            // Clone actions.
            if (m_Actions != null)
            {
                var actionCount = m_Actions.Length;
                var actions = new InputAction[actionCount];
                for (var i = 0; i < actionCount; ++i)
                {
                    var original = m_Actions[i];
                    actions[i] = new InputAction
                    {
                        m_Name = original.m_Name,
                        m_ActionMap = clone,
                    };
                }
                clone.m_Actions = actions;
            }

            // Clone bindings.
            if (m_Bindings != null)
            {
                var bindingCount = m_Bindings.Length;
                var bindings = new InputBinding[bindingCount];
                Array.Copy(m_Bindings, 0, bindings, 0, bindingCount);
                clone.m_Bindings = bindings;
            }

            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        // The state we persist is pretty much just a name, a flat list of actions, and a flat
        // list of bindings. The rest is state we keep at runtime when a map is in use.

        [SerializeField] internal string m_Name;
        [SerializeField] internal string m_Id; // Can't serialize System.Guid and Unity's GUID is editor only.
        [SerializeField] internal InputActionAsset m_Asset;

        /// <summary>
        /// List of actions in this map.
        /// </summary>
        [SerializeField] internal InputAction[] m_Actions;

        /// <summary>
        /// List of bindings in this map.
        /// </summary>
        /// <remarks>
        /// For singleton actions, we ensure this is always the same as <see cref="InputAction.m_SingletonActionBindings"/>.
        /// </remarks>
        [SerializeField] internal InputBinding[] m_Bindings;

        // These fields are caches. If m_Bindings is modified, these are thrown away
        // and re-computed only if needed.
        // NOTE: Because InputBindings are structs, m_BindingsForEachAction actually duplicates each binding
        //       (only in the case where m_Bindings has scattered references to actions).
        ////REVIEW: this will lead to problems when overrides are thrown into the mix
        [NonSerialized] internal InputBinding[] m_BindingsForEachAction;
        [NonSerialized] internal InputControl[] m_ControlsForEachAction;
        [NonSerialized] internal InputDevice[] m_DevicesForEachAction;
        ////REVIEW: this seems to make sense to have on the state; probably best to move it over there
        ////REVIEW: also, should this be integer indices instead of another array that needs scanning?
        [NonSerialized] internal InputAction[] m_ActionForEachBinding;

        [NonSerialized] internal int m_EnabledActionsCount;
        [NonSerialized] internal Guid m_Guid;

        // Action sets that are created internally by singleton actions to hold their data
        // are never exposed and never serialized so there is no point allocating an m_Actions
        // array.
        [NonSerialized] internal InputAction m_SingletonAction;

        [NonSerialized] internal int m_MapIndexInState = InputActionMapState.kInvalidIndex;

        /// <summary>
        /// Current execution state.
        /// </summary>
        /// <remarks>
        /// Initialized when map (or any action in it) is first enabled.
        /// </remarks>
        [NonSerialized] internal InputActionMapState m_State;
        [NonSerialized] internal InputBinding m_BindingMask;

        [NonSerialized] internal InlinedArray<Action<InputAction.CallbackContext>> m_ActionCallbacks;

        /// <summary>
        /// Return the list of bindings for just the given actions.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <remarks>
        /// The bindings for a single action may be contiguous in <see cref="m_Bindings"/> or may be scattered
        /// around. We don't keep persistent storage for these and instead set up a transient
        /// array if and when bindings are queried directly from an action. In the simple case,
        /// we don't even need a separate array but rather just need to find out which slice in the
        /// bindings array corresponds to which action.
        ///
        /// NOTE: Bindings for individual actions aren't queried by the system itself during normal
        ///       runtime operation so we only do this for cases where the user asks for the
        ///       information. If the user never asks for bindings or controls on a per-action basis,
        ///       none of this data gets initialized.
        /// </remarks>
        internal ReadOnlyArray<InputBinding> GetBindingsForSingleAction(InputAction action)
        {
            Debug.Assert(action != null);
            Debug.Assert(action.m_ActionMap == this);
            Debug.Assert(!action.isSingletonAction || m_SingletonAction == action);

            // See if we need to refresh.
            if (m_BindingsForEachAction == null)
                SetUpBindingArrayForEachAction();

            return new ReadOnlyArray<InputBinding>(m_BindingsForEachAction, action.m_BindingsStartIndex,
                action.m_BindingsCount);
        }

        /// <summary>
        /// For the given action, return the current list of devices used by its bound controls.
        /// </summary>
        /// <param name="action">Input action belonging to this action map.</param>
        /// <returns>(Possibly empty) array of devices used by action.</returns>
        /// <remarks>
        /// This function may allocate whenever the control setup of the action map changes.
        /// </remarks>
        internal ReadOnlyArray<InputDevice> GetDevicesForSingleAction(InputAction action)
        {
            Debug.Assert(m_State != null);
            Debug.Assert(m_MapIndexInState != InputActionMapState.kInvalidIndex);
            Debug.Assert(m_Actions != null);
            Debug.Assert(action != null);
            Debug.Assert(action.m_ActionMap == this);
            Debug.Assert(!action.isSingletonAction || m_SingletonAction == action);

            if (m_DevicesForEachAction == null)
            {
                if (m_State.totalDeviceCount == 0)
                    return new ReadOnlyArray<InputDevice>();

                var perActionDeviceListCorrespondsToDeviceListOfEntireState = true;
                if (m_SingletonAction != null)
                {
                    // For singleton action, device list always corresponds to the list of
                    // devices in our state.
                    action.m_DeviceStartIndex = 0;
                    action.m_DeviceCount = m_State.totalDeviceCount;
                }
                else
                {
                    // Try to find an action that doesn't use all of the devices in our state.
                    // If there's none, we can still just use the same list of devices that we
                    // have in our state.
                    var haveActionNotUsingAllDevices = false;
                    var deviceCount = m_State.totalDeviceCount;
                    for (var i = 0; i < m_Actions.Length; ++i)
                    {
                        var currentAction = m_Actions[i];

                        for (var n = 0; n < deviceCount; ++n)
                        {
                            // Get device.
                            InputDevice currentDevice;
                            if (m_DevicesForEachAction != null)
                                currentDevice = m_DevicesForEachAction[n];
                            else
                                currentDevice = m_State.devices[n];

                            // Try to find control that uses the device.
                            var isUsedByControl = false;
                            var controlsForAction = currentAction.controls;
                            for (var k = 0; k < controlsForAction.Count; ++k)
                            {
                                if (controlsForAction[k].device == currentDevice)
                                {
                                    isUsedByControl = true;
                                    break;
                                }
                            }

                            if (!isUsedByControl)
                            {
                                haveActionNotUsingAllDevices = true;
                                break;
                            }
                        }
                    }

                    // If we have at least one action that isn't using all the devices in our state,
                    // set up a separate array. We may still end up being able to share one list of
                    // devices between all the actions in our map.
                    if (haveActionNotUsingAllDevices)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        for (var i = 0; i < m_Actions.Length; ++i)
                        {
                            var currentAction = m_Actions[i];
                            currentAction.m_DeviceStartIndex = 0;
                            currentAction.m_DeviceCount = m_State.totalDeviceCount;
                        }
                    }
                }

                // If the list of devices used by this one and every action in it corresponds
                // to the list of devices used by our shared state, then just copy that list.
                if (perActionDeviceListCorrespondsToDeviceListOfEntireState)
                {
                    m_DevicesForEachAction = new InputDevice[m_State.totalDeviceCount];
                    for (var i = 0; i < m_State.totalDeviceCount; ++i)
                        m_DevicesForEachAction[i] = m_State.devices[i];
                }
            }

            return new ReadOnlyArray<InputDevice>(m_DevicesForEachAction, action.m_DeviceStartIndex,
                action.m_DeviceCount);
        }

        internal ReadOnlyArray<InputControl> GetControlsForSingleAction(InputAction action)
        {
            Debug.Assert(m_State != null);
            Debug.Assert(m_MapIndexInState != InputActionMapState.kInvalidIndex);
            Debug.Assert(m_Actions != null);
            Debug.Assert(action != null);
            Debug.Assert(action.m_ActionMap == this);
            Debug.Assert(!action.isSingletonAction || m_SingletonAction == action);

            // See if we need to refresh.
            if (m_ControlsForEachAction == null)
            {
                if (m_State.totalControlCount == 0)
                    return new ReadOnlyArray<InputControl>();

                if (m_SingletonAction != null)
                {
                    // For singleton action, all resolved controls in the state simply
                    // belong to the action.

                    m_ControlsForEachAction = m_State.controls;

                    action.m_ControlStartIndex = 0;
                    action.m_ControlCount = m_State.totalControlCount;
                }
                else
                {
                    // For "normal" maps, we rely on the per-action binding data set up in SetUpBindingArrayForEachAction().
                    // From that, we set up a sorted array of controls.

                    var mapIndices = m_State.FetchMapIndices(this);

                    var controlCount = mapIndices.controlCount;
                    var bindingCount = mapIndices.bindingCount;
                    var bindingStartIndex = mapIndices.bindingStartIndex;

                    if (m_BindingsForEachAction == null)
                        SetUpBindingArrayForEachAction();

                    // Go binding by binding in the array that has bindings already sorted for
                    // each action. Gather their controls and store the result on the actions
                    // while copying the control references over to a new array.
                    m_ControlsForEachAction = new InputControl[controlCount];
                    var bindingStates = m_State.bindingStates;
                    var controls = m_State.controls;
                    var currentAction = m_ActionForEachBinding[0];
                    var currentActionControlStartIndex = 0;
                    var currentActionControlCount = 0;
                    var currentControlIndex = 0;
                    for (var i = 0; i < bindingCount; ++i)
                    {
                        var actionForBinding = m_ActionForEachBinding[i];
                        if (actionForBinding != currentAction)
                        {
                            if (currentAction != null)
                            {
                                // Store final array slice.
                                currentAction.m_ControlStartIndex = currentActionControlStartIndex;
                                currentAction.m_ControlCount = currentActionControlCount;
                            }

                            // Switch to new action.
                            currentAction = actionForBinding;
                            currentActionControlStartIndex = currentControlIndex;
                            currentActionControlCount = 0;
                        }
                        if (actionForBinding == null)
                            continue; // Binding is not associated with an action.

                        // Copy controls from binding.
                        var bindingIndex = bindingStartIndex + i;
                        var controlCountForBinding = bindingStates[bindingIndex].controlCount;
                        Array.Copy(controls, bindingStates[bindingIndex].controlStartIndex, m_ControlsForEachAction,
                            currentControlIndex, controlCountForBinding);

                        currentControlIndex += controlCountForBinding;
                        currentActionControlCount += controlCountForBinding;
                    }

                    if (currentAction != null)
                    {
                        currentAction.m_ControlStartIndex = currentActionControlStartIndex;
                        currentAction.m_ControlCount = currentActionControlCount;
                    }
                }
            }

            return new ReadOnlyArray<InputControl>(m_ControlsForEachAction, action.m_ControlStartIndex,
                action.m_ControlCount);
        }

        private void SetUpBindingArrayForEachAction()
        {
            // Handle case where we don't have any bindings.
            if (m_Bindings == null)
                return;

            if (m_SingletonAction != null)
            {
                // Dead simple case: map is internally owned by action. The entire
                // list of bindings is specific to the action.

                Debug.Assert(m_Bindings == m_SingletonAction.m_SingletonActionBindings);

                m_BindingsForEachAction = m_Bindings;
                m_ActionForEachBinding = null; // No point in having this for singleton actions.

                m_SingletonAction.m_BindingsStartIndex = 0;
                m_SingletonAction.m_BindingsCount = m_Bindings.Length;
            }
            else
            {
                // Go through all bindings and slice them out to individual actions.

                Debug.Assert(m_Actions != null); // Action isn't a singleton so this has to be true.

                // Allocate array to retain resolved actions, if need be.
                var totalBindingsCount = m_Bindings.Length;
                if (m_ActionForEachBinding == null || m_ActionForEachBinding.Length != totalBindingsCount)
                    m_ActionForEachBinding = new InputAction[totalBindingsCount];

                // Reset state on each action. Important we have actions that are no longer
                // referred to by bindings.
                for (var i = 0; i < m_Actions.Length; ++i)
                {
                    m_Actions[i].m_BindingsCount = 0;
                    m_Actions[i].m_BindingsStartIndex = 0;
                }

                // Collect actions and count bindings.
                // After this loop, we can have one of two situations:
                // 1) The bindings for any action X start at some index N and occupy the next m_BindingsCount slots.
                // 2) The bindings for some or all actions are scattered across non-contiguous chunks of the array.
                for (var i = 0; i < m_Bindings.Length; ++i)
                {
                    // Look up action.
                    var actionForBinding = TryGetAction(m_Bindings[i].action);
                    m_ActionForEachBinding[i] = actionForBinding;
                    if (actionForBinding == null)
                        continue;

                    ++actionForBinding.m_BindingsCount;
                }

                // Collect the bindings and bundle them into chunks.
                var newBindingsArrayIndex = 0;
                InputBinding[] newBindingsArray = null;
                for (var sourceBindingIndex = 0; sourceBindingIndex < m_Bindings.Length;)
                {
                    var currentAction = m_ActionForEachBinding[sourceBindingIndex];
                    if (currentAction == null || currentAction.m_BindingsStartIndex != 0)
                    {
                        // Skip bindings not targeting an action or bindings whose actions we
                        // have already processed (when gathering bindings for a single actions scattered
                        // across the array we may be skipping ahead).
                        ++sourceBindingIndex;
                        continue;
                    }

                    // Bindings for current action start at current index.
                    currentAction.m_BindingsStartIndex = newBindingsArray != null
                        ? newBindingsArrayIndex
                        : sourceBindingIndex;

                    // Collect all bindings for the action.
                    var bindingCountForCurrentAction = currentAction.m_BindingsCount;
                    var sourceBindingToCopy = sourceBindingIndex;
                    for (var i = 0; i < bindingCountForCurrentAction; ++i)
                    {
                        if (m_ActionForEachBinding[sourceBindingToCopy] != currentAction)
                        {
                            // If this is the first action that has its bindings scattered around, switch to
                            // having a separate bindings array and copy whatever bindings we already processed
                            // over to it.
                            if (newBindingsArray == null)
                            {
                                newBindingsArray = new InputBinding[totalBindingsCount];
                                newBindingsArrayIndex = sourceBindingToCopy;
                                Array.Copy(m_Bindings, 0, newBindingsArray, 0, sourceBindingToCopy);
                            }

                            // Find the next binding belonging to the action. We've counted bindings for
                            // the action in the previous pass so we know exactly how many bindings we
                            // can expect.
                            do
                            {
                                ++sourceBindingToCopy;
                                Debug.Assert(sourceBindingToCopy < m_ActionForEachBinding.Length);
                            }
                            while (m_ActionForEachBinding[sourceBindingToCopy] != currentAction);
                        }
                        else if (sourceBindingIndex == sourceBindingToCopy)
                            ++sourceBindingIndex;

                        // Copy binding over to new bindings array, if need be.
                        if (newBindingsArray != null)
                            newBindingsArray[newBindingsArrayIndex++] = m_Bindings[sourceBindingToCopy];

                        ++sourceBindingToCopy;
                    }
                }

                if (newBindingsArray == null)
                {
                    // Bindings are already clustered by action in m_Bindings
                    // so we can just stick to having one array only.
                    m_BindingsForEachAction = m_Bindings;
                }
                else
                {
                    // Bindings are not clustered by action in m_Bindings so
                    // we had to allocate a separate array where the bindings are sorted.
                    m_BindingsForEachAction = newBindingsArray;
                }
            }
        }

        internal void ClearPerActionCachedBindingData()
        {
            m_BindingsForEachAction = null;
            m_ControlsForEachAction = null;
            m_DevicesForEachAction = null;
            m_ActionForEachBinding = null;
        }

        ////FIXME: this needs to be able to handle the case where two maps share state and one is enabled and one isn't
        internal void InvalidateResolvedData()
        {
            Debug.Assert(m_EnabledActionsCount == 0);

            if (m_State == null)
                return;

            ////TODO: keep state instance around for re-use

            if (m_Asset != null)
            {
                foreach (var map in m_Asset.m_ActionMaps)
                    map.m_State = null;
                m_Asset.m_ActionMapState = null;
            }
            else
            {
                m_State = null;
            }

            ClearPerActionCachedBindingData();
        }

        internal void ThrowIfModifyingBindingsIsNotAllowed()
        {
            if (enabled)
                throw new InvalidOperationException(
                    string.Format("Cannot modify bindings on action map '{0}' while the map is enabled", this));
        }

        internal void ResolveBindingsIfNecessary()
        {
            if (m_State == null)
                ResolveBindings();
        }

        /// <summary>
        /// Resolve all bindings to their controls and also add any action interactions
        /// from the bindings.
        /// </summary>
        /// <remarks>
        /// The best way is for this to happen once for each action map at the beginning of the game
        /// and to then enable and disable the maps as needed. However, the system will also re-resolve
        /// bindings if the control setup in the system changes (i.e. if devices are added or removed
        /// or if layouts in the system are changed).
        /// </remarks>
        internal void ResolveBindings()
        {
            // If we're part of an asset, we share state and thus binding resolution with
            // all maps in the asset.
            if (m_Asset)
            {
                var actionMaps = m_Asset.m_ActionMaps;
                Debug.Assert(actionMaps != null); // Should have at least us in the array.
                var actionMapCount = actionMaps.Length;

                // Start resolving.
                var resolver = new InputBindingResolver();

                // If there's a control scheme set on the asset, apply its binding mask.
                if (m_Asset.m_CurrentControlSchemeIndex != -1)
                    resolver.bindingMask = new InputBinding
                    {
                        groups = m_Asset.m_ControlSchemes[m_Asset.m_CurrentControlSchemeIndex].bindingGroup
                    };

                // Resolve all maps in the asset.
                for (var i = 0; i < actionMapCount; ++i)
                    resolver.AddActionMap(actionMaps[i]);

                // Install state.
                if (m_Asset.m_ActionMapState == null)
                {
                    var state = new InputActionMapState();
                    for (var i = 0; i < m_Asset.m_ActionMaps.Length; ++i)
                        m_Asset.m_ActionMaps[i].m_State = state;
                    m_Asset.m_ActionMapState = state;
                    m_State.Initialize(resolver);
                }
                else
                {
                    m_State.ClaimDataFrom(resolver);
                }
            }
            else
            {
                // Standalone action map (possibly a hidden one created for a singleton action).
                // We get our own private state.

                // Resolve all source paths.
                var resolver = new InputBindingResolver();
                resolver.AddActionMap(this);

                // Transfer final arrays into state.
                if (m_State == null)
                {
                    m_State = new InputActionMapState();
                    m_State.Initialize(resolver);
                }
                else
                {
                    m_State.ClaimDataFrom(resolver);
                    ClearPerActionCachedBindingData();
                }
            }
        }

        #region Serialization

        // Action maps are serialized in two different ways. For storage as imported assets in Unity's Library/ folder
        // and in player data and asset bundles as well as for surviving domain reloads, InputActionMaps are serialized
        // directly by Unity. For storage as source data in user projects, InputActionMaps are serialized indirectly
        // as JSON by setting up a separate set of structs that are then read and written using Unity's JSON serializer.

        [Serializable]
        public struct BindingJson
        {
            public string name;
            public string path;
            public string interactions;
            public string processors;
            public string groups;
            public string action;
            public bool chainWithPrevious;
            public bool isComposite;
            public bool isPartOfComposite;

            // This is for backwards compatibility with existing serialized action data as of 0.0.1-preview.
            // Ideally we should be able to nuke this before 1.0.
            public string modifiers;

            public InputBinding ToBinding()
            {
                return new InputBinding
                {
                    name = string.IsNullOrEmpty(name) ? null : name,
                    path = string.IsNullOrEmpty(path) ? null : path,
                    action = string.IsNullOrEmpty(action) ? null : action,
                    interactions = string.IsNullOrEmpty(interactions) ? (!string.IsNullOrEmpty(modifiers) ? modifiers : null) : interactions,
                    processors = string.IsNullOrEmpty(processors) ? null : processors,
                    groups = string.IsNullOrEmpty(groups) ? null : groups,
                    chainWithPrevious = chainWithPrevious,
                    isComposite = isComposite,
                    isPartOfComposite = isPartOfComposite,
                };
            }

            public static BindingJson FromBinding(InputBinding binding)
            {
                return new BindingJson
                {
                    name = binding.name,
                    path = binding.path,
                    action = binding.action,
                    interactions = binding.interactions,
                    processors = binding.processors,
                    groups = binding.groups,
                    chainWithPrevious = binding.chainWithPrevious,
                    isComposite = binding.isComposite,
                    isPartOfComposite = binding.isPartOfComposite,
                };
            }
        }

        [Serializable]
        internal struct ActionJson
        {
            public string name;
            public string id;
            public string expectedControlLayout;

            // Bindings can either be on the action itself (in which case the action name
            // for each binding is implied) or listed separately in the action file.
            public BindingJson[] bindings;

            public static ActionJson FromAction(InputAction action)
            {
                // Bindings don't go on the actions when we write them.
                return new ActionJson
                {
                    name = action.m_Name,
                    id = action.id.ToString(),
                    expectedControlLayout = action.m_ExpectedControlLayout,
                };
            }
        }

        [Serializable]
        internal struct MapJson
        {
            public string name;
            public string id;
            public ActionJson[] actions;
            public BindingJson[] bindings;

            public static MapJson FromMap(InputActionMap map)
            {
                ActionJson[] jsonActions = null;
                BindingJson[] jsonBindings = null;

                var actions = map.m_Actions;
                if (actions != null)
                {
                    var actionCount = actions.Length;
                    jsonActions = new ActionJson[actionCount];

                    for (var i = 0; i < actionCount; ++i)
                        jsonActions[i] = ActionJson.FromAction(actions[i]);
                }

                var bindings = map.m_Bindings;
                if (bindings != null)
                {
                    var bindingCount = bindings.Length;
                    jsonBindings = new BindingJson[bindingCount];

                    for (var i = 0; i < bindingCount; ++i)
                        jsonBindings[i] = BindingJson.FromBinding(bindings[i]);
                }

                return new MapJson
                {
                    name = map.name,
                    id = map.id.ToString(),
                    actions = jsonActions,
                    bindings = jsonBindings,
                };
            }
        }

        // We write JSON in a less flexible format than we allow to be read. JSON files
        // we read can just be flat lists of actions with the map name being contained in
        // the action name and containing their own bindings directly. JSON files we write
        // go map by map and separate bindings and actions.
        [Serializable]
        internal struct WriteFileJson
        {
            public MapJson[] maps;

            public static WriteFileJson FromMap(InputActionMap map)
            {
                return new WriteFileJson
                {
                    maps = new[] {MapJson.FromMap(map)}
                };
            }

            public static WriteFileJson FromMaps(IEnumerable<InputActionMap> maps)
            {
                var mapCount = Enumerable.Count(maps);
                if (mapCount == 0)
                    return new WriteFileJson();

                var mapsJson = new MapJson[mapCount];
                var index = 0;
                foreach (var map in maps)
                    mapsJson[index++] = MapJson.FromMap(map);

                return new WriteFileJson {maps = mapsJson};
            }
        }

        // A JSON representation of one or more sets of actions.
        // Contains a list of actions. Each action may specify the set it belongs to
        // as part of its name ("set/action").
        [Serializable]
        internal struct ReadFileJson
        {
            public ActionJson[] actions;
            public MapJson[] maps;

            public InputActionMap[] ToMaps()
            {
                var mapList = new List<InputActionMap>();
                var actionLists = new List<List<InputAction>>();
                var bindingLists = new List<List<InputBinding>>();

                // Process actions listed at toplevel.
                var actionCount = actions != null ? actions.Length : 0;
                for (var i = 0; i < actionCount; ++i)
                {
                    var jsonAction = actions[i];

                    if (string.IsNullOrEmpty(jsonAction.name))
                        throw new Exception(string.Format("Action number {0} has no name", i + 1));

                    ////REVIEW: make sure all action names are unique?

                    // Determine name of action map.
                    string mapName = null;
                    var actionName = jsonAction.name;
                    var indexOfFirstSlash = actionName.IndexOf('/');
                    if (indexOfFirstSlash != -1)
                    {
                        mapName = actionName.Substring(0, indexOfFirstSlash);
                        actionName = actionName.Substring(indexOfFirstSlash + 1);

                        if (string.IsNullOrEmpty(actionName))
                            throw new Exception(string.Format(
                                "Invalid action name '{0}' (missing action name after '/')", jsonAction.name));
                    }

                    // Try to find existing map.
                    InputActionMap map = null;
                    var mapIndex = 0;
                    for (; mapIndex < mapList.Count; ++mapIndex)
                    {
                        if (string.Compare(mapList[mapIndex].name, mapName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            map = mapList[mapIndex];
                            break;
                        }
                    }

                    // Create new map if it's the first action in the map.
                    if (map == null)
                    {
                        // NOTE: No map IDs supported on this path.
                        map = new InputActionMap(mapName);
                        mapIndex = mapList.Count;
                        mapList.Add(map);
                        actionLists.Add(new List<InputAction>());
                        bindingLists.Add(new List<InputBinding>());
                    }

                    // Create action.
                    var action = new InputAction(actionName);
                    action.m_Id = string.IsNullOrEmpty(jsonAction.id) ? null : jsonAction.id;
                    action.m_ExpectedControlLayout = !string.IsNullOrEmpty(jsonAction.expectedControlLayout)
                        ? jsonAction.expectedControlLayout
                        : null;
                    actionLists[mapIndex].Add(action);

                    // Add bindings.
                    if (jsonAction.bindings != null)
                    {
                        var bindingsForMap = bindingLists[mapIndex];
                        for (var n = 0; n < jsonAction.bindings.Length; ++n)
                        {
                            var jsonBinding = jsonAction.bindings[n];
                            var binding = jsonBinding.ToBinding();
                            binding.action = action.m_Name;
                            bindingsForMap.Add(binding);
                        }
                    }
                }

                // Process maps.
                var mapCount = maps != null ? maps.Length : 0;
                for (var i = 0; i < mapCount; ++i)
                {
                    var jsonMap = maps[i];

                    var mapName = jsonMap.name;
                    if (string.IsNullOrEmpty(mapName))
                        throw new Exception(string.Format("Map number {0} has no name", i + 1));

                    // Try to find existing map.
                    InputActionMap map = null;
                    var mapIndex = 0;
                    for (; mapIndex < mapList.Count; ++mapIndex)
                    {
                        if (string.Compare(mapList[mapIndex].name, mapName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            map = mapList[mapIndex];
                            break;
                        }
                    }

                    // Create new map if we haven't seen it before.
                    if (map == null)
                    {
                        map = new InputActionMap(mapName);
                        map.m_Id = string.IsNullOrEmpty(jsonMap.id) ? null : jsonMap.id;
                        mapIndex = mapList.Count;
                        mapList.Add(map);
                        actionLists.Add(new List<InputAction>());
                        bindingLists.Add(new List<InputBinding>());
                    }

                    // Process actions in map.
                    var actionCountInMap = jsonMap.actions != null ? jsonMap.actions.Length : 0;
                    for (var n = 0; n < actionCountInMap; ++n)
                    {
                        var jsonAction = jsonMap.actions[n];

                        if (string.IsNullOrEmpty(jsonAction.name))
                            throw new Exception(string.Format("Action number {0} in map '{1}' has no name", i + 1, mapName));

                        // Create action.
                        var action = new InputAction(jsonAction.name);
                        action.m_Id = string.IsNullOrEmpty(jsonAction.id) ? null : jsonAction.id;
                        action.m_ExpectedControlLayout = !string.IsNullOrEmpty(jsonAction.expectedControlLayout)
                            ? jsonAction.expectedControlLayout
                            : null;
                        actionLists[mapIndex].Add(action);

                        // Add bindings.
                        if (jsonAction.bindings != null)
                        {
                            var bindingList = bindingLists[mapIndex];
                            for (var k = 0; k < jsonAction.bindings.Length; ++k)
                            {
                                var jsonBinding = jsonAction.bindings[k];
                                var binding = jsonBinding.ToBinding();
                                binding.action = action.m_Name;
                                bindingList.Add(binding);
                            }
                        }
                    }

                    // Process bindings in map.
                    var bindingCountInMap = jsonMap.bindings != null ? jsonMap.bindings.Length : 0;
                    var bindingsForMap = bindingLists[mapIndex];
                    for (var n = 0; n < bindingCountInMap; ++n)
                    {
                        var jsonBinding = jsonMap.bindings[n];
                        var binding = jsonBinding.ToBinding();
                        bindingsForMap.Add(binding);
                    }
                }

                // Finalize arrays.
                for (var i = 0; i < mapList.Count; ++i)
                {
                    var map = mapList[i];

                    var actionArray = actionLists[i].ToArray();
                    var bindingArray = bindingLists[i].ToArray();

                    map.m_Actions = actionArray;
                    map.m_Bindings = bindingArray;

                    for (var n = 0; n < actionArray.Length; ++n)
                    {
                        var action = actionArray[n];
                        action.m_ActionMap = map;
                    }
                }

                return mapList.ToArray();
            }
        }

        // Load one or more action sets from JSON.
        public static InputActionMap[] FromJson(string json)
        {
            var fileJson = JsonUtility.FromJson<ReadFileJson>(json);
            return fileJson.ToMaps();
        }

        public static string ToJson(IEnumerable<InputActionMap> sets)
        {
            var fileJson = WriteFileJson.FromMaps(sets);
            return JsonUtility.ToJson(fileJson, true);
        }

        public string ToJson()
        {
            var fileJson = WriteFileJson.FromMap(this);
            return JsonUtility.ToJson(fileJson, true);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            m_State = null;
            m_MapIndexInState = InputActionMapState.kInvalidIndex;

            // Restore references of actions linking back to us.
            if (m_Actions != null)
            {
                var actionCount = m_Actions.Length;
                for (var i = 0; i < actionCount; ++i)
                    m_Actions[i].m_ActionMap = this;
            }

            // Make sure we don't retain any cached per-action data when using serialization
            // to docter around in action map configurations in the editor.
            m_ActionForEachBinding = null;
            m_BindingsForEachAction = null;
            m_ControlsForEachAction = null;
        }

        #endregion
    }
}
