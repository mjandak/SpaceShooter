using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public Vector2 Front 
    { 
        get => transform.up; 
        set => transform.up = value; 
    }
}
