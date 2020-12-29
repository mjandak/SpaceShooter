using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 2;
    private Vector3 dragOrigin;
    public Rigidbody2D _rb;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 step = (Input.mousePosition - dragOrigin);
            //_rb.AddForce(-1 * dir  * 0.8f, ForceMode2D.Force);

            var nextCameraPosition = transform.position - step * 0.02f;

            float x = step.x * 0.02f;
            float y = step.y * 0.02f;

            if (nextCameraPosition.x < 0 || nextCameraPosition.x > 3)
            {
                x = 0;
            }

            if (nextCameraPosition.y < 0 || nextCameraPosition.y > 10)
            {
                y = 0;
            }

            transform.position -= new Vector3(x, y, 0);

            dragOrigin = Input.mousePosition;
        }

    }
}
