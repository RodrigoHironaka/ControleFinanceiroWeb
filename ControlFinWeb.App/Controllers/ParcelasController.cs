using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ControlFinWeb.App.Controllers
{
    public class ParcelasController : Controller
    {
        private readonly RepositorioParcela Repositorio;
        private readonly IMapper Mapper;
        public ParcelasController(RepositorioParcela repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Parcela parcela = new Parcela();
        ParcelaVM parcelaVM = new ParcelaVM();
        IList<Parcela> parcelas = new List<Parcela>();
        IList<ParcelaVM> parcelasVM = new List<ParcelaVM>();
        GerarParcelasVM gerarParcelasVM = new GerarParcelasVM();

        public IActionResult ModalGerarParcelas()
        {
            return View(gerarParcelasVM);
        }

        [HttpPost]
        public String GerarNovasParcelas(GerarParcelasVM gerarParcelasVM)
        {
            Decimal valor = gerarParcelasVM.Valor;
            Int32 qtd = gerarParcelasVM.Qtd;
            DateTime primeiroVencimento = (DateTime)gerarParcelasVM.PrimeiroVencimento;
            Boolean replicar = gerarParcelasVM.Replicar;
            Decimal valorParcela = 0, restante = 0;
            Int64 numero = gerarParcelasVM.UltimoNumero;

            if (replicar)
                valorParcela = valor;
            else
            {
                valorParcela = Math.Round(valor / qtd, 2);
                restante = Math.Round(valor - (valorParcela * qtd), 2);
            }

            for (int i = 1; i <= qtd; i++)
            {
                if (restante > 0 && i == qtd )
                {
                    valorParcela += restante;
                    restante = 0;
                }
                else if (restante < 0 && i == qtd )
                {
                    valorParcela += restante;
                    restante = 0;
                }

                var novaParcela = new ParcelaVM()
                {
                    Numero = numero + i,
                    ParcelaDe = $"{i}/{qtd}",
                    ValorParcela = valorParcela,
                    ValorAberto = valorParcela,
                    ValorReajustado = valorParcela,
                    DataVencimento = primeiroVencimento.AddMonths(i),
                    SituacaoParcela = Dominio.ObjetoValor.SituacaoParcela.Pendente,
                };

                parcelasVM.Add(novaParcela);
            }

            var novasParcelas = JsonConvert.SerializeObject(parcelasVM);
            return novasParcelas;
        }
    }
}
