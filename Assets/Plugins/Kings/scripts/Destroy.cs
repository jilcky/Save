using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Helper script to allow for object destruction of an prefab from a button.
//This is usefull for spawned popups where a close button just destroys the pupup.
public class Destroy : MonoBehaviour
{
    public GameObject go;               //definition what to destroy
    /// <summary>
    /// Destroy the defined gameObject.
    /// </summary>
    public void DestroyGameobject()
    {
        Destroy(go);
    }
}
