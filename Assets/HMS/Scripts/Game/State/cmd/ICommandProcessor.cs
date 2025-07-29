using System.Threading;
using System.Threading.Tasks;

public interface ICommandProcessor
{
    void RegisterHandler<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand;
    void RegisterHandler<TCommand>(ICommandHandlerAsync<TCommand> handler) where TCommand : ICommand;
    bool Process<TCommand>(TCommand command) where TCommand : ICommand;
    Task<bool> Process<TCommand>(TCommand command, CancellationToken token) where TCommand : ICommand;
}