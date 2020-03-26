using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace 编辑.数据管理 {
    public class 数据管理_钥匙管理: MonoBehaviour {

        public 信息管理_钥匙 m_钥匙;
        public string m_Key;
        
        public void On获得钥匙_导入(string _输入) {
            m_钥匙.m_钥匙[_输入] += 1;
        }
        public void On获得钥匙() {
            m_钥匙.m_钥匙[m_Key] += 1;
        }


        public int m_带入修改变量;
        public void On引入修改钥匙变量数值( int _输入)
        {
            m_带入修改变量 = _输入;
        }
        public void On修改钥匙数量(string _输入)
        {
            m_钥匙.m_钥匙[_输入] = m_带入修改变量;
        }


    }
}