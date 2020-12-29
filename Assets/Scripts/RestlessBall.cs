using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestlessBall : MonoBehaviour, IObstacle
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Transform _gunPosition;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ushort _hitPoints;
    [SerializeField] private GameObject _explosion;
    private Vector2 _initPosition;
    private GameObject _player;
    private float _minMoveDist = 4f;
    private float _maxMoveDist = 20;
    private int[] _leftRightArray = new int[] { -1, 1 };
    private Vector3 _currentDir;
    private Vector3 _currentStartPosition;
    private float _currentDistance;
    private bool _stopped;

    public float Force;

    public Vector2 Dir => _rb.velocity.normalized;

    public Transform Transform => transform;

    public Collider2D Collider => _collider;

    // Start is called before the first frame update
    void Start()
    {
        _initPosition = transform.position;
        _player = GameObject.FindGameObjectWithTag("Player");
        //StartCoroutine(moveAndShoot());
        moveAndShoot();
    }

    private void FixedUpdate()
    {
        Debug.Log($"RestlessBall: speed: {_rb.velocity.magnitude}");

        if (_stopped)
        {

        }
        else
        {
            if ((transform.position - _currentStartPosition).magnitude >= _currentDistance)
            {
                //arrived
                Debug.Log($"RestlessBall: arrived at {transform.position}");
                _rb.velocity = 0.1f * _rb.velocity;
                _stopped = true;
                shoot();
                Invoke(nameof(moveAndShoot), 1f);
            }
            else
            {
                _rb.AddForce(_currentDir * Force, ForceMode2D.Force);
            }
        }
        
        _rb.AddForce(Vector2.down * 4f, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < 2) return;
        if (collision.gameObject.CompareTag("Laser"))
        {
            DealDamage(1);
        }
        else if (collision.gameObject.CompareTag("BluePlasma"))
        {
            DealDamage(4);
        }
        else if (collision.gameObject.CompareTag("Mine"))
        {
            DealDamage(4);
        }
        else
        {
            DealDamage(1);
        }
    }

    private void moveAndShoot()
    {
        var angle = Random.Range(80f, 90f);
        //if (Area.IsNotInside(transform.position + new Vector3(1, 0, 0) * _maxMoveDist))
        //{
        //    //has to move right
        //    _currentDir = Quaternion.Euler(0, 0, -angle) * new Vector3(0, -1, 0);
        //}
        //else if (Area.IsNotInside(transform.position + new Vector3(-1, 0, 0) * _maxMoveDist))
        //{
        //    //has to move left
        //    _currentDir = Quaternion.Euler(0, 0, angle) * new Vector3(0, -1, 0);
        //}
        //else
        //{
        //    _currentDir = Quaternion.Euler(0, 0, _leftRightArray[Random.Range(0, 1)] * angle) * Vector2.down;
        //}

        _currentDir = Quaternion.Euler(0, 0, _leftRightArray[Random.Range(0, 1)] * angle) * Vector2.down;
        _currentStartPosition = transform.position;
        _currentDistance = Random.Range(_minMoveDist, _maxMoveDist);

        if (Area.IsNotInside(_currentStartPosition + _currentDir * _currentDistance))
        {
            _currentDir = new Vector3(-1 * _currentDir.x, _currentDir.y, 0);
        }

        //RaycastHit2D r = Physics2D.Raycast(_currentStartPosition, _currentDir, _currentDistance, LayerMask.GetMask("Enemy"));
        //if (r.collider)
        //{
        //    Invoke(nameof(moveAndShoot), 0.25f);
        //    return;
        //}
        
        _stopped = false;
        Debug.Log($"RestlessBall: moving from {_currentStartPosition} to distance of {_currentDistance}");
        Debug.DrawLine(_currentStartPosition, _currentStartPosition + _currentDir * _currentDistance, Color.yellow, 1f);
    }

    private void shoot()
    {
        GameObject l1 = Instantiate(_laserPrefab, _gunPosition.position, Quaternion.identity);
        //GameObject l1 = _laserPool.Get();
        //l1.transform.position = _laser1.position;
        l1.GetComponent<Missile>().Front = (_player.transform.position - transform.position).normalized;
        l1.SetActive(true);
        Physics2D.IgnoreCollision(l1.GetComponent<Collider2D>(), _collider);
    }

    private void DealDamage(ushort hitPoints)
    {
        _hitPoints = (ushort)Mathf.Max(_hitPoints - hitPoints, 0);
        if (_hitPoints < 1)
        {
            var explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 2f);
            Destroy(gameObject);
        }
            
    }
}
