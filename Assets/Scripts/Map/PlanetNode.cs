using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpaceMap
{
    public class PlanetNode : MonoBehaviour
    {
        private const string _previousInfoText = "Last boss distance: ";
        [SerializeField]
        private GameObject _tick;
        [SerializeField]
        private TextMesh _previous;
        private GameObject _player;
        private GameObject _highlightedLink;
        private Color _initSpriteColor;

        [HideInInspector] public List<string> Neigbours;
        public bool EnablesPlayerToDefeatBoss;
        public bool ResetsPlayerHitPoints;
        public bool GivesPlayerDoubleGun;

        public bool Visited
        {
            get;
            private set;
        }

        public bool Highlighted { get; set; }

        public event Action<string> PlayerMovingdHere;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _initSpriteColor = GetComponent<SpriteRenderer>().color;

        }

        // Start is called before the first frame update
        void Start()
        {
            Visited = Map.State.Visited.Contains(name);
        }

        // Update is called once per frame
        void Update()
        {
            _tick.SetActive(Visited && Map.State.PlayerPosition != name);
            if (name == Map.State.PreviousPlanet)
                if (Map.State.PreviousDistance.HasValue)
                    _previous.text = $"{_previousInfoText}{Map.State.PreviousDistance}";
                else
                    _previous.text = $"{_previousInfoText}>2";
            else
                _previous.text = "";

            GetComponent<SpriteRenderer>().color = Map.State.BossPosition == name && Map.State.CanPlayerDefeatBoss ? Color.red : _initSpriteColor;
            //GetComponent<SpriteRenderer>().color = Map.State.BossPosition == name ? Color.red : _initSpriteColor;
            if (Highlighted)
            {
                GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }

        private void OnMouseUp()
        {
            if (!Neigbours.Contains(Map.State.PlayerPosition)) return;
            PlayerMovingdHere(name);
        }

        private void OnMouseEnter()
        {
            if (name == Map.State.PreviousPlanet)
            {
                Map.highLightPlanets(name, Map.State.PreviousDistance.HasValue ? (int)Map.State.PreviousDistance : 3);
            }

            if (!Neigbours.Contains(Map.State.PlayerPosition)) return;
            _highlightedLink = Map.getLink(Map.State.PlayerPosition, name);
            _highlightedLink.GetComponent<PlanetLink>().HighlightPath();
        }

        private void OnMouseExit()
        {
            Map.unhighLightPlanets();
            if (_highlightedLink == null) return;
            _highlightedLink.GetComponent<PlanetLink>().UnhighlightPath();
        }

        public void MovePlayerHere()
        {
            _player.transform.position = transform.position;
            //Visited = true;
            //if (!Map.State.Visited.Contains(name)) Map.State.Visited.Add(name);
            Map.State.PlayerPosition = name;
        }
    }


}
