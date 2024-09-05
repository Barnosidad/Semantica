using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace Semantica
{
    /*
        ? 1. Colocar el numero de linea en errores lexicos y sintacticos
        ? 2. Cambiar la clase token por atributos publicos utilizando el Get y el Set
        ? 3. Cambiar los constructores de la clase lexico usando parametros por default
        ? Error semantico al tener valores diferentes
        ? Busca y cambia valores de variables, castea o suelta errores
        ? Buscar tipos de datos o si es posible la asignacion
        ! 5. Asignacion -> Id = .Expresion; 1 solo esta opcion se evalua semanticamente
        !     Id -> ++; | Id -> --;| Id -> +=Expresion; | Id -> -=Expresion; 
        !     Id -> *= Expresion; | Id -> /=Expresion; | Id %=Expresion;

        
    */
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Stack<float> s;

        public Lenguaje(String nombre) : base(nombre)
        {
            listaVariables = new List<Variable>();
            s = new Stack<float>();
        }
        public Lenguaje() : base()
        {
            listaVariables = new List<Variable>();
            s = new Stack<float>();
        }
        // Programa  -> Librerias? Main
        public void Programa()
        {
            if (Contenido == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
        }
        // Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(Tipos.FinSentencia);
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        Variable.TipoDato Tipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        private bool ValidarInt(int valor, int maximo)
        {
            return valor >= 65535;
        }
        // Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = Tipo(Contenido);
            match(Tipos.TipoDato);
            Lista_Identificadores(tipo);
            match(Tipos.FinSentencia);
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        public void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        private void imprimeVariables()
        {
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.Nombre + " (" + v.Tipo + ") " + v.Valor);
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void Lista_Identificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(Contenido, t));
            match(Tipos.Identificador);
            if (Contenido == ",")
            {
                match(",");
                Lista_Identificadores(t);
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match(Tipos.Inicio);
            if (Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones();
            }
            match(Tipos.Fin);
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones();
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables? | Asignacion
        private void Instruccion()
        {
            if (Contenido == "Console")
            {
                Console();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }
        }
        /* Asignacion -> Identificador = Expresion;
        ! 5. Asignacion -> Id = Expresion; 1 solo esta opcion se evalua semanticamente
        !     Id -> ++; | Id -> --;| Id -> +=Expresion; | Id -> -=Expresion; 
        !     Id -> *= Expresion; | Id -> /=Expresion; | Id %=Expresion;
        */
        private void Asignacion()
        {
            string variable = Contenido;
            match(Tipos.Identificador);
            switch (Contenido)
            {
                case "=":
                    match(Tipos.Asignacion);
                    Expresion();                                    
                    break;
                case "++":
                    match(Tipos.IncTermino);
                    break;
                case "--":
                    match(Tipos.IncTermino);
                    break;
                case "+=":
                    match(Tipos.IncTermino);
                    Expresion();
                    break;
                case "-=":
                    match(Tipos.IncTermino);
                    Expresion();
                    break;
                case "*=":
                    match(Tipos.IncFactor);
                    Expresion();
                    break;
                case "/=":
                    match(Tipos.IncFactor);
                    Expresion();
                    break;
                case "%=":
                    match(Tipos.IncFactor);
                    Expresion();
                    break;
            }
            match(Tipos.FinSentencia);
            log.WriteLine(variable + " = " + s.Pop());   
            imprimeStack();
            
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
            if (Clasificacion == Tipos.Inicio)
            {
                BloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (Contenido == "else")
            {
                match("else");
                if (Clasificacion == Tipos.Inicio)
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
            if (Clasificacion == Tipos.Inicio)
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
            if (Clasificacion == Tipos.Inicio)
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
            if (Clasificacion == Tipos.Inicio)
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
            if (Contenido == "++")
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
            if (Contenido == "WriteLine" || Contenido == "Write")
            {
                match(Contenido);
                match("(");
                if (Clasificacion == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");
            }
            else
            {
                if (Contenido == "ReadLine")
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
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
                match(Tipos.OpTermino);
                Termino();
                float R2 = s.Pop();
                float R1 = s.Pop();
                switch (operador)
                {
                    case "+": s.Push(R1 + R2); break;
                    case "-": s.Push(R1 - R2); break;
                }
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
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
                match(Tipos.OpFactor);
                Factor();
                float R2 = s.Pop();
                float R1 = s.Pop();
                switch (operador)
                {
                    case "*": s.Push(R2 * R1); break;
                    case "/": s.Push(R1 / R2); break;
                    case "%": s.Push(R1 % R2); break;
                }
            }
        }
        private void imprimeStack()
        {
            log.WriteLine("Stack: ");
            foreach (float e in s.Reverse())
            {
                log.Write(e + " " );
            }
            log.WriteLine();
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                s.Push(float.Parse(Contenido));
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                // Casteo a nivel sintactico
                if (Clasificacion == Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
            }
        }
    }
}