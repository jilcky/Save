using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class 画质设置_阴影: MonoBehaviour {
    UniversalAdditionalCameraData m_镜头设置;
    public Toggle DrawShadows; //镜头绘制阴影

    string _标记 = "画质设置_阴影";

     void Start () {
        bool _Use;
        m_镜头设置 = GameObject.FindObjectOfType<UniversalAdditionalCameraData> ();
        _Use = 通用计算.存档_Getbool (_标记 + DrawShadows.ToString ());
        DrawShadows.onValueChanged.AddListener (
            delegate {
                SetDrawShadows (DrawShadows.isOn);
            }
        );
    }

    public void SetDrawShadows (bool _输入) {
        m_镜头设置.renderShadows = _输入;
        通用计算.存档_Setbool (_标记 + DrawShadows.ToString (), _输入);
    }

}