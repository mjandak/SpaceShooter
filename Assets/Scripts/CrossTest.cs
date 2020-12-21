using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossTest : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public float Cross;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cross = Vector3.Cross(A.up, B.up).z;
    }
}
