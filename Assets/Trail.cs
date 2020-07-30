using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public TrailRenderer TrailRenderer;
    public float X;
    public float Y;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TrailRenderer.material.SetTextureScale("_MainTex", new Vector2(X, Y));
        transform.Translate(new Vector2(0, 1) * Time.deltaTime, Space.World);
    }
}
