using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCannon : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _barrels;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private Collider2D _collider;

    [Tooltip("Fire rate in seconds.")]
    public float FireRate;
    [Tooltip("Firing start delay in seconds.")]
    public float FireStartDelay;
    public float ShotImpulse;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(shoot));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator shoot()
    {
        yield return new WaitForSeconds(FireStartDelay);

        while (true)
        {
            foreach (GameObject barrel in _barrels)
            {
                var l = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                l.GetComponent<Missile>().Front = -barrel.transform.up;
                l.GetComponent<Missile>().Impulse = ShotImpulse;
                Physics2D.IgnoreCollision(l.GetComponent<Collider2D>(), _collider);
            }
            yield return new WaitForSeconds(FireRate);
        }
    }

    private void ParentExplode()
    {
        Explode();
    }

    private void Explode()
    {
        StopCoroutine(nameof(shoot));
    }
}
