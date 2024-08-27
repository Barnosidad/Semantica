using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    //Requerimiento: Número de linea donde se encuentra el error
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log) : base("Error: " + mensaje)
        {
            log.WriteLine("Error: " + mensaje);
        }
    }
}