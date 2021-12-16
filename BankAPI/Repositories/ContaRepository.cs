using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Repositories
{
    public class ContaRepository
    {
        private readonly BankAPIContext _context;


        public ContaRepository(BankAPIContext context)
        {
            _context = context;
        }
        //Método de Saldo

        public decimal Saldo(HistoricoModel model)
        {
            var totalSaldo = _context.Historico.Where(a => a.ContaBancaria == model.ContaBancaria)
                    .Sum(a => a.Operacao == Models.Data.Entities.HistoricoOperacao.Debito
                        ? -a.Valor : a.Valor);

            return totalSaldo;
        }

        //Ação para inserir créditos

        public void Credito(HistoricoEntity historicoEntity)
        {
            try
            {
                //_context.Historico.AddRangeAsync(historicoEntity);
                _context.Historico.AddAsync(historicoEntity);
                _context.SaveChanges();
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Debito

        public void Debito(HistoricoEntity historicoEntity)
        {
            try
            {
                //_context.Historico.AddRangeAsync(historicoEntity);
                _context.Historico.AddAsync(historicoEntity);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        //Extrato

        public List<ContaExtrato> FindByAccount(int contaBancaria)
        {
            try
            {
                var extrato = _context.Historico.OrderBy(_context => _context.Data)
                    .Where(_context => _context.ContaBancaria == contaBancaria)
                    .Select(_context =>
                        new ContaExtrato
                        {
                            Operacao = (_context.Operacao == 0) ? "Crédito" : "Débito",
                            Valor = _context.Valor,
                            Data = _context.Data
                        }).ToList();

                return extrato;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }

            return new List<ContaExtrato>();
        }

        //Relatório

        public List<RelatorioModel> FindBy(int contaBancaria, int anual)
        {
            try
            {
                var result = _context.Historico
                    .Where(context => context.ContaBancaria == contaBancaria && context.Data.Year == anual)
                    .GroupBy(item => item.Data.Month).Select(context => new RelatorioModel
                    {
                        Mes = context.Key, //context.FirstOrDefault().Data.Month,
                        Credito = context.Where(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).Sum(c => c.Valor),
                        Debito = context.Where(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).Sum(c => c.Valor),
                        Saldo = context.Sum(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Debito ? -c.Valor : c.Valor)
                    }).ToList();

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<RelatorioModel>();
            }
        }
    }
}

