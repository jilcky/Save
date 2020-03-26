using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class 箭头: MonoBehaviour {

    // Start is called before the first frame update
    public GameObject mFrom;
    public GameObject mTo;
    public Color mColor;

    public Vector3 _中间距离;
    public Transform _箭头;
    public float 角度;
    //1.计算出两边的中心点
    void Start () {

    }
    private void OnDrawGizmos () {
        // _中间距离 = mFrom.transform.position - (mFrom.transform.position - mTo.transform.position) / 2;
        // _箭头.position = _中间距离;
       
        // Vector3 direction =  mTo.transform.position - mFrom.transform.position ;
        // float angle =360-Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        //     // 将当前物体的角度设置为对应角度
        // _箭头.eulerAngles = new Vector3(0, 0, angle);

        // float 长度 =  Vector3.Distance(mFrom.transform.position,mTo.transform.position);
        
        // _箭头.GetComponent<SpriteRenderer>().size = new Vector2(1,长度);

    }
    private void Update () {

    }

}