using System;
using System.IO;

namespace Aera
{
    internal class LsCommand : ICommand
    {
        public string Name => "ls";
        public string Description => "Lists files and folders in the current directory";
        public string Usage => "Usage: ls";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, _s tool)
        {
            string dir = Directory.GetCurrentDirectory();

            tool.cwlc($"Directory: {dir}", "DarkCyan");

            foreach (var d in Directory.GetDirectories(dir))
                tool.cwlc($"[DIR]  {Path.GetFileName(d)}", "Yellow");

            foreach (var f in Directory.GetFiles(dir))
                tool.cwl($"[FILE] {Path.GetFileName(f)}");
        }

        public void ExecutePipe(string input, string[] args, _s tool)
        {
            tool.cwlc("ls: cannot accept piped input", "Red");
        }
    }
}
