using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

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
        // Programa  -> Librerias? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
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
            match(Tipos.Inicio);
            if(Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones();
            }
            match(Tipos.Fin);
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if(Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones();
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables? | Asignacion
        private void Instruccion()
        {
            if(Contenido == "Console")
            {
                Console();
            }
            else if(Contenido == "if")
            {
                If();
            }
            else if(Contenido == "while")
            {
                While();
            }
            else if(Contenido == "do")
            {
                Do();
            }
            else if(Contenido == "for")
            {
                For();
            }
            else if(Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }
        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            match(Tipos.Identificador);
            match(Tipos.Asignacion);
            Expresion();
            match(Tipos.FinSentencia);
        }
        /* If -> if (Condicion) bloqueInstrucciones | instruccion
            (else bloqueInstrucciones | instruccion)?
        */
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if(Clasificacion == Tipos.Inicio)
            {
                BloqueInstrucciones();
            }
            else 
            {
                Instruccion();
            }
            if(Contenido == "else")
            {
                match("else");
                if(Clasificacion == Tipos.Inicio)
                {
                    BloqueInstrucciones();
                }
                else 
                {
                    Instruccion();
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OpRelacional);
            Expresion();
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if(Clasificacion == Tipos.Inicio)
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }
        /* Do -> do 
                bloqueInstrucciones | intruccion 
            while(Condicion);
        */
        private void Do()
        {
            match("do");
            if(Clasificacion == Tipos.Inicio)
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(Tipos.FinSentencia);
        }
        /* For -> for(Asignacion; Condicion; Incremento) 
            BloqueInstrucciones | Intruccion
        */
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            match(Tipos.FinSentencia);
            Condicion();
            match(Tipos.FinSentencia);
            Incremento();
            match(")");
            if(Clasificacion == Tipos.Inicio)
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }
        // Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if(Contenido == "++")
            {
                match("++");
            }
            else
            {
                match("--");
            }
        }
        /* Console -> Console.(WriteLine|Write) (cadena?); |
                Console.(Read | ReadLine) ();
        */
        private void Console()
        {
            match("Console");
            match(".");
            if(Contenido == "WriteLine" || Contenido == "Write")
            {
                match(Contenido);
                match("(");
                if(Clasificacion == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");
            }
            else
            {
                if(Contenido == "ReadLine")
                {
                    match("ReadLine");
                }
                else
                {
                    match("Read");
                }
                match("(");
                match(")");
            }
            match(Tipos.FinSentencia);
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
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if(Clasificacion == Tipos.OpTermino)
            {
                match(Tipos.OpTermino);
                Termino();
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if(Clasificacion == Tipos.OpFactor)
            {
                match(Tipos.OpFactor);
                Factor();
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if(Clasificacion == Tipos.Numero)
            {
                match(Tipos.Numero);
            }
            else if(Clasificacion == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}