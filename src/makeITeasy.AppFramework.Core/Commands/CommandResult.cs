using makeITeasy.AppFramework.Models;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class CommandResult
    {
        public CommandState Result { get; set; } = CommandState.Error;
        public string Message { get; set; }
    }

    public class CommandResult<T> : CommandResult where T : IBaseEntity
    {
        public T Entity { get; set; }

        public static CommandResult<T> CreateDefault()
        {
            CommandResult<T> result = new CommandResult<T>() { Result = CommandState.Error };

            return result;
        }
    }
}
