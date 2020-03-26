using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

[System.Serializable]
public class AssetReferenceEvent : UnityEvent<AssetReference> { }

[System.Serializable]
public class StringEvent : UnityEvent<string> { }
public class BoolEvent : UnityEvent<bool> { }

namespace 编辑.触发器 {

    public class 触发器_广播: MonoBehaviour {
        public AssetReferenceEvent m_AssetReference广播器;
        public void On广播Asset (AssetReference _导入) {
            m_AssetReference广播器.Invoke (_导入);
        }
    }

}