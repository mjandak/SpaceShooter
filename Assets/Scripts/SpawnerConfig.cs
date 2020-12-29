using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerConfig", menuName = "ScriptableObjects/SpawnerConfig", order = 1)]
public class SpawnerConfig : ScriptableObject
{
    public List<EnemySpawnDef> Enemies;
}
