using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIPositionSwitcher : MonoBehaviour
{
    public List<Vector2> rects = new List<Vector2>();
    private RectTransform rectT = null;

    private void Awake()
    {
        rectT = gameObject.GetComponent<RectTransform>();
        SetRectId(0);
    }

    public void SetRectId(int id)
    {
        if(id >= 0 && id < rects.Count)
        {
            Vector2 newPos = rects[id];

            //Vector3 oldPos = rectT.localPosition;
            //rectT.localPosition += (newPos-oldPos);

            //Vector2 oldPos = rectT.anchoredPosition;
            //rectT.anchoredPosition += (newPos - oldPos);
            rectT.anchoredPosition = newPos;
        }
    }
    /*
    public bool test = false;
    public int setId = 0;

    private void Update()
    {
        if(test == true)
        {
            test = false;
            SetRectId(setId);
        }
    }

    */
}
