using UnityEngine;
using UnityEngine.InputSystem;
public class 生物_控制中枢: MonoBehaviour {

    [Header ("通用部分")]
    [HideInInspector]
    public Animator m_Animator;
    [HideInInspector]
    public CharacterController m_CharacterController;
    [HideInInspector]
    public CapsuleCollider m_CapsuleCollide;
    [HideInInspector]
    public 生物_移动 m_移动;
     [HideInInspector]
    public 生物_角度 m_角度;
      [HideInInspector]
    //public 生物_攻击 m_攻击;
    private void Awake () {
        m_Animator = GetComponentsInChildren<Animator> ()[1];
        m_CharacterController = GetComponent<CharacterController> ();
        m_CapsuleCollide = GetComponent<CapsuleCollider> ();

        m_移动 = GetComponent<生物_移动> ();
        m_角度 = GetComponent<生物_角度> ();
       // m_攻击 = GetComponent<生物_攻击> ();
    }

}