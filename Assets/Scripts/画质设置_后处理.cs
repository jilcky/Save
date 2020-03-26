using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


public class 画质设置_后处理: MonoBehaviour {
    public VolumeProfile m_后处理设置;
    public Toggle[] m_开关列表;
    public Dropdown m_抗锯齿类型;
    public Dropdown m_抗锯齿_SMAA质量;
    public Toggle m_后处理开关;
    UniversalAdditionalCameraData m_镜头设置;
    // Start is called before the first frame update



    void Start () {
        m_镜头设置 = GameObject.FindObjectOfType<UniversalAdditionalCameraData> ();
        bool _Use;
        int _Sel;

        //读取启用后处理
        _Use = 通用计算.读取判断(PlayerPrefs.GetInt ("画质设置_后处理", 1));
        On后处理使用(_Use);
        m_后处理开关.onValueChanged.AddListener (
            delegate {
                On后处理使用(m_后处理开关.isOn);
            }
        );

        #region 抗锯齿部分
        //抗锯齿使用
        m_抗锯齿类型.options.Clear ();
        m_抗锯齿类型.AddOptions (通用计算.下拉列表输出(AntialiasingMode.GetNames (typeof (AntialiasingMode))));
        //添加事件
        m_抗锯齿类型.onValueChanged.AddListener (
            delegate {
                On后处理_抗锯齿(m_抗锯齿类型.value);
            }
        );
        _Sel = PlayerPrefs.GetInt ("画质设置_后处理_抗锯齿", 1);
        On后处理_抗锯齿(_Sel);
        #endregion

        #region SMAA抗锯齿
        //SMAA抗锯齿质量
        m_抗锯齿_SMAA质量.options.Clear ();
        m_抗锯齿_SMAA质量.AddOptions (通用计算.下拉列表输出(AntialiasingMode.GetNames (typeof (AntialiasingQuality))));
        m_抗锯齿_SMAA质量.onValueChanged.AddListener (
            delegate {
                On后处理_抗锯齿_SMAA质量(m_抗锯齿_SMAA质量.value);
            }
        );
        _Sel = PlayerPrefs.GetInt ("画质设置_后处理_抗锯齿_SMAA质量", 1);
        On后处理_抗锯齿_SMAA质量(_Sel);
        #endregion

        #region 后处理子项目
        //读取设置
        foreach (var item_set in m_后处理设置.components) {
            foreach (var item_obj in m_开关列表) {
                if (item_set.name == item_obj.name) {
                    _Use = 通用计算.读取判断(PlayerPrefs.GetInt ("画质设置_后处理" + item_set.name, 1));
                    item_obj.isOn = _Use;
                    item_set.active = _Use;
                    break;
                }
            }
        }

        //赋予修改设置的能力
        foreach (var item in m_开关列表) {
            item.onValueChanged.AddListener (
                delegate {
                    On设置_启用(item.isOn, item.name);
                }
            );
        }
        #endregion

    }
    public GameObject m_后处理子页面;
    public void On后处理使用(bool _输入) {
        m_镜头设置.renderPostProcessing = _输入;
        m_后处理开关.isOn = _输入;
        m_后处理子页面.SetActive (_输入);
        PlayerPrefs.SetInt ("画质设置_后处理", 通用计算.输入判断(_输入));
    }

    public void On后处理_抗锯齿(int _输入) {
        m_抗锯齿类型.value = _输入;
        m_镜头设置.antialiasing = (AntialiasingMode) _输入;
        m_抗锯齿_SMAA质量.gameObject.SetActive(m_镜头设置.antialiasing == AntialiasingMode.SubpixelMorphologicalAntiAliasing);
        PlayerPrefs.SetInt ("画质设置_后处理_抗锯齿", _输入);
    }

    public void On后处理_抗锯齿_SMAA质量(int _输入) {
        m_抗锯齿_SMAA质量.value = _输入;
        Debug.Log(_输入);
        m_镜头设置.antialiasingQuality = (AntialiasingQuality)_输入;
        PlayerPrefs.SetInt ("画质设置_后处理_抗锯齿_SMAA质量", _输入);
    }

    void On设置_启用(bool _输入, string _目标名称) {
        foreach (var item in m_后处理设置.components) {
            if (item.name == _目标名称) {
                item.active = _输入;
                PlayerPrefs.SetInt ("画质设置_后处理" + _目标名称, 通用计算.输入判断(_输入));
                break;
            }
        }
    }
    void Update () {

    }
}