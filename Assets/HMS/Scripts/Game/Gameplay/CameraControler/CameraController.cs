using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : CameraMovementInputBase
{
    [SerializeField]private InputService inputs;
    [SerializeField]private Camera _camera;
    [SerializeField]private int mindistanse;
    [SerializeField]private int maxdistanse;
    [SerializeField]private float move_sensitivity = 0.1f;
    [SerializeField]private float mouse_sensitivity = 1;
    [SerializeField]private float strangemove;
    [SerializeField]private float upperlimit;
    [SerializeField]private float bottomlimit;
    [SerializeField]private float rightlimit;
    [SerializeField]private float leftlimit;
    private string controlsystem;
    private float OldOrthographicSize = 1;

    // [SerializeField]private LayerMask _clickableMask;

    private bool _dragEnabled;

    // private void CameraMove() 
    // {
    //     if (controlsystem == "Keyboard") 
    //     {
    //         transform.position = transform.position + new Vector3(inputs.move.x * move_sensitivity, inputs.move.y * move_sensitivity, 0);
    //     }
    //     else 
    //     {
    //         if (Input.GetMouseButtonDown(0))
    //         {
    //             transform.position = transform.position + new Vector3(inputs.move.x * move_sensitivity, inputs.move.y * move_sensitivity, 0);
    //         }
    //     }

    //     float scroll = inputs.focus.y * mouse_sensitivity;
        
    //     // scroll up
    //     if (scroll > 0f & _camera.orthographicSize < maxdistanse){
    //         _camera.orthographicSize++;
    //     }
    //     // scroll down
    //     if (scroll < 0f & mindistanse < _camera.orthographicSize){
    //         _camera.orthographicSize--;
    //     }
    // }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(leftlimit, upperlimit), new Vector2(rightlimit, upperlimit));
        Gizmos.DrawLine(new Vector2(leftlimit, bottomlimit), new Vector2(rightlimit, bottomlimit));
        Gizmos.DrawLine(new Vector2(leftlimit, upperlimit), new Vector2(leftlimit, bottomlimit));
        Gizmos.DrawLine(new Vector2(rightlimit, bottomlimit), new Vector2(rightlimit, upperlimit));
    }

    private void CameraLimit() 
    {
        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, leftlimit, rightlimit),
            Mathf.Clamp(transform.position.y, bottomlimit, upperlimit),
            transform.position.z
        );
    }

    private void VisibleLayerMap()
    {
        if(_camera.orthographicSize != OldOrthographicSize)
        {
            string visibleLayer = "Biome";
            if(1 < _camera.orthographicSize & _camera.orthographicSize <= 2)visibleLayer = "Region";
            if(2 < _camera.orthographicSize & _camera.orthographicSize <= 5)visibleLayer = "Country";
            string[] layers = new string[6]{"Default", "TransparentFX", "Ignore Raycast", "Water", "UI", visibleLayer};
            _camera.cullingMask = LayerMask.GetMask(layers);
        }
        OldOrthographicSize = _camera.orthographicSize;
    }

    void Update()
    {
        // PlayerInput input = FindObjectOfType<PlayerInput>();
        // controlsystem = input.currentControlScheme;
        // CameraMove();
        // VisibleLayerMap();
        CameraLimit();
    }

    protected override void Avake()
    {
        base.Avake();

        _camera = GetComponent<Camera>();
    }

    protected override ICameraMovementHandler CreateCameraMovementHandler()
    {
        return new CameraMovementHandler(_properties);
    }

    protected override Vector3 ReadInputDelta()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // if (IsClickedonGround())
            // {
                _dragEnabled = true;
            // }

            return Vector3.zero;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragEnabled = false;
            return Vector3.zero;
        }

        if (_dragEnabled && Input.GetMouseButton(0))
        {
            // return Input.mousePositionDelta;
            return Input.mousePosition;
        }

        return Vector3.zero;
    }

    // private bool IsClickedonGround()
    // {
    //     var pointerScreenPosition = Input.mousePosition;
    //     var ray = _camera.ScreenPointToRay(pointerScreenPosition);
    //     var result = Physics.Raycast(ray, out _, float.MaxValue, _clickableMask.value);
    //     return result;
    // }
}
