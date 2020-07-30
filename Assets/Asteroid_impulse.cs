using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid_impulse : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float Impulse;

    public Vector2 Front
    {
        get { return -transform.up; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(Front * Impulse, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
