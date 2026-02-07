using System;
using System.IO;
using System.Linq;

namespace Aera
{
    internal class DuCommand : ICommand
    {
        public string Name => "du";
        public string Description => "Shows directory disk usage";
        public string Usage => "Usage: du [directory]";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            string dir = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

            if (!Directory.Exists(dir))
            {
                tool.WriteLineColored("Directory not found.", "Red");
                return;
            }

            long size = Directory
                .EnumerateFiles(dir, "*", SearchOption.AllDirectories)
                .Sum(f => new FileInfo(f).Length);

            tool.WriteLine($"{size} bytes");
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            Execute(new[] { input.Trim() }, tool);
        }
    }
}