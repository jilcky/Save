using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamekit3D {
    public class InteractOnButton : InteractOnTrigger {

        public string buttonName = "X";
        public UnityEvent OnButtonPress;

        //启动可以用
        bool canExecuteButtons = false;
        //条件可以使用
        [SerializeField]
        bool canUse = true;
        public void OnChangeUse (bool _输入) {
            canUse = _输入;
            if (canUse) {
                m_按钮.On启动(this);
            } else {
                m_按钮.On关闭();
            }
        }
        InteractOnButton反馈处理 m_按钮;
        private void OnEnable () {
            m_按钮 = GameObject.FindObjectOfType<InteractOnButton反馈处理> ();
        }

        protected override void ExecuteOnEnter (Collider other) {
            if (0 != (layers.value & 1 << other.gameObject.layer)) {
                canExecuteButtons = true;
                if (canUse) {
                    m_按钮.On启动(this);
                }
            }
        }

        protected override void ExecuteOnExit (Collider other) {
            if (0 != (layers.value & 1 << other.gameObject.layer)) {
                canExecuteButtons = false;
                if (canUse) {
                    m_按钮.On关闭();
                }
            }
        }

        void Update () {
            //等待对应按键按下
            if (canExecuteButtons && Input.GetButtonDown (buttonName)) {
                On执行();
            }
        }
        public void On执行() {
            if (canUse && canExecuteButtons ) {
                OnButtonPress.Invoke ();
            }
        }

    }
}