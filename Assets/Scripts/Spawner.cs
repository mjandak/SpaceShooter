using SpaceMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private Player _player;
    private BinTree<float> _index;
    private List<Interval> _intervals;

    //public List<EnemySpawnDef> Enemies;
    public float X_Min;
    public float X_Max;
    public float Y;
    public Transform Start_Min;
    public Transform Start_Max;
    public GameObject Target_Min;
    public GameObject Target_Max;
    public SpawnerConfig Configuration;
    /// <summary>
    /// Planet id player is moving to.
    /// </summary>
    //public static string TargetPlanet;
    //public static bool EnablesPlayerToDefeatBoss;
    //public static bool ResetsPlayerHitPoints;
    //[Tooltip("Length of flight in seconds.")]
    //public float FlightLength;
    //public GameObject ArrivingLabel;
    //public GameObject BossKiller;
    //public HUDHitPoints HitPointsLabel;

    private void Awake()
    {
        _intervals = new List<Interval>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!Configuration) throw new ArgumentNullException(nameof(Configuration));

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _player.HasDied += () => { Invoke(nameof(Player_HasDied), 3f); };
        _player.Hit += (hitPoints) => { PlayerState.SetHitPoints(hitPoints); };
        _player.HitPoints = PlayerState.HitPoints;

        float minimum = 0;
        float weightSum = Configuration.Enemies.Sum(e => e.Weight);
        foreach (EnemySpawnDef e in Configuration.Enemies)
        {
            var interval = new Interval
            {
                Minimum = minimum,
                Enemy = e
            };
            _intervals.Add(interval);
            minimum = interval.Minimum + e.Weight / weightSum;
        }
        _index = new BinTree<float>(_intervals.Select(i => i.Minimum).ToArray());

        StartCoroutine(nameof(Spawn));
        //StartCoroutine(nameof(EndFlight));
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

            float d = Start_Max.position.x - Start_Min.position.x;
            float r = Random.Range(0, d);
            Vector2 spawnPosition = new Vector2(Start_Min.position.x + r, Y);


            int count = Random.Range(1, def.MaxSpawnNumber + 1); //max is exclusive

            for (int i = 0; i < count; i++)
            {
                float gap = i * def.Gap;
                GameObject enemy = Instantiate(def.Prefab, new Vector2(spawnPosition.x + gap, spawnPosition.y), Quaternion.identity);
                Vector3 target = new Vector2(_player.transform.position.x + gap, _player.transform.position.y);
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript)
                {
                    enemyScript.Front = (target - enemy.transform.position).normalized;
                }
            }

            //yield return new WaitForSeconds(1f);

            //EnemySpawnDef def2 = getEnemyPrefab(Random.Range(0f, 1f));
            //Vector2 spawnPosition2 = new Vector2(Start_Min.position.x + ((r + (d * 0.6f)) % d), Y);

            //int count2 = Random.Range(1, def2.MaxSpawnNumber + 1); //max is exclusive

            //for (int i = 0; i < count2; i++)
            //{
            //    float gap = i * def2.Gap;
            //    GameObject enemy = Instantiate(def2.Prefab, new Vector2(spawnPosition2.x + gap, spawnPosition2.y), Quaternion.identity);
            //    Vector3 target = new Vector2(_player.transform.position.x + gap, _player.transform.position.y);
            //    enemy.GetComponent<Enemy>().Front = (target - enemy.transform.position).normalized;
            //}

            yield return new WaitForSeconds(2.0f);
        }
    }

    private EnemySpawnDef getEnemyPrefab(float p)
    {
        return _intervals[_index.FindIdx(p)].Enemy;
    }

    //private IEnumerator EndFlight()
    //{
    //    yield return new WaitForSeconds(FlightLength);
    //    StopCoroutine(nameof(Spawn));
    //    if (ResetsPlayerHitPoints)
    //    {
    //        Map.State.ResetPlayerHitPoints();
    //    }
    //    if (EnablesPlayerToDefeatBoss && !Map.State.CanPlayerDefeatBoss)
    //    {
    //        BossKiller.SetActive(true);
    //    }
    //    else
    //    {
    //        ArrivingLabel.SetActive(true);
    //        yield return new WaitForSeconds(5f);
    //        SceneManager.LoadScene("Map");
    //    }

    //}

    public void Stop()
    {
        StopCoroutine(nameof(Spawn));
    }
}

public class BinTree<T> where T : IComparable
{
    public class Node
    {
        public Node Left;
        public Node Right;
        public int Idx;
    }

    private IList<T> _list;

    public Node Root { get; private set; }
    public BinTree(IList<T> orderedList)
    {
        _list = orderedList;
        Root = getTree(0, (ushort)_list.Count);
    }

    private Node getTree(ushort firstIdx, ushort Count)
    {
        if (Count == 0) throw new ArgumentException(nameof(Count));
        var n = new Node();
        if (Count == 1)
        {
            n.Idx = firstIdx;
            return n;
        }
        ushort leftIdx = firstIdx;
        ushort rightIdx = (ushort)(firstIdx + (Count / 2));
        n.Idx = rightIdx;
        n.Left = getTree(leftIdx, (ushort)(rightIdx - leftIdx));
        n.Right = getTree(rightIdx, (ushort)(Count - (rightIdx - leftIdx)));
        return n;
    }

    private int _findIdx(T value, Node node)
    {
        if (_list[node.Idx].CompareTo(value) == 0)
            return node.Idx;
        if (value.CompareTo(_list[node.Idx]) > 0 && node.Right != null)
            return _findIdx(value, node.Right);
        if (node.Left != null)
            return _findIdx(value, node.Left);
        return node.Idx;
    }

    public int FindIdx(T value)
    {
        return _findIdx(value, Root);
    }

    public override string ToString()
    {
        return convertToJSON(Root);
    }

    private string convertToJSON(Node node)
    {
        if (node == null) return "null";
        return $"{{ idx: {node.Idx}, L: {convertToJSON(node.Left)}, R: {convertToJSON(node.Right)} }}";
    }
}

[Serializable]
public class EnemySpawnDef
{
    public GameObject Prefab;
    public float Weight;
    public ushort MaxSpawnNumber;
    public float Gap;
}

public class Interval
{
    public float Minimum;
    public EnemySpawnDef Enemy;
}
