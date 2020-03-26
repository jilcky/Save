using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class 画质设置_帧率: MonoBehaviour {
    public Dropdown m_帧率;
    public enum 帧率
    {
        极低,低,中,高,极高
    }
    
    
    private void Start () {
        int _Sel;
        m_帧率.options.Clear ();
         m_帧率.AddOptions (通用计算.下拉列表输出(帧率.GetNames (typeof (帧率))));
        //添加事件
        m_帧率.onValueChanged.AddListener (
            delegate {
                On修改帧率(m_帧率.value);
            }
        );
        _Sel = PlayerPrefs.GetInt ("画质设置_帧率", 1);
        On修改帧率(m_帧率.value);
    }

    void On修改帧率(int _输入) {
        m_帧率.value = _输入;
        帧率  _选项 =  (帧率)_输入;
        int _数值  = (int) _选项;
        Application.targetFrameRate = _数值*10 + 20  ;
        PlayerPrefs.SetInt ("画质设置_帧率",_输入);
    }

}