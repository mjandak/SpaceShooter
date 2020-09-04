using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyPart: MonoBehaviour, IFront
{
    //private float _downThrust = 20f;
    [Header("Components")]
    [SerializeField]
    private SpriteRenderer _sr;
    [SerializeField]
    private Collider2D _collider;
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private GameObject _explosion;

    [Header("Params")]
    public Vector2 Direction;
    public float Force;
    public float Torque;
    public ushort HitPoints;

    public Vector2 Front 
    { 
        get => -transform.up;
        set
        {
            transform.Rotate(Vector3.forward, Vector2.SignedAngle(Front, value));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //detach from parent game object
        transform.parent = null;

        _rb.AddForce(Direction.normalized * Force, ForceMode2D.Impulse);
        _rb.AddTorque(Torque, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (Area.IsNotInside(transform.position)) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < 2) return;
        if (HitPoints == 0) return;
        HitPoints--;
        if (HitPoints < 1)
        {
            HitPoints = 0;
            Explode();
        }

    }

    private void Explode()
    {
        _sr.enabled = false;
        _collider.enabled = false;
        GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject);
    }
}
