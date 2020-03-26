using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gamekit3D {
    public class CameraSettings : MonoBehaviour {
        public enum InputChoice {
            KeyboardAndMouse,
            Controller,
        }

        [Serializable]
        public struct InvertSettings {
            public bool invertX;
            public bool invertY;
        }

        public Transform follow;
        public Transform lookAt;
        public CinemachineFreeLook keyboardAndMouseCamera;
        public CinemachineFreeLook controllerCamera;
        public InputChoice inputChoice;
        public InvertSettings keyboardAndMouseInvertSettings;
        public InvertSettings controllerInvertSettings;
        public bool allowRuntimeCameraSettingsChanges;

        public CinemachineFreeLook Current {
            get { return inputChoice == InputChoice.KeyboardAndMouse ? keyboardAndMouseCamera : controllerCamera; }
        }

        void Reset () {
            Transform keyboardAndMouseCameraTransform = transform.Find ("KeyboardAndMouseFreeLookRig");
            if (keyboardAndMouseCameraTransform != null)
                keyboardAndMouseCamera = keyboardAndMouseCameraTransform.GetComponent<CinemachineFreeLook> ();

            Transform controllerCameraTransform = transform.Find ("ControllerFreeLookRig");
            if (controllerCameraTransform != null)
                controllerCamera = controllerCameraTransform.GetComponent<CinemachineFreeLook> ();

            PlayerController playerController = FindObjectOfType<PlayerController> ();
            if (playerController != null && playerController.name == "Ellen") {
                follow = playerController.transform;

                lookAt = follow.Find ("HeadTarget");

                if (playerController.cameraSettings == null)
                    playerController.cameraSettings = this;
            }
        }

        void Awake () {
            UpdateCameraSettings ();
        }

        void Update () {
            if (allowRuntimeCameraSettingsChanges) {
                UpdateCameraSettings ();
            }
        }

        void UpdateCameraSettings () {
            keyboardAndMouseCamera.Follow = follow;
            keyboardAndMouseCamera.LookAt = lookAt;
            keyboardAndMouseCamera.m_XAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertX;
            keyboardAndMouseCamera.m_YAxis.m_InvertInput = keyboardAndMouseInvertSettings.invertY;

            controllerCamera.m_XAxis.m_InvertInput = controllerInvertSettings.invertX;
            controllerCamera.m_YAxis.m_InvertInput = controllerInvertSettings.invertY;
            controllerCamera.Follow = follow;
            controllerCamera.LookAt = lookAt;

            keyboardAndMouseCamera.Priority = inputChoice == InputChoice.KeyboardAndMouse ? 1 : 0;
            controllerCamera.Priority = inputChoice == InputChoice.Controller ? 1 : 0;
        }

        public GameObject GetUI选择(Vector2 _坐标) {
            GameObject obj = null;

            GraphicRaycaster[] graphicRaycasters = FindObjectsOfType<GraphicRaycaster> ();
            PointerEventData eventData = new PointerEventData (EventSystem.current);
            eventData.pressPosition = _坐标; //Input.mousePosition;
            eventData.position = _坐标; //Input.mousePosition;
            List<RaycastResult> list = new List<RaycastResult> ();

            foreach (var item in graphicRaycasters) {
                item.Raycast (eventData, list);
                if (list.Count > 0) {
                    for (int i = 0; i < list.Count; i++) {
                        obj = list[i].gameObject;
                    }
                }
            }
            return obj;
        }
        public Text _文本;
        public Text _time;
        public GameObject 触控_0号目标;
        public GameObject 触控_1号目标;
        public float 敏感度 = 1-0.68f;
        public void On触控_0号(InputAction.CallbackContext _输入) {

            var 变量 = _输入.ReadValue<UnityEngine.InputSystem.LowLevel.TouchState> ();
            _time.text = _输入.time.ToString ();
            _文本.text = 变量.phaseId.ToString ();
            //1 准备 
            //2 拖动
            //3 松开
            if (变量.phaseId == 1) {
                触控_0号目标 = GetUI选择(变量.position);
            }
            if (变量.phaseId == 2 && 触控_0号目标 == null) {
                controllerCamera.m_XAxis.m_InputAxisValue = 变量.delta.x*敏感度;
                controllerCamera.m_YAxis.m_InputAxisValue = 变量.delta.y*敏感度;
            }
            if (变量.phaseId == 3) {
                controllerCamera.m_XAxis.m_InputAxisValue = 0;
                controllerCamera.m_YAxis.m_InputAxisValue = 0;
            }
        }
        public void On触控_1号(InputAction.CallbackContext _输入) {

            var 变量 = _输入.ReadValue<UnityEngine.InputSystem.LowLevel.TouchState> ();
            _time.text = _输入.time.ToString ();
            _文本.text = 变量.phaseId.ToString ();
            //1 准备 
            //2 拖动
            //3 松开
            if (变量.phaseId == 1) {
                触控_1号目标 = GetUI选择(变量.position);
            }
            if (变量.phaseId == 2 && 触控_1号目标 == null && 触控_0号目标 != null) {
                controllerCamera.m_XAxis.m_InputAxisValue = 变量.delta.x*敏感度;
                controllerCamera.m_YAxis.m_InputAxisValue = 变量.delta.y*敏感度;
            }
            if (变量.phaseId == 3) {
                controllerCamera.m_XAxis.m_InputAxisValue = 0;
                controllerCamera.m_YAxis.m_InputAxisValue = 0;
            }
        }


    }
}