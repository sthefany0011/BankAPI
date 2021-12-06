using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Models
{
    public class RetornoModel
    {
        public bool Sucesso { get; set; }
        public int ContaBancaria { get; set; }
        public decimal Saldo { get; set; }

    }
}
