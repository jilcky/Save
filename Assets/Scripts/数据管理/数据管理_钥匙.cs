using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace 编辑.数据管理 {
    [CreateAssetMenu (fileName = "数据管理_钥匙", menuName = "~/管理_钥匙", order = 0)]
    public class 信息管理_钥匙: ScriptableObject {
        public bool 设置保存;

        [Serializable]
        public class 钥匙: SerializableDictionary<string, int> { }
        public 钥匙 m_钥匙;

        [ContextMenu ("On添加")]
        public void On添加() {
            m_钥匙.Add ("", 0);
        }

        [ContextMenu ("OnSave")]
        public void OnSave () {
            PlayerPrefs.SetString ("钥匙存档", JsonUtility.ToJson (m_钥匙));
        }
        private void OnEnable () {

        }
        private void OnDisable () {
            if (!设置保存) {
                m_钥匙 = JsonUtility.FromJson<钥匙> (PlayerPrefs.GetString (
                    "钥匙存档", ""
                ));
            }
        }

    }
}