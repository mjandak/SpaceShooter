using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class HUDHitPoints : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    [SerializeField]
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _player.Hit += _player_Hit;
        _text.text = new string('I', 10);
    }

    private void _player_Hit(ushort obj)
    {

        _text.text = new string('I', obj);
    }
}
