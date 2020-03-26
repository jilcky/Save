using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.Events;

namespace 编辑.触发器 {
    public class 触发器_交互键输入: MonoBehaviour {
        public UnityEvent m_一般事件;
        public void On交互() {
            m_一般事件.Invoke ();
        }
    }
}