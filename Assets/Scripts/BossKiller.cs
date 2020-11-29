using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpaceMap;

public class BossKiller : MonoBehaviour
{
    [SerializeField]
    private GameObject _bossKillerFoundInfo;

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) yield break;
        _bossKillerFoundInfo.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(5f);
        Map.State.CanPlayerDefeatBoss = true;
        Map.State.PlayerPosition = FlightScene.TargetPlanet;
        FlightScene.GoToMapScene();
    }
}
