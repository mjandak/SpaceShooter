using Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private GameObject _player;

    public List<EnemySpawnDef> Enemies;
    public float X_Min;
    public float X_Max;
    public float Y;
    public Transform Start_Min;
    public Transform Start_Max;
    public GameObject Target_Min;
    public GameObject Target_Max;
    /// <summary>
    /// Planet id player is moving to.
    /// </summary>
    public static string TargetPlanet;
    public static bool EnablesPlayerToDefeatBoss;
    [Tooltip("Length of flight in seconds.")]
    public float FlightLength;
    public GameObject ArrivingLabel;
    public GameObject BossKiller;

    //public bool EnablesPlayerToDefeatBoss
    //{
    //    get { return Map.Map.getPlanet(TargetPlanet).GetComponent<PlanetNode>().EnablesPlayerToDefeatBoss; }
    //}

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _player.GetComponent<Player>().HasDied += () => { Invoke(nameof(Player_HasDied), 3f); };
        StartCoroutine(nameof(Spawn));
        StartCoroutine(nameof(EndFlight));
    }

    private void Player_HasDied()
    {
        SceneManager.LoadScene("Death");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Start_Min.position, Start_Max.position);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            EnemySpawnDef def = getEnemyPrefab(Random.Range(0f, 1f));
            int count = Random.Range(1, def.MaxSpawnNumber + 1); //max is exclusive
            Vector2 spawnPosition = new Vector2(Random.Range(X_Min, X_Max), Y);
            for (int i = 0; i < count; i++)
            {
                float gap = i * def.Gap;
                GameObject enemy = Instantiate(def.Prefab, new Vector2(spawnPosition.x + gap, spawnPosition.y), Quaternion.identity);
                Vector3 target = new Vector2(_player.transform.position.x + gap, _player.transform.position.y);
                enemy.GetComponent<Enemy>().Front = (target - enemy.transform.position).normalized;
            }

            yield return new WaitForSeconds(3.0f);
        }
    }

    private EnemySpawnDef getEnemyPrefab(float p)
    {
        if (p >= 1) return Enemies.Last();
        if (p <= 0) return Enemies.First();

        float min = 0;

        for (int i = 0; i < Enemies.Count; i++)
        {
            float max = min + Enemies[i].Probability;
            if (p >= min && p < max)
            {
                return Enemies[i];
            }
            min = max;
        }
        throw new Exception("No prefab to spawn found.");
    }

    private IEnumerator EndFlight()
    {
        yield return new WaitForSeconds(FlightLength);
        StopCoroutine(nameof(Spawn));
        if (EnablesPlayerToDefeatBoss && !Map.Map.State.CanPlayerDefeatBoss)
        {
            BossKiller.SetActive(true);
        }
        else
        {
            ArrivingLabel.SetActive(true);
            yield return new WaitForSeconds(5f);
            Map.Map.State.PlayerPosition = TargetPlanet;
            SceneManager.LoadScene("Map");
        }

    }
}

[Serializable]
public class EnemySpawnDef
{
    public GameObject Prefab;
    public float Probability;
    public ushort MaxSpawnNumber;
    public float Gap;
}
