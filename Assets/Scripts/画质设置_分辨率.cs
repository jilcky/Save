using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class 画质设置_分辨率: MonoBehaviour {
    public Dropdown m_设置选项;
    public enum Size {
        Size360,
        Size480,
        Size720,
        Size1080
    }
    public int[] SizeSet = { 360, 480, 720, 1080 };
    public Text m_Text;
    Vector2 获取硬件分辨率;
    private void Awake () {

    }
   
    float _倍率;
    private void Start () {

        _倍率 = (float) Screen.width / (float) Screen.height;

        int _Sel;
        m_设置选项.options.Clear ();
        m_设置选项.AddOptions (通用计算.下拉列表输出(Size.GetNames (typeof (Size))));
        //添加事件
        m_设置选项.onValueChanged.AddListener (
            delegate {
                On修改帧率(m_设置选项.value);
            }
        );
        _Sel = PlayerPrefs.GetInt ("画质设置_分辨率", 1);
        On修改帧率(m_设置选项.value);
    }

    void On修改帧率(int _输入) {
        m_设置选项.value = _输入;
        float H = (float) SizeSet[_输入];
        float W = H * _倍率;
        Screen.SetResolution ((int) W, (int) H, true);
        PlayerPrefs.SetInt ("画质设置_分辨率", _输入);
    }

}