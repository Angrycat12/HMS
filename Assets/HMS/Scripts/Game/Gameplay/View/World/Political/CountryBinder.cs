using R3;
using UnityEngine;

public class CountryBinder : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private CountryViewModel _viewModel;

    public void Bind(CountryViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}