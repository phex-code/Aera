using System;
using System.Linq;

namespace Aera
{
    internal class SudoCommand : ICommand
    {
        private readonly CommandManager manager;

        public string Name => "sudo";
        public string Description => "Executes a command with elevated privileges";
        public string Usage => "Usage: sudo <command>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public SudoCommand(CommandManager mgr)
        {
            manager = mgr;
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

            bool previousSudo = tool.IsSudo;
            tool.IsSudo = true;

            try
            {
                string commandName = args[0];
                string[] commandArgs = args.Skip(1).ToArray();

                manager.ExecuteSudo(commandName, commandArgs, tool);
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
