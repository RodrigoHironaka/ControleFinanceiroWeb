using ControlFinWeb.App.ViewModels.Acesso;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using Utils.Seguranca;

namespace ControlFinWeb.App.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly RepositorioUsuario Repositorio;
        public UsuariosController(RepositorioUsuario repositorio)
        {
            Repositorio = repositorio;
        }

        Usuario usuario = new Usuario();
        UsuarioVM usuarioVM = new UsuarioVM();

        public IActionResult Index()
        {
            return View(Repositorio.ObterPorParametros(x => x.Situacao == Situacao.Ativo));
        }

        public IActionResult Editar(Int64 ID = 0)
        {
            if (ID > 0)
            {
                usuario = Repositorio.ObterPorId(ID);

                usuarioVM = new UsuarioVM
                {
                    ID = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email
                };
            }

            return View(usuarioVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(UsuarioVM usuarioVM)
        {
            if (ModelState.IsValid)
            {
                if (usuarioVM.ID > 0)
                    usuario = Repositorio.ObterPorId(usuarioVM.ID);

                usuario.Nome = usuarioVM.Nome;
                usuario.Email = usuarioVM.Email;

                if (usuarioVM.ID == 0)
                {
                    var senha = new Criptografia(SHA512.Create()).Criptografar(usuarioVM.Senha);
                    usuario.Senha = senha;
                    Repositorio.Salvar(usuario);
                }
                else
                    Repositorio.Alterar(usuario);

                return RedirectToAction("Index");
            }

            return View(usuarioVM);
        }
    }
}
