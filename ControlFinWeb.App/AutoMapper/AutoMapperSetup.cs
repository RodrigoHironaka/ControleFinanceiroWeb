using AutoMapper;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.App.ViewModels;
using ControlFinWeb.Dominio.Dominios;
using ControlFinWeb.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.AutoMapper
{
    //LEMBRETE: CreateMap<Origem, Destino>
    public class AutoMapperSetup : Profile
    {
        public AutoMapperSetup()
        {
            #region ViewModel Para Dominio
            CreateMap<FormaPagamentoVM, FormaPagamento>();
            CreateMap<GastoVM, Gasto>();
            CreateMap<RendaVM, Renda>();
            CreateMap<SubGastoVM, SubGasto>()
                .AfterMap((src, dest) =>{ dest.Gasto = new Gasto { Id = src.GastoId }; }); //outro exemplo: .ForPath(dest => dest.Gasto.Id, m =>  m.MapFrom(src => src.GastoId));
            CreateMap<PessoaVM, Pessoa>()
                .AfterMap((src, dest) => { dest.PessoaRendas = new MapperConfiguration(cfg => cfg.CreateMap<PessoaRendasVM, PessoaRendas>().AfterMap((src, dest) => { dest.TipoRenda = new Renda { Id = src.TipoRendaId }; })).CreateMapper().Map<List<PessoaRendas>>(src.PessoaRendasVM); });
            CreateMap<BancoVM, Banco>()
                 .AfterMap((src, dest) => { dest.PessoaRefBanco = new Pessoa { Id = src.PessoaId }; });
            CreateMap<CartaoVM, Cartao>()
                .AfterMap((src, dest) => { dest.Banco = new Banco { Id = src.BancoId }; });
            #endregion

            #region Dominio Para ViewModel
            CreateMap<FormaPagamento, FormaPagamentoVM>();
            CreateMap<Gasto, GastoVM>();
            CreateMap<Renda, RendaVM>();
            CreateMap<SubGasto, SubGastoVM>()
                .AfterMap((src, dest) =>{dest.GastoVM = AutoMapperConfig<Gasto, GastoVM>.Mapear(src.Gasto); }); //Outro Exemplo: dest.GastoVM = new MapperConfiguration(cfg => cfg.CreateMap<Gasto, GastoVM>()).CreateMapper().Map<GastoVM>(src.Gasto);
            CreateMap<Pessoa, PessoaVM>()
                .AfterMap((src, dest) => { dest.PessoaRendasVM = new MapperConfiguration(cfg => cfg.CreateMap<PessoaRendas, PessoaRendasVM>().AfterMap((src, dest) => { dest.TipoRendaVM = AutoMapperConfig<Renda, RendaVM>.Mapear(src.TipoRenda); })).CreateMapper().Map<List<PessoaRendasVM>>(src.PessoaRendas); });
            CreateMap<Banco, BancoVM>()
                .AfterMap((src, dest) => { dest.PessoaRefBancoVM = AutoMapperConfig<Pessoa, PessoaVM>.Mapear(src.PessoaRefBanco); });
            CreateMap<Cartao, CartaoVM>()
                .AfterMap((src, dest) => { dest.BancoVM = AutoMapperConfig<Banco, BancoVM>.Mapear(src.Banco); });
            #endregion
        }

    }
}
