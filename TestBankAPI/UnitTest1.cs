using BankAPI.Controllers;
using BankAPI.Data.Entities;
using BankAPI.Models;
using BankAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace TestBankAPI
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreditoEmContaComSucesso()
        {
            var mockContaRepository = new Mock<IContaRepository>();

            mockContaRepository.Setup(a => a.Credito(It.IsAny<HistoricoEntity>()))
                .Returns(true);

            mockContaRepository.Setup(a => a.Saldo(It.IsAny<HistoricoModel>()))
                .Returns(10);

            var controller = new ContaBancariasController(mockContaRepository.Object);

            var resp = controller.PostCredito(new BankAPI.Models.HistoricoModel
            {
                ContaBancaria = 10,
                Valor = 10
            });

            var aux = resp as OkObjectResult;
            Assert.IsTrue(aux != null);

            //Assert.Equals((aux.Value as OperacaoRetornoModel).Saldo, 10);
        }
    }
}