using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace 编辑.触发器 {
    public class 触发器_机关交互处理: MonoBehaviour {
        
        public 交互状态 _交互状态;

        public 交互状态 m_交互状态 {
            get {
                return _交互状态;
            }
            set {
                value = _交互状态;
            }
        }

        public 接触状态 _接触状态;

        public 接触状态 m_接触状态 {
            get {
                return _接触状态;
            }
            set {
                value = _接触状态;
            }
        }

        public enum 交互状态 {
            未交互,
            交互开始,
            交互执行中,
            交互结束
        }
        public enum 接触状态 {
            接触,
            离开
        }

    public GameObject m_接触物体;
    public void On传递接触( GameObject _导入 )
    {
        if (m_交互状态 == 交互状态.未交互 )
        {
            m_接触状态 = 接触状态.接触;
            m_接触物体 = _导入;
        }
    }
        public void On传递离开( GameObject _导入 )
    {
        if (m_交互状态 == 交互状态.未交互 )
        {
            m_接触状态 = 接触状态.离开;
            m_接触物体 = null;
        }
    }
        public void OnUse()
        {
            
        }
    
    }
}