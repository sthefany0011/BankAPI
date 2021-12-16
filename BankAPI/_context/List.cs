using System;
using System.Runtime.Serialization;

namespace _context
{
    [Serializable]
    internal class List<T> : Exception
    {
        private int contaBancaria;
        private int anual;

        public List()
        {
        }

        public List(string message) : base(message)
        {
        }

        public List(int contaBancaria, int anual)
        {
            this.contaBancaria = contaBancaria;
            this.anual = anual;
        }

        public List(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected List(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}