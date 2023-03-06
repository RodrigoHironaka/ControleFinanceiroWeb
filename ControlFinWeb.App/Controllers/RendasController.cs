using AutoMapper;
using ControlFinWeb.App.AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.Controllers
{
    public class RendasController : Controller
    {
        private readonly RepositorioRenda Repositorio;
        private readonly IMapper Mapper;
        public RendasController(RepositorioRenda repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Renda renda = new Renda();
        RendaVM rendaVM = new RendaVM();

        public IActionResult Index()
        {
            IEnumerable<Renda> rendas = Repositorio.ObterTodos();
            List<RendaVM> rendasVM = Mapper
                .Map<List<RendaVM>>(rendas);
            return View(rendasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                renda = Repositorio.ObterPorId(Id);
                rendaVM = Mapper.Map<RendaVM>(renda);
            }
            return View(rendaVM);
        }

        [HttpPost]
        public IActionResult Editar(RendaVM rendaVM)
        {
            if (ModelState.IsValid)
            {
                if (rendaVM.Id > 0)
                {
                    renda = Repositorio.ObterPorId(rendaVM.Id);
                    renda = Mapper.Map(rendaVM, renda);
                    renda.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(renda);
                }
                else
                {
                    renda = Mapper.Map(rendaVM, renda);
                    renda.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(renda);
                }
                return new EmptyResult();
            }
            return View(rendaVM);
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            renda = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(renda);
            return Json(renda.Nome + "excluído com sucesso");
        }
    }
}
