using System;
using System.IO;

namespace Aera
{
    internal class CatCommand : ICommand
    {
        public string Name => "cat";
        public string Description => "Prints file contents";
        public string Usage => "Usage: cat <file>";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, _s tool)
        {
            if (args.Length != 1)
            {
                tool.cwl("Usage: cat <file>");
                return;
            }

            string path = Path.GetFullPath(args[0]);

            if (!File.Exists(path))
            {
                tool.cwlc("cat: file not found", "Red");
                return;
            }

            if ((File.GetAttributes(path) & FileAttributes.Directory) != 0)
            {
                tool.cwlc("cat: cannot read a directory", "Red");
                return;
            }

            foreach (var line in File.ReadLines(path))
                tool.cwl(line);
        }

        public void ExecutePipe(string input, string[] args, _s tool)
        {
            if (args.Length != 0)
            {
                tool.cwlc("cat: cannot combine piped input with files", "Red");
                return;
            }

            if (!string.IsNullOrWhiteSpace(input))
                tool.cwl(input.TrimEnd());
        }
    }
}
