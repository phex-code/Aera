using System.Diagnostics;

namespace Aera.Commands
{
    internal class NanoCommand : ICommand
    {
        public string Name => "nano";
        public string Description => "Edit document contents";
        public string Usage => "Usage: nano <file>";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;

        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, ShellContext tool)
        {
            if (args.Length == 0)
            {
                tool.WriteLineColored("nano: missing file operand", "Red");
                return;
            }

            string file = args[0];

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "python3", // or "python"
                    Arguments = $"nano.py \"{file}\"",
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                var process = Process.Start(psi);

                process.WaitForExit(); // <<< THIS is the important part
            }
            catch (Exception ex)
            {
                tool.WriteLineColored($"nano error: {ex.Message}", "Red");
            }
        }

        public void ExecutePipe(string input, string[] args, ShellContext tool)
        {
            tool.WriteLineColored("nano: cannot be used in a pipe", "Red");
        }
    }
}