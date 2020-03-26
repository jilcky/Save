using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace 编辑 {
    public class UI_面向相机: MonoBehaviour {
        public Vector3 m_偏移;
        public float m_偏移距离 = 1f;
        Transform m_目标;
        Transform m_相机;
        void Start () {
            m_目标 = this.transform;
            m_相机 = Camera.main.transform;
            //_Camera = Camera.main;
        }

        // Update is called once per frame
        void Update () {

            var _Q = Quaternion.LookRotation (m_相机.transform.forward * m_偏移距离, m_目标.transform.forward).eulerAngles;

            this.transform.rotation = Quaternion.Euler (m_偏移 + new Vector3 (0, _Q.y, 0));
        }
        //进入平面内

    }
}