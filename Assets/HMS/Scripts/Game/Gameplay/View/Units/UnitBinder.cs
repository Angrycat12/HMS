using UnityEngine;

public class UnitBinder : MonoBehaviour
{
    private UnitViewModel _viewModel;
    public void Bind(UnitViewModel viewModel)
    {
        _viewModel = viewModel;
    }
}