using UnityEngine;
using UnityEngine.Events;

namespace 编辑.触发器 {
    public class 触发器_碰撞: MonoBehaviour {
        
        public UnityEvent m_开始事件;
        public UnityEvent m_持续事件;
        public UnityEvent m_结束事件;
        // private void OnCollisionEnter(Collision other) {
        //      m_开始事件.Invoke ();
        // }
        // private void OnCollisionExit2D(Collision2D other) {
        //      m_结束事件.Invoke ();
        // }

        private void OnTriggerEnter (Collider other) {
            m_开始事件.Invoke ();
        }
        private void OnTriggerStay (Collider other) {
            m_持续事件.Invoke ();
        }
        private void OnTriggerExit (Collider other) {
            m_结束事件.Invoke ();
        }

        // Update is called once per frame
    }
}