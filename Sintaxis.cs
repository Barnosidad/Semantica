using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Sintaxis : Lexico
    {
        public Sintaxis(string nombre) : base(nombre)
        {
            nextToken();
        }
        public Sintaxis() : base()
        {
            nextToken();
        }
        public void match(string espera)
        {
            if (Contenido == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un (" + espera + ") y lo que se recibio fue (" + Contenido + ")", log, linea);
            }
        }
        public void match(Tipos espera)
        {
            if (Clasificacion == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un (" + espera + ") y lo que se recibio fue (" + Contenido + ")", log,linea);
            }
        }
    }
}