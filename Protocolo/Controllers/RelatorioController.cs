using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;


/*
namespace Protocolo.Controllers
{
    [Authorize]
    public class RelatorioController : BaseController
    {
        public RelatorioController()
            : base(new ProtocoloEntities())
        {

        }

        [ActionName("ProtocoloAssunto")]
        public ActionResult ProcessSubject()
        {
            PreencherViewBag();

            return View("ProcessSubject", new SearchSubject());
        }

        [HttpPost, ActionName("ProtocoloAssunto")]
        public ActionResult ProcessSubject(SearchSubject searchModel)
        {
            if (PeriodoInvalido(searchModel))
            {
                searchModel.GroupedResult = null;

                return View("ProcessSubject", searchModel);
            }

            searchModel.DataFim = AdjustDataFim(searchModel.DataFim);

            var protocolosGrouped = from p in db.Protocolos.AsEnumerable()
                join t in db.TiposDocumento
                    on p.TipoDocumentoId equals t.Id
                join s in db.Setores
                    on p.SetorAtualId equals s.Id

                where (searchModel.TipoDocumentoId == null || p.TipoDocumentoId == searchModel.TipoDocumentoId)
                    && (searchModel.AssuntoId == null || p.AssuntoId == searchModel.AssuntoId)
                    && (searchModel.SetorId == null || p.SetorAtualId == searchModel.SetorId)
                    && (string.IsNullOrEmpty(searchModel.Status) || p.Status == searchModel.Status)
                    && (searchModel.Atrasados == false || IsAtrasado(p).Compile().Invoke(p))
                    && ((searchModel.DataInicio == null || p.DataAbertura >= searchModel.DataInicio) 
                    && (searchModel.DataFim == null || p.DataAbertura <= searchModel.DataFim))

                orderby p.Numero, p.Ano
                group p by p.Assunto into a
                select new Group<Assunto, Models.Protocolo>
                {
                    Key = a.Key,
                    Values = a
                };

            if (protocolosGrouped.Any())
            {
                PreencherSearchModel(searchModel);

                searchModel.GroupedResult = protocolosGrouped.OrderBy(g => g.Key.Descricao).ToList();
            }
            else
            {
                searchModel.GroupedResult = null;

                Danger("Não foram encontrados resultados para essa consulta.");
            }

            PreencherViewBag();

            PreencherInstituicao(searchModel);

            return View("ProcessSubject", searchModel);
        }

        [HttpGet]
        public JsonResult FluxoProcesso(int id)
        {
            var fluxoProtocolo = db.FluxoProtocolo
                .Include(f => f.Protocolo)
                .Include(f => f.Protocolo.TipoDocumento)
                .Include(f => f.Protocolo.Pessoa)
                .Include(f => f.Setor)
                .Include(f => f.UsuarioRecebimento)
                .Where(f => 
                    f.Protocolo.Id == id && f.Protocolo.Status != StatusProtocolo.Cancelado.Codigo
                 )
                .OrderBy(f => f.DataEntrada)
                .ToList();

            // Guarda o protocolo para futura referência
            var protocolo = fluxoProtocolo.First().Protocolo;

            var searchModel = new SearchFlow
            {
                NumeroAno = protocolo.NumeroAno,
                TipoDocumento = protocolo.TipoDocumento.Descricao,
                Assunto = protocolo.Assunto.Descricao,
                Requerente = protocolo.Pessoa.NomeRazao,
                FluxoProtocolo = fluxoProtocolo
            };

            // Pega o único registro de instituição do banco caso exista
            if (db.Intituicoes.Any())
            {
                searchModel.Instituicao = db.Intituicoes.Single();
            }

            return Json(RenderRazorViewToString("_ProcessFlow", searchModel), JsonRequestBehavior.AllowGet);
        }

          [ActionName("ProtocolosGeral")]
          public ActionResult GeneralProcess()
          {
              PreencherViewBag();

              return View("GeneralProcess", new SearchGeneral());
          }

          [HttpPost, ActionName("ProtocolosGeral")]
          public ActionResult GeneralProcess(SearchGeneral searchModel)
          {
              if (PeriodoInvalido(searchModel))
              {
                  searchModel.Result = null;

                  return View("GeneralProcess", searchModel);
              }

              searchModel.DataFim = AdjustDataFim(searchModel.DataFim);

              var protocolos = from p in db.Protocolos.AsEnumerable()
                  join t in db.TiposDocumento
                      on p.TipoDocumentoId equals t.Id
                  join a in db.Assuntos
                      on p.AssuntoId equals a.Id
                  join s in db.Setores
                      on p.SetorAtualId equals s.Id

                  where (searchModel.TipoDocumentoId == null || p.TipoDocumentoId == searchModel.TipoDocumentoId)
                      && (searchModel.AssuntoId == null || p.AssuntoId == searchModel.AssuntoId)
                      && (searchModel.SetorId == null || p.SetorAtualId == searchModel.SetorId)
                      && (string.IsNullOrEmpty(searchModel.Status) || p.Status == searchModel.Status)
                      && (searchModel.Atrasados == false || IsAtrasado(p).Compile().Invoke(p))
                      && ((searchModel.DataInicio == null || p.DataAbertura >= searchModel.DataInicio) 
                      && (searchModel.DataFim == null || p.DataAbertura <= searchModel.DataFim))

                  orderby p.Numero, p.Ano
                  select p;

              if (protocolos.Any())
              {
                  PreencherSearchModel(searchModel);

                  searchModel.Result = protocolos.ToList();
              }
              else
              {
                  searchModel.Result = null;

                  Danger("Não foram encontrados resultados para essa consulta.");
              }

              PreencherViewBag();

              PreencherInstituicao(searchModel);

              return View("GeneralProcess", searchModel);
          }


          [HttpPost, ActionName("AtendimentoGeral")]
          public ActionResult GeneralProcess(SearchGeneral searchModel)
          {
              if (PeriodoInvalido(searchModel))
              {
                  searchModel.Result = null;

                  return View("GeneralProcess", searchModel);
              }

              searchModel.DataFim = AdjustDataFim(searchModel.DataFim);

              var atendimentos = from p in db.Atendimentos.AsEnumerable()
                                 join t in db.FuncionarioClientes
                                     on p.FuncionarioClienteId equals t.Id
                                 join a in db.Clientes
                                     on p.FuncionarioCliente.ClienteId equals a.Id
                                 join s in db.Sistemas
                                     on p.Tela.SistemaId equals s.Id
                                 join i in db.Usuarios
                                     on p.UsuarioId equals i.Id

                                 where (searchModel.SolicitanteId== null || p.FuncionarioClienteId == searchModel.SolicitanteId)
                                     && (searchModel.ClienteId == null || p.FuncionarioCliente.ClienteId == searchModel.ClienteId)
                                     && (searchModel.SistemaId == null || p.Tela.SistemaId == searchModel.SistemaId)
                                     && ((searchModel.DataInicio == null || p.data_abertura >= searchModel.DataInicio)
                                     && (searchModel.DataFim == null || p.data_fechamento <= searchModel.DataFim))

                               orderby p.Id
                               select p;

              if (atendimentos.Any())
              {
                  PreencherSearchModel(searchModel);

                  searchModel.Result = atendimentos.ToList();
              }
              else
              {
                  searchModel.Result = null;

                  Danger("Não foram encontrados resultados para essa consulta.");
              }

              PreencherViewBag();

              PreencherInstituicao(searchModel);

              return View("GeneralProcess", searchModel);
          }
        

        [ActionName("ProtocolosSetor")]
        public ActionResult ProcessSector()
        {
            PreencherViewBag();

            return View("ProcessSector", new SearchSector());
        }

        [HttpPost, ActionName("ProtocolosSetor")]
        public ActionResult ProcessSector(SearchSector searchModel)
        {
            if (PeriodoInvalido(searchModel))
            {
                searchModel.GroupedResult = null;

                return View("ProcessSector", searchModel);
            }

            searchModel.DataFim = AdjustDataFim(searchModel.DataFim);

            var protocolosGrouped = from p in db.Protocolos.AsEnumerable()
                join t in db.TiposDocumento
                    on p.TipoDocumentoId equals t.Id
                join a in db.Assuntos
                    on p.AssuntoId equals a.Id

                where (searchModel.TipoDocumentoId == null || p.TipoDocumentoId == searchModel.TipoDocumentoId)
                    && (searchModel.AssuntoId == null || p.AssuntoId == searchModel.AssuntoId)
                    && (searchModel.SetorId == null || p.SetorAtualId == searchModel.SetorId)
                    && (string.IsNullOrEmpty(searchModel.Status) || p.Status == searchModel.Status)
                    && (searchModel.Atrasados == false || IsAtrasado(p).Compile().Invoke(p))
                    && ((searchModel.DataInicio == null || p.DataAbertura >= searchModel.DataInicio) 
                    && (searchModel.DataFim == null || p.DataAbertura <= searchModel.DataFim))

                orderby p.Numero, p.Ano
                group p by p.SetorAtual into s
                select new Group<Setor, Models.Protocolo>
                {
                    Key = s.Key,
                    Values = s
                };

            if (protocolosGrouped.Any())
            {
                PreencherSearchModel(searchModel);

                searchModel.GroupedResult = protocolosGrouped.OrderBy(g => g.Key.Nome).ToList();
            }
            else
            {
                searchModel.GroupedResult = null;

                Danger("Não foram encontrados resultados para essa consulta.");
            }

            PreencherViewBag();

            PreencherInstituicao(searchModel);

            return View("ProcessSector", searchModel);
        }

        [ActionName("ProtocolosAnoSetor")]
        public ActionResult ProcessSectorYear()
        {
            PreencherViewBag();

            ViewBag.Ano = new SelectList(db.NumeroProtocolo.Select(n => n.Ano).OrderBy(ano => ano));

            return View("ProcessSectorYear", new SearchSectorYear());
        }

        [HttpPost, ActionName("ProtocolosAnoSetor")]
        public ActionResult ProcessSectorYear(SearchSectorYear searchModel)
        {
            if (PeriodoInvalido(searchModel))
            {
                searchModel.Result = null;

                ViewBag.Ano = new SelectList(db.NumeroProtocolo.Select(n => n.Ano).OrderBy(ano => ano));

                return View("ProcessSectorYear", searchModel);
            }

            searchModel.DataFim = AdjustDataFim(searchModel.DataFim);

            var models = db.Protocolos.AsEnumerable()
                .Where(p => 
                    (searchModel.TipoDocumentoId == null || p.TipoDocumentoId == searchModel.TipoDocumentoId)
                    && (searchModel.AssuntoId == null || p.AssuntoId == searchModel.AssuntoId)
                    && (searchModel.SetorId == null || p.SetorAtualId == searchModel.SetorId)
                    && (string.IsNullOrEmpty(searchModel.Status) || p.Status == searchModel.Status)
                    && (searchModel.Atrasados == false || IsAtrasado(p).Compile().Invoke(p))
                    && ((searchModel.DataInicio == null || p.DataAbertura >= searchModel.DataInicio) 
                    && (searchModel.DataFim == null || p.DataAbertura <= searchModel.DataFim))
                )
                .GroupBy(p => new
                {
                    Setor = p.SetorAtual,
                    Mes = p.DataAbertura.Month
                })
                .Select(g => new {
                    Setor = g.Key.Setor,
                    Mes = g.Key.Mes,
                    Total = g.Count()
                })
                .OrderBy(g => g.Setor.Nome)
                .ThenBy(g => g.Mes)
                .ToList();

            if (models.Any())
            {
                PreencherSearchModel(searchModel);

                searchModel.Result = new Dictionary<Setor, Dictionary<int, int>>();

                Dictionary<int, int> totalMes;

                foreach (var model in models)
                {
                    // Se o setor já foi adicionado ao dictionary, adiciona o mês e a quantidade
                    if (searchModel.Result.TryGetValue(model.Setor, out totalMes))
                    {
                        totalMes.Add(model.Mes, model.Total);
                    }
                    // se não, adiciona tudo 
                    else
                    {
                        totalMes = new Dictionary<int, int>();
                        totalMes.Add(model.Mes, model.Total);

                        searchModel.Result.Add(model.Setor, totalMes);
                    }
                }

                searchModel.TotalGeralPorSetor = new Dictionary<Setor, int>();

                foreach (var result in searchModel.Result)
                {
                    var keys = result.Value.Keys;
                    var values = result.Value;
                    var total = 0;

                    foreach (var key in keys)
                    {
                        total += values[key];
                    }

                    searchModel.TotalGeralPorSetor.Add(result.Key, total);
                }

                searchModel.TotalGeralPorMes = new Dictionary<int, int>();

                foreach (var mesTotal in searchModel.Result.Values)
                {
                    var keys = mesTotal.Keys;
                    int total;

                    foreach(var key in keys)
                    {
                        if (searchModel.TotalGeralPorMes.TryGetValue(key, out total))
                        {
                            total += mesTotal[key];

                            searchModel.TotalGeralPorMes[key] = total;
                        }
                        else
                        {
                            total = mesTotal[key];

                            searchModel.TotalGeralPorMes.Add(key, total);
                        }
                    }
                }

                searchModel.TotalGeral = 0;

                foreach(var totalGeral in searchModel.TotalGeralPorMes.Values)
                {
                    searchModel.TotalGeral += totalGeral;
                }
            }
            else
            {
                searchModel.Result = null;

                Danger("Não foram encontrados resultados para essa consulta.");
            }

            PreencherViewBag();

            ViewBag.Ano = new SelectList(db.NumeroProtocolo.Select(n => n.Ano).OrderBy(ano => ano));

            PreencherInstituicao(searchModel);

            return View("ProcessSectorYear", searchModel);
        }

        [ActionName("ProtocolosTipoDocumento")]
        public ActionResult ProcessDocumentType()
        {
            PreencherViewBag();

            return View("ProcessDocumentType", new SearchDocumentType());
        }

        [HttpPost, ActionName("ProtocolosTipoDocumento")]
        public ActionResult ProcessDocumentType(SearchDocumentType searchModel)
        {
            if (PeriodoInvalido(searchModel))
            {
                searchModel.GroupedResult = null;

                return View("ProcessDocumentType", searchModel);
            }

            searchModel.DataFim = AdjustDataFim(searchModel.DataFim);

            var protocolosGrouped = from p in db.Protocolos.AsEnumerable()
                join s in db.Setores
                    on p.SetorAtualId equals s.Id
                join a in db.Assuntos
                    on p.AssuntoId equals a.Id

                where (searchModel.TipoDocumentoId == null || p.TipoDocumentoId == searchModel.TipoDocumentoId)
                    && (searchModel.AssuntoId == null || p.AssuntoId == searchModel.AssuntoId)
                    && (searchModel.SetorId == null || p.SetorAtualId == searchModel.SetorId)
                    && (string.IsNullOrEmpty(searchModel.Status) || p.Status == searchModel.Status)
                    && (searchModel.Atrasados == false || IsAtrasado(p).Compile().Invoke(p))
                    && ((searchModel.DataInicio == null || p.DataAbertura >= searchModel.DataInicio) 
                    && (searchModel.DataFim == null || p.DataAbertura <= searchModel.DataFim))

                orderby p.Numero, p.Ano
                group p by p.TipoDocumento into t
                select new Group<TipoDocumento, Models.Protocolo>
                {
                    Key = t.Key,
                    Values = t
                };

            if (protocolosGrouped.Any())
            {
                PreencherSearchModel(searchModel);

                searchModel.GroupedResult = protocolosGrouped.OrderBy(g => g.Key.Descricao).ToList();
            }
            else
            {
                searchModel.GroupedResult = null;

                Danger("Não foram encontrados resultados para essa consulta.");
            }

            PreencherViewBag();

            PreencherInstituicao(searchModel);

            return View("ProcessDocumentType", searchModel);
        }

        private bool PeriodoInvalido(SearchGenericReportModel searchModel)
        {
            if (searchModel.DataInicio != null && searchModel.DataFim != null && searchModel.DataInicio > searchModel.DataFim)
            {
                Danger("Data Inícial deve ser inferior a Data Final.");

                PreencherViewBag();

                return true;
            }

            return false;
        }

        private void PreencherViewBag()
        {
            ViewBag.TipoDocumentoId = new SelectList(db.TiposDocumento.OrderBy(t => t.Descricao), "Id", "Descricao");
            ViewBag.AssuntoId = new SelectList(db.Assuntos.OrderBy(a => a.Descricao), "Id", "Descricao");
            ViewBag.SetorId = new SelectList(db.Setores.OrderBy(s => s.Nome), "Id", "Nome");
            ViewBag.Status = new SelectList(StatusProtocolo.ToOrderedList(), "Codigo", "Descricao");
        }

        // Pega o único registro de instituição do banco caso exista
        private void PreencherInstituicao(SearchGenericReportModel searchModel)
        {
            if (db.Intituicoes.Any())
            {
                searchModel.Instituicao = db.Intituicoes.Single();
            }
        }

        private void PreencherSearchModel(SearchGenericReportModel searchModel)
        {
            if (searchModel.TipoDocumentoId == null)
            {
                searchModel.TipoDocumento = "Todos";
            }
            else
            {
                searchModel.TipoDocumento = db.TiposDocumento.Find(searchModel.TipoDocumentoId).Descricao;
            }

            if (searchModel.AssuntoId == null)
            {
                searchModel.Assunto = "Todos";
            }
            else
            {
                searchModel.Assunto = db.Assuntos.Find(searchModel.AssuntoId).Descricao;
            }

            if (searchModel.SetorId == null)
            {
                searchModel.Setor = "Todos";
            }
            else
            {
                searchModel.Setor = db.Setores.Find(searchModel.SetorId).Nome;
            }

            if (searchModel.Status == null)
            {
                searchModel.DescricaoStatus = "Todos";
            }
            else
            {
                searchModel.DescricaoStatus = StatusProtocolo.DescricaoFromCodigo(searchModel.Status);
            }
        }

        private Expression<Func<Models.Protocolo, bool>> IsAtrasado(Models.Protocolo protocolo)
        {
            if (protocolo.Assunto == null)
            {
                return p => false;
            }

            var prazo = protocolo.Assunto.Prazo;

            var diasCriacao = DateTime.Now.Date.Subtract(protocolo.DataAbertura.Date).Days;

            return p => (diasCriacao > prazo);
        }

        private DateTime? AdjustDataFim(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : dateTime;
        }
    }
}
        */