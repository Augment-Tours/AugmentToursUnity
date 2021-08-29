using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject saber;
    void Start()
    {
        saber = GameObject.Find("Seber-Tig");
        saber.AddComponent<Lean.Touch.LeanSelectableByFinger>();
        saber.AddComponent<Lean.Touch.LeanDragTranslate>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
