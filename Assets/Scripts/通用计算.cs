using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class 通用计算 {
    public static int 输入判断(bool _输入) {
        if (_输入) {
            return 1;
        } else {
            return 0;
        }
    }
    public static bool 读取判断(int _输入) {
        if (_输入 > 0) {
            return true;
        } else {
            return false;
        }
    }

    public static bool 存档_Getbool(string _输入名称)
    {
        bool _Use = 读取判断( PlayerPrefs.GetInt(_输入名称,1));
        return _Use;
    }
      public static void 存档_Setbool(string _输入名称,bool _设置)
    {
         PlayerPrefs.SetInt (_输入名称,输入判断(_设置));
    }

        public static List<Dropdown.OptionData> 下拉列表输出(string[] _输入) {
        List<Dropdown.OptionData> _Out = new List<Dropdown.OptionData> ();
        foreach (var item in _输入) {
            Dropdown.OptionData _O = new Dropdown.OptionData ();
            _O.text = item;
            _Out.Add (_O);
        }
        return _Out;
    }
}
