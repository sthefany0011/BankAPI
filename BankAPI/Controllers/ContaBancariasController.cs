using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAPI.Models;
using BankAPI.Data;
using BankAPI.Data.Entities;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaBancariasController : ControllerBase
    {
        private readonly BankAPIContext _context;

        public ContaBancariasController(BankAPIContext context)
        {
            _context = context;
        }
      

        // POST: api/ContaBancarias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Credito")]
        public IActionResult PostCredito([FromBody] HistoricoModel model)
        {
            //var totalCredito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).Sum(_context => _context.Valor);
            //var totalDebito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).Sum(_context => _context.Valor);

            //var totalSaldo = model.Valor + (totalCredito - totalDebito);


            //var total = new
            //{
            //    totalCredito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria
            //        && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito)
            //            .Sum(_context => _context.Valor),
            //    totalDebito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria
            //        && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito)
            //            .Sum(_context => _context.Valor),
            //    totalSaldo = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria)
            //            .Sum(_context => _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito
            //                ? -_context.Valor : _context.Valor) + model.Valor
            //};

            decimal Saldo(HistoricoModel model)
            {
                decimal totalCredito = _context.Historico.Where(context => context.ContaBancaria == model.ContaBancaria 
                    && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).Sum(context => context.Valor);
                decimal totalDebito = _context.Historico.Where(context => context.ContaBancaria == model.ContaBancaria 
                    && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).Sum(context => context.Valor);

                return (totalCredito -= totalDebito) + model.Valor;
            }

            var _historicoEntity = new HistoricoEntity
            {
                Valor = model.Valor,
                Operacao = 0,
                Data = DateTime.Now,
                ContaBancaria = model.ContaBancaria,

            };

            // se valor > zero
            if (model.Valor > 0)
            {

                try
                {
                    //Adicionar no banco de dados SEM PASSAR NO SERVICE
                    _context.Historico.AddRangeAsync(_historicoEntity);
                    _context.SaveChanges();


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }   

                // retornar sucesso = true ou false, valor creditado, saldo
                return Ok( new OperacaoRetornoModel { 
                    Saldo  = Saldo(model),
                    Valor = model.Valor
                });
            }
            else
            {
                return BadRequest(new { msg = "Valor R$ " + model.Valor + " é inválido" });
            }
        }

        [HttpPost("Debito")]
        public IActionResult PostDebito([FromBody] HistoricoModel model)
        {
            var total = new {
                    totalCredito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria
                        && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito)
                        .Sum(_context => _context.Valor),
                    totalDebito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria
                        && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito)
                        .Sum(_context => _context.Valor),
                    totalSaldo = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria)
                        .Sum(_context => _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito 
                            ? -_context.Valor : _context.Valor) - model.Valor
            };


            var totalSaldo = (total.totalSaldo) - model.Valor;

            if (total.totalSaldo < 0)
            {
                return BadRequest(new
                {
                    msg = "Valor do saldo execedido!"
                });
            }

            var _historicoEntity = new HistoricoEntity
            {
                Valor = model.Valor,
                Operacao = Models.Data.Entities.HistoricoOperacao.Debito,
                Data = DateTime.Now,
                ContaBancaria = model.ContaBancaria,
            };

            // se valor > zero
            if (model.Valor > 0)
            {

                try
                {
                    //Adicionar no banco de dados SEM PASSAR NO SERVICE
                    _context.Historico.AddRangeAsync(_historicoEntity);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                // retornar sucesso = true ou false, valor debitado, saldo
                // retornar sucesso = true ou false, valor creditado, saldo
                return Ok(new OperacaoRetornoModel
                {
                    Saldo = total.totalSaldo,
                    Valor = model.Valor
                });
            }
            else
            {
                return BadRequest(new { msg = "Valor R$ " + model.Valor + " é inválido" });
            }

        }
        [HttpGet("{contaBancaria}")]
        public ActionResult<HistoricoEntity> Get(int contaBancaria)
        {
            Console.WriteLine(contaBancaria);

            var extrato = _context.Historico.OrderBy(_context =>_context.Data)
                .Where(_context => _context.ContaBancaria == contaBancaria)
                .Select(_context =>
                    new {
                        operacao = (_context.Operacao == 0) ? "Crédito" : "Débito",
                        Valor = _context.Valor,
                        Data = _context.Data
                    }
                ).ToList();

            //"2021-12-03T09:35:30:2220Z" Format Dates / Formatar data para padrão brasileiro no csharp

            var totalCredito = _context.Historico
                .Where(_context => _context.ContaBancaria == contaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito)
                .Sum(_context => _context.Valor);

            var totalDebito = _context.Historico
                .Where(_context => _context.ContaBancaria == contaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito)
                .Sum(_context => _context.Valor);

            var totalSaldo = totalCredito - totalDebito;

            var result = new
            {
                Operacoes = extrato,
                Saldo = totalSaldo,
            };

            return Ok(result);
        }
        public class SomatoriaMes
        {
            public int Mes { get; set; }
            public decimal Valor { get; set; }
        }
        //Relatorio
        [HttpGet]

        public ActionResult<HistoricoEntity> RelatorioGet(int contaBancaria, int anual)
        {

            //var result = new
            //{
            //    credito = _context.Historico
            //      .Where(_context => _context.ContaBancaria == contaBancaria && _context.Data.Year == anual && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito)
            //      .GroupBy(item => item.Data.Month)
            //      .Select(item => new SomatoriaMes { Mes = item.Key, Valor = item.Sum(i => i.Valor) }).ToList(),
            //    debito = _context.Historico
            //     .Where(_context => _context.ContaBancaria == contaBancaria && _context.Data.Year == anual && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).GroupBy(item => item.Data.Month)
            //     .Select(item => new SomatoriaMes { Mes = item.Key, Valor = item.Sum(i => i.Valor) }).ToList(),

            //    saldo = _context.Historico
            //      .Where(_context => _context.ContaBancaria == contaBancaria && _context.Data.Year == anual && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).GroupBy(item => item.Data.Month)
            //      .Select(item => new SomatoriaMes { Mes = item.Key, Valor = item.Sum(i => i.Valor) }).ToList()
            //};

            //var result = new
            //{
            var result = _context.Historico
                .Where(context => context.ContaBancaria == contaBancaria && context.Data.Year == anual)
                .GroupBy(item => item.Data.Month)
                .Select(context => new
                {
                    //mes=,
                    credito = context.Where(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).Sum(c => c.Valor),
                    debito = context.Where(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).Sum(c => c.Valor),
                    saldo = context.Sum(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Debito ? -c.Valor : c.Valor)
                });
            //}

            //foreach (var item in result.saldo)
            //{
            //    item.Valor -= result.debito.FirstOrDefault(debitos => debitos.Mes == item.Mes)?.Valor ?? 0;
            //};
    

            return Ok(result);
        }

    }
}

