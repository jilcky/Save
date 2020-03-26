using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace 编辑 {
    public class 管理_切换房间: MonoBehaviour {
        public AssetReference m_房间;
        public void OnScene改变() {
            m_房间.LoadSceneAsync();
        }
        public void OnScene导入改变(AssetReference _输入) {
            _输入.LoadSceneAsync ();
        }
    }
}