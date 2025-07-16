using System;

class Program
{
    static void Main()
    {
        string[] parts = Console.ReadLine().Split();
        int a = int.Parse(parts[0]);
        int b = int.Parse(parts[1]);
        Console.WriteLine(a + b);
    }
}