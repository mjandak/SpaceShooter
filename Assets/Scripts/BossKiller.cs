using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossKiller : MonoBehaviour
{
    [SerializeField]
    private GameObject _bossKillerFoundInfo;

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        _bossKillerFoundInfo.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(5f);
        Map.Map.State.CanPlayerDefeatBoss = true;
        Map.Map.State.PlayerPosition = Spawner.TargetPlanet;
        SceneManager.LoadScene("Map");
    }
}
