using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Collider2D e1;
    public Collider2D e2;
    public float collidesDist;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        collidesDist = e1.Distance(e2).distance;
        //Debug.Log($"collider dist: {e1.Distance(e2).isValid}");
    }
}
