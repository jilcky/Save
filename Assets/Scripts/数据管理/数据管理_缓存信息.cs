using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using 编辑.触发器;

namespace 编辑.数据管理 {
    [CreateAssetMenu (fileName = "管理_缓存信息", menuName = "~/缓存信息", order = 0)]
    public class 数据管理_缓存信息: ScriptableObject {
        public 场景缓存数据 m_场景缓存数据;
        [System.Serializable]
        public class 场景缓存数据 {
            public GameObject m_机关标记;
        }
    }
}