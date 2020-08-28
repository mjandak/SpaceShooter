﻿using System.Collections;
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
        /// <summary>
        /// Distance between player and boss.
        /// </summary>
        private ushort? _distance;

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

            getPlanet(State.PlayerPosition).MovePlayerHere();
            UpdateDistance();
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
            if (_distance == null)
            {
                BossDistance.text = $"Boss distance: >2";
            }
            else
            {
                BossDistance.text = $"Boss distance: {_distance}";
            }

        }

        private void PlayerMovingTo(string planetId)
        {
            //MovePlayer(planetId);
            //if (State.PlayerPosition == State.BossPosition)
            //{
            //    Debug.Log("Clash!");
            //    SceneManager.LoadScene("Boss");
            //    return;
            //}
            //MoveBoss();
            //if (State.PlayerPosition == State.BossPosition)
            //{
            //    Debug.Log("Clash!");
            //    SceneManager.LoadScene("Boss");
            //    return;
            //}
            //UpdateDistance();
            //StartFlight(planetId);

            MoveBoss();
            if (State.BossPosition == State.PlayerPosition && State.BossPreviousPlanet == planetId)
            {
                Debug.Log("Clash!");
                SceneManager.LoadScene("Boss");
            }
            MovePlayer(planetId);
            UpdateDistance();

            if (State.PlayerPosition == State.BossPosition)
            {
                Debug.Log("Clash!");
                SceneManager.LoadScene("Boss");
            }
            else
            {
                StartFlight(planetId);
            }
        }

        private void MovePlayer(string planetId)
        {
            State.PreviousPlanet = State.PlayerPosition;
            State.PreviousDistance = _distance;
            PlanetNode planet = getPlanet(planetId);
            planet.MovePlayerHere();
        }

        private void StartFlight(string planetId)
        {
            PlanetNode planet = getPlanet(planetId);
            //State.PreviousPlanet = State.PlayerPosition;
            //State.PreviousDistance = _bossDistance;
            if (planet.Visited)
            {
                planet.MovePlayerHere();
                return;
            }
            State.Visited.Add(planetId);
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
            State.PreviousDistance = _distance;
            State.PreviousPlanet = State.PlayerPosition;
            UpdateDistance();

        }

        private void UpdateDistance()
        {
            List<GameObject> playerLinkedPlanets = getLinkedPlanets(State.PlayerPosition);
            if (playerLinkedPlanets.Any(p => p.name == State.BossPosition))
            {
                _distance = 1;
                Debug.Log($"Updated distance: {_distance}");
                return;
            }

            foreach (GameObject p in playerLinkedPlanets)
            {
                var ps = getLinkedPlanets(p.name);
                if (ps.Any(p2 => p2.name == State.BossPosition))
                {
                    _distance = 2;
                    Debug.Log($"Updated distance: {_distance}");
                    return;
                }
            }
            _distance = null;
            Debug.Log($"Updated distance: {_distance}");
        }

        private void MoveBoss()
        {
            //List<string> path = getShortestPath(State.BossPosition, State.PlayerPosition);
            //Debug.Log($"Path to player: {string.Join(",", path)}");
            //State.BossPreviousPlanet = State.BossPosition;
            //State.BossPosition = path[path.Count - 2];
            //Debug.Log($"Boss moved to {State.BossPosition}.");

            if (!_distance.HasValue)
            {
                List<GameObject> linkedPlanets = getLinkedPlanets(State.BossPosition);
                GameObject planet = linkedPlanets[UnityEngine.Random.Range(0, linkedPlanets.Count - 1)];
                State.BossPreviousPlanet = State.BossPosition;
                State.BossPosition = planet.name;
                Debug.Log($"Boss moved to {State.BossPosition}.");
                return;
            }

            List<string> possiblePlanets = getPlanetsAtDistance(State.BossPosition, _distance.Value);

            //if (State.PreviousDistance.HasValue)
            //{
            //    List<string> possiblePreviousPlanets = getPlanetsAtDistance(State.BossPreviousPlanet, State.PreviousDistance.Value);
            //    string[] temp = possiblePlanets.ToArray();
            //    possiblePlanets.Clear();
            //    foreach (string p in temp)
            //    {
            //        foreach (string previous_p in possiblePreviousPlanets)
            //        {
            //            if (IsPlanetReachableFromPosition(p, previous_p))
            //            {
            //                possiblePlanets.Add(p);
            //                break;
            //            }
            //        }
            //    }
            //    possiblePlanets = possiblePlanets.Distinct().ToList();
            //}

            string[] temp = possiblePlanets.ToArray();
            possiblePlanets.Clear();
            foreach (var p in temp)
            {
                foreach (var n in getPlanet(p).Neigbours)
                {
                    if (State.PreviousDistance.HasValue)
                    {
                        if (getShortestPath(State.BossPreviousPlanet, n).Count - 1 == State.PreviousDistance)
                        {
                            possiblePlanets.Add(p);
                            break;
                        }
                    }
                    else
                    {
                        if (getShortestPath(State.BossPreviousPlanet, n).Count - 1 > 2)
                        {
                            possiblePlanets.Add(p);
                            break;
                        }
                    }

                }
            }
            string targetPlanet = possiblePlanets[UnityEngine.Random.Range(0, possiblePlanets.Count - 1)];
            var path = getShortestPath(State.BossPosition, targetPlanet);
            State.BossPreviousPlanet = State.BossPosition;
            State.BossPosition = path[path.Count - 2];
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

        public static PlanetNode getPlanet(string planetId)
        {
            if (!_planets.ContainsKey(planetId))
                throw new Exception($"Planet with id {planetId} doesn't exist");
            return _planets[planetId].GetComponent<PlanetNode>();
        }

        public static GameObject getLink(string id)
        {
            if (_links.ContainsKey(id))
                return _links[id];
            return null;
        }

        private List<string> getShortestPath(string startPlanet, string endPlanet)
        {
            Queue<string> Q = new Queue<string>();
            //Dictionary<string, string> visited = new Dictionary<string, string>();
            var visited = new List<ValueTuple<string, string>>();
            Q.Enqueue(startPlanet);
            visited.Add((null, startPlanet));

            while (Q.Count > 0)
            {
                string e = Q.Dequeue();

                //if (e == nameToSearchFor)
                //return e;
                //break;
                foreach (string friend in getPlanet(e).Neigbours)
                {
                    if (friend == endPlanet)
                    {
                        visited.Add(new ValueTuple<string, string>(e, friend));
                        Q.Clear();
                        break;
                    }

                    if (!visited.Contains(new ValueTuple<string, string>(e, friend)))
                    {
                        Q.Enqueue(friend);
                        visited.Add(new ValueTuple<string, string>(e, friend));
                    }
                }
            }
            //return null;

            var path = new List<string>();
            var currentPathNode = endPlanet;
            while (true)
            {
                path.Add(currentPathNode);
                if (currentPathNode == startPlanet) break;
                //var neighbours = getPlanet(currentPathNode).GetComponent<PlanetNode>().Neigbours;
                //var pathNode = neighbours.First(s => visited.Contains(s));
                var previousNode = visited.First(x => x.Item2 == currentPathNode);
                currentPathNode = previousNode.Item1;
            }
            return path;
        }

        private bool IsPlanetReachableFromPosition(string planet, string position)
        {
            foreach (string p in getPlanet(position).Neigbours)
            {
                if (p == planet) return true;
            }
            return false;
        }

        private List<string> getPlanetsAtDistance(string planet, ushort distance)
        {
            var Q = new Queue<string>();
            var visited = new List<string>();
            Q.Enqueue(planet);
            visited.Add(planet);
            ushort currentLevel = 0;
            while (true)
            {
                if (currentLevel == distance) return Q.ToList();
                var count = Q.Count;
                for (int i = 0; i < count; i++)
                {
                    foreach (string n in getPlanet(Q.Dequeue()).Neigbours)
                    {
                        if (visited.Contains(n)) continue;
                        Q.Enqueue(n);
                        visited.Add(n);
                    }
                }
                currentLevel++;
            }
        }
    }

    public class MapState
    {
        public List<string> Visited;
        public string PlayerPosition;
        public string BossPosition;
        public bool CanPlayerDefeatBoss;
        /// <summary>
        /// Player's previous planet
        /// </summary>
        public string PreviousPlanet;
        /// <summary>
        /// Boss previous planet.
        /// </summary>
        public string BossPreviousPlanet;
        public ushort? PreviousDistance;
    }
}


