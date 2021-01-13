using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Tooltip("Seconds elapsed before effect is destroyed.")]
    public float DestroyTime;

    public Vector2 Front 
    { 
        get => transform.up; 
        set => transform.up = value; 
    }
}
