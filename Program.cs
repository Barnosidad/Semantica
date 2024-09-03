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
