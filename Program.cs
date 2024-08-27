using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Sintaxis L = new Sintaxis("prueba.cpp"))
                {
                    L.match(Token.Tipos.Numero);
                    L.match("+"); // L.match(Token.tipos.OpTermino);
                    L.match(Token.Tipos.Identificador);
                    L.match(Token.Tipos.FinSentencia);
                    /*
                    while(!L.finArchivo())
                    {
                        L.nextToken();
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
