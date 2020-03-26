using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 箭头绘制管理: MonoBehaviour {
    public GameObject m箭头样本;
    public List<GameObject> m_箭头列表;
    [System.Serializable]
    public class m_采样信息 {
        public GameObject mFrom;
        public GameObject mTo;
        public GameObject m箭头;
        public float mLong;
    }
    public List<m_采样信息> m_方向列表;
    public bool m_刷新;
    void Start () {

    }

    void Update () {

    }

    public float m_计时器;
    public float m_计时器倒计时;
    private void OnDrawGizmos () {
        m_计时器 += Time.deltaTime;
        if (m_计时器 > m_计时器倒计时 && m_刷新) {
            On编辑器刷新();
            m_计时器 = 0;
        }
    }

    [ContextMenu ("On编辑器刷新")]
    public void On编辑器刷新() {
        箭头标记[] _箭头列表 = GameObject.FindObjectsOfType<箭头标记> ();

        //1.清理
        foreach (var item in m_箭头列表) {
            DestroyImmediate (item);
        }
        m_箭头列表.Clear ();
        m_方向列表.Clear ();
        //2.加载

        foreach (var item in _箭头列表) {
            if (item.mTo != null) {
                m_采样信息 _样本 = new m_采样信息();
                _样本.mFrom = item.gameObject;
                _样本.mTo = item.mTo;
                _样本.mLong = Vector3.Distance (_样本.mFrom.transform.position, _样本.mTo.transform.position);
                //设置箭头
                _样本.m箭头 = Instantiate (m箭头样本);
                _样本.m箭头.transform.SetParent (this.transform);
                _样本.m箭头.transform.name = _样本.mFrom.name + " |_到_| " + _样本.mTo.name;

                //箭头位置
                _样本.m箭头.transform.position = _样本.mFrom.transform.position - (_样本.mFrom.transform.position - _样本.mTo.transform.position) / 2;

                //箭头角度
                Vector3 direction = _样本.mTo.transform.position - _样本.mFrom.transform.position;
                float angle = 360 - Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg;
                _样本.m箭头.transform.eulerAngles = new Vector3 (0, 0, angle);

                
                //箭头长度
                float _长度 = Vector3.Distance (_样本.mFrom.transform.position, _样本.mTo.transform.position);
                _长度 = Mathf.Clamp(_长度,3,_长度-12);
                _样本.m箭头.GetComponent<SpriteRenderer> ().size = new Vector2 (1, _长度);

                m_箭头列表.Add (_样本.m箭头);
                m_方向列表.Add (_样本);

            }
        }

    }
}