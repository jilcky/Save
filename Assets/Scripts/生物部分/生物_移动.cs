using UnityEngine;
using UnityEngine.InputSystem;

public class 生物_移动: MonoBehaviour {
    生物_控制中枢 m_中枢;
    private void Awake () {
        m_中枢 = GetComponent<生物_控制中枢> ();
        if (m_脚底光标 == null) {
            m_脚底光标 = this.transform.Find ("脚底光标").transform;

        }
        On移动(Vector2.zero);
    }
    public float m_移动速度;
    public bool Can移动;
    public Vector2 m_移动输入 {
        get;
        set;
    }

    public float m_跳跃高度;
    public void On跳跃() { }
    public bool is地面;

    public bool m_防止掉落 {
        get {
            RaycastHit hit = new RaycastHit ();
            var _sp = m_中枢.m_角度.m_方向参考器.transform.forward * m_移动速度 * 0.168f;
            Debug.DrawRay (transform.position + _sp + new Vector3 (0, 1, 0), transform.TransformDirection (Vector3.down) * hit.distance, Color.yellow);
            if (Physics.Raycast (transform.position + _sp + new Vector3 (0, 1, 0), transform.TransformDirection (Vector3.down), out hit, Mathf.Infinity, 3)) {
                return true;
            } else {
                return false;
            }
        }
    }

    Transform m_脚底光标 = null;

    [HideInInspector]
    public float m_地面最小距离;

    public void On移动输入(InputAction.CallbackContext _输入)
    {
        if (_输入.performed)
        {
            On移动(_输入.ReadValue<Vector2>());
        }
    }
    public void On移动(Vector2 输入) {
        
        m_移动输入 = 输入;
        m_中枢.m_角度.m_方向参考器.rotation = m_中枢.m_角度.m_移动角度;

        if (Can移动) {
            if (m_防止掉落) {
                m_中枢.m_CharacterController.SimpleMove (m_中枢.m_角度.m_方向参考器.transform.forward * m_移动速度 * Vector2.Distance (Vector2.zero, m_移动输入));
            }
            if (m_移动输入 != Vector2.zero) {
                this.transform.rotation = m_中枢.m_角度.m_移动角度;
            }
            m_中枢.m_Animator.SetFloat ("移动", Mathf.Clamp (Vector2.Distance (Vector2.zero, m_移动输入) * m_移动速度, 0, m_移动速度));
        } else {
            m_中枢.m_Animator.SetFloat ("移动", 0);
        }
    }

}