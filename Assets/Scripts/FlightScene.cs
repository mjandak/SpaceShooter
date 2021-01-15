using SpaceMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlightScene : MonoBehaviour
{
    /// <summary>
    /// Planet id player is moving to.
    /// </summary>
    public static string TargetPlanet;
    public static SpawnerConfig SpawnerConfig;
    public static bool EnablesPlayerToDefeatBoss;
    public static bool ResetsPlayerHitPoints;
    public static bool GivesPlayerDoubleGun;

    [Tooltip("Length of flight in seconds.")]
    public float FlightLength;

    [SerializeField] private GameObject _arrivingLabel;
    [SerializeField] private GameObject _bossKiller;
    [SerializeField] private Spawner _spawner;

    private void Awake()
    {
        if (SpawnerConfig != null)
        {
            _spawner.Configuration = SpawnerConfig;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(EndFlight));
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.S))
        {
            GoToMapScene();
        }
#endif
    }

    private IEnumerator EndFlight()
    {
        yield return new WaitForSeconds(FlightLength);
        if (_spawner.enabled) _spawner.Stop();
        if (EnablesPlayerToDefeatBoss && !Map.State.CanPlayerDefeatBoss)
        {
            _bossKiller.SetActive(true);
        }
        else
        {
            _arrivingLabel.SetActive(true);
            yield return new WaitForSeconds(5f);
            GoToMapScene();
        }
    }

    public static void GoToMapScene()
    {
        if (ResetsPlayerHitPoints) PlayerState.ResetHitPoints();
        PlayerState.HasDoubleGun |= GivesPlayerDoubleGun;
        SceneManager.LoadScene("Map");
    }
}
