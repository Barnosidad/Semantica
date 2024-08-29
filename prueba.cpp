using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(String[] args)
{
    char e;
    float pi;
    int rex;
    Console.Write("Proyecto 6");
    Console.WriteLine(" - ITQ");
    Console.ReadLine();
    pi = (3 + 5) * 8 - (10 - e) / 2; // 61;
    pi++;                            // 62
    e--;                             // 3
    pi += e;                         // 65
    pi -= 5;                         // 60
    e -= 3;                          // 0
    pi *= 10;                        // 600
    e += 2;                          // 2
    pi /= e;                         // 300
    rex=pi;
    rex%=2;
    int a;
    if (1 == 2)
    {
        if (2 == 2)
            Console.WriteLine("Entró al IF");
        a = 100;
    }
    else
    {
        a = 200;
        Console.WriteLine("Entró al ELSE");
    }
}