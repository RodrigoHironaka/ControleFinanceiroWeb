using AutoMapper;
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
            #endregion

            #region Dominio Para ViewModel
            CreateMap<FormaPagamento, FormaPagamentoVM>();
            CreateMap<Gasto, GastoVM>();
            CreateMap<Renda, RendaVM>();
            CreateMap<SubGasto, SubGastoVM>()
                .AfterMap((src, dest) =>{dest.GastoVM = AutoMapperConfig<Gasto, GastoVM>.Mapear(src.Gasto); }); //Outro Exemplo: dest.GastoVM = new MapperConfiguration(cfg => cfg.CreateMap<Gasto, GastoVM>()).CreateMapper().Map<GastoVM>(src.Gasto);
            #endregion
        }

    }
}
