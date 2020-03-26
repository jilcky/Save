using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace 编辑 {
    public class 触控检测: MonoBehaviour{
        public bool 测试;
        public GameObject 目标;
        public Text _位移变量;
        public Text _目标名称;
        public Text _鼠标;
        private void Update () {
            //Debug.Log(目标.name);
            //_目标名称.text = GetUI选择().name;
        }

        public void  On返回(InputAction.CallbackContext _输入)
        {
                _位移变量.text = _输入.time.ToString();
                _鼠标.text = _输入.valueType.ToString();
                _目标名称.text = _输入.ReadValue<UnityEngine.InputSystem.LowLevel.TouchState>().delta.ToString();
        }

        public GameObject GetUI选择() {
            GameObject obj = null;

            GraphicRaycaster[] graphicRaycasters = FindObjectsOfType<GraphicRaycaster> ();
            PointerEventData eventData = new PointerEventData (EventSystem.current);
            eventData.pressPosition = Input.mousePosition;
            eventData.position = Input.mousePosition;
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
    }
}