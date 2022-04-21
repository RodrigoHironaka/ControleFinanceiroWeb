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
        public RendasController(RepositorioRenda repositorio)
        {
            Repositorio = repositorio;
        }

        Renda renda = new Renda();
        RendaVM rendaVM = new RendaVM();

        public IActionResult Index()
        {
            var rendasVM = new List<RendaVM>();
            var rendas = Repositorio.ObterTodos();
            foreach (Renda renda in rendas)
                rendasVM.Add(AutoMapperConfig<Renda, RendaVM>.Mapear(renda));
            return View(rendasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                renda = Repositorio.ObterPorId(Id);
                rendaVM = AutoMapperConfig<Renda, RendaVM>.Mapear(renda);
            }
            return View(rendaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(RendaVM rendaVM)
        {
            if (ModelState.IsValid)
            {
                if (rendaVM.Id > 0)
                {
                    var rendaRep = Repositorio.ObterPorId(rendaVM.Id);
                    renda = AutoMapperConfig<RendaVM, Renda>.Mapear(rendaVM);
                    renda.DataGeracao = rendaRep.DataGeracao;
                    renda.UsuarioCriacao = rendaRep.UsuarioCriacao;
                    renda.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(renda);
                }
                else
                {
                    renda = AutoMapperConfig<RendaVM, Renda>.Mapear(rendaVM);
                    renda.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(renda);
                }
                return RedirectToAction("Index");
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
