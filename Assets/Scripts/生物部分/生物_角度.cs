using UnityEngine;
using UnityEngine.InputSystem;
public class 生物_角度: MonoBehaviour {
    public Vector2 m_射击输入 { get; set; }
    生物_控制中枢 m_中枢;
    [HideInInspector]
    public Transform m_方向参考器;
    public Vector3 _角度;
    private void Update() {
        _角度 = m_世界角度.eulerAngles;
    }
    private void Awake () {
        m_中枢 = GetComponent<生物_控制中枢> ();
        m_方向参考器 = new GameObject ().transform;
        m_方向参考器.name = "方向参考器";
        m_方向参考器.SetParent (transform);
    }
    public void On射击角度(Vector2 _输入) {
        m_射击输入 = _输入;
    }
    public Quaternion m_射击角度 {
        set{}
        get {
            Quaternion 角度缓存 = new Quaternion ();
            if (m_射击输入 != Vector2.zero) {
                角度缓存 = Quaternion.LookRotation (new Vector3 (m_射击输入.x, 0, m_射击输入.y));
            }

            角度缓存 = Quaternion.Euler (m_世界角度.eulerAngles + 角度缓存.eulerAngles);
            return 角度缓存;
        }
    }
    public Quaternion m_移动角度 {
        set{}
        get {
            Quaternion 角度缓存 = new Quaternion ();
            if (m_中枢.m_移动.m_移动输入 != Vector2.zero) {
                角度缓存 = Quaternion.LookRotation (new Vector3 (m_中枢.m_移动.m_移动输入.x, 0, m_中枢.m_移动.m_移动输入.y));
                角度缓存 = Quaternion.Euler (m_世界角度.eulerAngles + 角度缓存.eulerAngles);
            } else {
                角度缓存 = this.transform.rotation;
            }
            //          角度缓存 = Quaternion.Euler (m_世界角度.eulerAngles + 角度缓存.eulerAngles);
            return 角度缓存;
        }
    }

    
    
    public bool is参考相机 = true;
    public Vector3 _参考坐标;
    public Quaternion m_世界角度 {
        get {
            Quaternion 角度缓存 = new Quaternion ();
            if (!is参考相机) {
                角度缓存 = Quaternion.LookRotation (this.transform.position - _参考坐标);
            } else {
                角度缓存 = Quaternion.LookRotation (this.transform.position - Camera.main.transform.position);
            }
            角度缓存 = Quaternion.Euler (0, 角度缓存.eulerAngles.y, 0);
            return 角度缓存;
        }
        set {

        }
    }
}