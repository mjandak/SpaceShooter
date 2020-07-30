using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using TMPro;

namespace Map
{
    public class Map : MonoBehaviour
    {
        //private List<GameObject> visited = new List<GameObject>();
        //private List<GameObject> lineRenderers = new List<GameObject>();
        private static Dictionary<string, GameObject> _planets;
        private static Dictionary<string, GameObject> _links;
        private ushort? _bossDistance;

        public static MapState State;
        public GameObject Planets;
        public GameObject Links;
        public Player Player;
        public GameObject BossKiller;
        public TextMeshProUGUI BossDistance;

        static Map()
        {
            var initPosition = "P";
            State = new MapState()
            {
                BossPosition = "Station",
                PlayerPosition = initPosition,
                Visited = new List<string>() { initPosition }
            };
        }

        // Start is called before the first frame update
        void Start()
        {
            //DrawLinesToNeighbours(TestPlanet.transform);
            //int i = 0;
            //foreach (Transform planet in transform)
            //{
            //    //if (i == 1) break;
            //    DrawLinesToNeighbours(planet);
            //    //i++;
            //}

            //reset dictionary, game objects are destroyed when leaving scene
            _planets = new Dictionary<string, GameObject>();
            foreach (Transform p in Planets.transform)
            {
                _planets.Add(p.name, p.gameObject);
                p.GetComponent<PlanetNode>().PlayerMovingdHere += PlayerMovingTo;
                List<GameObject> linkedPlanets = getLinkedPlanets(p.name);
                p.gameObject.GetComponent<PlanetNode>().Neigbours = linkedPlanets.Select(lp => lp.name).ToList();
            }

            //reset dictionary, game objects are destroyed when leaving scene
            _links = new Dictionary<string, GameObject>();
            foreach (Transform l in Links.transform)
            {
                PlanetLink planetLink = l.GetComponent<PlanetLink>();
                _links.Add(planetLink.A.name + "|" + planetLink.B.name, l.gameObject);
            }

            getPlanet(State.PlayerPosition).GetComponent<PlanetNode>().MovePlayerHere();
            UpdateBossDistance();
        }

        // Update is called once per frame
        void Update()
        {
            //visited.Clear();
            //foreach (GameObject lr in lineRenderers)
            //{
            //    DestroyImmediate(lr);
            //}
            //foreach (Transform planet in transform)
            //{
            //    //if (i == 1) break;
            //    DrawLinesToNeighbours(planet);
            //    //i++;
            //}
            BossKiller.SetActive(State.CanPlayerDefeatBoss);
            if (_bossDistance == null)
            {
                BossDistance.text = $"Boss distance: >2";
            }
            else
            {
                BossDistance.text = $"Boss distance: {_bossDistance}";
            }
            
        }

        private void PlayerMovingTo(string planetId)
        {
            if (planetId == State.BossPosition)
            {
                MoveBoss();
                if (State.BossPosition == State.PlayerPosition)
                {
                    Debug.Log("Clash!");
                    //if (State.CanPlayerDefeatBoss)
                    //{
                    //    SceneManager.LoadScene("Boss");
                    //    return;
                    //}
                    SceneManager.LoadScene("Boss");
                    return;
                }
                else
                {
                    StartFlight(planetId);
                    return;
                }
            }
            else
            {
                MoveBoss();
                if (planetId == State.BossPosition)
                {
                    Debug.Log("Clash!");
                    //if (State.CanPlayerDefeatBoss)
                    //{
                    //    SceneManager.LoadScene("Boss");
                    //    return;
                    //}
                    SceneManager.LoadScene("Boss");
                    return;
                }
                else
                {
                    StartFlight(planetId);
                    return;
                }
            }
        }

