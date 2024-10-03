using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace Semantica
{
    /*
        Hecho:  1. Colocar el numero de linea en errores lexicos y sintacticos
        Hecho:  2. Cambiar la clase token por atributos publicos utilizando el Get y el Set
        Hecho:  3. Cambiar los constructores de la clase lexico usando parametros por default
        Hecho:  Error semantico al tener valores diferentes
        ! Busca y cambia valores de variables, castea o suelta errores
        ? Buscar tipos de datos o si es posible la asignacion
        ? char => 0 a 255
        ? int => 0 a 65535
        ? float => 0 a deus
        ? Anadir mas a expresion
        ! Parte dos la venganza
        Hecho:  1. Usar el metodo find en lugar de foreach
        Hecho:  2.  Validar que no existan las variables duplicadas
        Hecho:  3.  Validar que existan las variables en las expresiones matematicas
            * en la asignacion pues
        Hecho:  4. 1.5 + 1.4 = 3 <- Float porque float + float = float
        Hecho: Meter el valor de la variable al stack
        Hecho: Asignar una expresion matematica a la variable al momento de declararla
        Hecho: Emular if
        Hecho: Emular read y read line, validar que en read line se capturen solo numeros e implementar una excepcion
        Hecho: Emular el do
        Hecho: Emular el for -- vale 20 puntos, a medias
        Hecho: Emular el while -- vale 15 points, a medias
        Hecho: Emular el write y write line
        Hecho: Castear, quitar la parte alta del byte
        Hecho: 14. Desarrollar lista de concatenaciones, variables separadas por comas
        Trabajando: El for aumenta al final y compara al principio
    */
    public class Lenguaje : Sintaxis
    {
        private List <Variable> listaVariables;
        private Stack <float> s;
        private Variable.TipoDato tipoDatoExpresion;
        public Lenguaje(String nombre) : base (nombre)
        {
            log.WriteLine("Analisis sintactico");
            listaVariables = new List<Variable>();
            s = new Stack<float>();
        }
        public Lenguaje() : base()
        {
            log.WriteLine("Analizador sintactico");
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
            log.WriteLine("---Lista de variables---");
            foreach(Variable v in listaVariables)
            {
                log.WriteLine(v.Nombre + " (" + v.Tipo + ") " + v.Valor);
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        // ListaIdentificadores -> identificador (asignacion, listaIdentificadores | ,ListaIdentificadores)?, este reemplazaria al de arriba
        private void Lista_Identificadores(Variable.TipoDato t)
        {
            if(listaVariables.Exists(x => x.Nombre == Contenido))
            {
                throw new Error("Variable repetida (" + Contenido + ") en la linea: " + linea, log);
            }
            else
            {
                listaVariables.Add(new Variable(Contenido,t));
                string id_v = Contenido;
                int cTemp = caracter - (Contenido.Length + 1), lTemp = linea;
                match(Tipos.Identificador);
                if(Clasificacion == Tipos.Asignacion)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    Asignacion(true);
                    if(Contenido == ",")
                    {
                        match(",");
                        Lista_Identificadores(t);
                    }
                }
                else if(Contenido == ",")
                {
                    match(",");
                    Lista_Identificadores(t);
                }
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecutar)
        {
            match(Tipos.Inicio);
            if(Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones(ejecutar);
            }
            match(Tipos.Fin);
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if(Clasificacion != Tipos.Fin)
            {
                ListaInstrucciones(ejecutar);
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables? | Asignacion
        private void Instruccion(bool ejecutar)
        {
            if(Contenido == "Console")
            {
                Console(ejecutar);
            }
            else if(Contenido == "if")
            {
                If(ejecutar);
            }
            else if(Contenido == "while")
            {
                While(ejecutar);
            }
            else if(Contenido == "do")
            {
                Do(ejecutar);
            }
            else if(Contenido == "for")
            {
                For(ejecutar);
            }
            else if(Clasificacion == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion(ejecutar);
                match(Tipos.FinSentencia);
            }
        }
        private Variable.TipoDato valorToTipo(float valor)
        {
            if(Math.Abs(valor) % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            else if(Math.Abs(valor) <= 255) return Variable.TipoDato.Char;
            else if(Math.Abs(valor) <= 65535) return Variable.TipoDato.Int;
            else return Variable.TipoDato.Float;
        }
        private bool analisisSemantico(Variable v, float nuevoValor)
        {
            if(v.Tipo < tipoDatoExpresion) return false;
            else if(nuevoValor%1 == 0)
            {
                if(v.Tipo == Variable.TipoDato.Char)
                {
                    if (nuevoValor <= 255) return true;
                    return false;
                }
                if(v.Tipo == Variable.TipoDato.Int)
                {
                    if (nuevoValor <= 65536) return true;
                    return false;
                }
                return true;
            }
            else
            {
                if(v.Tipo == Variable.TipoDato.Char || v.Tipo == Variable.TipoDato.Int) return false;
                return true;
            }
        }
        // Asignacion -> Identificador = Expresion;
        //            -> Identificador ++; semanticamente, solo se anade
        //            -> Identificador --; semanticamente, solo se resta
        //            -> Identificador += Expresion;
        //            -> Identificador -= Expresion; 
        //            -> Identificador *= Expresion;
        //            -> Identificador /= Expresion;
        //            -> Identificador %= Expresion;
        private void Asignacion(bool ejecutar)
        {
            string variable = Contenido;
            match(Tipos.Identificador);
            string operacion = Contenido;
            Variable v = listaVariables.Find(x => x.Nombre == variable) ?? throw new Error("Semantico, la variable de nombre (" + variable + ") no existe", log,linea);
            float nuevoValor = v.Valor;
            tipoDatoExpresion = Variable.TipoDato.Char;
            switch(Clasificacion)
            {
                case Tipos.Asignacion:
                    match(Tipos.Asignacion);
                    if(Contenido == "Console")
                    {
                        string aux_s;
                        char aux_c;
                        match(Contenido);
                        match(".");
                        if(Contenido == "Read")
                        {
                            match("Read");
                            match("(");
                            aux_c = (char) System.Console.Read();
                            if(!char.IsNumber(aux_c)) throw new Error("Semantico, por favor ingresa solo numeros en la cadena", log, linea);
                            else nuevoValor = float.Parse(Char.ToString(aux_c));
                            match(")");
                        }
                        else
                        {
                            match("ReadLine");
                            match("(");
                            aux_s = System.Console.ReadLine() ?? throw new Error("Semantico, introduce algo para leer", log, linea);
                            if(!aux_s.All(x => Char.IsNumber(x) || Char.IsPunctuation(x))) throw new Error("Semantico, por favor ingresa solo numeros en la cadena", log, linea);
                            else nuevoValor = float.Parse(aux_s);
                            match(")");
                        }
                    }
                    else
                    {
                        Expresion();
                        nuevoValor = s.Pop();
                    }
                break;
                case Tipos.IncTermino:
                    match(Tipos.IncTermino);
                    switch(operacion)
                    {
                        case "++" : nuevoValor++; break;
                        case "--" : nuevoValor--; break;
                        case "+=" : Expresion(); nuevoValor += s.Pop(); break;
                        case "-=" : Expresion(); nuevoValor -= s.Pop(); break;
                    }
                break;
                case Tipos.IncFactor:
                    operacion = Contenido;
                    match(Tipos.IncFactor);
                    Expresion();
                    switch(operacion)
                    {
                        case "*=" : nuevoValor *=s.Pop(); break;
                        case "/=" : nuevoValor /=s.Pop(); break;
                        case "%=" : nuevoValor %=s.Pop(); break;
                    }
                break;
            }
            // match(Tipos.FinSentencia);
            if(tipoDatoExpresion < valorToTipo(nuevoValor)) tipoDatoExpresion = valorToTipo(nuevoValor);
            if(analisisSemantico(v,nuevoValor))
            {
                if(ejecutar) v.Valor = nuevoValor;
            }
            else
            {
                throw new Error("Semantico, no puedo asignar un (" + tipoDatoExpresion + ") a un tipo (" + v.Tipo + ")", log, linea);
            }
            log.WriteLine("El nuevo valor de " + v.Nombre + ", es: " + v.Valor);
        }
        /* If -> if (Condicion) bloqueInstrucciones | instruccion
            (else bloqueInstrucciones | instruccion)?
        */
        private void If(bool ejecutar)
        {
            match("if");
            match("(");
            bool resultado = Condicion();
            match(")");
            if(Clasificacion == Tipos.Inicio)
            {
                BloqueInstrucciones(resultado && ejecutar);
            }
            else 
            {
                Instruccion(resultado && ejecutar);
            }
            if(Contenido == "else")
            {
                match("else");
                if(Clasificacion == Tipos.Inicio)
                {
                    BloqueInstrucciones(!resultado && ejecutar);
                }
                else 
                {
                    Instruccion(!resultado && ejecutar);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion();
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion();
            float R2 = s.Pop();
            float R1 = s.Pop();
            switch(operador)
            {
                case ">":
                    return R1 > R2;
                case ">=":
                return R1 >= R2;
                case "<":
                    return R1 < R2;
                case "<=":
                    return R1 <= R2;
                case "==":
                    return R1 == R2;
                default:
                    return R1 != R2;
            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecutar)
        {
            int cTemp = caracter - 6, lTemp = linea;
            bool resultado = false;
            do
            {
                match("while");
                match("(");
                resultado = Condicion() && ejecutar;
                match(")");
                if(Clasificacion == Tipos.Inicio)
                {
                    BloqueInstrucciones(ejecutar && resultado);
                }
                else
                {
                    Instruccion(ejecutar && resultado);
                }
                if(resultado)
                { 
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                }
            }while(resultado);
        }
        /* Do -> do 
                bloqueInstrucciones | intruccion 
            while(Condicion);
        */
        private void Do(bool ejecutar)
        {
            int cTemp = caracter - 3, lTemp = linea;
            bool resultado = false;
            do
            {
                match("do");
                if(Clasificacion == Tipos.Inicio)
                {
                    BloqueInstrucciones(ejecutar & resultado);
                }
                else
                {
                    Instruccion(ejecutar && resultado);
                }
                match("while");
                match("(");
                resultado = Condicion() && ejecutar;
                match(")");
                match(Tipos.FinSentencia);
                if(resultado)
                { 
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                }
            }while(resultado);
        }
        /* For -> for(Asignacion; Condicion; Incremento) 
            BloqueInstrucciones | Intruccion
        */
        private void For(bool ejecutar)
        {
            match("for");
            match("(");
            if(Clasificacion == Tipos.TipoDato) Variables();
            else if(Clasificacion == Tipos.Identificador)
            {
                Asignacion(ejecutar);
                match(Tipos.FinSentencia);
            }
            // ! esto seria la condicion
            int cTemp = caracter, lTemp = linea, cTemp_AI, cTemp_AF;
            bool resultado = false;
            do
            {
                resultado = Condicion() && ejecutar;
                match(Tipos.FinSentencia);
                // ! la solucion no seria condicionar al principio, sino directamente moverlo
                cTemp_AI = caracter;
                Asignacion(false);
                cTemp_AF = caracter;
                match(")");
                if(Clasificacion == Tipos.Inicio)
                {
                    BloqueInstrucciones(ejecutar && resultado);
                }
                else
                {
                    Instruccion(ejecutar && resultado);
                }
                // ! Aqui se haria el aumento del for, la comprobacion queda donde mismo
                if(resultado)
                {
                    caracter = cTemp_AI - 1;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    // ! realizar la lectura solo de la parte que quiero
                    while(caracter < cTemp_AF)
                    {
                        Asignacion(resultado && ejecutar);
                    }
                }
                if(resultado)
                { 
                    caracter = cTemp - 1;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                }
            }while(resultado);
        }
        /* Console -> Console.(WriteLine|Write) (cadena?);
        */
        private void Console(bool ejecutar)
        {
            match("Console");
            match(".");
            string nueva_linea;
            if(Contenido == "WriteLine")
            {
                match(Contenido);
                nueva_linea = "\n";
            }
            else
            {
                match(Contenido);
                nueva_linea = "";
            }
            match("(");
            if(Clasificacion == Tipos.Cadena)
            {
                if(ejecutar) System.Console.Write(Contenido.Replace('"',' ').TrimEnd().TrimStart());
                match(Tipos.Cadena);
                if(Contenido == "+")
                {
                    string concatenacion = listaDeConcatenacion();
                    if(ejecutar) System.Console.Write(concatenacion);
                }
                else if(ejecutar) System.Console.Write(nueva_linea);
            }
            match(")");
            match(Tipos.FinSentencia);
        }
        // listaDeConcatenacion -> + (variable|cadena) +?
        private string listaDeConcatenacion()
        {
            string c = "";
            match("+");
            if(Clasificacion == Tipos.Identificador)
            {
                Variable v = listaVariables.Find(x => x.Nombre == Contenido) ?? throw new Error("Semantico, esa variable no existe", log,linea);
                match(Tipos.Identificador);
                c += v.Valor;
            }
            else if(Clasificacion == Tipos.Cadena)
            {
                c += Contenido.Replace('"',' ');
                match(Tipos.Cadena);
            }
            if(Contenido == "+")
            {
                c += listaDeConcatenacion();
            }
            return c;
        }
        // Main      -> static void Main(string[] args) BloqueInstrucciones
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones(true);
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
                if(tipoDatoExpresion < valorToTipo(float.Parse(Contenido))) tipoDatoExpresion = valorToTipo(float.Parse(Contenido));
                match(Tipos.Numero);
            }
            else if(Clasificacion == Tipos.Identificador)
            {
                // S.variable al stack
                Variable v = listaVariables.Find(x => x.Nombre == Contenido) ?? throw new Error("No existe esa variable en la linea" + linea, log);
                if(tipoDatoExpresion < v.Tipo) tipoDatoExpresion = v.Tipo;
                s.Push(v.Valor);
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TipoDato aCastear = Variable.TipoDato.Char;
                match("(");
                // Casteo a nivel sintactico
                if(Clasificacion == Tipos.TipoDato)
                {
                    huboCast = true;
                    aCastear = Tipo(Contenido);
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if(huboCast && aCastear != Variable.TipoDato.Float)
                {
                    // 12
                    // sacar elemento del stack
                    tipoDatoExpresion = aCastear;
                    float valor = s.Pop();
                    // castearlo, residuo de la division entre 255
                    if(aCastear == Variable.TipoDato.Char)
                    {
                        valor %= 256;
                    }
                    else
                    {
                        valor %= 65536;
                    }
                    s.Push(valor);
                }
            }
        }
    }
}