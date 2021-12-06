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

            var totalCredito = _context.Historico.Where(_context =>_context.ContaBancaria==model.ContaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).Sum(_context => _context.Valor);
            var totalDebito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).Sum(_context => _context.Valor);

            var totalSaldo = model.Valor + (totalCredito - totalDebito) ;

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
                return Ok(
                new
                {
                    msg = "Operação realizada com Sucesso, valor creditado: R$ " + model.Valor
                        + " Saldo Atual: R$ " + totalSaldo
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
            var totalCredito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Credito).Sum(_context => _context.Valor);
            var totalDebito = _context.Historico.Where(_context => _context.ContaBancaria == model.ContaBancaria && _context.Operacao == Models.Data.Entities.HistoricoOperacao.Debito).Sum(_context => _context.Valor);

            var totalSaldo = (totalCredito - totalDebito) - model.Valor;

            if (totalSaldo < 0)
            {
                return BadRequest(new { 
                    msg = "Você não pode debitar mais do que o saldo" 
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
                return Ok(
                new
                {
                    msg = "Operação realizada com Sucesso, valor debitado: R$ " + model.Valor
                        + " Saldo Atual: R$ " + totalSaldo
                });
            }

            else
            {
                return BadRequest(new { msg = "Valor R$ " + model.Valor + " é  inválido" });
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

        //Relatorio
        [HttpGet]

        public ActionResult<HistoricoEntity> RelatorioGet(int contaBancaria, int anual)
        {

            
            var relatorio = _context.Historico
                  .Where(_context => _context.ContaBancaria == contaBancaria && _context.Data.Year == anual)
               
                  .Select(_context =>     
                      new {
                          Mês = _context.Data.Month,
                          operacao = (_context.Operacao == 0) ? "Crédito" : "Débito",
                      }
                  ).ToList().Sum(_context => _context.Valor);

            return Ok(relatorio);
        }

    }
}

