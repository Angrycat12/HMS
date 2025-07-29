using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CommandProcessor : ICommandProcessor
{
    private readonly IGameStateProvider _gameStateProvider;
    private readonly Dictionary<Type, object> _handlesMap = new();
    private readonly Dictionary<Type, object> _asyncHandlesMap = new();

    public CommandProcessor(IGameStateProvider gameStateProvider)
    {
        _gameStateProvider = gameStateProvider;
    }

    public void RegisterHandler<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand
    {
        _handlesMap[typeof(TCommand)] = handler;
    }

    public void RegisterHandler<TCommand>(ICommandHandlerAsync<TCommand> handler) where TCommand : ICommand
    {
        _asyncHandlesMap[typeof(TCommand)] = handler;
    }

    public bool Process<TCommand>(TCommand command) where TCommand : ICommand
    {
        if (_handlesMap.TryGetValue(typeof(TCommand), out var handler))
        {
            var typedHandler = (ICommandHandler<TCommand>)handler;
            var result = typedHandler.Handle(command);

            if (result)
            {
                _gameStateProvider.SaveGameState();
            }

            return result;
        }

        return false;
    }
    
    public async Task<bool> Process<TCommand>(TCommand command, CancellationToken token) where TCommand : ICommand
    {
        if (_asyncHandlesMap.TryGetValue(typeof(TCommand), out var handler))
        {
            var typedHandler = (ICommandHandlerAsync<TCommand>)handler;
            var result = await typedHandler.Handle(command, token);

            if (result)
            {
                _gameStateProvider.SaveGameState();
            }

            return result;
        }

        return false;
    }
}
