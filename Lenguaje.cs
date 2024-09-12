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
        ? char => 0 a 255
        ? int => 0 a 65535
        ? float => 0 a deus
        ? Anadir mas a expresion
    */
    public class Lenguaje : Sintaxis
    {
        private List <Variable> listaVariables;
        private Stack <float> s;

        public Lenguaje(String nombre) : base (nombre)
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
            if(Contenido == "using")
            {
                Librerias();
            }
        }
        Variable.TipoDato Tipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch(TipoDato)
            {
                case "int" : tipo = Variable.TipoDato.Int; break;
                case "float" : tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        // Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = Tipo(Contenido);
            match(Tipos.TipoDato);
            Lista_Identificadores(tipo);
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
        private void imprimeVariables()
        {
            foreach(Variable v in listaVariables)
            {
                log.WriteLine(v.Nombre + " (" + v.Tipo + ") " + v.Valor);
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void Lista_Identificadores(Variable.TipoDato t)
        {
            if(buscarVariable(Contenido))
            {
                throw new Error("Variable repetida (" + Contenido + ")", log,linea);
            }
            else
            {
                listaVariables.Add(new Variable(Contenido,t));
                match(Tipos.Identificador);
                if(Contenido == ",")
                {
                    match(",");
                    Lista_Identificadores(t);
                }
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
        private bool rangoTipos(float valor, Variable.TipoDato? tipo, ref Variable.TipoDato limite)
        {
            bool dentro;
            switch(tipo)
            {
                case Variable.TipoDato.Char: 
                if(Math.Abs(valor)<256) dentro = true;
                else
                {
                    if(Math.Abs(valor) > 65535) limite = Variable.TipoDato.Float;
                    else limite = Variable.TipoDato.Int;
                    dentro = false;
                }
                break;
                case Variable.TipoDato.Int:
                    if(Math.Abs(valor)<65536) dentro = true;
                    else
                    {
                        if(Math.Abs(valor) > 65535) limite = Variable.TipoDato.Float;
                        dentro = false;
                    }
                    break;
                default:
                    dentro = true;
                break;
            }
            return dentro;
        }
        // Asignacion -> Identificador = Expresion;
        //            -> Identificador ++; semanticamente, solo se anade
        //            -> Identificador --; semanticamente, solo se resta
        //            -> Identificador += Expresion;
        //            -> Identificador -= Expresion; 
        //            -> Identificador *= Expresion;
        //            -> Identificador /= Expresion;
        //            -> Identificador %= Expresion;
        private void Asignacion()
        {
            string variable = Contenido;
            if(!buscarVariable(variable))
            {
                throw new Error("Semantico, variable (" + Contenido + ") no delcarada previamente  ",log,linea);
            }
            else
            {
                match(Tipos.Identificador);
                string operacion = Contenido;
                Variable.TipoDato objetivo = Variable.TipoDato.Float;
                float valor = 0;
                switch(Clasificacion)
                {
                    case Tipos.Asignacion:
                        match(Tipos.Asignacion);
                        Expresion();
                        imprimeStack();
                        valor = s.Pop();
                    break;
                    case Tipos.IncTermino:
                        match(Tipos.IncTermino);
                        switch(operacion)
                        {
                            case "++" : valor++; break;
                            case "--" : valor--; break;
                            case "+=" : Expresion(); valor+= s.Pop(); break;
                            case "-=" : Expresion(); valor-= s.Pop(); break;
                        }
                    break;
                    case Tipos.IncFactor:
                        operacion = Contenido;
                        match(Tipos.IncFactor);
                        Expresion();
                        switch(operacion)
                        {
                            case "*=" : traeVariable(variable).Valor*=s.Pop(); break;
                            case "/=" : traeVariable(variable).Valor/=s.Pop(); break;
                            case "%=" : traeVariable(variable).Valor%=s.Pop(); break;
                        }
                    break;
                }
                if(!rangoTipos(traeVariable(variable).Valor, traeVariable(variable).Tipo, ref objetivo))
                {
                    throw new Error("Semantico, no es posible un dato de tipo (" + objetivo + ")" + " a un dato de tipo (" + traeVariable(variable).Tipo + ") en la linea: " + linea, log);
                }
                match(Tipos.FinSentencia);
            }
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
                string operador = Contenido;
                match(Tipos.OpTermino);
                Termino();
                float R2 = s.Pop();
                float R1 = s.Pop();
                switch(operador)
                {
                    case "+" : s.Push(R1 + R2); break;
                    case "-" : s.Push(R1 - R2); break;
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
            if(Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
                match(Tipos.OpFactor);
                Factor();
                float R2 = s.Pop();
                float R1 = s.Pop();
                switch(operador)
                {
                    case "*" : s.Push(R2 * R1); break;
                    case "/" : s.Push(R1 / R2); break;
                    case "%" : s.Push(R1 % R2); break;
                }
            }
        }
        private void imprimeStack()
        {
            log.WriteLine("Stack: ");
            foreach(float e in s.Reverse())
            {
                log.Write(e + " ");
            }
            log.WriteLine();
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if(Clasificacion == Tipos.Numero)
            {
                s.Push(float.Parse(Contenido));
                match(Tipos.Numero);
            }
            else if(Clasificacion == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                // Casteo a nivel sintactico
                if(Clasificacion == Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
            }
        }
        // Buscar variable
        private bool buscarVariable(string nombre)
        {
            return listaVariables.Exists(x => x.Nombre == nombre); 
        }
        private Variable traeVariable(string nombre)
        {
            return listaVariables.Find(x => x.Nombre == nombre) is null ? throw new Error("No existe esa variable en la linea " + linea, log) : listaVariables.Find(x => x.Nombre == nombre);
        }
    }
}