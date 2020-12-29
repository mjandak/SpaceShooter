using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceMap
{
    [ExecuteInEditMode]
    public class PlanetLink : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer _lr;

        public GameObject A;
        public GameObject B;
        public SpawnerConfig SpawnerConfig;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (A == null) return;
            if (B == null) return;
            _lr.SetPositions(new[] {
            new Vector3(A.transform.position.x, A.transform.position.y, -1),
            new Vector3(B.transform.position.x, B.transform.position.y, -1) }
            );
        }

        public void HighlightPath()
        {
            _lr.startColor = Color.blue;
            _lr.endColor = Color.blue;
        }

        public void UnhighlightPath()
        {
            _lr.startColor = Color.grey;
            _lr.endColor = Color.grey;
        }
    }
}
