using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.Events;

namespace 编辑.触发器 {
    public class 触发器_触碰bool广播: MonoBehaviour {
        public BoolEvent m_广播;
            private void OnTriggerEnter (Collider other) {
            m_广播.Invoke (true);
        }
      
        private void OnTriggerExit (Collider other) {
            m_广播.Invoke (false);
        }
    }}