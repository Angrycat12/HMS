using R3;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RoadBinder : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private RoadViewModel _viewModel;

    public void Bind(RoadViewModel viewModel)
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