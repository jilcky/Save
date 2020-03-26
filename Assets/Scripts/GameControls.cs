// GENERATED AUTOMATICALLY FROM 'Assets/Setting/GameControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameControls"",
    ""maps"": [
        {
            ""name"": ""行为"",
            ""id"": ""c5195d24-8761-4e5b-a520-d2d730263fa5"",
            ""actions"": [
                {
                    ""name"": ""交互"",
                    ""type"": ""Button"",
                    ""id"": ""a867a1b5-aae5-46ef-949e-4eaf5e90a9ea"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""暂停"",
                    ""type"": ""Button"",
                    ""id"": ""df70193a-915d-47b9-8263-73bc0a025a19"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Tap""
                },
                {
                    ""name"": ""取消"",
                    ""type"": ""Button"",
                    ""id"": ""92799841-344a-468c-89b1-cbe94e43bcd9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""确定"",
                    ""type"": ""Button"",
                    ""id"": ""3411183c-5dca-472d-988c-beac06b22dda"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""移动"",
                    ""type"": ""Button"",
                    ""id"": ""2e4083e5-4a3b-45ac-a67f-31076d2a2b00"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""攻击"",
                    ""type"": ""Button"",
                    ""id"": ""e3d92ef0-2c33-4a6a-9eec-89e4d0b648e3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""跳跃"",
                    ""type"": ""Button"",
                    ""id"": ""0757c476-2f4a-412c-bdd1-7fe5b51e949e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""触控0"",
                    ""type"": ""Value"",
                    ""id"": ""48da60a0-b877-4077-86b7-7164d448beee"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""触控1"",
                    ""type"": ""Value"",
                    ""id"": ""9ae5990a-1667-4d42-90e3-e11eb1f48d65"",
                    ""expectedControlType"": ""Touch"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""46d7923b-ac91-4027-a7a4-8b3baf66b726"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""攻击"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dfd2cf38-a22d-45c8-b7da-da29283160c5"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""跳跃"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6a08e89-4ed2-474f-a70f-53cc70702697"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""确定"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""231e55a4-ac76-43b3-bfe3-41054a14ca97"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""取消"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fcd2dbca-e987-48e7-a26d-827dfcdc0f76"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""暂停"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b9bd79b-1a6f-425d-933e-4aad205eb40a"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""交互"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""8fcb7fe1-dbd6-4b68-9230-33262c05ea48"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""移动"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2eaea37e-b010-4a30-9558-3f5032320926"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""移动"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""68840990-8e47-4026-8dd7-99ccd826e650"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""移动"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f38da071-3b03-41eb-8d35-7dd7b2ad6908"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""移动"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f9631820-25fd-44e9-bc89-786f9156a23e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""移动"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8198011e-5f03-480a-abeb-05b30597c07a"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""移动"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03f77c8f-6e83-486a-8641-1d29b8a0f240"",
                    ""path"": ""<Touchscreen>/touch0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""触控0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b299b6ad-12c2-46cc-b725-10023fe8be29"",
                    ""path"": ""<Touchscreen>/touch1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""触控1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // 行为
        m_行为 = asset.FindActionMap("行为", throwIfNotFound: true);
        m_行为_交互 = m_行为.FindAction("交互", throwIfNotFound: true);
        m_行为_暂停 = m_行为.FindAction("暂停", throwIfNotFound: true);
        m_行为_取消 = m_行为.FindAction("取消", throwIfNotFound: true);
        m_行为_确定 = m_行为.FindAction("确定", throwIfNotFound: true);
        m_行为_移动 = m_行为.FindAction("移动", throwIfNotFound: true);
        m_行为_攻击 = m_行为.FindAction("攻击", throwIfNotFound: true);
        m_行为_跳跃 = m_行为.FindAction("跳跃", throwIfNotFound: true);
        m_行为_触控0 = m_行为.FindAction("触控0", throwIfNotFound: true);
        m_行为_触控1 = m_行为.FindAction("触控1", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // 行为
    private readonly InputActionMap m_行为;
    private I行为Actions m_行为ActionsCallbackInterface;
    private readonly InputAction m_行为_交互;
    private readonly InputAction m_行为_暂停;
    private readonly InputAction m_行为_取消;
    private readonly InputAction m_行为_确定;
    private readonly InputAction m_行为_移动;
    private readonly InputAction m_行为_攻击;
    private readonly InputAction m_行为_跳跃;
    private readonly InputAction m_行为_触控0;
    private readonly InputAction m_行为_触控1;
    public struct 行为Actions
    {
        private @GameControls m_Wrapper;
        public 行为Actions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @交互 => m_Wrapper.m_行为_交互;
        public InputAction @暂停 => m_Wrapper.m_行为_暂停;
        public InputAction @取消 => m_Wrapper.m_行为_取消;
        public InputAction @确定 => m_Wrapper.m_行为_确定;
        public InputAction @移动 => m_Wrapper.m_行为_移动;
        public InputAction @攻击 => m_Wrapper.m_行为_攻击;
        public InputAction @跳跃 => m_Wrapper.m_行为_跳跃;
        public InputAction @触控0 => m_Wrapper.m_行为_触控0;
        public InputAction @触控1 => m_Wrapper.m_行为_触控1;
        public InputActionMap Get() { return m_Wrapper.m_行为; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(行为Actions set) { return set.Get(); }
        public void SetCallbacks(I行为Actions instance)
        {
            if (m_Wrapper.m_行为ActionsCallbackInterface != null)
            {
                @交互.started -= m_Wrapper.m_行为ActionsCallbackInterface.On交互;
                @交互.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On交互;
                @交互.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On交互;
                @暂停.started -= m_Wrapper.m_行为ActionsCallbackInterface.On暂停;
                @暂停.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On暂停;
                @暂停.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On暂停;
                @取消.started -= m_Wrapper.m_行为ActionsCallbackInterface.On取消;
                @取消.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On取消;
                @取消.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On取消;
                @确定.started -= m_Wrapper.m_行为ActionsCallbackInterface.On确定;
                @确定.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On确定;
                @确定.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On确定;
                @移动.started -= m_Wrapper.m_行为ActionsCallbackInterface.On移动;
                @移动.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On移动;
                @移动.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On移动;
                @攻击.started -= m_Wrapper.m_行为ActionsCallbackInterface.On攻击;
                @攻击.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On攻击;
                @攻击.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On攻击;
                @跳跃.started -= m_Wrapper.m_行为ActionsCallbackInterface.On跳跃;
                @跳跃.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On跳跃;
                @跳跃.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On跳跃;
                @触控0.started -= m_Wrapper.m_行为ActionsCallbackInterface.On触控0;
                @触控0.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On触控0;
                @触控0.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On触控0;
                @触控1.started -= m_Wrapper.m_行为ActionsCallbackInterface.On触控1;
                @触控1.performed -= m_Wrapper.m_行为ActionsCallbackInterface.On触控1;
                @触控1.canceled -= m_Wrapper.m_行为ActionsCallbackInterface.On触控1;
            }
            m_Wrapper.m_行为ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @交互.started += instance.On交互;
                @交互.performed += instance.On交互;
                @交互.canceled += instance.On交互;
                @暂停.started += instance.On暂停;
                @暂停.performed += instance.On暂停;
                @暂停.canceled += instance.On暂停;
                @取消.started += instance.On取消;
                @取消.performed += instance.On取消;
                @取消.canceled += instance.On取消;
                @确定.started += instance.On确定;
                @确定.performed += instance.On确定;
                @确定.canceled += instance.On确定;
                @移动.started += instance.On移动;
                @移动.performed += instance.On移动;
                @移动.canceled += instance.On移动;
                @攻击.started += instance.On攻击;
                @攻击.performed += instance.On攻击;
                @攻击.canceled += instance.On攻击;
                @跳跃.started += instance.On跳跃;
                @跳跃.performed += instance.On跳跃;
                @跳跃.canceled += instance.On跳跃;
                @触控0.started += instance.On触控0;
                @触控0.performed += instance.On触控0;
                @触控0.canceled += instance.On触控0;
                @触控1.started += instance.On触控1;
                @触控1.performed += instance.On触控1;
                @触控1.canceled += instance.On触控1;
            }
        }
    }
    public 行为Actions @行为 => new 行为Actions(this);
    public interface I行为Actions
    {
        void On交互(InputAction.CallbackContext context);
        void On暂停(InputAction.CallbackContext context);
        void On取消(InputAction.CallbackContext context);
        void On确定(InputAction.CallbackContext context);
        void On移动(InputAction.CallbackContext context);
        void On攻击(InputAction.CallbackContext context);
        void On跳跃(InputAction.CallbackContext context);
        void On触控0(InputAction.CallbackContext context);
        void On触控1(InputAction.CallbackContext context);
    }
}
