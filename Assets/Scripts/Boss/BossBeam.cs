using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBeam : MonoBehaviour
{
    private bool _on = false;
    private bool _allowDealDamage = true;
    private GameObject _player;
    private int _layerMask;

    [SerializeField]
    private LineRenderer _lr;
    [SerializeField]
    private ParticleSystem _ps;
    [SerializeField]
    private ParticleSystem _ps2;

    [Tooltip("In seconds.")]
    public float OnLength;
    [Tooltip("In seconds.")]
    public float OffLength;
    [Tooltip("How many hit points of damage the beam deals.")]
    public ushort Damage;

    private void Awake()
    {
        _layerMask = LayerMask.GetMask("Player");
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(beam));
    }

    // Update is called once per frame
    void Update()
    {
        if (!_on)
        {
            _lr.enabled = false;
            //if (_ps.isPlaying) _ps.Stop();
            return;
        }
        _lr.enabled = true;
        //if (_ps.isStopped) _ps.Play();
        _lr.SetPosition(0, transform.position);
        RaycastHit2D hit = Physics2D.Raycast(_lr.transform.position, Vector2.down, 20f, _layerMask);
        if (hit.collider) //collider property of the result will be NULL if nothing was hit
        {
            _lr.SetPosition(1, hit.point);
            _ps2.gameObject.transform.position = hit.point;
            if (_ps2.isStopped) _ps2.Play();
            dealDmg();
        }
        else
        {
            if (_ps2.isPlaying) _ps2.Stop();
            _lr.SetPosition(1, transform.position + Vector3.down * 40f);
        }
    }

    private void dealDmg()
    {
        if (!_allowDealDamage) return;
        _player.GetComponent<Player>().DealDamage(Damage);
        _allowDealDamage = false;
        Invoke(nameof(switchAllowDmg), 1f);
    }

    private void switchAllowDmg()
    {
        _allowDealDamage = !_allowDealDamage;
    }

    private IEnumerator beam()
    {
        while (true)
        {
            _on = !_on;
            if (_on)
            {
                _ps.Play();
                yield return new WaitForSeconds(OnLength);
            }
            else
            {
                _ps.Stop();
                yield return new WaitForSeconds(OffLength);
            }
        }
    }

    private void ParentExplode()
    {
        Explode();
    }

    private void Explode()
    {
        StopCoroutine(nameof(beam));
        _on = false;
        if (_ps.isPlaying) _ps.Stop();
        if (_ps2.isPlaying) _ps2.Stop();
    }
}
