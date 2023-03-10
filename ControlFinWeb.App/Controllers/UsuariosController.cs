using AutoMapper;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.App.ViewModels.Acesso;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Utils.Seguranca;

namespace ControlFinWeb.App.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly RepositorioUsuario Repositorio;
        private readonly IMapper Mapper;
        public UsuariosController(RepositorioUsuario repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        Usuario usuario = new Usuario();
        UsuarioVM usuarioVM = new UsuarioVM();
        //IList<UsuarioVM> usuariosVM = new List<UsuarioVM>();
        //IList<Usuario> usuarios = new List<Usuario>();

        public IActionResult Index()
        {
            IEnumerable<Usuario> usuarios = Repositorio.ObterTodos();
            List<UsuarioVM> usuariosVM = Mapper.Map<List<UsuarioVM>>(usuarios);
            return View(usuariosVM);
        }

        public IActionResult Editar(Int64 ID = 0)
        {
            if (ID > 0)
            {
                usuario = Repositorio.ObterPorId(ID);

                usuarioVM = new UsuarioVM
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    TipoUsuario = usuario.TipoUsuario,
                    Situacao = usuario.Situacao
                };
            }

            return View(usuarioVM);
        }

        [HttpPost]
        public IActionResult Editar(UsuarioVM usuarioVM)
        {
            if (ModelState.IsValid)
            {
                if (usuarioVM.Id > 0)
                    usuario = Repositorio.ObterPorId(usuarioVM.Id);

                usuario.Nome = usuarioVM.Nome;
                usuario.Email = usuarioVM.Email;
                usuario.TipoUsuario = usuarioVM.TipoUsuario;
                usuario.Situacao = usuarioVM.Situacao;
                if (!string.IsNullOrEmpty(usuarioVM.Senha)) 
                    usuario.Senha = new Criptografia(SHA512.Create()).Criptografar(usuarioVM.Senha); 

                if (usuarioVM.Id == 0)
                    Repositorio.Salvar(usuario);
                else
                    Repositorio.Alterar(usuario);

                return new EmptyResult();
            }

            return View(usuarioVM);
        }

        [HttpGet]
        public IActionResult Inativar(Int64 id)
        {
            var usuario = Repositorio.ObterPorId(id);
            usuario.Situacao = Situacao.Inativo;
            Repositorio.Alterar(usuario);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Ativar(Int64 id)
        {
            var usuario = Repositorio.ObterPorId(id);
            usuario.Situacao = Situacao.Ativo;
            Repositorio.Alterar(usuario);

            return RedirectToAction("Index");

        }
    }
}
