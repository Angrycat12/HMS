using UnityEngine;

public class CameraMovementHandler : ICameraMovementHandler
{
    private readonly CameraMovementProperties _properties;
    private Vector3 _cachedCCameraPosition;

    public CameraMovementHandler(CameraMovementProperties properties)
    {
        _properties = properties;
        _cachedCCameraPosition = properties.Pivot.position;
    }
    public void Move(Vector3 inputDelta)
    {
        _cachedCCameraPosition -= new Vector3(inputDelta.x, 0, inputDelta.y) * _properties.Speed;

        _properties.Pivot.position = Vector3.Lerp(_properties.Pivot.position, _cachedCCameraPosition,
            Time.deltaTime / _properties.Smoothness);
    }
}