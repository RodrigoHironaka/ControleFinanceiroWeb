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
                .AfterMap((src, dest) => { dest.Gasto = new Gasto { Id = src.GastoId }; }); //outro exemplo: .ForPath(dest => dest.Gasto.Id, m =>  m.MapFrom(src => src.GastoId));
            CreateMap<PessoaVM, Pessoa>().AfterMap((src, dest) =>
            {
                dest.PessoaRendas = new MapperConfiguration(cfg => cfg.CreateMap<PessoaRendasVM, PessoaRendas>()
                .AfterMap((src, dest) => { dest.TipoRenda = new Renda { Id = src.TipoRendaId }; }))
                .CreateMapper().Map<List<PessoaRendas>>(src.PessoaRendasVM);
            });
            CreateMap<BancoVM, Banco>()
                 .AfterMap((src, dest) => { dest.PessoaRefBanco = new Pessoa { Id = src.PessoaRefBancoId }; });
            CreateMap<CartaoVM, Cartao>()
                .AfterMap((src, dest) => { dest.Banco = new Banco { Id = src.BancoId }; });
            CreateMap<HoraExtraVM, HoraExtra>()
               .AfterMap((src, dest) => { dest.Pessoa = new Pessoa { Id = src.PessoaId }; });
            CreateMap<ContaBancariaVM, ContaBancaria>()
               .AfterMap((src, dest) => { dest.Banco = new Banco { Id = src.BancoId }; dest.Caixa = src.CaixaId > 0 ? new Caixa { Id = src.CaixaId } : null; });
            CreateMap<ContaVM, Conta>()
              .AfterMap((src, dest) =>
              {
                  dest.SubGasto = new SubGasto { Id = src.SubGastoId };
                  dest.Pessoa = src.PessoaId > 0 ? new Pessoa { Id = src.PessoaId } : null;

                  dest.Parcelas = new MapperConfiguration(cfg => cfg.CreateMap<ParcelaVM, Parcela>()
                 .AfterMap((src, dest) =>
                 {
                     dest.FormaPagamento = src.FormaPagamentoId > 0 ? new FormaPagamento { Id = src.FormaPagamentoId } : null;
                     dest.UsuarioCriacao = src.UsuarioCriacaoId > 0 ? new Usuario { Id = src.UsuarioCriacaoId } : null;
                     dest.UsuarioAlteracao = src.UsuarioAlteracaoId > 0 ? new Usuario { Id = src.UsuarioAlteracaoId } : null;
                 }))
                 .CreateMapper().Map<List<Parcela>>(src.ParcelasVM);

                  dest.Arquivos = new MapperConfiguration(cfg => cfg.CreateMap<ArquivoVM, Arquivo>()
                 .AfterMap((src, dest) =>
                 {
                     dest.Fatura = src.FaturaId > 0 ? new Fatura { Id = src.FaturaId } : null;
                     //dest.Conta = new Conta { Id = src.ContaId };
                 }))
                 .CreateMapper().Map<List<Arquivo>>(src.ArquivosVM);
              });
            CreateMap<FaturaVM, Fatura>()
                .AfterMap((src, dest) =>
                {
                    dest.Cartao = new Cartao { Id = src.CartaoId };
                    dest.FaturaItens = new MapperConfiguration(cfg => cfg.CreateMap<FaturaItensVM, FaturaItens>()
                    .AfterMap((src, dest) =>
                    {
                        dest.SubGasto = new SubGasto { Id = src.SubGastoId };
                        dest.Pessoa = src.PessoaId > 0 ? new Pessoa { Id = src.PessoaId } : null;
                        dest.CartaoCredito = new Fatura { Id = src.CartaoCreditoId };
                    }))
                    .CreateMapper().Map<List<FaturaItens>>(src.FaturaItensVM);
                });
            CreateMap<FaturaItensVM, FaturaItens>()
                .AfterMap((src, dest) =>
                {
                    dest.SubGasto = new SubGasto { Id = src.SubGastoId };
                    dest.Pessoa = src.PessoaId > 0 ? new Pessoa { Id = src.PessoaId } : null;
                    dest.CartaoCredito = new Fatura { Id = src.CartaoCreditoId };
                });
            #endregion

            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            #region Dominio Para ViewModel
            CreateMap<FormaPagamento, FormaPagamentoVM>();
            CreateMap<Gasto, GastoVM>();
            CreateMap<Renda, RendaVM>();
            CreateMap<SubGasto, SubGastoVM>()
                .AfterMap((src, dest) =>
                {
                    dest.GastoVM = AutoMapperConfig<Gasto, GastoVM>.Mapear(src.Gasto);
                    //Outro Exemplo: dest.GastoVM = new MapperConfiguration(cfg => cfg.CreateMap<Gasto, GastoVM>()).CreateMapper().Map<GastoVM>(src.Gasto);
                });
            CreateMap<Pessoa, PessoaVM>()
                .AfterMap((src, dest) =>
                {
                    dest.PessoaRendasVM = new MapperConfiguration(cfg => cfg.CreateMap<PessoaRendas, PessoaRendasVM>().AfterMap((src, dest) =>
                {
                    dest.TipoRendaVM = AutoMapperConfig<Renda, RendaVM>.Mapear(src.TipoRenda);
                }))
                    .CreateMapper().Map<List<PessoaRendasVM>>(src.PessoaRendas);
                });
            CreateMap<Banco, BancoVM>()
                .AfterMap((src, dest) => { dest.PessoaRefBancoVM = AutoMapperConfig<Pessoa, PessoaVM>.Mapear(src.PessoaRefBanco); });
            CreateMap<Cartao, CartaoVM>()
                .AfterMap((src, dest) => { dest.BancoVM = AutoMapperConfig<Banco, BancoVM>.Mapear(src.Banco); });
            CreateMap<HoraExtra, HoraExtraVM>()
               .AfterMap((src, dest) => { dest.PessoaVM = AutoMapperConfig<Pessoa, PessoaVM>.Mapear(src.Pessoa); });
            CreateMap<ContaBancaria, ContaBancariaVM>()
             .AfterMap((src, dest) =>
             {
                 dest.BancoVM = AutoMapperConfig<Banco, BancoVM>.Mapear(src.Banco);
                 dest.CaixaVM = AutoMapperConfig<Caixa, CaixaVM>.Mapear(src.Caixa);
             });
            CreateMap<Conta, ContaVM>()
             .AfterMap((src, dest) =>
             {
                 dest.SubGastoVM = AutoMapperConfig<SubGasto, SubGastoVM>.Mapear(src.SubGasto);
                 dest.SubGastoVM.GastoVM = AutoMapperConfig<Gasto, GastoVM>.Mapear(src.SubGasto.Gasto);
                 dest.PessoaVM = AutoMapperConfig<Pessoa, PessoaVM>.Mapear(src.Pessoa);

             })
             .AfterMap((src, dest) =>
              {
                  dest.ParcelasVM = new MapperConfiguration(cfg => cfg.CreateMap<Parcela, ParcelaVM>().AfterMap((src, dest) =>
                  {
                      dest.FormaPagamentoVM = AutoMapperConfig<FormaPagamento, FormaPagamentoVM>.Mapear(src.FormaPagamento);
                      dest.UsuarioCriacaoVM = AutoMapperConfig<Usuario, UsuarioVM>.Mapear(src.UsuarioCriacao);
                      dest.UsuarioAlteracaoVM = AutoMapperConfig<Usuario, UsuarioVM>.Mapear(src.UsuarioAlteracao);
                  }))
                  .CreateMapper().Map<List<ParcelaVM>>(src.Parcelas);

                  dest.ArquivosVM = new MapperConfiguration(cfg => cfg.CreateMap<Arquivo, ArquivoVM>().AfterMap((src, dest) =>
                  {
                      dest.ContaVM = AutoMapperConfig<Conta, ContaVM>.Mapear(src.Conta);
                  }))
                  .CreateMapper().Map<List<ArquivoVM>>(src.Arquivos);

              });
            CreateMap<Fatura, FaturaVM>()
                .AfterMap((src, dest) => 
                { 
                    dest.CartaoVM = AutoMapperConfig<Cartao, CartaoVM>.Mapear(src.Cartao);
                })
                 .AfterMap((src, dest) =>
                 {
                     dest.FaturaItensVM = new MapperConfiguration(cfg => cfg.CreateMap<FaturaItens, FaturaItensVM>().AfterMap((src, dest) =>
                     {
                         dest.SubGastoVM = AutoMapperConfig<SubGasto, SubGastoVM>.Mapear(src.SubGasto);
                         dest.PessoaVM = AutoMapperConfig<Pessoa, PessoaVM>.Mapear(src.Pessoa);

                     }))
                     .CreateMapper().Map<List<FaturaItensVM>>(src.FaturaItens);

                 });
            CreateMap<FaturaItens, FaturaItensVM>()
                .AfterMap((src, dest) =>
                {
                    dest.SubGastoVM = AutoMapperConfig<SubGasto, SubGastoVM>.Mapear(src.SubGasto);
                    dest.PessoaVM = AutoMapperConfig<Pessoa, PessoaVM>.Mapear(src.Pessoa);
                    dest.CartaoCreditoVM = AutoMapperConfig<Fatura, FaturaVM>.Mapear(src.CartaoCredito);
                });
            #endregion
        }

    }
}
