using AutoMapper;
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

        public IActionResult AbrirModalGerarParcelas()
        {
            return View(parcelaVM);
        }

        public String GerarNovasParcelas(string valor, string qtd, string primeiroVencimento, string replicar)
        {
            Decimal Valor = Decimal.Parse(valor);
            Int32 Qtd = Int32.Parse(qtd);
            Qtd = Qtd > 0 ? Qtd : 1;
            DateTime PrimeiroVencimento = DateTime.Parse(primeiroVencimento);
            Boolean Replicar = Boolean.Parse(replicar);

            Decimal valorParcela = 0, restante = 0;

            if (Replicar)
                valorParcela = Valor;
            else
            {
                valorParcela = Math.Round(Valor / Qtd, 2);
                restante = Math.Round(Valor - (valorParcela * Qtd), 2);
            }

            for (int i = 0; i < Qtd; i++)
            {
                if (restante > 0 && i == Qtd - 1)
                {
                    valorParcela += restante;
                    restante = 0;
                }

                var novaParcela = new ParcelaVM()
                {
                    //Numero = parcelasVM.Count > 0 ? parcelasVM.Count + 1 : 1,
                    ParcelaDe = $"{i + 1}/{Qtd}",
                    ValorParcela = valorParcela,
                    ValorAberto = valorParcela,
                    ValorReajustado = valorParcela,
                    DataVencimento = PrimeiroVencimento.AddMonths(i),
                    SituacaoParcela = Dominio.ObjetoValor.SituacaoParcela.Pendente,
                };

                parcelasVM.Add(novaParcela);
            }

            var novasParcelas = JsonConvert.SerializeObject(parcelasVM);
            return novasParcelas;
        }
    }
}
