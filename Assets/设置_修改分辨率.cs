using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class 设置_修改分辨率 : MonoBehaviour
{
    public Text m_Text;
    Vector2  获取硬件分辨率 ;
    private void Awake() {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        float _倍率 =  (float)Screen.width/(float)Screen.height;
         int 宽度 = (int)(720f * _倍率);
         Debug.Log(_倍率);
         Screen.SetResolution (  宽度 , 720 , true);
        
        获取硬件分辨率 = new Vector2 (Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
        m_Text.text =  "分辨率*"+Screen.width.ToString() ;
    }
}
