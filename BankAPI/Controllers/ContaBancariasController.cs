using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BankAPI.Models;
using BankAPI.Data;
using BankAPI.Data.Entities;
using BankAPI.Repositories;

namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaBancariasController : ControllerBase
    {
        private readonly BankAPIContext _context;
        private readonly ContaRepository _contaRepository;

        public object moment { get; private set; }

        public ContaBancariasController(BankAPIContext context, ContaRepository contaRepository)
        {
            _context = context;
            _contaRepository = contaRepository;
        }


        decimal Saldo(HistoricoModel model)
        {
            var totalSaldo = _context.Historico.Where(a => a.ContaBancaria == model.ContaBancaria)
                    .Sum(a => a.Operacao == Models.Data.Entities.HistoricoOperacao.Debito
                        ? -a.Valor : a.Valor);

            return totalSaldo;
        }


        // POST: api/ContaBancarias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Credito")]
        public IActionResult PostCredito([FromBody] HistoricoModel model)
        {
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
                return Ok(new OperacaoRetornoModel
                {
                    Saldo = Saldo(model),
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

            if (Saldo(model) <= model.Valor)
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
                    Saldo = Saldo(model),
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
            HistoricoModel model = new HistoricoModel { ContaBancaria = contaBancaria };

            var extrato = _context.Historico.OrderBy(_context => _context.Data)
                .Where(_context => _context.ContaBancaria == contaBancaria)
                .Select(_context =>
                    new
                    {
                        operacao = (_context.Operacao == 0) ? "Crédito" : "Débito",
                        Valor = _context.Valor,
                        Data = _context.Data
                    }
                ).ToList();

            //"2021-12-03T09:35:30:2220Z" Format Dates / Formatar data para padrão brasileiro no csharp


            var result = new
            {
                Operacoes = extrato,
                Saldo = Saldo(model)
            };

            //var Saldo = Saldo(model);

            return Ok(result);
        }

        //Relatorio
        [HttpGet]

        public ActionResult<HistoricoEntity> RelatorioGet(int contaBancaria, int anual)
        {


            var result = _context.Historico
                .Where(context => context.ContaBancaria == contaBancaria && context.Data.Year == anual)
                .GroupBy(item => item.Data.Month).Select(context => new
                {
                    Mês = context.Key, //context.FirstOrDefault().Data.Month,
                    Credito = context.Where(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).Sum(c => c.Valor),
                    Debito = context.Where(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).Sum(c => c.Valor),
                    Saldo = context.Sum(c => c.Operacao == Models.Data.Entities.HistoricoOperacao.Debito ? -c.Valor : c.Valor)
                });


            return Ok(result);
        }

    }
}

