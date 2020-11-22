using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SpaceMap;

public class BossScene : MonoBehaviour
{
    [SerializeField]
    private Boss _boss;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private TextMeshProUGUI _victoryInfo;
    [SerializeField]
    private HUDHitPoints _hitPointsLabel;

    private void Awake()
    {
        _victoryInfo.enabled = false;
        _player.HitPoints = Map.State.PlayerHitPoints;
        _boss.Destroyed += () => Invoke(nameof(showVictoryInfo), 5f);
        _player.HasDied += () => Invoke(nameof(loadDeathScene), 3f);
        _player.Hit += (hitPoints) => { Map.State.SetPlayerHitPoints(hitPoints); _hitPointsLabel.Refresh(); };
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
