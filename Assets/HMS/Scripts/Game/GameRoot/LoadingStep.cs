using System;
using System.Threading.Tasks;

public class LoadingStep
{
    public string Description;
    public Func<Task> ActionAsync;

    public LoadingStep(string description, Func<Task> actionAsync)
    {
        Description = description;
        ActionAsync = actionAsync;
    }
}