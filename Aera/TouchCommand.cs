using System;
using System.IO;
using System.Linq;

namespace Aera
{
    internal class TouchCommand : ICommand
    {
        public string Name => "touch";
        public string Description => "Creates files or updates timestamps";
        public string Usage => "Usage: touch <file>";

        public bool AcceptsPipeInput => false;   // touch should not read stdin
        public bool IsDestructive => false;       // creating files is allowed

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, _s tool)
        {
            if (args.Length == 0 || args.Contains("--help"))
            {
                ShowHelp(tool);
                return;
            }

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                    continue;

                string path = Path.GetFullPath(arg);

                try
                {
                    if (File.Exists(path))
                    {
                        File.SetLastWriteTime(path, DateTime.Now);
                        tool.cwlc($"Updated: {Path.GetFileName(path)}", "Yellow");
                    }
                    else
                    {
                        using (File.Create(path)) { }
                        tool.cwlc($"Created: {Path.GetFileName(path)}", "Green");
                    }
                }
                catch (Exception ex)
                {
                    tool.cwlc($"touch: {ex.Message}", "Red");
                }
            }
        }

        public void ExecutePipe(string input, string[] args, _s tool)
        {
            tool.cwl("touch: does not accept piped input");
        }

        private void ShowHelp(_s tool)
        {
            tool.cwl("Usage: touch <file> [file...]");
            tool.cwl("");
            tool.cwl("Creates empty files or updates modification times.");
        }
    }
}
