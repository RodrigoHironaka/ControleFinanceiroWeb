using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.App.ViewModels.Acesso;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.IO;
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
        Usuario cloneUsuario;

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
                    Situacao = usuario.Situacao,
                    Imagem = usuario.Imagem,
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
                {
                    usuario = Repositorio.ObterPorId(usuarioVM.Id);
                    cloneUsuario = (Usuario)usuario.Clone();
                }
                    
                usuario.Nome = usuarioVM.Nome;
                usuario.Email = usuarioVM.Email;
                usuario.TipoUsuario = usuarioVM.TipoUsuario;
                usuario.Situacao = usuarioVM.Situacao;
                if (!string.IsNullOrEmpty(usuarioVM.Senha)) 
                    usuario.Senha = new Criptografia(SHA256.Create()).Criptografar(usuarioVM.Senha);

                if (usuarioVM.Id == 0)
                    Repositorio.Salvar(usuario);

                if (usuarioVM.ImagemUpload != null)
                {
                    var imgPrefixo = usuario.Id + "_";
                    if (!UploadArquivo(usuarioVM.ImagemUpload, imgPrefixo))
                    {
                        return View(usuarioVM);
                    }
                    usuario.Imagem = imgPrefixo + usuarioVM.ImagemUpload.FileName;
                }

                CompararAlteracoes();

                return RedirectToAction("Index");
            }

            return View(usuarioVM);
        }

        [HttpGet]
        public IActionResult Inativar(Int64 id)
        {
            usuario = Repositorio.ObterPorId(id);
            cloneUsuario = (Usuario)usuario.Clone();
            usuario.Situacao = Situacao.Inativo;
            CompararAlteracoes();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Ativar(Int64 id)
        {
            usuario = Repositorio.ObterPorId(id);
            cloneUsuario = (Usuario)usuario.Clone();
            usuario.Situacao = Situacao.Ativo;
            CompararAlteracoes();

            return RedirectToAction("Index");

        }

        private bool UploadArquivo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo.Length <= 0) return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Usuarios", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Usuarios"));
            var arqs = dir.GetFiles("*.*");
            foreach (var arq in arqs)
            {
                if (arq.Name.StartsWith(usuario.Id.ToString()))
                    arq.Delete();
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                arquivo.CopyToAsync(stream);
            }

            return true;
        }

        private void CompararAlteracoes()
        {
            var comparer = new ObjectsComparer.Comparer<Usuario>();
            comparer.AddComparerOverride<Int64>(DoNotCompareValueComparer.Instance, member => member.Name.Contains("Id"));
            comparer.AddComparerOverride<String>(DoNotCompareValueComparer.Instance, member => member.Name.StartsWith("_"));
            var igual = comparer.Compare(cloneUsuario, usuario, out IEnumerable<Difference> diferencas);
            if (!igual)
                Repositorio.EditarRegistrarLog(usuario, diferencas, Configuracao.Usuario, String.Format("Usuário[{0}]", usuario.Id));
        }
    }
}
