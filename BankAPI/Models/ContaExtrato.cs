using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Models
{
    public class ContaExtrato
    {
        public string Operacao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }

        public ContaExtrato()
        {
        }

        public ContaExtrato(string operacao, decimal valor, DateTime data)
        {
            Operacao = operacao;
            Valor = valor;
            Data = data;
        }
    }
}
