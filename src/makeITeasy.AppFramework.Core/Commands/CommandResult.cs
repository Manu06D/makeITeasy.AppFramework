using makeITeasy.AppFramework.Models;

namespace makeITeasy.AppFramework.Core.Commands
{
    public class CommandResult
    {
        public CommandState Result { get; set; } = CommandState.Error;
        public string Message { get; set; }

        public CommandResult(CommandState result, string message)
        {
            Result = result;
            Message = message;
        }

        public CommandResult()
        {

        }
    }

    public class CommandResult<T> : CommandResult where T : IBaseEntity
    {
        public T Entity { get; set; }

        public CommandResult(CommandState result, string message = null) : base(result, message)
        {
        }

        public CommandResult()
        {

        }

        public static CommandResult<T> CreateDefault()
        {
            CommandResult<T> result = new CommandResult<T>() { Result = CommandState.Error };

            return result;
        }
    }
}
