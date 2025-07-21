using System;
using UnityEngine;

[Serializable]
public class CameraMovementProperties
{
    public Transform Pivot;
    public float Speed = 1;
    public float Smoothness = 0.2f;
}