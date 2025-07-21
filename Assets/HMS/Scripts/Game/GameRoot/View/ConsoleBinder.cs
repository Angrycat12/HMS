using R3;
using UnityEngine;

public class ConsoleBinder_ : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private ConsoleViewModel_ _viewModel;

    public void Bind(ConsoleViewModel_ viewModel)
    {
        _viewModel = viewModel;
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}