using R3;
using UnityEngine;

public class CityBinder : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private CityViewModel _viewModel;

    public void Bind(CityViewModel viewModel)
    {
        _viewModel = viewModel;
    }
    
    private void OnEnable()
    {
        if (_viewModel is not null)
        {

        }
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }
}