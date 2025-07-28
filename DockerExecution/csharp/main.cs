using System;

class Program {
    static void Main() {
        int n = int.Parse(Console.ReadLine());
        int sum = 0;
        for (int i = 1; i <= n; i++) {
            if (i % 2 == 1) sum += i;
        }
        Console.WriteLine(sum);
    }
}