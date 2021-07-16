using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArDescription : MonoBehaviour
{
    string arName;
    string btnName;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("in touch");
    }

    // Update is called once per frame
    void Update()
    {




        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
           
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Debug.DrawRay(ray.origin,ray.direction* 10f, Color.red);
            RaycastHit Hit;
            
            Debug.Log("hit " + Physics.Raycast(ray, out Hit));

            if (Physics.Raycast(ray, out Hit))
            {
                //Debug.Log("touch count " + Input.touchCount);
                btnName = Hit.transform.name;
                Debug.Log("clicked AR Model " + btnName);
            }
            //    for (int i = 0; i + touchCorrection < Input.touchCount; ++i)
            //{

            //    Debug.Log("touch");
            //    if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
            //    {
            //        // Construct a ray from the current touch coordinates
            //        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
            //        if (Physics.Raycast(ray, out hit))
            //        {

            //            //hit.transform.gameObject.SendMessage("OnMouseDown");
            //        }
            //    }
            //}

        }
    }
}

