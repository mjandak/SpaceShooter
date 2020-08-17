using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstantKiller : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    [SerializeField]
    private GameObject _boss;
    [SerializeField]
    private GameObject _beam;
    private bool _done;

    private void Awake()
    {
        _beam.SetActive(false);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (Map.Map.State.CanPlayerDefeatBoss)
        {
            _boss.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            _boss.SetActive(false);
            gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            Kill();
        }
    }

    private void Kill()
    {
        transform.position = new Vector2(_player.transform.position.x, transform.position.y);
        _beam.SetActive(true);
    }
}
