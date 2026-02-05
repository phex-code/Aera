namespace Aera
{
    internal class UserInfoCommand : ICommand
    {
        public string Name => "userinfo";
        public string Description => "Displays user information";
        public string Usage => "Usage: <sudo>(optional) userinfo";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, _s tool)
        {
            if (args.Length != 0)
            {
                tool.cwl("Usage: userinfo");
                return;
            }

            tool.ShowUser(false);
        }

        public void ExecutePipe(string input, string[] args, _s tool)
        {
            tool.cwl("userinfo: does not accept piped input");
        }
    }
    internal class WhoAmICommand : ICommand
    {
        public string Name => "whoami";
        public string Description => "Displays user name";
        public string Usage => "Usage: whoami";

        public bool AcceptsPipeInput => false;
        public bool IsDestructive => false;
        public string[] Aliases => Array.Empty<string>();

        public void Execute(string[] args, _s tool)
        {
            if (args.Length != 0)
            {
                tool.cwl("Usage: whoami");
                return;
            }

            tool.cwlc(tool.un(), "Blue");
        }

        public void ExecutePipe(string input, string[] args, _s tool)
        {
            tool.cwl("whoami: does not accept piped input");
        }
    }
}
