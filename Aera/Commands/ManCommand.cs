using System.IO;

namespace Aera.Commands
{
    internal class ManCommand : ICommand
    {
        public string Name => "man";
        public string Description => "Displays manual pages for commands";
        public string Usage => "Usage: man <command>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLineColored("man: missing command operand", "Red");
                return;
            }

            string command = args[0].ToLower();
            string manualPath = Path.Combine("Manuals", $"{command}.txt");

            if (!File.Exists(manualPath))
            {
                tool.WriteLine($"No manual entry for '{command}'.");
                return;
            }

            try
            {
                var content = File.ReadAllText(manualPath);
                tool.WriteLine(content);
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"man error: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("man: cannot be used in a pipe", "Red");
        }
    }
}