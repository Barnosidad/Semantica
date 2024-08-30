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
        public void setValor(float valor)
        {
            this.valor = valor;
        }
        public string getNombre()
        {
            return this.nombre;
        }
        public TipoDato getTipo()
        {
            return this.tipo;
        }
        public float getValor()
        {
            return this.valor;
        }
    }
}