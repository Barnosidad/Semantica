using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Sintaxis_1;

namespace Semantica
{
    /*
        ? 1. Colocar el numero de linea en errores lexicos y sintacticos
        ! 2. Cambiar la clase token por atributos publicos utilizando el Get y el Set
        ! 3. Cambiar los constructores de la clase lexico usando parametros por default
    */
    public class Lenguaje : Sintaxis
    {
        public Lenguaje()
        {
            
        }
        
        public Lenguaje(String nombre) : base (nombre)
        {

        }
        // Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Librerias();
            Variables();
            Main();
        }
        // Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(Tipos.FinSentencia);
            if(getContenido() == "using")
            {
                Librerias();
            }
        }
        // Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            match(Tipos.TipoDato);
            Lista_Identificadores();
            match(Tipos.FinSentencia);
            if(getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        public void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if(getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void Lista_Identificadores()
        {
            match(Tipos.Identificador);
            if(getContenido() == ",")
            {
                match(",");
                Lista_Identificadores();
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {

        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            
        }
        // Instruccion -> Console | If | While | do | For | Asignacion
        // Asignacion -> Identificador = Expresion;
        /* If -> if (Condicion) bloqueInstrucciones | instruccion
            (else bloqueInstrucciones | instruccion)?
        */
        //Condicion -> Expresion operadorRelacional Expresion
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        /* Do -> do 
                bloqueInstrucciones | intruccion 
            while(Condicion);
        */
        /* For -> for(Asignacion Condicion; Incremento) 
            BloqueInstrucciones | Intruccion
        */
        // Incremento -> Identificador ++ | --
        /* Console -> Console.(WriteLine|Write) (cadena); |
                Console.(Read | ReadLine) ();
        */
        // Main      -> static void Main(string[] args) BloqueInstrucciones
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("String");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones();
        }
        // Expresion -> Termino MasTermino
        // MasTermino -> (OperadorTermino Termino)?
        // Termino -> Factor PorFactor
        // PorFactor -> (OperadorFactor Factor)?
        // Factor -> numero | identificador | (Expresion)
    }
}