using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IFront
{
    private float _downThrust = Consts.idle;

    [Header("Components")]
    [SerializeField]
    private SpriteRenderer _sr;
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private Collider2D _collider;
    [SerializeField]
    private ParticleSystem _explosionParticleSystem;
    [SerializeField]
    private Transform _laser1;
    [SerializeField]
    private Transform _laser2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject[] _engines;
    [SerializeField]
    private GameObject[] _parts;

    [Header("Params")]
    [SerializeField]
    private ushort _hitPoints;
    private GameObject _player;
    public float Force;
    [Tooltip("Shooting rate in seconds.")]
    public float ShootRate;


    public Vector2 Front
    {
        get { return -transform.up; }
        set { transform.up = -value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Invoke(nameof(changeVelocity), 2f);
        _player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(nameof(Shoot));
    }

    private void changeVelocity()
    {
        //_velocity = new Vector2(1, -1).normalized * Speed / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //if (IsPlayerInSight())
        //{
        //    Shoot();
        //}
        if (Area.IsNotInside(transform.position)) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        //_rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
        _rb.AddForce(Vector2.down * _downThrust, ForceMode2D.Force);
        _rb.AddForce(Front * Force, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"relative velocity: {collision.relativeVelocity.magnitude}");
        if (collision.relativeVelocity.magnitude < 2) return;
        if (_hitPoints == 0) return;
        checked
        {
            _hitPoints--;
        }
        if (_hitPoints < 1)
        {
            Explode();
        }
    }

    private void OnBecameInvisible()
    {
        //Destroy(gameObject);
    }

    private void Explode()
    {
        _rb.mass = _rb.mass * 0.1f;
        Force = 0f;
        _collider.enabled = false;
        _sr.enabled = false;
        StopCoroutine(nameof(Shoot));
        _explosionParticleSystem.Play();

        foreach (GameObject e in _engines)
        {
            e.SetActive(false);
        }

        foreach (GameObject part in _parts)
        {
            part.SetActive(true);
        }

        Destroy(gameObject, 3f);
    }

    private bool IsPlayerInSight()
    {
        var directionToPlayer = _player.transform.position - transform.position;
        return Vector3.Angle(Front, directionToPlayer) < 10;
    }

    private IEnumerator Shoot()
    {
        while (true)
        {
            //if (!_isVisible) return;
            if (!Player.IsDead && IsPlayerInSight())
            {
                GameObject l1 = Instantiate(_laserPrefab, _laser1.position, Quaternion.identity);
                //GameObject l1 = _laserPool.Get();
                l1.transform.position = _laser1.position;
                l1.GetComponent<Missile>().Front = Front;
                l1.SetActive(true);
                Physics2D.IgnoreCollision(l1.GetComponent<Collider2D>(), _collider);
                //l1.GetComponentInChildren<Laser>().Source = "Enemy";

                if (_laser2 != null)
                {
                    GameObject l2 = Instantiate(_laserPrefab, _laser2.position, Quaternion.identity);
                    //GameObject l2 = _laserPool.Get();
                    l2.transform.position = _laser2.position;
                    l2.GetComponent<Missile>().Front = Front;
                    l2.SetActive(true);
                    Physics2D.IgnoreCollision(l2.GetComponent<Collider2D>(), _collider);
                    //l2.GetComponentInChildren<Laser>().Source = "Enemy";
                }

                //_laserFire = Time.time + laserRate;
                yield return new WaitForSeconds(ShootRate);
            }
            else
            {
                //returning 0 will make it wait 1 frame
                yield return 0;
            }
        }
    }
}
