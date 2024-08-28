using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Sintaxis_1;

namespace Semantica
{
    /*
        ? 1. Colocar el numero de linea en errores lexicos y sintacticos
        ? 2. Cambiar la clase token por atributos publicos utilizando el Get y el Set
        ? 3. Cambiar los constructores de la clase lexico usando parametros por default
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
            if(Contenido == "using")
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
            if(Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        public void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if(Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void Lista_Identificadores()
        {
            match(Tipos.Identificador);
            if(Contenido == ",")
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
        private void Instruccion()
        {

        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {

        }
        /* If -> if (Condicion) bloqueInstrucciones | instruccion
            (else bloqueInstrucciones | instruccion)?
        */
        private void If()
        {

        }
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {

        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {

        }
        /* Do -> do 
                bloqueInstrucciones | intruccion 
            while(Condicion);
        */
        private void Do()
        {

        }
        /* For -> for(Asignacion Condicion; Incremento) 
            BloqueInstrucciones | Intruccion
        */
        private void For()
        {

        }
        // Incremento -> Identificador ++ | --
        private void Incremento()
        {

        }
        /* Console -> Console.(WriteLine|Write) (cadena); |
                Console.(Read | ReadLine) ();
        */
        private void Console()
        {

        }
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
        private void Expresion()
        {

        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {

        }
        // Termino -> Factor PorFactor
        private void Termino()
        {

        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            
        }
    }
}