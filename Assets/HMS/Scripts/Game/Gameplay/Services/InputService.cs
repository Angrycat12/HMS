using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : MonoBehaviour
{
    public Vector2 move {get; private set;}
    public Vector2 focus{get; private set;}
    public float select {get; set;}

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnFocus(InputValue value)
    {
        focus = value.Get<Vector2>();
    }

    public void OnSelect(InputValue value)
    {
        select = value.Get<float>();
    }
}