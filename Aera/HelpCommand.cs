namespace Aera
{
    internal class HelpCommand : ICommand
    {
        private readonly CommandManager _manager;
        public HelpCommand(CommandManager mgr) => _manager = mgr;

        public string Name => "help";
        public string Description => "Lists all available commands";
        public string Usage => "Usage: help <command>(optional)";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => new[] { "man" };

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                _manager.ShowAll(tool);
                return;
            }

            if (args.Contains("--help"))
            {
                _manager.ShowCommandHelp(this, tool);
                return;
            }

            var name = args[0].ToLower();

            if (!_manager.TryGet(name, out var cmd))
            {
                tool.WriteLine($"No manual entry for '{name}'.");
                return;
            }

            _manager.ShowCommandHelp(cmd, tool);
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("help(man): cannot be used in a pipe", "Red");
        }
    }
}
