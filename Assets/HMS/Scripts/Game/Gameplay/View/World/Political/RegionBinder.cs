using R3;
using UnityEngine;

public class RegionBinder : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private RegionViewModel _viewModel;

    public void Bind(RegionViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}