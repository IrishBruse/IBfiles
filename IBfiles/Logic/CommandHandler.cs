namespace IBfiles.Logic;

using System;
using System.Diagnostics;

public static class CommandHandler
{
    public static void Run(string file, string args, string input)
    {
        string arguments = args.Replace("%1", input);
        Process proc = new()
        {
            StartInfo = new()
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true
            }
        };

        proc.StartInfo.FileName = file;
        proc.StartInfo.Arguments = arguments;
        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        proc.StartInfo.UseShellExecute = false;

        proc.Start();

        proc.WaitForExit();

        string stdErr = proc.StandardError.ReadToEnd();

        if (!string.IsNullOrEmpty(stdErr))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(stdErr);
            Console.ResetColor();
        }

        string stdOut = proc.StandardOutput.ReadToEnd();

        if (!string.IsNullOrEmpty(stdOut))
        {
            Console.WriteLine(stdOut);
        }
    }
}
