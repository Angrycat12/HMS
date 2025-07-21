using UnityEngine;

public class ObjectSelect : MonoBehaviour
{
    [SerializeField] private InputService inputs;
    [SerializeField] private Camera Camera;
    public bool OnSelected          {get; private set;} = false;
    public GameObject SelectedObject{get; private set;} 

    RaycastHit2D Cast()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin ,-Vector2.up);
        return hit;
    }  

    void Update()
    {
        if(inputs.select == 1)
        {
            RaycastHit2D hit = Cast();
            if(hit.collider != null)
            {
                OnSelected = true;
                SelectedObject = hit.collider.gameObject;
                Debug.Log(hit.collider.gameObject.name);
            }
            else
            {
                OnSelected = false;
                Debug.Log("None object selected");
            }
            inputs.select = 0;
        }
    }
}
