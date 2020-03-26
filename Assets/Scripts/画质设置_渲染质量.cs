using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class 画质设置_渲染质量: MonoBehaviour {

    public UniversalRenderPipelineAsset[] m_渲染设置;
    public Slider Active; //镜头绘制阴影

    string _标记 = "画质设置_渲染质量";
    画质设置_游戏画质 m_游戏画质;
    void Start () {
        m_游戏画质 = GetComponent<画质设置_游戏画质>();
        var _Sel = PlayerPrefs.GetFloat ("画质设置_渲染质量", 1);
        SetActive (_Sel);
        Active.onValueChanged.AddListener (
            delegate {
                SetActiveSelf ();
            }
        );
    }
    public void SetActiveSelf () {
        SetActive (Active.value);
    }
    public void SetActive (float _输入) {
        Active.value = _输入;
        m_渲染设置[m_游戏画质.m_游戏画质.value].renderScale = _输入;
        PlayerPrefs.SetFloat ("画质设置_渲染质量", _输入);
    }

}