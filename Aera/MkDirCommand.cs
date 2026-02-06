using System;
using System.IO;

namespace Aera
{
    internal class MkdirCommand : ICommand
    {
        public string Name => "mkdir";
        public string Description => "Creates a directory";
        public string Usage => "Usage: mkdir <directory>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => true;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length != 1)
            {
                tool.WriteLine("Usage: mkdir <directory>");
                return;
            }

            string path = Path.GetFullPath(args[0]);

            if (Directory.Exists(path))
            {
                tool.WriteLineColor("mkdir: directory already exists", "Yellow");
                return;
            }

            try
            {
                Directory.CreateDirectory(path);
                tool.WriteLineColor("Directory created.", "Green");
            }
            catch (UnauthorizedAccessException)
            {
                tool.WriteLineColor("mkdir: permission denied", "Red");
            }
            catch (Exception ex)
            {
                tool.WriteLineColor($"mkdir: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColor("mkdir: cannot be used in a pipe", "Red");
        }
    }
}
