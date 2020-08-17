using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IFront
{
    private readonly Vector2 _rightBrakeDir = new Vector2(-1, 0);
    private readonly Vector2 _leftBrakeDir = new Vector2(1, 0);
    private readonly Vector2 _upBrakeDir = new Vector2(0, -1);
    private readonly Vector2 _downBrakeDir = new Vector2(0, 1);

    private Vector2 _moveDir;
    private int prevLeft;
    private int prevRight;
    private int prevUp;
    private int prevDown;
    private float _leftBrake;
    private float _rightBrake;
    private float _upBrake;
    private float _downBrake;
    private Vector2 _brakeDir;
    private float _currentBrakeForce;
    private float _initialDrag;

    [Header("Components")]
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private Collider2D _cldr;
    [SerializeField]
    private SpriteRenderer _sr;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _gunPosition;

    [Header("Params")]
    public float Force;
    public ushort HitPoints;
    public float BrakeForce;
    public float BrakeForceFallSpeed;

    public event Action HasDied;
    public event Action<ushort> Hit;

    public static bool IsDead
    {
        get;
        private set;
    }

    public Vector2 Front 
    { 
        get => transform.up;
        set => transform.up = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        _initialDrag = _rb.drag;
        IsDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        //movedir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        var right = Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        var left = Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
        var up = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        var down = Input.GetKey(KeyCode.DownArrow) ? -1 : 0;

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            _rightBrake = 1;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            _leftBrake = 1;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            _upBrake = 1;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            _downBrake = 1;
        }

        //leftBrake = (prevLeft == -1 && left == 0) ? -prevLeft : 0f;
        //rightBrake = (prevRight == 1 && right == 0) ? -prevRight : 0f;
        //upBrake = (prevUp == 1 && up == 0) ? -prevUp : 0f;
        //downBrake = (prevDown == -1 && down == 0) ? -prevDown : 0f;

        //_brakeDir = (_brakeDir + new Vector2(rightBrake + leftBrake, upBrake + downBrake)).normalized;
        //if (_brakeDir.magnitude > 0)
        //{
        //    _currentBrakeForce = BrakeForce;
        //}


        _moveDir = new Vector2(left + right, up + down).normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var m = Instantiate(_laserPrefab, _gunPosition.transform.position, Quaternion.identity);
            m.GetComponent<Missile>().Front = Vector2.up;
            Physics2D.IgnoreCollision(m.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        prevLeft = left;
        prevRight = right;
        prevUp = up;
        prevDown = down;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < 2) return;
        if (collision.gameObject.CompareTag("Wall")) return;
        if (collision.gameObject.CompareTag("Boss"))
            DealDamage(5);
        DealDamage(1);
    }

    public void DealDamage(ushort points)
    {
        if (HitPoints == 0) return;
        HitPoints = (ushort)Mathf.Max(HitPoints - points, 0);
        Hit?.Invoke(HitPoints);
        Debug.Log($"Player hit. {HitPoints} hit points remain.");
        if (HitPoints < 1) Explode();
    }

    private void Explode()
    {
        _cldr.enabled = false;
        _sr.enabled = false;
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        IsDead = true;
        HasDied?.Invoke();
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        //_rb.drag = _initialDrag;
        //print(_currentBrakeForce.ToString());
        _rb.AddForce(_moveDir * Force);
        //_rb.AddForce(_brakeDir * _currentBrakeForce);
        //print(_brakeDir.ToString());
        //_rb.velocity = _moveDir * Force;

        if (_rightBrake > 0)
        {
            //_rb.AddForce(new Vector2(-1, 0) * BrakeForce, ForceMode2D.Impulse);
            //rightBrake = 0;
            //_rightBrake = Mathf.MoveTowards(_rightBrake, 0, BrakeForce / BrakeForceFallSpeed * Time.fixedDeltaTime);
            //_rb.AddForce(_rightBrakeDir * _rightBrake * BrakeForce);
            //_rb.velocity /= 2;
            //if (_rb.velocity.x > 0)
            //{
            //    _rb.drag = _rb.drag * 3;
            //}
            //else
            //{
            //    _rightBrake = 0;
            //}

        }
        if (_leftBrake > 0)
        {
            //_rb.AddForce(new Vector2(1, 0) * BrakeForce, ForceMode2D.Impulse);
            //leftBrake = 0;
            //_leftBrake = Mathf.MoveTowards(_leftBrake, 0, BrakeForce / BrakeForceFallSpeed * Time.fixedDeltaTime);
            //_rb.AddForce(_leftBrakeDir * _leftBrake);
        }
        if (_upBrake > 0)
        {
            //_rb.AddForce(new Vector2(0, -1) * BrakeForce, ForceMode2D.Impulse);
            //upBrake = 0;
            //_upBrake = Mathf.MoveTowards(_upBrake, 0, BrakeForce / BrakeForceFallSpeed * Time.fixedDeltaTime);
            //_rb.AddForce(_upBrakeDir * _upBrake);
        }
        if (_downBrake > 0)
        {
            //_rb.AddForce(new Vector2(0, 1) * BrakeForce, ForceMode2D.Impulse);
            //downBrake = 0;
            //_downBrake = Mathf.MoveTowards(_downBrake, 0, BrakeForce / BrakeForceFallSpeed * Time.fixedDeltaTime);
            //_rb.AddForce(_downBrakeDir * _downBrake);
        }
    }
}
