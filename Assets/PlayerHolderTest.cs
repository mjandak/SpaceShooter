using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolderTest : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;

    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _rb.AddForce(Vector2.up * 1f, ForceMode2D.Force);
       // _rb.MovePosition(Vector2.up * Speed * Time.deltaTime);
    }
}
