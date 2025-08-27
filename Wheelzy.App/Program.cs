using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Wheelzy App running...");
        // Example: process the current folder (be careful — this will rewrite .cs files)
        var changed = await CsCodeFixer.ProcessFolderAsync(Environment.CurrentDirectory);
        Console.WriteLine($"Changed files: {changed}");
    }
}
