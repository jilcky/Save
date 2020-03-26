using System;
using System.Collections;
using Gamekit3D;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    public static PlayerInput Instance {
        get { return s_Instance; }
    }

    protected static PlayerInput s_Instance;

    [HideInInspector]
    public bool playerControllerInputBlocked;

    protected Vector2 m_Movement;
    protected Vector2 m_Camera;
    protected bool m_Jump;
    protected bool m_Attack;
    protected bool m_Pause;
    protected bool m_ExternalInputBlocked;

    public Vector2 MoveInput {
        get {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector2 CameraInput {
        get {
            if (playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Camera;
        }
    }

    public bool JumpInput {
        get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Attack {
        get { return m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Pause {
        get { return m_Pause; }
    }

    WaitForSeconds m_AttackInputWait;
    Coroutine m_AttackWaitCoroutine;

    const float k_AttackInputDuration = 0.03f;

    public bool HaveControl () {
        return !m_ExternalInputBlocked;
    }
    PlayerController _控制器;
    void Awake () {
        _控制器 = GetComponent<PlayerController> ();
        m_AttackInputWait = new WaitForSeconds (k_AttackInputDuration);

        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException ("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
    }
    #region 指令输入

    IEnumerator AttackWait () {
        m_Attack = true;

        yield return m_AttackInputWait;

        m_Attack = false;
    }
    public void ReleaseControl () {
        m_ExternalInputBlocked = true;
    }

    public void GainControl () {
        m_ExternalInputBlocked = false;
    }
    public void On移动(InputAction.CallbackContext _输入) {
        Vector2 _V2 = _输入.ReadValue<Vector2> ();
        m_Movement.Set (_V2.x, _V2.y);
    }

    public void On攻击(InputAction.CallbackContext _输入) {
        if (_输入.phase == InputActionPhase.Performed) {

            if (m_AttackWaitCoroutine != null)
                StopCoroutine (m_AttackWaitCoroutine);

            m_AttackWaitCoroutine = StartCoroutine (AttackWait ());
        }
    }

    public void On跳跃(InputAction.CallbackContext _输入) {
        if (_输入.phase == InputActionPhase.Performed) {
            _控制器.On跳跃();
        }
    }
    #endregion

    public 前进角度 m_前进角度;
    public enum 前进角度 {
        镜头方向,
        固定方向,
        目标方向
    }
    public float m_前进方向_固定;

    public float m_前进方向_镜头
    {
        get {
            return Quaternion.LookRotation (this.transform.position - Camera.main.transform.position).eulerAngles.y;
        }
    }

    public float m_前进方向_目标 {
        get {
            return Quaternion.LookRotation (this.transform.position - m_前进方向_目标_物体.position).eulerAngles.y;
        }
    }
    public Transform m_前进方向_目标_物体;
    public void On切换_前进方向_目标(Transform _输入) {
        m_前进方向_目标_物体 = _输入;
    }

    public float m_前进方向() {
        float _角度 = 0;
        switch (m_前进角度) {
            case 前进角度.固定方向:
                _角度 = m_前进方向_固定;
                break;
            case 前进角度.目标方向:
                _角度 = m_前进方向_目标;
                break;
            case 前进角度.镜头方向:
                _角度 = m_前进方向_镜头;
                break;
        }
        return _角度;
    }
    public void On切换_前进模式_int(int _输入) {
        m_前进角度 = (前进角度) _输入;
    }
    public void On切换_前进模式(前进角度 _输入)
    {
        m_前进角度 = _输入;
    }

}

