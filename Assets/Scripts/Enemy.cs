using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    [SerializeField] private Transform _rayCastPointL;
    [SerializeField] private Transform _rayCastPointR;
    [SerializeField] private Transform _rayCastPointM;
    private Vector2 _movementDir;
    [SerializeField] private float _collisionAvoidDetectionReach;

    [Header("Params")]
    [SerializeField]
    private ushort _hitPoints;
    private GameObject _player;
    public float Force;
    [Tooltip("Shooting rate in seconds.")]
    public float ShootRate;
    public Vector2 Dir { get => _movementDir; }

    /// <summary>
    /// Direction enemy is facing, normalized.
    /// </summary>
    public Vector2 Front
    {
        get { return -transform.up; }
        set
        {
            transform.Rotate(Vector3.forward, Vector2.SignedAngle(Front, value));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Invoke(nameof(changeVelocity), 2f);
        _player = GameObject.FindGameObjectWithTag("Player");
        _movementDir = Front;
        StartCoroutine(nameof(Shoot));
        StartCoroutine(PreventCollision());
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
        _rb.AddForce(_movementDir * Force, ForceMode2D.Force);
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
        Vector3 directionToPlayer = _player.transform.position - transform.position;

        //if (Vector3.Angle(Front, directionToPlayer) < 10)
        //{
        //    //RaycastHit2D hit = Physics2D.Raycast(transform.position, Front, directionToPlayer.magnitude, LayerMask.GetMask("Enemy"));
        //    //return !hit;
        //    //if (hit)
        //    //{
        //    //    return false;
        //    //}
        //    //else
        //    //{
        //    //    return true;
        //    //}

        //    RaycastHit2D hit = Physics2D.Raycast(_rayCastPoint.position, Front, directionToPlayer.magnitude, LayerMask.GetMask("Enemy"));
        //    //Debug.DrawRay(_rayCastPoint.position, directionToPlayer, Color.red);
        //    Debug.DrawLine(_rayCastPoint.position, _rayCastPoint.position + (Vector3)Front * directionToPlayer.magnitude, Color.red);
        //    return !hit;
        //}
        //return false;

        if (Vector3.Angle(Front, directionToPlayer) < 10)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Front, directionToPlayer.magnitude, LayerMask.GetMask("Enemy"));
            foreach (RaycastHit2D h in hit)
            {
                if (h.transform != transform)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
        //var directionToPlayer = _player.transform.position - transform.position;
        //return Vector3.Angle(Front, directionToPlayer) < 10;
    }

    private RaycastHit2D isEnemyInSight()
    {
        float distance = _collisionAvoidDetectionReach;
        RaycastHit2D closestHit = default;
        RaycastHit2D hit = raycast(_rayCastPointL.position, Quaternion.Euler(0, 0, 30) * Front, distance, LayerMask.GetMask("Enemy"), true);
        closestHit = getCloserHit(hit, closestHit);
        hit = raycast(_rayCastPointL.position, Front, distance, LayerMask.GetMask("Enemy"), true);
        closestHit = getCloserHit(hit, closestHit);
        hit = raycast(_rayCastPointM.position, Front, distance, LayerMask.GetMask("Enemy"), true);
        closestHit = getCloserHit(hit, closestHit);
        hit = raycast(_rayCastPointR.position, Front, distance, LayerMask.GetMask("Enemy"), true);
        closestHit = getCloserHit(hit, closestHit);
        hit = hit = raycast(_rayCastPointR.position, Quaternion.Euler(0, 0, -30) * Front, distance, LayerMask.GetMask("Enemy"), true);
        closestHit = getCloserHit(hit, closestHit);

        //if (closestHit)
        //{
        //    return closestHit.transform.gameObject.GetComponent<Enemy>();
        //}
        //return null;
        return closestHit;
    }

    private RaycastHit2D getCloserHit(RaycastHit2D @new, RaycastHit2D old)
    {
        if (@new)
        {
            if (!old)
            {
                return @new;
            }
            else
            {
                if (@new.distance < old.distance)
                {
                    return @new;
                }
            }
        }
        return old;
    }

    private RaycastHit2D raycast(Vector3 start, Vector3 dir, float dist, LayerMask mask, bool debugLine)
    {
        RaycastHit2D result = Physics2D.Raycast(start, dir, dist, mask);
        if (!debugLine) return result;
        Debug.DrawLine(start, start + dir.normalized * dist, Color.yellow);
        return result;
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

    private IEnumerator PreventCollision()
    {
        while (true)
        {
            RaycastHit2D h = isEnemyInSight();
            if (h)
            {
                Enemy e = h.transform.GetComponent<Enemy>();
                if (e == null) goto NextCycle;
                if (_enemiesInSight.ContainsKey(e))
                {
                    if (_enemiesInSight[e] > h.distance)
                    {
                        //enemy is getting closer
                        float coeff = h.distance / _collisionAvoidDetectionReach;
                        if (Vector3.Cross(Front, e.Dir).z < 0)
                        {
                            //enemy goes from left to right
                            _movementDir = Quaternion.Euler(0, 0, 60 - 30 * coeff) * Front;
                        }
                        else
                        {
                            //enemy goes from right to left 
                            _movementDir = Quaternion.Euler(0, 0, -60 + 30 * coeff) * Front;
                        }
                    }
                    else
                    {
                        //enemy is not getting closer
                        _movementDir = Front;
                    }
                }
                else
                {
                    _enemiesInSight[e] = h.distance;
                }
            }
            else
            {
                _movementDir = Front;
            }

        NextCycle:
            yield return new WaitForSeconds(0.25f);
        }
    }

    private Dictionary<Enemy, float> _enemiesInSight = new Dictionary<Enemy, float>(10);
}
