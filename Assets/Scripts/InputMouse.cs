using System;
using UnityEngine;

public class InputMouse : MonoBehaviour, IInputService
{
    public event Action<Vector3> OnClick;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke(Input.mousePosition);
        }
    }
}