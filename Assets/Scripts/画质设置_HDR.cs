using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class 画质设置_HDR: MonoBehaviour {
    Camera m_镜头设置;
    public Toggle Active; //镜头绘制阴影

    string _标记 = "画质设置_HDR";

     void Start () {
        bool _Use;
        m_镜头设置 = Camera.main;
        _Use = 通用计算.存档_Getbool (_标记);
        Active.onValueChanged.AddListener (
            delegate {
                SetActive (Active.isOn);
            }
        );

        
    }
    public void SetActive (bool _输入) {
        m_镜头设置.allowHDR = _输入;
        通用计算.存档_Setbool (_标记, _输入);
    }

}