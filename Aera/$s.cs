using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Aera
{
    internal class _s
    {
        public bool IsSudo { get; set; }
        public bool CaptureMode { get; set; }

        private readonly StringBuilder buffer = new();
        public string[] uinf = new string[2];

        /* ================= OUTPUT ================= */

        public void cwl(string t)
        {
            if (CaptureMode)
                buffer.AppendLine(t);
            else
                Console.WriteLine(t);
        }

        public void cw(string t)
        {
            if (CaptureMode)
                buffer.Append(t);
            else
                Console.Write(t);
        }

        public void cwc(string t, string c)
        {
            if (CaptureMode)
            {
                buffer.Append(t);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(c, true, out var color))
                Console.ForegroundColor = color;

            Console.Write(t);
            Console.ResetColor();
        }

        public void cwlc(string t, string c)
        {
            if (CaptureMode)
            {
                buffer.AppendLine(t);
                return;
            }

            if (Enum.TryParse<ConsoleColor>(c, true, out var color))
                Console.ForegroundColor = color;

            Console.WriteLine(t);
            Console.ResetColor();
        }

        public string FlushPipe()
        {
            string result = buffer.ToString();
            buffer.Clear();
            return result;
        }

        public string cgi() => Console.ReadLine();

        /* ================= USER BOOTSTRAP ================= */

        public string[] cu()
        {
            cwl("Create User");

            string username;
            do
            {
                cwc("Enter username: ", "Yellow");
                username = cgi();
            } while (string.IsNullOrWhiteSpace(username));

            string password;
            do
            {
                cwc("Enter password: ", "Yellow");
                password = cgi();
            } while (string.IsNullOrWhiteSpace(password));

            uinf[0] = username;
            uinf[1] = password;

            File.WriteAllLines("user.ss", uinf);

            cwlc($"User {username} created.", "Green");
            Thread.Sleep(1200);
            Console.Clear();

            return uinf;
        }

        public void uwinf(string[] inf)
        {
            uinf = inf;
        }

        public void clog()
        {
            while (true)
            {
                cwc("Password: ", "Cyan");
                string pass = cgi();

                if (pass == uinf[1])
                {
                    cwlc("Login success", "Green");
                    Thread.Sleep(1000);
                    Console.Clear();
                    return;
                }

                cwlc("Invalid password", "Red");
                Thread.Sleep(1000);
                Console.Clear();
            }
        }

        /* ================= USER INFO ================= */

        public void ShowUser(bool sudo)
        {
            cwlc("User Information:", "DarkCyan");
            cwlc($" - Username: {uinf[0]}", "DarkCyan");

            if (!sudo)
                cwlc($" - Password: {"".PadLeft(uinf[1].Length, '*')}", "DarkCyan");
            else
                cwlc($" - Password: {uinf[1]}", "DarkCyan");
        }

        public string un() => uinf[0];

        /* ================= SUDO ================= */

        public bool AuthenticateSudo()
        {
            cwc("[sudo] Enter password: ", "Yellow");
            string attempt = cgi();

            if (attempt != uinf[1])
            {
                cwlc("No sudo: authentication failed.", "Red");
                return false;
            }

            cwlc("Access granted.", "Green");
            IsSudo = true;
            return true;
        }
        public bool Confirm(string message, bool defaultYes = false)
        {
            var suffix = defaultYes ? "(Y/n)" : "(y/N)";
            cwc("! ", "yellow");
            cw($"{message} {suffix} ");

            var input = cgi()?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(input))
                return defaultYes;

            return input == "y" || input == "yes";
        }

    }
}