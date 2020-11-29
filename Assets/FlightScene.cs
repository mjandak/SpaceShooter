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
    public static bool EnablesPlayerToDefeatBoss;
    public static bool ResetsPlayerHitPoints;

    [Tooltip("Length of flight in seconds.")]
    public float FlightLength;

    [SerializeField] private GameObject ArrivingLabel;
    [SerializeField] private GameObject BossKiller;
    [SerializeField] private Spawner _spawner;

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
            BossKiller.SetActive(true);
        }
        else
        {
            ArrivingLabel.SetActive(true);
            yield return new WaitForSeconds(5f);
            GoToMapScene();
        }
    }

    public static void GoToMapScene()
    {
        if (ResetsPlayerHitPoints) Map.State.ResetPlayerHitPoints();
        SceneManager.LoadScene("Map");
    }
}
