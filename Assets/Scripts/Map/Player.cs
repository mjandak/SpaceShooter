using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceMap
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private TextMesh _text;

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

