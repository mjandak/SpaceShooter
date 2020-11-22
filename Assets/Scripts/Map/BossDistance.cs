using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SpaceMap
{
    public class BossDistance : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMeshPro;

        public event Action<string, int> MouseEnter;
        public event Action MouseExit;
        [HideInInspector]
        public int? Distance;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Distance == null)
            {
                _textMeshPro.text = $"Boss distance: >2";
            }
            else
            {
                _textMeshPro.text = $"Boss distance: {Distance}";
            }
        }

        private void OnMouseOver()
        {

        }

        private void OnMouseEnter()
        {
            int d = Distance.HasValue ? Distance.Value : 3;
            MouseEnter?.Invoke(Map.State.PlayerPosition, d);
        }

        private void OnMouseExit()
        {
            //int d = (Text.text == ">2") ? 3 : int.Parse(Text.text);
            MouseExit?.Invoke();
        }
    }
}