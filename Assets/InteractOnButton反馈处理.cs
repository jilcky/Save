using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit3D;
using UnityEngine.InputSystem;
public class InteractOnButton反馈处理 : MonoBehaviour
{public InteractOnButton m_按钮目标;
    public void On启动(InteractOnButton _目标) {
        m_按钮目标 = _目标;
    }
    public void On发射信息() {
        if (m_按钮目标 != null) {

            m_按钮目标.On执行();
        }
    }
    public void On按钮输入(InputAction.CallbackContext _输入)
    {
        if (_输入.phase == InputActionPhase.Performed)
        {
            On发射信息();
        }
    }
    public void On关闭() {
        m_按钮目标 = null;
    }


}
