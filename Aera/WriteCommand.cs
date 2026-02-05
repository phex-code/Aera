using System;

namespace Aera
{
    internal class WriteCommand : ICommand
    {
        public string Name => "write";
        public string Description => "Writes formatted output to the console";
        public string Usage => "Usage: write <type> <text>";

        public bool AcceptsPipeInput => true;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, _s tool)
        {
            if (args.Length < 2)
            {
                tool.cwl("Usage: write <type> <text>");
                tool.cwl("Types: error, warning, success, text");
                return;
            }

            string type = args[0].ToLower();
            string text = string.Join(" ", args, 1, args.Length - 1);

            Write(type, text, tool);
        }

        // pipe support
        public void ExecutePipe(string input, string[] args, _s tool)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            string type = args.Length > 0 ? args[0].ToLower() : "text";

            Write(type, input.TrimEnd(), tool);
        }

        private void Write(string type, string text, _s tool)
        {
            switch (type)
            {
                case "error":
                    tool.cwc("ERROR: ", "Red");
                    tool.cwl(text);
                    break;

                case "warning":
                    tool.cwc("WARNING: ", "Yellow");
                    tool.cwl(text);
                    break;

                case "success":
                    tool.cwc("SUCCESS: ", "Green");
                    tool.cwl(text);
                    break;

                case "text":
                    tool.cwl(text);
                    break;

                default:
                    tool.cwl($"write: unknown type '{type}'");
                    tool.cwl("Types: error, warning, success, text");
                    break;
            }
        }
    }
}