        private void StartFlight(string planetId)
        {
            PlanetNode planet = getPlanet(planetId).GetComponent<PlanetNode>();
            State.PreviousPlanet = State.PlayerPosition;
            State.PreviousDistance = _bossDistance;
            if (planet.Visited)
            {
                planet.MovePlayerHere();
                UpdateBossDistance();
                return;
            }
            Spawner.TargetPlanet = planetId;
            Spawner.EnablesPlayerToDefeatBoss = planet.EnablesPlayerToDefeatBoss;
            SceneManager.LoadScene("Flight");
            return;
        }

        public void Wait()
        {
            MoveBoss();
            if (State.BossPosition == State.PlayerPosition)
            {
                Debug.Log("Clash!");
                //if (State.CanPlayerDefeatBoss)
                //{
                //    SceneManager.LoadScene("Boss");
                //    return;
                //}
                SceneManager.LoadScene("Boss");
                return;
            }
            State.PreviousDistance = _bossDistance;
            State.PreviousPlanet = State.PlayerPosition;
            UpdateBossDistance();
            
        }

        private void UpdateBossDistance()
        {
            List<GameObject> playerLinkedPlanets = getLinkedPlanets(State.PlayerPosition);
            if (playerLinkedPlanets.Any(p => p.name == State.BossPosition))
            {
                _bossDistance = 1;
                Debug.Log($"Updated distance: {_bossDistance}");
                return;
            }

            foreach (GameObject p in playerLinkedPlanets)
            {
                var ps = getLinkedPlanets(p.name);
                if (ps.Any(p2 => p2.name == State.BossPosition))
                {
                    _bossDistance = 2;
                    Debug.Log($"Updated distance: {_bossDistance}");
                    return;
                }
            }
            _bossDistance = null;
            Debug.Log($"Updated distance: {_bossDistance}");
        }

        /// <summary>
        /// Randomly move boss.
        /// </summary>
        private void MoveBoss()
        {
            var linkedPlanets = getLinkedPlanets(State.BossPosition);
            var planet = linkedPlanets[UnityEngine.Random.Range(0, linkedPlanets.Count - 1)];
            State.BossPosition = planet.name;
            Debug.Log($"Boss moved to {State.BossPosition}.");
        }

        public List<GameObject> getLinkedPlanets(string planetId)
        {
            var result = new List<GameObject>();
            foreach (Transform p in Links.transform)
            {
                var planetLink = p.GetComponent<PlanetLink>();
                if (planetLink.A.name == planetId)
                {
                    result.Add(planetLink.B);
                    continue;
                }
                if (planetLink.B.name == planetId)
                {
                    result.Add(planetLink.A);
                    continue;
                }
            }
            return result;
        }

        public static GameObject getPlanet(string planetId)
        {
            if (!_planets.ContainsKey(planetId))
                throw new Exception($"Planet with id {planetId} doesn't exist");
            return _planets[planetId];

            //foreach (Transform p in Planets.transform)
            //{
            //    if (p.name == planetId)
            //        return p.gameObject;
            //}
            //return null;
        }

        public static GameObject getLink(string id)
        {
            if (_links.ContainsKey(id))
                return _links[id];
            return null;
        }

        //private void DrawLinesToNeighbours(Transform planet)
        //{
        //    PlanetNode planetNode = planet.gameObject.GetComponent<PlanetNode>();
        //    foreach (GameObject neighbour in planetNode.Neigbours)
        //    {
        //        if (neighbour == null) continue;
        //        if (visited.Contains(neighbour)) continue;
        //        var lineRndr = Instantiate(PlanetLink).GetComponent<LineRenderer>();
        //        lineRenderers.Add(lineRndr.gameObject);
        //        lineRndr.SetPositions(new[] { planet.position, neighbour.transform.position });
        //        lineRndr.startWidth = 0.2f;
        //        lineRndr.endWidth = 0.2f;
        //    }
        //    visited.Add(planet.gameObject);
        //}
    }

    public class MapState
    {
        public List<string> Visited;
        public string PlayerPosition;
        public string BossPosition;
        public bool CanPlayerDefeatBoss;
        public string PreviousPlanet;
        public ushort? PreviousDistance;
    }
}


