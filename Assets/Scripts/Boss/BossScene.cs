﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossScene : MonoBehaviour
{
    [SerializeField]
    private Boss _boss;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private TextMeshProUGUI _victoryInfo;

    private void Awake()
    {
        _victoryInfo.enabled = false;
        _boss.Destroyed += () => Invoke(nameof(showVictoryInfo), 5f);
        _player.HasDied += () => Invoke(nameof(loadDeathScene), 3f); ;
    }

    private void showVictoryInfo()
    {
        _victoryInfo.enabled = true;
    }

    private void loadDeathScene()
    {
        SceneManager.LoadScene("Death");
    }
}