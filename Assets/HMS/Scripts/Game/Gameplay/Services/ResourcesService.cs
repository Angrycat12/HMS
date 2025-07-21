using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;


public class ResourcesService : IDisposable
{
    public readonly ObservableList<ResourceViewModel> Resources = new();

    private readonly Dictionary<ResourceType, ResourceViewModel> _resourcesMap = new();
    private readonly ICommandProcessor _cmd;
    private readonly CompositeDisposable _disposables = new();

    public ResourcesService(ObservableList<Resource> resources, ICommandProcessor cmd)
    {
        _cmd = cmd;

        resources.ForEach(CreateResourceViewModel);
        _disposables.Add(resources.ObserveAdd().Subscribe(e => CreateResourceViewModel(e.Value)));
        _disposables.Add(resources.ObserveRemove().Subscribe(e => RemoveResourceViewModel(e.Value)));
    }

    public bool AddResources(ResourceType resourceType, int amount)
    {
        var command = new CmdResourcesAdd(resourceType, amount);

        return _cmd.Process(command);
    }

    public bool TrySpendResources(ResourceType resourceType, int amount)
    {
        var command = new CmdResourcesSpend(resourceType, amount);

        return _cmd.Process(command);
    }

    public bool IsEnoughResources(ResourceType resourceType, int amount)
    {
        if (_resourcesMap.TryGetValue(resourceType, out var resourceViewModel))
        {
            return resourceViewModel.Amount.CurrentValue >= amount;
        }

        return false;
    }

    public Observable<int> ObserveResource(ResourceType resourceType)
    {
        if (_resourcesMap.TryGetValue(resourceType, out var resourceViewModel))
        {
            return resourceViewModel.Amount;
        }

        throw new Exception($"Resource of type {resourceType} doesn't exist");
    }

    private void CreateResourceViewModel(Resource resource)
    {
        var resourceViewModel = new ResourceViewModel(resource);
        _resourcesMap[resource.ResourceType] = resourceViewModel;

        Resources.Add(resourceViewModel);
    }

    private void RemoveResourceViewModel(Resource resource)
    {
        if (_resourcesMap.TryGetValue(resource.ResourceType, out var resourceViewModel))
        {
            Resources.Remove(resourceViewModel);
            _resourcesMap.Remove(resource.ResourceType);
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}