using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Collider2D _collider;
    public float Force;
    private Vector2  _direction;
    private float _rotationSpeed = 10f;
    [SerializeField] private Effector2D _effector;
    [SerializeField] private Collider2D _effectorCollider;
    [SerializeField] private GameObject _explosionForce;
    [SerializeField] private GameObject _explosion;


    // Start is called before the first frame update
    void Start()
    {
        _direction = Vector2.down;
        _rb.AddTorque(0.025f, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _rb.AddForce(Quaternion.Euler(0, 0, Mathf.Sin(Time.fixedTime) * 10f) * _direction * Force);
        //_rb.MoveRotation(_rb.rotation + _rotationSpeed * Time.fixedDeltaTime);
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //_effectorCollider.enabled = _effector.enabled = true;
        //Explode();
        var explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        var force = Instantiate(_explosionForce, transform.position, Quaternion.identity);
        Destroy(force, 0.5f);
        Destroy(explosion, 2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    //private void Explode()
    //{
    //    //_rb.mass = 0f;
    //    //Force = 0f;
    //    _collider.enabled = false;
    //    _sr.enabled = false;
    //    _explosionParticleSystem.Play();
    //    _explosionForce.SetActive(true);
    //    gameObject.SetActive(false);
    //    Destroy(gameObject, 3f);
    //}
}
