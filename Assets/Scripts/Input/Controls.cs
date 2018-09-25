// GENERATED AUTOMATICALLY FROM 'Assets/Settings/Controls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Input;


namespace Sevakon.Input
{
    [Serializable]
    public class Controls : InputActionAssetReference
    {
        public Controls()
        {
        }
        public Controls(InputActionAsset asset)
            : base(asset)
        {
        }
        private bool m_Initialized;
        private void Initialize()
        {
            // Gameplay
            m_Gameplay = asset.GetActionMap("Gameplay");
            m_Gameplay_Fire = m_Gameplay.GetAction("Fire");
            if (m_GameplayFireActionStarted != null)
                m_Gameplay_Fire.started += m_GameplayFireActionStarted.Invoke;
            if (m_GameplayFireActionPerformed != null)
                m_Gameplay_Fire.performed += m_GameplayFireActionPerformed.Invoke;
            if (m_GameplayFireActionCancelled != null)
                m_Gameplay_Fire.cancelled += m_GameplayFireActionCancelled.Invoke;
            m_Gameplay_Look = m_Gameplay.GetAction("Look");
            if (m_GameplayLookActionStarted != null)
                m_Gameplay_Look.started += m_GameplayLookActionStarted.Invoke;
            if (m_GameplayLookActionPerformed != null)
                m_Gameplay_Look.performed += m_GameplayLookActionPerformed.Invoke;
            if (m_GameplayLookActionCancelled != null)
                m_Gameplay_Look.cancelled += m_GameplayLookActionCancelled.Invoke;
            m_Gameplay_Jump = m_Gameplay.GetAction("Jump");
            if (m_GameplayJumpActionStarted != null)
                m_Gameplay_Jump.started += m_GameplayJumpActionStarted.Invoke;
            if (m_GameplayJumpActionPerformed != null)
                m_Gameplay_Jump.performed += m_GameplayJumpActionPerformed.Invoke;
            if (m_GameplayJumpActionCancelled != null)
                m_Gameplay_Jump.cancelled += m_GameplayJumpActionCancelled.Invoke;
            m_Gameplay_Move = m_Gameplay.GetAction("Move");
            if (m_GameplayMoveActionStarted != null)
                m_Gameplay_Move.started += m_GameplayMoveActionStarted.Invoke;
            if (m_GameplayMoveActionPerformed != null)
                m_Gameplay_Move.performed += m_GameplayMoveActionPerformed.Invoke;
            if (m_GameplayMoveActionCancelled != null)
                m_Gameplay_Move.cancelled += m_GameplayMoveActionCancelled.Invoke;
            // Menu
            m_Menu = asset.GetActionMap("Menu");
            m_Initialized = true;
        }
        private void Uninitialize()
        {
            m_Gameplay = null;
            m_Gameplay_Fire = null;
            if (m_GameplayFireActionStarted != null)
                m_Gameplay_Fire.started -= m_GameplayFireActionStarted.Invoke;
            if (m_GameplayFireActionPerformed != null)
                m_Gameplay_Fire.performed -= m_GameplayFireActionPerformed.Invoke;
            if (m_GameplayFireActionCancelled != null)
                m_Gameplay_Fire.cancelled -= m_GameplayFireActionCancelled.Invoke;
            m_Gameplay_Look = null;
            if (m_GameplayLookActionStarted != null)
                m_Gameplay_Look.started -= m_GameplayLookActionStarted.Invoke;
            if (m_GameplayLookActionPerformed != null)
                m_Gameplay_Look.performed -= m_GameplayLookActionPerformed.Invoke;
            if (m_GameplayLookActionCancelled != null)
                m_Gameplay_Look.cancelled -= m_GameplayLookActionCancelled.Invoke;
            m_Gameplay_Jump = null;
            if (m_GameplayJumpActionStarted != null)
                m_Gameplay_Jump.started -= m_GameplayJumpActionStarted.Invoke;
            if (m_GameplayJumpActionPerformed != null)
                m_Gameplay_Jump.performed -= m_GameplayJumpActionPerformed.Invoke;
            if (m_GameplayJumpActionCancelled != null)
                m_Gameplay_Jump.cancelled -= m_GameplayJumpActionCancelled.Invoke;
            m_Gameplay_Move = null;
            if (m_GameplayMoveActionStarted != null)
                m_Gameplay_Move.started -= m_GameplayMoveActionStarted.Invoke;
            if (m_GameplayMoveActionPerformed != null)
                m_Gameplay_Move.performed -= m_GameplayMoveActionPerformed.Invoke;
            if (m_GameplayMoveActionCancelled != null)
                m_Gameplay_Move.cancelled -= m_GameplayMoveActionCancelled.Invoke;
            m_Menu = null;
            m_Initialized = false;
        }
        public void SwitchAsset(InputActionAsset newAsset)
        {
            if (newAsset == asset) return;
            if (m_Initialized) Uninitialize();
            asset = newAsset;
        }
        public void DuplicateAndSwitchAsset()
        {
            SwitchAsset(ScriptableObject.Instantiate(asset));
        }
        // Gameplay
        private InputActionMap m_Gameplay;
        private InputAction m_Gameplay_Fire;
        [SerializeField] private ActionEvent m_GameplayFireActionStarted;
        [SerializeField] private ActionEvent m_GameplayFireActionPerformed;
        [SerializeField] private ActionEvent m_GameplayFireActionCancelled;
        private InputAction m_Gameplay_Look;
        [SerializeField] private ActionEvent m_GameplayLookActionStarted;
        [SerializeField] private ActionEvent m_GameplayLookActionPerformed;
        [SerializeField] private ActionEvent m_GameplayLookActionCancelled;
        private InputAction m_Gameplay_Jump;
        [SerializeField] private ActionEvent m_GameplayJumpActionStarted;
        [SerializeField] private ActionEvent m_GameplayJumpActionPerformed;
        [SerializeField] private ActionEvent m_GameplayJumpActionCancelled;
        private InputAction m_Gameplay_Move;
        [SerializeField] private ActionEvent m_GameplayMoveActionStarted;
        [SerializeField] private ActionEvent m_GameplayMoveActionPerformed;
        [SerializeField] private ActionEvent m_GameplayMoveActionCancelled;
        public struct GameplayActions
        {
            private Controls m_Wrapper;
            public GameplayActions(Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Fire { get { return m_Wrapper.m_Gameplay_Fire; } }
            public ActionEvent FireStarted { get { return m_Wrapper.m_GameplayFireActionStarted; } }
            public ActionEvent FirePerformed { get { return m_Wrapper.m_GameplayFireActionPerformed; } }
            public ActionEvent FireCancelled { get { return m_Wrapper.m_GameplayFireActionCancelled; } }
            public InputAction @Look { get { return m_Wrapper.m_Gameplay_Look; } }
            public ActionEvent LookStarted { get { return m_Wrapper.m_GameplayLookActionStarted; } }
            public ActionEvent LookPerformed { get { return m_Wrapper.m_GameplayLookActionPerformed; } }
            public ActionEvent LookCancelled { get { return m_Wrapper.m_GameplayLookActionCancelled; } }
            public InputAction @Jump { get { return m_Wrapper.m_Gameplay_Jump; } }
            public ActionEvent JumpStarted { get { return m_Wrapper.m_GameplayJumpActionStarted; } }
            public ActionEvent JumpPerformed { get { return m_Wrapper.m_GameplayJumpActionPerformed; } }
            public ActionEvent JumpCancelled { get { return m_Wrapper.m_GameplayJumpActionCancelled; } }
            public InputAction @Move { get { return m_Wrapper.m_Gameplay_Move; } }
            public ActionEvent MoveStarted { get { return m_Wrapper.m_GameplayMoveActionStarted; } }
            public ActionEvent MovePerformed { get { return m_Wrapper.m_GameplayMoveActionPerformed; } }
            public ActionEvent MoveCancelled { get { return m_Wrapper.m_GameplayMoveActionCancelled; } }
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public InputActionMap Clone() { return Get().Clone(); }
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        }
        public GameplayActions @Gameplay
        {
            get
            {
                if (!m_Initialized) Initialize();
                return new GameplayActions(this);
            }
        }
        // Menu
        private InputActionMap m_Menu;
        public struct MenuActions
        {
            private Controls m_Wrapper;
            public MenuActions(Controls wrapper) { m_Wrapper = wrapper; }
            public InputActionMap Get() { return m_Wrapper.m_Menu; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public InputActionMap Clone() { return Get().Clone(); }
            public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        }
        public MenuActions @Menu
        {
            get
            {
                if (!m_Initialized) Initialize();
                return new MenuActions(this);
            }
        }
        [Serializable]
        public class ActionEvent : UnityEvent<InputAction.CallbackContext>
        {
        }
    }
}
