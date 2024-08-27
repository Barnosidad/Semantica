using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Sintaxis_1
{
    public class Lexico : Token, IDisposable
    {
        private StreamReader archivo;
        public StreamWriter log;
        protected StreamWriter asm;
        protected int linea;
        const int F = -1;
        const int E = -2;
        int[,] TRAND =
        {
            {0, 1, 2,26 ,1, 9,10, 8,18,12,27,12,14,15,19,17,17,21,22,24,25,26,26, 0,31},
            {F, 1, 1, F, 1, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, 2, 3, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {E, E, 4, E, 5, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, F},
            {F, F, 4, F, 5, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {E, E, 7, E, E, 6, 6, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, F},
            {E, E, 7, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, F},
            {F, F, 7, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, 11, F, F, 11, F, F, F, F, F, F, F, F, F, F, F, F,F,F, F, F},
            {F, F, F, F, F, F, 11, F, 11, F, F, F, F, F, F, F, F, F, F, F, F,F,F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, 13, F, F, F, F, F, F, F, F, F, F, F, F, F,F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, 16, F, F, F, F, F, F, F, F, F,F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, 16, F, F, F, F, F, F, F, F,F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, 20, F, F, F, F, F, F, F, F, F, F, F, F, F,F, F, F},
            {F, F, F, F, F, F, F, F, 20, F, F, F, F, F, F, F, F, F, F, F, F, F,F, F, F},
            {F, F, F, F, F, F, F, F, 20, F, F, F, F, F, F, F, F, F, F, F, F, F,F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,22,23,22,22,E,22, 22,F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {F, F, F, F, F, F, F, F,13,29,28, F, F, F, F, F, F, F, F, F, F, F, F, F, F},
            {28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,0,28, 0, F},
            {29,29,29,29,29,29,29,29,29,30,29,29,29,29,29,29,29,29,29,29,29,E,29, 29,F},
            {29,29,29,29,29,29,29,29,29,30, 0,29,29,29,29,29,29,29,29,29,29,E,29, 29,F},
            {E, E, 32, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E, E,E},
            {F,F, 32, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F, F}

        };
        public Lexico() // Constructor
        {
            linea = 0;
            log = new StreamWriter("prueba.log");
            log.AutoFlush = true;
            asm = new StreamWriter("prueba.asm");
            asm.AutoFlush = true;
            log.WriteLine("Analizador Lexico");
            log.WriteLine("Autor: Gonzalez Hernandez Diego");
            asm.WriteLine("; Autor: Gonzalez Hernandez Diego");

            if (!File.Exists("prueba.cpp"))
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
            archivo = new StreamReader("prueba.cpp");
        }
        public Lexico(string nombre) // Constructor
        {
            linea = 0;
            log = new StreamWriter(Path.GetFileNameWithoutExtension(nombre) + ".log");
            log.AutoFlush = true;
            asm = new StreamWriter(Path.GetFileNameWithoutExtension(nombre) + ".asm");
            asm.AutoFlush = true;
            log.WriteLine("Analizador Lexico");
            log.WriteLine("Autor: Gonzalez Hernandez Diego");
            asm.WriteLine("; Autor: Gonzalez Hernandez Diego");

            if (Path.GetExtension(nombre) != ".cpp")
            {
                throw new Error("El archivo " + nombre + " no tiene extension CPP", log);
            }
            if (!File.Exists(nombre))
            {
                throw new Error("El archivo " + nombre + " no existe", log);
            }
            archivo = new StreamReader(nombre);
        }
        public void Dispose() // Destructor
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }
        int Columna(char c)
        {
            if (finArchivo())
            {
                return 21;
            }
            else if (c == '\n')
            {
                return 23;
            }
            else if (char.IsWhiteSpace(c))
            {
                return 0;
            }
            else if (char.ToUpper(c) == 'E')
            {
                return 4;
            }
            else if (char.IsLetter(c))
            {
                return 1;

            }
            else if (char.IsDigit(c))
            {
                return 2;
            }
            else if (c == '.')
            {
                return 3;
            }
            else if (c == '+')
            {
                return 5;
            }
            else if (c == '-')
            {
                return 6;
            }
            else if (c == ';')
            {
                return 7;
            }
            else if (c == '=')
            {
                return 8;
            }
            else if (c == '*')
            {
                return 9;
            }
            else if (c == '/')
            {
                return 10;
            }
            else if (c == '%')
            {
                return 11;
            }
            else if (c == '&')
            {
                return 12;
            }
            else if (c == '|')
            {
                return 13;
            }
            else if (c == '!')
            {
                return 14;
            }
            else if (c == '>')
            {
                return 15;
            }
            else if (c == '<')
            {
                return 16;
            }
            else if (c == '?')
            {
                return 17;
            }
            else if (c == '"')
            {
                return 18;
            }
            else if (c == '{')
            {
                return 19;
            }
            else if (c == '}')
            {
                return 20;
            }
            else if (c == '$')
            {
                return 24;
            }
            else
            {
                return 22;
            }
        }
        private void Clasificar(int Estado)
        {
            switch (Estado)
            {
                case 01: setClasificacion(Tipos.Identificador); break;
                case 02: setClasificacion(Tipos.Numero); break;
                case 08: setClasificacion(Tipos.FinSentencia); break;
                case 09:
                case 10: setClasificacion(Tipos.OpTermino); break;
                case 11: setClasificacion(Tipos.IncTermino); break;
                case 27:
                case 12: setClasificacion(Tipos.OpFactor); break;
                case 13: setClasificacion(Tipos.IncFactor); break;
                case 14:
                case 15: setClasificacion(Tipos.Caracter); break;
                case 16: setClasificacion(Tipos.OpLogico); break;
                case 17: setClasificacion(Tipos.OpRelacional); break;
                case 18: setClasificacion(Tipos.Asignacion); break;
                case 19: setClasificacion(Tipos.OpLogico); break;
                case 20: setClasificacion(Tipos.OpRelacional); break;
                case 21: setClasificacion(Tipos.OpTernario); break;
                case 22: setClasificacion(Tipos.Cadena); break;
                case 24: setClasificacion(Tipos.Inicio); break;
                case 25: setClasificacion(Tipos.Fin); break;
                case 26: setClasificacion(Tipos.Caracter); break;

            }
        }
        public void nextToken()
        {
            char c;
            string buffer = "";
            int Estado = 0;
            while (Estado >= 0)
            {
                c = (char)archivo.Peek();

                Estado = TRAND[Estado, Columna(c)];
                Clasificar(Estado);

                if (Estado >= 0)
                {
                    if (Estado == 0)
                    {
                        buffer = "";
                    }
                    else if (Estado > 0)
                    {
                        buffer += c;
                    }
                    archivo.Read();
                }
            }
            if (Estado == E)
            {
                if (getClasificacion() == Tipos.Numero)
                {
                    throw new Error(" Se espera un digito " + buffer, log);
                }
                else if (getClasificacion() == Tipos.Cadena)
                {
                    throw new Error(" Se espera cierre de cadena " + buffer, log);
                }
                else if (getClasificacion() == Tipos.OpFactor)
                {
                    throw new Error(" Se espera un cierre de comentario\n " + buffer, log);
                }
            }
            setContenido(buffer);
            if (getClasificacion() == Tipos.Identificador)
            {
                switch (getContenido())
                {
                    case "char":
                    case "int":
                    case "float": setClasificacion(Tipos.TipoDato); break;
                    case "if":
                    case "else":
                    case "switch": setClasificacion(Tipos.Condicion); break;
                    case "while":
                    case "do":
                    case "for": setClasificacion(Tipos.Ciclo); break;
                }
            }
            log.WriteLine(getContenido() + " = " + getClasificacion());
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}