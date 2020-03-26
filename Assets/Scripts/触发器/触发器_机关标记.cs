using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.Events;
using 编辑.数据管理;


namespace 编辑.触发器 {
    [System.Serializable]
    public class 触发器_机关标记: MonoBehaviour {
        private 触发器_机关交互处理 m_交互控制器;
        public  信息管理_钥匙.钥匙 m_执行需求;
        public  string m_执行命令;
        
         [ContextMenu ("On添加")]
        public void On添加() {
            m_执行需求.Add ("", 0);
        }

        private void Awake() {
            m_交互控制器 = GameObject.FindObjectOfType<触发器_机关交互处理>();
        }
        private void OnTriggerEnter(Collider other) {
            if ( other.tag == "Player")
            {   
                m_交互控制器.On传递接触(this.gameObject);
            }
        }
        private void OnTriggerExit(Collider other) {
             if ( other.tag == "Player")
            {
                m_交互控制器.On传递离开(this.gameObject);
            }
        }
        
    }

}