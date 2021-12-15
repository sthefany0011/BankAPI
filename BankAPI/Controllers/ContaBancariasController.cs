﻿using System;
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

   

        public ContaBancariasController(BankAPIContext context, ContaRepository contaRepository)
        {
            _context = context;
            _contaRepository = contaRepository;
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

               
                //Adicionar no banco de dados SEM PASSAR NO SERVICE
                _contaRepository.Credito(_historicoEntity);

                
                // retornar sucesso = true ou false, valor creditado, saldo
                return Ok(new OperacaoRetornoModel
                {
                    Saldo = _contaRepository.Saldo(model),
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

            if (_contaRepository.Saldo(model) <= model.Valor)
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
                _contaRepository.Debito(_historicoEntity);


                // retornar sucesso = true ou false, valor creditado, saldo
                return Ok(new OperacaoRetornoModel
                {
                    Saldo = _contaRepository.Saldo(model),
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

            //HistoricoModel model = new HistoricoModel { ContaBancaria = contaBancaria };

            var extrato = _contaRepository.FindByAccount(contaBancaria);

            //"2021-12-03T09:35:30:2220Z" Format Dates / Formatar data para padrão brasileiro no csharp

            //(contaBancaria);
            var result = new
            {
                Operacoes = extrato,
                Saldo = _contaRepository.Saldo(new HistoricoModel { ContaBancaria = contaBancaria })
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

