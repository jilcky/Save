using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Serialization;

public class 获取_触屏输入: MonoBehaviour, IDragHandler,IEndDragHandler {
    // Start is called before the first frame update
    public CinemachineVirtualCamera m_虚拟镜头;
    public CinemachinePOV m_信息;
    public float 敏感度;

    void Start () {
        m_信息 = m_虚拟镜头.GetCinemachineComponent<CinemachinePOV>();
    }

    public void OnDrag (PointerEventData eventData) {
        Debug.Log (eventData.delta);
        m_信息.m_HorizontalAxis.m_InputAxisValue = eventData.delta.x*敏感度;
        m_信息.m_VerticalAxis.m_InputAxisValue = eventData.delta.y*敏感度;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_信息.m_HorizontalAxis.m_InputAxisValue = 0;
         m_信息.m_VerticalAxis.m_InputAxisValue = 0;
        
    }
    // Update is called once per frame
    void Update () {

    }
}