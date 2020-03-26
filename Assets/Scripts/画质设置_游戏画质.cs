using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class 画质设置_游戏画质: MonoBehaviour {
    public Dropdown m_游戏画质;
    RenderPipelineAsset[] m渲染设置列表;
    画质设置_渲染质量 m_渲染质量;
    private void Start () {
        m_渲染质量  = GetComponent<画质设置_渲染质量>();
        
        int _Sel;
        m_游戏画质.options.Clear ();
        m渲染设置列表 = new RenderPipelineAsset[3];
        for (int i = 0; i < 3 ; i++) {
            m渲染设置列表[i] = QualitySettings.GetRenderPipelineAssetAt (i);
            Dropdown.OptionData _O = new Dropdown.OptionData ();
            _O.text = m渲染设置列表[i].ToString ();
            m_游戏画质.options.Add (_O);
        }

        //添加事件
        m_游戏画质.onValueChanged.AddListener (
            delegate {
                On修改画质(m_游戏画质.value);
            }
        );
        _Sel = PlayerPrefs.GetInt ("画质设置_游戏画质", 1);
    }

    void On修改画质(int _输入)
    {
        m_游戏画质.value = _输入;
        QualitySettings.SetQualityLevel(_输入);
        m_渲染质量.SetActiveSelf();
        PlayerPrefs.SetInt ("画质设置_游戏画质",_输入);
    }

}