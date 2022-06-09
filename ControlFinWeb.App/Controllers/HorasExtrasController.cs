﻿using App1.Repositorio.Configuracao;
using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.Controllers
{
    public class HorasExtrasController : Controller
    {
        private readonly RepositorioHoraExtra Repositorio;
        private readonly IMapper Mapper;

        public HorasExtrasController(RepositorioHoraExtra repositorio, IMapper mapper)
        {
            Repositorio = repositorio;
            Mapper = mapper;
        }

        HoraExtra horaExtra = new HoraExtra();
        HoraExtraVM horaExtraVM = new HoraExtraVM();

        public IActionResult Index()
        {
            IEnumerable<HoraExtra> horasExtras = Repositorio.ObterTodos();
            List<HoraExtraVM> horasExtrasVM = Mapper.Map<List<HoraExtraVM>>(horasExtras);
            return View(horasExtrasVM);
        }

        public IActionResult Editar(Int64 Id = 0)
        {
            if (Id > 0)
            {
                horaExtra = Repositorio.ObterPorId(Id);
                horaExtraVM = Mapper.Map<HoraExtraVM>(horaExtra);
            }

            ViewBag.PessoaId = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", Id);
            return View(horaExtraVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(HoraExtraVM horaExtraVM)
        {
            if (ModelState.IsValid)
            {
                if (horaExtraVM.Id > 0)
                {
                    horaExtra = Repositorio.ObterPorId(horaExtraVM.Id);
                    horaExtra = Mapper.Map(horaExtraVM, horaExtra);
                    horaExtra.UsuarioAlteracao = Configuracao.Usuario;
                    Repositorio.Alterar(horaExtra);
                }
                else
                {
                    horaExtra = Mapper.Map(horaExtraVM, horaExtra);
                    horaExtra.UsuarioCriacao = Configuracao.Usuario;
                    Repositorio.Salvar(horaExtra);
                }

                return RedirectToAction("Index");
            }
            ViewBag.PessoaId = new SelectList(new RepositorioPessoa(NHibernateHelper.ObterSessao()).ObterPorParametros(x => x.Situacao == Situacao.Ativo), "Id", "Nome", horaExtraVM.Id);
           
            return View(horaExtraVM);
           
           
        }

        [HttpPost]
        public JsonResult Deletar(Int64 Id)
        {
            horaExtra = Repositorio.ObterPorId(Id);
            Repositorio.Excluir(horaExtra);
            return Json(horaExtra.Nome + "excluído com sucesso");
        }
    }
}