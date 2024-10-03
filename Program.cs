using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lenguaje L = new Lenguaje())
                {
                    
                    int i,j;
                    for (i = 1; i <= 5; i++)
                    {
                        for (j = 1; j <= i; j++)
                        {
                            if (j % 2 == 0)
                                Console.Write("*");
                            else
                                Console.Write("-");
                        }
                        Console.WriteLine("");
                    }
                    L.Programa();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
