using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryInfo : MonoBehaviour
{
    [SerializeField]
    private Boss _boss;
    [SerializeField]
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text.enabled = false;
        _boss.Destroyed += () => Invoke(nameof(showText), 5f);
    }

    private void showText()
    {
        _text.enabled = true;
    }
}
