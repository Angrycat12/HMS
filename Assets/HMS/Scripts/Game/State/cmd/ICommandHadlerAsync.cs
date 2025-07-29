using System.Threading;
using System.Threading.Tasks;

public interface ICommandHandlerAsync<TCommand> where TCommand : ICommand
{
    Task<bool> Handle(TCommand command, CancellationToken token);
}