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
       
        private readonly IContaRepository _contaRepository;

   

        public ContaBancariasController( IContaRepository contaRepository)
        {
        
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
               if(_contaRepository.Credito(_historicoEntity))
                {
                    // retornar sucesso = true ou false, valor creditado, saldo
                    return Ok(new OperacaoRetornoModel
                    {
                        Saldo = _contaRepository.Saldo(model),
                        Valor = model.Valor
                    });
                }

                return BadRequest(new { msg = "Ocorreum um erro ao adicionar no banco" });

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


            //(contaBancaria);
            var result = new
            {
                Operacoes = extrato,
                Saldo = _contaRepository.Saldo(new HistoricoModel { ContaBancaria = contaBancaria })
            };

            return Ok(result);
        }

        //Relatorio
        [HttpGet]

        public ActionResult<HistoricoEntity> RelatorioGet(int contaBancaria, int anual)
        {
            return Ok(_contaRepository.FindBy(contaBancaria,anual));
        }

    }
}

