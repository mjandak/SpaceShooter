using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using SpaceMap;

public class HUDHitPoints : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text.text = new string('I', PlayerState.HitPoints);
        PlayerState.HitPointsChanged += refresh;
    }

    private void refresh()
    {
        _text.text = new string('I', PlayerState.HitPoints);
    }
}
