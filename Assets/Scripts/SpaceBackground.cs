using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBackground : MonoBehaviour
{
    public float ScrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Material material = GetComponent<MeshRenderer>().material;
        Vector2 offset = material.mainTextureOffset;
        offset.y += ScrollSpeed * Time.deltaTime;
        material.mainTextureOffset = offset;
    }
}
