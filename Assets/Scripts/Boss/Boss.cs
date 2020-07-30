using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IFront
{
    private GameObject _player;
    private Vector3 _target;

    [Header("Components")]
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private Collider2D _cl;
    [SerializeField]
    private SpriteRenderer _sr;
    [SerializeField]
    private GameObject _explosionPrefab;

    [Header("Params")]
    public float Force;
    public ushort HitPoints;
    public GameObject[] Parts;

    public Vector2 Front
    {
        get => -transform.up;
        set => transform.up = -value;
    }

    public event Action Destroyed;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _target = new Vector2(_player.transform.position.x, transform.position.y);

        StartCoroutine(nameof(checkPlayer));
    }

    private void FixedUpdate()
    {
        _rb.AddForce((_target - transform.position).normalized * Force, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < 2) return;
        if (HitPoints == 0) return;
        HitPoints--;
        if (HitPoints < 1)
        {
            BroadcastMessage("ParentExplode");
            Explode();
        }
    }

    private void Explode()
    {
        Force = 0;
        StopCoroutine(nameof(checkPlayer));
        _sr.enabled = _cl.enabled = false;
        var explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();

        foreach (GameObject part in Parts)
        {
            part.SetActive(true);
        }

        Destroyed?.Invoke();

        Destroy(gameObject, 5f);
    }

    private IEnumerator checkPlayer()
    {
        while (true)
        {
            _target = new Vector2(_player.transform.position.x, transform.position.y);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
