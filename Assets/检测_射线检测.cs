using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 检测_射线检测 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
          RaycastHit hit = new RaycastHit ();

            if (Physics.Raycast (transform.position + new Vector3(0,0,0.14f), transform.TransformDirection (Vector3.down), out hit, Mathf.Infinity, 3)) {
                Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.down) * hit.distance, Color.yellow);
                // m_脚底光标.position = hit.point;
            } else {
                //  m_脚底光标.position = this.transform.position;
            }

            //            if (hit.collider.tag == "地面") {

            //          }

          
    }
}
