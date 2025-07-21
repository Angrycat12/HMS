using R3;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RiverBinder : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private RiverViewModel _viewModel;

    public void Bind(RiverViewModel viewModel)
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