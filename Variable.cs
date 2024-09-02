using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Semantica
{
    public class Variable
    {
        public enum TipoDato
        {
            Char,Int,Float
        }
        private string nombre;
        private TipoDato tipo;
        private float valor;
        public Variable(string nombre,TipoDato tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.valor = 0;
        }
        public float Valor
        {
            get => valor;
            set => valor = value;
        }
        public string Nombre
        {
            get => nombre;
            set => nombre = value;
        }
        public TipoDato Tipo
        {
            get => tipo;
            set => tipo = value;
        }
    }
}