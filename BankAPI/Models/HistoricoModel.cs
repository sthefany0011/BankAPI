using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Models
{
    public class HistoricoModel
    {
        public int ContaBancaria { get; set; }
        public decimal Valor { get; set; }

        public HistoricoModel(int contaBancaria, decimal valor)
        {
            ContaBancaria = contaBancaria;
            Valor = valor;
        }
    }
}
