namespace IBfiles.Logic;

using System.Diagnostics;

public static class CommandHandler
{
    public static void Run(string file, string args, string input)
    {
        string arguments = args.Replace("%1", input);
        Process proc = new();

        proc.StartInfo.FileName = file;
        proc.StartInfo.Arguments = arguments;
        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        proc.StartInfo.UseShellExecute = true;

        proc.Start();

        proc.WaitForExit();
    }
}
