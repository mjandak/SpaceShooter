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
    private BinTree<float> _index;
    private List<Interval> _intervals;

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



    private void Awake()
    {
        _intervals = new List<Interval>();

        float minimum = 0;

        foreach (EnemySpawnDef e in Enemies)
        {
            var interval = new Interval
            {
                Minimum = minimum,
                Enemy = e
            };
            _intervals.Add(interval);
            minimum = interval.Minimum + e.Probability;
        }

        _index = new BinTree<float>(_intervals.Select(i => i.Minimum).ToArray());
    }

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
        return _intervals[_index.FindIdx(p)].Enemy;
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
    public float Probability;
    public ushort MaxSpawnNumber;
    public float Gap;
}

public class Interval
{
    public float Minimum;
    public EnemySpawnDef Enemy;
}
