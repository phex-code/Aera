using System;
using System.Linq;

namespace Aera
{
    internal class SudoCommand : ICommand
    {
        private readonly CommandManager _manager;

        public string Name => "sudo";
        public string Description => "Executes a command with elevated privileges";
        public string Usage => "Usage: sudo <command>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public SudoCommand(CommandManager mgr)
        {
            _manager = mgr;
        }

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLine("Usage: sudo <command>");
                return;
            }

            if (!tool.AuthenticateSudo())
                return;

            var previousSudo = tool.IsSudo;
            tool.IsSudo = true;

            try
            {
                var commandName = args[0];
                string[] commandArgs = args.Skip(1).ToArray();

                _manager.ExecuteSudo(commandName, commandArgs, tool);
            }
            finally
            {
                tool.IsSudo = previousSudo;
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLine("sudo: does not accept piped input");
        }
    }
}
