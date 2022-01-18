using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize]
    public class RelatorioGecoiController : BaseController
    {

        public RelatorioGecoiController()
            : base(new ProtocoloEntities())
        {

        }


        [ActionName("AtendimentosGeral")]
        public ActionResult AtendimentosGeral()
        {
            PreencherViewBag();

          //  ViewBag.Situacao = new SelectList(new SituacaoHistoricoAtendimento().ListaSituacao(),
                //    "status",
                //    "descricao");

           
            return View("GeneralAtend", new SearchGeneral());
        }

        [HttpPost, ActionName("AtendimentosGeral")]
        public ActionResult AtendimentosGeral(SearchGeneral searchModel)
        {
            if (PeriodoInvalido(searchModel))
            {
                searchModel.Result = null;

                return View("GeneralAtend", searchModel);
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
                               join u in db.Motivos
                                       on p.MotivoId equals u.id
                               join c in db.StatusAtendimentos
                                       on p.StatusAtendimentoId equals c.Id

                               where (searchModel.SolicitanteId == null || p.FuncionarioClienteId == searchModel.SolicitanteId)
                               && (searchModel.ClienteId == null || p.FuncionarioCliente.ClienteId == searchModel.ClienteId)
                               && (searchModel.SistemaId == null || p.Tela.SistemaId == searchModel.SistemaId)
                               && (searchModel.UsuarioId == null || p.UsuarioId == searchModel.UsuarioId)
                               && (searchModel.MotivoId == null || p.MotivoId == searchModel.MotivoId)
                               && (searchModel.StatusId == null || p.StatusAtendimentoId == searchModel.StatusId)

                               && ((searchModel.DataInicio == null || p.data_abertura >= searchModel.DataInicio)
                               && (searchModel.DataFim == null || p.data_fechamento <= searchModel.DataFim)
                               )

                               

                               orderby p.Id
                               select p;


            if (atendimentos.Any())
            {
                PreencherSearchModel(searchModel);

                searchModel.Result = atendimentos.ToList();
                return View("imprimirAtend", searchModel);
            }
            else
            {
                searchModel.Result = null;

                Danger("Não foram encontrados resultados para essa consulta.");
                PreencherViewBag();
                return View("GeneralAtend", new SearchGeneral());
            }

            

            
        }

        private void PreencherSearchModel(SearchGenericReportModel searchModel)
        {
            if (searchModel.SolicitanteId == null)
            {
                searchModel.Solicitante = "Todos";
            }
            else
            {
                searchModel.Solicitante = db.FuncionarioClientes.Find(searchModel.SolicitanteId).Nome;
            }

            if (searchModel.ClienteId == null)
            {
                searchModel.Cliente = "Todos";
            }
            else
            {
                searchModel.Cliente = db.Clientes.Find(searchModel.ClienteId).RazaoSocial;
            }

            if (searchModel.SistemaId == null)
            {
                searchModel.Sistema = "Todos";
            }
            else
            {
                searchModel.Sistema = db.Sistemas.Find(searchModel.SistemaId).Nome;
            }

            if (searchModel.MotivoId == null)
            {
                searchModel.Motivo = "Todos";
            }
            else
            {
                searchModel.Motivo = db.Motivos.Find(searchModel.MotivoId).descricao;
            }

            if (searchModel.UsuarioId == null)
            {
                searchModel.Usuario = "Todos";
            }
            else
            {
                searchModel.Usuario = db.Usuarios.Find(searchModel.UsuarioId).Logon;
            }

            if (searchModel.StatusId == null)
            {
                searchModel.Status = "Todos";
            }
            else
            {
                searchModel.Status = db.StatusAtendimentos.Find(searchModel.StatusId).descricao;
            }
         
            
        }


        [ActionName("TarefasGeral")]
        public ActionResult TarefasGeral()
        {
            PreencherViewBag();

            return View("GeneralTarefa", new SearchGeneralTarefa());
        }

        [HttpPost, ActionName("TarefasGeral")]
        public ActionResult TarefasGeral(SearchGeneralTarefa searchModel)
        {
            if (PeriodoInvalido(searchModel))
            {
                searchModel.Result = null;

                return View("GeneralTarefa", searchModel);
            }

            searchModel.DataFim = AdjustDataFim(searchModel.DataFim);

            var tarefa = from p in db.Tarefas.AsEnumerable()
            
                               join t in db.FuncionarioClientes on p.FuncionarioClienteId equals t.Id
                               join a in db.Clientes on p.FuncionarioCliente.ClienteId equals a.Id
                               join s in db.Sistemas on p.Tela.SistemaId equals s.Id
                               join i in db.Usuarios on p.UsuarioId equals i.Id
                               join o in db.Usuarios on p.PessoaId equals o.Id     /*   <--  adicionando solicitante no relatório*/
                               join u in db.Motivos on p.MotivoId equals u.id
                               join c in db.StatusTarefas on p.StatusTarefaId equals c.Id



                         where (searchModel.SolicitanteId == null || p.FuncionarioClienteId == searchModel.SolicitanteId)
                               && (searchModel.ClienteId == null || p.FuncionarioCliente.ClienteId == searchModel.ClienteId)
                               && (searchModel.SistemaId == null || p.Tela.SistemaId == searchModel.SistemaId)
                               && (searchModel.UsuarioId == null || p.UsuarioId == searchModel.UsuarioId)
                               && (searchModel.PessoaId == null || p.PessoaId == searchModel.PessoaId)   /*   <--  adicionando solicitante no relatório*/
                               && (searchModel.MotivoId == null || p.MotivoId == searchModel.MotivoId)
                               && (searchModel.SituacaoId == null || p.StatusTarefaId == searchModel.SituacaoId)
                               && ((searchModel.DataInicio == null || p.data_abertura >= searchModel.DataInicio)
                               && (searchModel.DataFim == null || p.data_conclusao <= searchModel.DataFim))
                               
                               orderby p.Id
                               select p;


            if (tarefa.Any())
            {
                PreencherSearchModel(searchModel);

                searchModel.Result = tarefa.ToList();
                return View("imprimirTarefa", searchModel);
            }
            else
            {
                searchModel.Result = null;

                Danger("Não foram encontrados resultados para essa consulta.");
                PreencherViewBag();
                return View("GeneralTarefa", new SearchGeneralTarefa());
            }
        }

        private void PreencherSearchModels(SearchGenericReportModel searchModel)
        {
            if (searchModel.SolicitanteId == null)
            {
                searchModel.Solicitante = "Todos";
            }
            else
            {
                searchModel.Solicitante = db.FuncionarioClientes.Find(searchModel.SolicitanteId).Nome;
            }

            if (searchModel.ClienteId == null)
            {
                searchModel.Cliente = "Todos";
            }
            else
            {
                searchModel.Cliente = db.Clientes.Find(searchModel.ClienteId).RazaoSocial;
            }

            if (searchModel.SistemaId == null)
            {
                searchModel.Sistema = "Todos";
            }
            else
            {
                searchModel.Sistema = db.Sistemas.Find(searchModel.SistemaId).Nome;
            }

            if (searchModel.MotivoId == null)
            {
                searchModel.Motivo = "Todos";
            }
            else
            {
                searchModel.Motivo = db.Motivos.Find(searchModel.MotivoId).descricao;
            }

            if (searchModel.UsuarioId == null)
            {
                searchModel.Usuario = "Todos";
            }
            else
            {
                searchModel.Usuario = db.Usuarios.Find(searchModel.UsuarioId).Logon;
            }

            if (searchModel.PessoaId == null)    /*   <--  adicionando solicitante no relatório*/
            {
                searchModel.Pessoa = "Todos";
            }
            else
            {
                searchModel.Pessoa = db.Usuarios.Find(searchModel.PessoaId).Nome;
            }

            if (searchModel.SituacaoId == null)
            {
                searchModel.Situacao = "Todos";
            }
            else
            {
                searchModel.Status = db.StatusTarefas.Find(searchModel.StatusId).descricao;
            }

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

        private DateTime? AdjustDataFim(DateTime? dateTime)
            {
                return dateTime.HasValue ? dateTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : dateTime;
            }


        private void PreencherViewBag()
        {
            ViewBag.SistemaId = new SelectList(db.Sistemas.OrderBy(t => t.Nome), "Id", "Nome");
            ViewBag.ClienteId = new SelectList(db.Clientes.OrderBy(a => a.RazaoSocial), "Id", "RazaoSocial");
            ViewBag.SolicitanteId = new SelectList(db.FuncionarioClientes.OrderBy(s => s.Nome), "Id", "Nome");
            ViewBag.Motivoid = new SelectList(db.Motivos.OrderBy(s => s.descricao), "Id", "Descricao");
            ViewBag.Usuarioid = new SelectList(db.Usuarios.OrderBy(s => s.Logon), "Id", "Logon");
            ViewBag.Pessoaid = new SelectList(db.Usuarios.OrderBy(s => s.Logon), "Id", "Logon");
            ViewBag.StatusId = new SelectList(db.StatusAtendimentos.OrderBy(s => s.descricao), "Id", "descricao");
            ViewBag.SituacaoId = new SelectList(db.StatusTarefas.OrderBy(s => s.descricao), "Id", "descricao");
        }   
    }

   
}