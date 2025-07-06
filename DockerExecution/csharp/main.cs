using System;

class Program
{
    static void Main()
    {
        string line = Console.ReadLine();
        if (line == null)
        {
            Console.WriteLine("No input provided.");
            return;
        }

        string[] parts = line.Split();
        int a = int.Parse(parts[0]);
        int b = int.Parse(parts[1]);

        Console.WriteLine(a + b);
    }
}