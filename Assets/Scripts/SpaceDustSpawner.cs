using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceDustSpawner : MonoBehaviour
{
    public Transform Start_Min;
    public Transform Start_Max;
    public List<GameObject> Objects;
    private Vector3 spawnLine;

    // Start is called before the first frame update
    void Start()
    {
        spawnLine = Start_Max.position - Start_Min.position;
        StartCoroutine(spawn());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator spawn()
    {
        while (true)
        {
            var pos = Start_Min.position + spawnLine * Random.Range(0f, 1f);
            Instantiate(Objects[Random.Range(0, Objects.Count - 1)], pos, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        }
    }
}
