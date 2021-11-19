using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_EagleParent : MonoBehaviour
{
    private GameObject child;
    
    // Start is called before the first frame update
    void Start()
    {
        child = transform.Find("EagleGFX").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        DestroyWithChild();
    }

    //µ±GFXÀ¿Õˆ ±∏˙◊≈À¿Õˆ
    private void DestroyWithChild()
    {
        if(!child)
        {
            Destroy(gameObject);
        }
    }
}
