using System;
using UnityEngine.EventSystems;

////TODO: come up with an action response system that doesn't require hooking and unhooking all those delegates

namespace UnityEngine.Experimental.Input.Plugins.UI
{
    /// <summary>
    /// Input module that takes its input from <see cref="InputAction">input actions</see>.
    /// </summary>
    public class UIActionInputModule : UIInputModule
    {
        /// <summary>
        /// An <see cref="InputAction"/> delivering a <see cref="Vector2">2D screen position
        /// </see> used as a cursor for pointing at UI elements.
        /// </summary>
        public InputActionProperty point
        {
            get { return m_PointAction; }
            set
            {
                if (m_PointAction != null && m_ActionsHooked)
                    m_PointAction.action.performed -= m_ActionCallback;
                m_PointAction = value;
                if (m_PointAction != null && m_ActionsHooked)
                    m_PointAction.action.performed += m_ActionCallback;
            }
        }

        /// <summary>
        /// An <see cref="InputAction"/> delivering a <see cref="Vector2">2D motion vector
        /// </see> used for sending <see cref="AxisEventData"/> events.
        /// </summary>
        public InputActionProperty move
        {
            get { return m_MoveAction; }
            set
            {
                if (m_MoveAction != null && m_ActionsHooked)
                    m_MoveAction.action.performed -= m_ActionCallback;
                m_PointAction = value;
                if (m_PointAction != null && m_ActionsHooked)
                    m_PointAction.action.performed += m_ActionCallback;
            }
        }

        public InputActionProperty leftClick;

        public InputActionProperty middleClick;

        public InputActionProperty rightClick;

        public InputActionProperty scroll;

        public void OnDestroy()
        {
            if (m_ActionsHooked)
                UnhookActions();
        }

        public void OnEnable()
        {
            HookActions();
        }

        public void OnDisable()
        {
            UnhookActions();
        }

        public override void Process()
        {
            throw new NotImplementedException();
        }

        protected void ProcessPointer(Pointer pointer)
        {
            if (pointer == null)
                throw new ArgumentNullException("pointer");
        }

        private void HookActions()
        {
            if (m_ActionCallback == null)
                m_ActionCallback = m_ActionQueue.RecordAction;

            m_ActionsHooked = true;

            var pointAction = m_PointAction.action;
            if (pointAction != null)
                pointAction.performed += m_ActionCallback;

            var moveAction = m_MoveAction.action;
            if (moveAction != null)
                moveAction.performed += m_ActionCallback;
        }

        private void UnhookActions()
        {
            m_ActionsHooked = false;

            var pointAction = m_PointAction.action;
            if (pointAction != null)
                pointAction.performed -= m_ActionCallback;

            var moveAction = m_MoveAction.action;
            if (moveAction != null)
                moveAction.performed -= m_ActionCallback;
        }

        /// <summary>
        /// An <see cref="InputAction"/> delivering a <see cref="Vector2">2D screen position
        /// </see> used as a cursor for pointing at UI elements.
        /// </summary>
        [Tooltip("Action that delivers a Vector2 of screen coordinates.")]
        [SerializeField]
        private InputActionProperty m_PointAction;

        /// <summary>
        /// An <see cref="InputAction"/> delivering a <see cref="Vector2">2D motion vector
        /// </see> used for sending <see cref="AxisEventData"/> events.
        /// </summary>
        [Tooltip("Action that delivers a relative motion Vector2.")]
        [SerializeField]
        private InputActionProperty m_MoveAction;

        [Tooltip("Button action that represents a left click.")]
        [SerializeField]
        private InputActionProperty m_LeftClickAction;

        [Tooltip("Button action that represents a middle click.")]
        [SerializeField]
        private InputActionProperty m_MiddleClickAction;

        [Tooltip("Button action that represents a right click.")]
        [SerializeField]
        private InputActionProperty m_RightClickAction;

        [Tooltip("Vector2 action that represents horizontal and vertical scrolling.")]
        [SerializeField]
        private InputActionProperty m_ScrollAction;

        [NonSerialized] private bool m_ActionsHooked;
        [NonSerialized] private Action<InputAction.CallbackContext> m_ActionCallback;

        /// <summary>
        /// Queue where we record action events.
        /// </summary>
        /// <remarks>
        /// The callback-based interface isn't of much use to us as we cannot respond to actions immediately
        /// but have to wait until <see cref="Process"/> is called by <see cref="EventSystem"/>. So instead
        /// we trace everything that happens to the actions we're linked to by recording events into this queue
        /// and then during <see cref="Process"/> we replay any activity that has occurred since the last
        /// call to <see cref="Process"/> and translate it into <see cref="BaseEventData">UI events</see>.
        /// </remarks>
        [NonSerialized] private InputActionQueue m_ActionQueue;
    }
}
