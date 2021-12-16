using BankAPI.Data.Entities;
using BankAPI.Models;
using System.Collections.Generic;

namespace BankAPI.Repositories
{
    public interface IContaRepository
    {
        bool Credito(HistoricoEntity historicoEntity);
        void Debito(HistoricoEntity historicoEntity);
        List<RelatorioModel> FindBy(int contaBancaria, int anual);
        List<ContaExtrato> FindByAccount(int contaBancaria);
        decimal Saldo(HistoricoModel model);
    }
}