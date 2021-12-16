using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Models
{
    public class RelatorioModel
    {
        public int Mes { get; set; }
        public decimal Credito { get; set; }
        public decimal Debito { get; set; }
        public decimal Saldo { get; set; }

        public RelatorioModel()
        {
        }

        public RelatorioModel(int mes, decimal credito, decimal debito, decimal saldo)
        {
            Mes = mes;
            Credito = credito;
            Debito = debito;
            Saldo = saldo;
        }
    }
}
