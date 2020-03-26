using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class OrientationHandler : MonoBehaviour
{
    [System.Serializable] public class mEvent : UnityEvent { }

    [System.Serializable]
    public class C_TransformModifications
    {
        C_TransformModifications()
        {
            scale = new Vector3(1f, 1f, 1f);
        }

        public Vector2 position;
        public Vector3 scale;
    }

    [System.Serializable]
    public class C_OrientationSelection
    {
        public RectTransform target;
        public C_TransformModifications landscape;
        public C_TransformModifications portrait;
    }

    [System.Serializable]
    public enum E_DetectionType
    {
        deviceOrientation,
        byResolution
    }

    public E_DetectionType detectionType;

    //internal memory for which mode was last set
    enum E_Orientation
    {
        none, /*initial unknown*/
        landscape,
        portrait
    }
    private E_Orientation lastSetOrientation = E_Orientation.none;

    public List<C_OrientationSelection> transformModifications = new List<C_OrientationSelection>();

    DeviceOrientation lastOrientation = 0;
    Resolution lastResolution = new Resolution();

    //events
    public mEvent OnOrientationChange;
    public mEvent OnSwitchToLandscape;
    public mEvent OnSwitchToPortrait;

    [ReadOnlyInspector]public Vector2Int res;
    //[ReadOnlyInspector] public DeviceOrientation orientation;

    private void Start()
    {
        checkOrientation();
    }
    void Update()
    {
        checkOrientation();

        res.x = Screen.width;
        res.y = Screen.height;
        //orientation = Input.deviceOrientation;
    }

    void checkOrientation()
    {
        if(hasOrientationChanged() == false)
        {
            return;
        }

        switch (detectionType)
        {
            case E_DetectionType.deviceOrientation:
                switch (Input.deviceOrientation)
                {

                    case DeviceOrientation.LandscapeLeft:
                    case DeviceOrientation.LandscapeRight:
                        //goto landscape mode
                        switchToLandscape();
                        break;
                    case DeviceOrientation.Portrait:
                    case DeviceOrientation.PortraitUpsideDown:
                        //goto portrait mode
                        switchToPortrait();
                        break;
                    case DeviceOrientation.FaceDown:
                    case DeviceOrientation.FaceUp:
                    case DeviceOrientation.Unknown:
                        //do nothing
                        break;
                }
                break;
            case E_DetectionType.byResolution:
                if(Screen.height > Screen.width)
                {
                    switchToPortrait();
                }
                else
                {
                    //applies also for height == width
                    switchToLandscape();
                }
                break;
        }
    }

    bool hasOrientationChanged()
    {
        switch (detectionType)
        {
            case E_DetectionType.deviceOrientation:

                if (lastOrientation != Input.deviceOrientation)
                {
                    return false;
                }
                else
                {
                    lastOrientation = Input.deviceOrientation;
                    return true;
                }
                //break;
            case E_DetectionType.byResolution:
                if((lastResolution.height != Screen.height) || (lastResolution.width != Screen.width))
                {
                    lastResolution.height = Screen.height;
                    lastResolution.width = Screen.width;
                    return true;
                }
                else
                {
                    return false;
                }
                //break;
            default:
                return false;
                //break;
        }
    }

    void switchToLandscape()
    {
        //block double sets
        if(lastSetOrientation != E_Orientation.landscape)
        {
            foreach(C_OrientationSelection os in transformModifications)
            {
                setTransform(os.target, os.landscape);
            }
            lastSetOrientation = E_Orientation.landscape;

            OnOrientationChange.Invoke();
            OnSwitchToLandscape.Invoke();
        }
    }

    void switchToPortrait()
    {
        if (lastSetOrientation != E_Orientation.portrait)
        {
            foreach (C_OrientationSelection os in transformModifications)
            {
                setTransform(os.target, os.portrait);
            }
            lastSetOrientation = E_Orientation.portrait;

            OnOrientationChange.Invoke();
            OnSwitchToPortrait.Invoke();
        }
    }

    void setTransform(RectTransform target, C_TransformModifications mod)
    {
        if (target != null)
        {
            target.anchoredPosition = mod.position;
            target.localScale = mod.scale;
        }
        else
        {
            Debug.LogWarning("Missing transform for orientation change. OrientationHandler.cs at " + gameObject.name);
        }
    }
}
