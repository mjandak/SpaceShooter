using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFront
{
    Vector2 Front { get; set; }
}

interface IObstacle
{
    Vector2 Dir { get; }
    Transform Transform { get; }

    Collider2D Collider { get; }
}
