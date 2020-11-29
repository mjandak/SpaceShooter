using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBackground : MonoBehaviour
{
    public float ScrollSpeed;
    public float ScrollSpeed2;
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = material.mainTextureOffset;
        offset.y += ScrollSpeed * Time.deltaTime;
        material.mainTextureOffset = offset;

        Vector2 secondTextureOffset = material.GetTextureOffset("_SecondTex") + new Vector2(0, ScrollSpeed2 * Time.deltaTime);
        material.SetTextureOffset("_SecondTex", secondTextureOffset);
    }
}
