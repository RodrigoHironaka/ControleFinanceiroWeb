using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.Controllers
{
    public class ContasBancariasController : Controller
    {
        private readonly RepositorioContaBancaria Repositorio;
        private readonly IMapper Mapper;

        public ContasBancariasController(RepositorioContaBancaria repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        ContaBancaria contaBancaria = new ContaBancaria();
        ContaBancariaVM contaBancariaVM = new ContaBancariaVM();

        public IActionResult Index()
        {
            IEnumerable<ContaBancaria> contasBancarias = Repositorio.ObterTodos();
            List<ContaBancariaVM> contasBancariasVM = Mapper.Map<List<ContaBancariaVM>>(contasBancarias);
            return View(contasBancariasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                contaBancaria = Repositorio.ObterPorId(Id);
                contaBancariaVM = Mapper.Map<ContaBancariaVM>(contaBancaria);
            }

            ViewBag.BancoId = new SelectList(new RepositorioBanco(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DadosCompletos", Id);
            return View(contaBancariaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(ContaBancariaVM contaBancariaVM)
        {
            if (ModelState.IsValid)
            {
                if (contaBancariaVM.Id > 0)
                {
                    contaBancaria = Repositorio.ObterPorId(contaBancariaVM.Id);
                    contaBancaria = Mapper.Map(contaBancariaVM, contaBancaria);
                    contaBancaria.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(contaBancaria);
                }
                else
                {
                    contaBancaria = Mapper.Map(contaBancariaVM, contaBancaria);
                    contaBancaria.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(contaBancaria);
                }

                return new EmptyResult();
            }
            ViewBag.BancoId = new SelectList(new RepositorioBanco(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "DadosCompletos", contaBancariaVM.Id);
            return View(contaBancariaVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            contaBancaria = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(contaBancaria);
            return Json(contaBancaria.Nome + "excluído com sucesso");
        }
    }
}
