using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private GameObject _particleFXPrefab;

    public float Impulse;

    public Vector2 Front
    {
        get => transform.up;
        set
        {
            transform.Rotate(Vector3.forward, Vector2.SignedAngle(Front, value));

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(Front * Impulse, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (Area.IsNotInside(transform.position)) Destroy(gameObject);
    }

    //private void FixedUpdate()
    //{
    //    _rb.AddForce(Vector2.down * Consts.idle, ForceMode2D.Force);
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 point = collision.GetContact(0).point;
        GameObject particles = Instantiate(_particleFXPrefab, point, Quaternion.identity);
        //particles.transform.parent = collision.transform;
        if (collision.gameObject.CompareTag("Player"))
        {
            //particles.GetComponent<HitEffect>().Front = (Vector2.up + collision.gameObject.GetComponent<Rigidbody2D>().velocity.normalized).normalized;
            particles.GetComponent<HitEffect>().Front = Vector2.up;
        }
        else
        {
            particles.GetComponent<HitEffect>().Front = collision.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
        }
        particles.transform.SetParent(collision.transform, true);
        particles.transform.localScale = Vector3.one;
        Destroy(gameObject);
        //ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        //Destroy(particles, ps.main.duration + ps.main.startLifetime.constant + 1f);
        Destroy(particles, particles.GetComponent<HitEffect>().DestroyTime);
    }
}
