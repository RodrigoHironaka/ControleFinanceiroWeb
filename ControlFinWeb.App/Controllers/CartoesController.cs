using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;


namespace ControlFinWeb.App.Controllers
{
    public class CartoesController : Controller
    {
        private readonly RepositorioCartao Repositorio;
        private readonly IMapper Mapper;

        public CartoesController(RepositorioCartao repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Cartao cartao = new Cartao();
        CartaoVM cartaoVM = new CartaoVM();

        public IActionResult Index()
        {
            IEnumerable<Cartao> cartoes = Repositorio.ObterTodos();
            List<CartaoVM> cartoesVM = Mapper.Map<List<CartaoVM>>(cartoes);
            return View(cartoesVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                cartao = Repositorio.ObterPorId(Id);
                cartaoVM = Mapper.Map<CartaoVM>(cartao);
            }

            ViewBag.BancoId = new SelectList(new RepositorioBanco(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            return View(cartaoVM);
        }

        [HttpPost]
        public IActionResult Editar(CartaoVM cartaoVM)
        {
            if (ModelState.IsValid)
            {
                if (cartaoVM.Id > 0)
                {
                    cartao = Repositorio.ObterPorId(cartaoVM.Id);
                    cartao = Mapper.Map(cartaoVM, cartao);
                    cartao.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(cartao);
                }
                else
                {
                    cartao = Mapper.Map(cartaoVM, cartao);
                    cartao.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(cartao);
                }

                return new EmptyResult();
            }
            ViewBag.BancoId = new SelectList(new RepositorioBanco(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", cartaoVM.Id);
            return View(cartaoVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            cartao = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(cartao);
            return Json(cartao.Nome + "excluído com sucesso");
        }
    }
}
