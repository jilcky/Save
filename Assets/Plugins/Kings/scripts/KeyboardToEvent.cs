using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script to arbitrary convert KeyUp and KeyDown to events.
/// </summary>

public class KeyboardToEvent : MonoBehaviour
{
    [System.Serializable] public class mEvent : UnityEvent { }


    [System.Serializable]
    public class C_KeyEvent
    {
        public KeyCode key;
        public mEvent keyDown;
        public mEvent keyUp;
    }

    public C_KeyEvent[] keyEvents;

    void Update()
    {
        foreach (C_KeyEvent keyEvent in keyEvents) {
            if (Input.GetKeyDown(keyEvent.key))
            {
                keyEvent.keyDown.Invoke();
            }
            if (Input.GetKeyUp(keyEvent.key))
            {
                keyEvent.keyUp.Invoke();
            }
        }

    }
}
