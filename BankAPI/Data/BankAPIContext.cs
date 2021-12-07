using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankAPI.Models;
using BankAPI.Models.Data.Entities;

namespace BankAPI.Data
{
    public class BankAPIContext : DbContext
    {
        internal HistoricoOperacao Operacao;

        public BankAPIContext(DbContextOptions<BankAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Entities.HistoricoEntity> Historico { get; set; }
    }
}