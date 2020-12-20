using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float Speed;
    public Vector3 RotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Speed = 3f;
        RotationSpeed = new Vector3(0, 0, 0.25f);
        Destroy(gameObject, 9);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * Speed * Time.deltaTime, Space.World);
        transform.Rotate(RotationSpeed);
    }
}
