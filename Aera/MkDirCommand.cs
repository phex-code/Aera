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

        public void Execute(string[] args, _s tool)
        {
            if (args.Length != 1)
            {
                tool.cwl("Usage: mkdir <directory>");
                return;
            }

            string path = Path.GetFullPath(args[0]);

            if (Directory.Exists(path))
            {
                tool.cwlc("mkdir: directory already exists", "Yellow");
                return;
            }

            try
            {
                Directory.CreateDirectory(path);
                tool.cwlc("Directory created.", "Green");
            }
            catch (UnauthorizedAccessException)
            {
                tool.cwlc("mkdir: permission denied", "Red");
            }
            catch (Exception ex)
            {
                tool.cwlc($"mkdir: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, _s tool)
        {
            tool.cwlc("mkdir: cannot be used in a pipe", "Red");
        }
    }
}
