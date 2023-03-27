using ControlFinWeb.App.ViewModels.Acesso;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Utils.Seguranca;

namespace ControlFinWeb.App.Controllers
{
    public class LoginController : Controller
    {
        private readonly RepositorioUsuario RepositorioUsuario;
        private readonly RepositorioLogModificacao RepositorioLog;

        public LoginController(RepositorioUsuario repositorioUsuario, RepositorioLogModificacao repositorioLog)
        {
            RepositorioUsuario = repositorioUsuario;
            RepositorioLog = repositorioLog;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Entrar");
        }

        public IActionResult Entrar(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new LoginVM() { ReturnUrl = ReturnUrl });
        }

        [HttpPost]
        public IActionResult Entrar(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var usuario = RepositorioUsuario.ObterPorParametros(x => x.Email == loginVM.Email && x.Situacao == Dominio.ObjetoValor.Situacao.Ativo).FirstOrDefault();

                if (usuario != null && usuario.Senha == new Criptografia(SHA256.Create()).Criptografar(loginVM.Senha) && usuario.Autorizado == Dominio.ObjetoValor.SimNao.Sim)
                {
                    Autenticar(usuario);

                    if (Url.IsLocalUrl(loginVM.ReturnUrl) && loginVM.ReturnUrl.Length > 1 && loginVM.ReturnUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase) && !loginVM.ReturnUrl.StartsWith("//", StringComparison.OrdinalIgnoreCase) && !loginVM.ReturnUrl.StartsWith("/\\", StringComparison.OrdinalIgnoreCase))
                        return Redirect(loginVM.ReturnUrl);
                    else
                    {
                        RepositorioLog.RegistrarLog("Acesso ao sistema!", usuario, $"Login[{usuario.Id}]");
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewData["Mensagem"] = "<div class='alert alert-danger'>Usuário\\Senha incorretos ou não autorizado!</div>";
                }
            }
            return View(new LoginVM());
        }

        private async void Autenticar(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
            };

            var identidadeDeUsuario = new ClaimsIdentity(claims, "Usuario");

            var claimPrincipal = new ClaimsPrincipal(identidadeDeUsuario);

            var propriedadesDeAutenticacao = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.ToLocalTime().AddHours(8),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, propriedadesDeAutenticacao);
        }

        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Entrar");
        }
    }
}
