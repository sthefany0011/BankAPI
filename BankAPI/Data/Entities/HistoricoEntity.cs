using BankAPI.Models.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Data.Entities
{
    public class HistoricoEntity
    {
       
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}")]
        public DateTime Data { get; set; }

        [DisplayFormat(DataFormatString ="{0:F2}")]
        public decimal Valor { get; set; }

        public HistoricoOperacao Operacao { get; set; }
        public int ContaBancaria { get; set; }
        public static decimal Saldo { get; internal set; }
       
    }
}
