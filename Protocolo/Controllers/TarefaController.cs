using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Protocolo.Models;

namespace Protocolo.Controllers
{
    [Authorize]
    public class TarefaController : BaseController
    {
        public TarefaController() : base(new ProtocoloEntities())
        {

        }


        [Authorize(Roles = "Administrador,Gestor")]
        [HttpPost, ActionName("FiltrarTarefa")]
        public ActionResult FiltrarTarefa(SearchGeneralTarefa FiltrosTela)
        {


            var t = db.Tarefas.AsQueryable();
            if (FiltrosTela.ClienteId > 0)
                t = t.Where(c => c.FuncionarioCliente.ClienteId == FiltrosTela.ClienteId);

            if (FiltrosTela.SolicitanteId > 0)
                t = t.Where(c => c.FuncionarioCliente.Id == FiltrosTela.SolicitanteId);

            if (FiltrosTela.SituacaoId > 0)
                t = t.Where(c => c.StatusTarefa.Id == FiltrosTela.SituacaoId);

            if (FiltrosTela.MotivoId > 0)
                t = t.Where(c => c.Motivo.id == FiltrosTela.MotivoId);

            if (FiltrosTela.SistemaId > 0)
                t = t.Where(c => c.Tela.Sistema.Id == FiltrosTela.SistemaId);

            if (FiltrosTela.UsuarioId > 0)
                t = t.Where(c => c.Usuario.Id == FiltrosTela.UsuarioId);

            if (FiltrosTela.PessoaId > 0)
                t = t.Where(c => c.PessoaId == FiltrosTela.PessoaId);

            if (FiltrosTela.DataInicio != null)
            {
                t = t.Where(c => c.data_abertura >= FiltrosTela.DataInicio);
            }

            if (FiltrosTela.DataFim != null)
            {
                FiltrosTela.DataFim = AdjustDataFim(FiltrosTela.DataFim);
                PeriodoInvalido(FiltrosTela);
                t = t.Where(c => c.data_conclusao <= FiltrosTela.DataFim);
            }


            if (Request.IsAjaxRequest())
                return PartialView("_IndexGrid", t.ToList());

            PreencherViewBag();

            SearchGeneralTarefa Filtros = new SearchGeneralTarefa();

            t = t.Where(c => c.StatusTarefaId == 1);




            Filtros.Tarefas = t.ToList();


            //-- se não foi chamado pelo ajax (botão do indexsearch) traz apenas os filtros

            return View("Index", Filtros);
        }

        // GET: Tarefa
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index(SearchGeneralTarefa FiltrosTela, Tarefa tarefa, Usuario UsuarioLogado)
        {

            Usuario USUARIOLOGADO = GetUsuarioLogado();

            StatusTarefa Situacao = db.StatusTarefas.Find(1);



            if (Session["ResponsavelFiltro"] == null)
            {
                Session["ResponsavelFiltro"] = USUARIOLOGADO;
            }
            if (Session["StatusFiltro"] == null)
            {
                Session["StatusFiltro"] = Situacao;
            }

            Usuario ResponsavelFiltro = (Usuario)Session["ResponsavelFiltro"];
            StatusTarefa StatusFiltro = (StatusTarefa)Session["StatusFiltro"];

            var t = db.Tarefas.AsQueryable();
            if (FiltrosTela.ClienteId > 0)
                t = t.Where(c => c.FuncionarioCliente.ClienteId == FiltrosTela.ClienteId);

            if (FiltrosTela.SolicitanteId > 0)
                t = t.Where(c => c.FuncionarioCliente.Id == FiltrosTela.SolicitanteId);

            if (FiltrosTela.SituacaoId > 0)
                t = t.Where(c => c.StatusTarefa.Id == FiltrosTela.SituacaoId);

            if (FiltrosTela.MotivoId > 0)
                t = t.Where(c => c.Motivo.id == FiltrosTela.MotivoId);

            if (FiltrosTela.SistemaId > 0)
                t = t.Where(c => c.Tela.Sistema.Id == FiltrosTela.SistemaId);

            if (FiltrosTela.UsuarioId > 0)
                t = t.Where(c => c.Usuario.Id == FiltrosTela.UsuarioId);

            if (FiltrosTela.PessoaId > 0)
                t = t.Where(c => c.PessoaId == UsuarioLogado.Id);

            if (FiltrosTela.DataInicio != null)
            {
                t = t.Where(c => c.data_abertura >= FiltrosTela.DataInicio);
            }

            if (FiltrosTela.DataFim != null)
            {
                FiltrosTela.DataFim = AdjustDataFim(FiltrosTela.DataFim);
                PeriodoInvalido(FiltrosTela);
                t = t.Where(c => c.data_conclusao <= FiltrosTela.DataFim);
            }

            if (Request.IsAjaxRequest())

                return PartialView("_IndexGrid", t.ToList());

            PreencherViewBag();

            SearchGeneralTarefa Filtros = new SearchGeneralTarefa();







            t = t.Where(c => c.Pessoa.Id == USUARIOLOGADO.Id);
            t = t.Where(c => c.StatusTarefaId == 1);

            Session["FiltroUsuario"] = FiltrosTela.Usuario;

          //  ViewBag.responsavel = new SelectList(db.Pessoas.ToList(), null, null, UsuarioLogado.Id);

            Filtros.Tarefas = t.ToList();


            //-- se não foi chamado pelo ajax (botão do indexsearch) traz apenas os filtros

            return View("Index", Filtros);
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
            ViewBag.StatusId = new SelectList(db.StatusAtendimentos.OrderBy(s => s.descricao), "Id", "descricao");
            ViewBag.SituacaoId = new SelectList(db.StatusTarefas.OrderBy(s => s.descricao), "Id", "descricao");
            ViewBag.PessoaId = new SelectList(db.Usuarios.OrderBy(s => s.Logon), "Id", "logon");
        }

        // GET: Tarefa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarefa tarefa = db.Tarefas.Find(id);
            if (tarefa == null)
            {
                return HttpNotFound();
            }
            return View(tarefa);
        }

        // GET: Tarefa/Create
        public ActionResult Create()
        {
            var solicitante = db.FuncionarioClientes
               .Select(u => new
               {
                   Nome = u.Nome + " / " + u.Cliente.RazaoSocial,
                   identificador = u.Id
               }).ToList();

            var telas = db.Telas
              .Select(u => new
              {
                  Nome = u.Descricao + " / " + u.Sistema.Nome,
                  ident = u.Id
              }).ToList();

            ViewBag.FuncionarioClienteId = new SelectList(solicitante.OrderBy(s => s.Nome), "identificador", "Nome");
            ViewBag.TelaId = new SelectList(telas.OrderBy(s => s.Nome), "ident", "Nome");
            ViewBag.PrioridadeId = new SelectList(db.prioridades.OrderBy(s => s.Descricao), "Id", "Descricao");
            ViewBag.MotivoId = new SelectList(db.Motivos.OrderBy(s => s.descricao), "Id", "Descricao");
            return View();
        }

        // POST: Tarefa/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UsuarioId,AtendimentoId,MotivoId,Prioridadeid,Descrição,TelaId,FuncionarioClienteId")] Tarefa tarefa, HttpPostedFileBase anexo)
        {

            tarefa.Usuario = GetUsuarioLogado();
            tarefa.data_abertura = DateTime.Now;
            tarefa.data_prevista = DateTime.Now.AddDays(30);
            tarefa.data_conclusao = DateTime.Now;


            if (ModelState.IsValid)
            {
                db.Tarefas.Add(tarefa);
                db.SaveChanges();


                var historico = new TarefaHistorico
                {
                    TarefaId = tarefa.Id,
                    Datahistorico = DateTime.Now,
                    Descricao = "Abertura do Atendimento",
                    Usuario = GetUsuarioLogado(),
                    StatusTarefaId = 1,
                    PessoaId = GetUsuarioLogado().Id
                };
                db.TarefasHistorico.Add(historico);

                var tarefaAnexo = CreateAnexo(anexo);

                if (tarefaAnexo != null)
                {
                    tarefaAnexo.TarefaId = tarefa.Id;

                    db.TarefasAnexo.Add(tarefaAnexo);
                }
                db.SaveChanges();

                Success("Registro incluido com sucesso !");

                var tarefaPersisted = db.Tarefas.Find(tarefa.Id);

                return RedirectToAction("Edit", tarefaPersisted);
            }

            ViewBag.FuncionarioClienteId = new SelectList(db.FuncionarioClientes.OrderBy(s => s.Cliente), "Id", "Nome", tarefa.FuncionarioClienteId);
            ViewBag.MotivoId = new SelectList(db.Motivos.OrderBy(s => s.descricao), "id", "descricao", tarefa.MotivoId);
            ViewBag.TelaId = new SelectList(db.Telas.OrderBy(s => s.Sistema), "Id", "Descricao", tarefa.TelaId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios.OrderBy(s => s.Logon), "Id", "Logon", tarefa.UsuarioId);
            return View(tarefa);



        }

        // GET: Tarefa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarefa tarefa = db.Tarefas.Find(id);
            if (tarefa == null)
            {
                return HttpNotFound();
            }
            ViewBag.FuncionarioClienteId = new SelectList(db.FuncionarioClientes, "Id", "Nome", tarefa.FuncionarioClienteId);
            ViewBag.MotivoId = new SelectList(db.Motivos.OrderBy(s => s.descricao), "id", "descricao", tarefa.MotivoId);
            ViewBag.TelaId = new SelectList(db.Telas, "Id", "Descricao", tarefa.TelaId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios.OrderBy(s => s.Logon),"Id", "Logon", tarefa.UsuarioId);
            ViewBag.StatusTarefaId = new SelectList(db.StatusTarefas.OrderBy(s => s.descricao),"Id", "descricao", tarefa.StatusTarefaId);
            ViewBag.PessoaId = new SelectList(db.Usuarios.OrderBy(s => s.Logon), "Id", "Logon", tarefa.PessoaId);



            return View("Edit", tarefa);
        }

        // POST: Tarefa/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UsuarioId,AtendimentoId,MotivoId,Prioridade,Descrição,TelaId,FuncionarioClienteId, HistoricoObservacao")] Tarefa tarefa, int StatusTarefaId, int PessoaId, HttpPostedFileBase anexo)
        {

            var usuarioLogado = GetUsuarioLogado();

            var tarefaPersisted = db.Tarefas.Find(tarefa.Id);

            tarefaPersisted.Descrição = tarefa.Descrição;


            db.Entry(tarefaPersisted).State = EntityState.Modified;
            if (!string.IsNullOrEmpty(tarefa.HistoricoObservacao))
            {
                var ultimoHistorico = tarefa.TarefasHistorico.LastOrDefault();

                db.TarefasHistorico.Add(new TarefaHistorico
                {
                    TarefaId = tarefa.Id,
                    Datahistorico = DateTime.Now,
                    Descricao = tarefa.HistoricoObservacao,
                    Usuario = GetUsuarioLogado(),
                    StatusTarefaId = StatusTarefaId,
                    PessoaId = PessoaId

                });
            }


            if (anexo != null)
            {
                var tarefaAnexo = CreateAnexo(anexo);

                if (tarefaAnexo != null)
                {
                    tarefaAnexo.TarefaId = tarefa.Id;

                    db.TarefasAnexo.Add(tarefaAnexo);
                }

            }

            db.SaveChanges();

            Success("Dados alterados com sucesso.");

            return RedirectToAction("Edit");
        }

        // GET: Tarefa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tarefa tarefa = db.Tarefas.Find(id);
            if (tarefa == null)
            {
                return HttpNotFound();
            }
            return View(tarefa);
        }

        // POST: Tarefa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tarefa tarefa = db.Tarefas.Find(id);
            db.Tarefas.Remove(tarefa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public FileResult DownloadAnexo(int id)
        {
            var anexo = db.TarefasAnexo.Find(id);

            return File(anexo.Arquivo, anexo.TipoArquivo, anexo.NomeArquivo);
        }

        private TarefaAnexo CreateAnexo(HttpPostedFileBase anexo)
        {
            if (anexo == null || anexo.ContentLength == 0)
            {
                return null;
            }

            var tarefaAnexo = new TarefaAnexo
            {
                NomeArquivo = Path.GetFileName(anexo.FileName),
                TipoArquivo = anexo.ContentType,
            };

            using (var reader = new BinaryReader(anexo.InputStream))
            {
                tarefaAnexo.Arquivo = reader.ReadBytes(anexo.ContentLength);
            }

            return tarefaAnexo;
        }

        [HttpPost]
        [ActionName("AdicionarAnexo")]
        public ActionResult AddAnexo(int id, HttpPostedFileBase anexo, string returnUrl)
        {
            var tarefaAnexo = CreateAnexo(anexo);

            /* if (tarefaAnexo == null)
             {
                 ModelState.AddModelError("Arquivo", "Por favor selecione um arquivo");
             }
             else
             {*/
            tarefaAnexo.TarefaId = id;

            db.TarefasAnexo.Add(tarefaAnexo);

            var ultimoHistorico = db.Tarefas.Find(id).TarefasHistorico.LastOrDefault();

            db.TarefasHistorico.Add(new TarefaHistorico
            {
                TarefaId = tarefaAnexo.TarefaId,
                Datahistorico = DateTime.Now,
                Descricao = "Anexo \"" + tarefaAnexo.NomeArquivo + "\" Adicionado",
                UsuarioId = GetUsuarioLogado().Id,
                StatusTarefaId = ultimoHistorico.StatusTarefaId
            });

            db.SaveChanges();

            Success("Anexo adicionado com sucesso.");
            //}

            var result = Details(id);

            ViewBag.returnUrl = returnUrl;

            return result;
        }

        [HttpPost]
        [ActionName("DeletarAnexo")]
        public ActionResult DelAnexo(int id, string returnUrl)
        {
            var tarefaAnexo = db.TarefasAnexo.Find(id);
            var ultimoHistorico = db.Tarefas.Find(tarefaAnexo.TarefaId).TarefasHistorico.LastOrDefault();

            db.TarefasAnexo.Remove(tarefaAnexo);

            db.TarefasHistorico.Add(new TarefaHistorico
            {
                TarefaId = tarefaAnexo.TarefaId,
                Datahistorico = DateTime.Now,
                Descricao = "Anexo \"" + tarefaAnexo.NomeArquivo + "\" deletado",
                UsuarioId = GetUsuarioLogado().Id,
                StatusTarefaId = ultimoHistorico.StatusTarefaId
            });

            db.SaveChanges();

            Success("Anexo deletado com sucesso.");

            return RedirectToAction("Edit", new { id = tarefaAnexo.TarefaId, returnUrl = returnUrl });
        }


        //Dasboard 

        public ActionResult TarefasAberta()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var TarefasAberta = from p in db.Tarefas
                                where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                                where p.StatusTarefaId == 1
                                select p;

            return View(TarefasAberta.ToList());
        }

        public ActionResult TarefasResolvidas()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var TarefasResolvidas = from p in db.Tarefas
                                    where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                                    where p.StatusTarefaId == 10
                                    select p;

            return View(TarefasResolvidas.ToList());
        }

        public ActionResult TarefasReabertas()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var TarefasReabertas = from p in db.Tarefas
                                   where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                                   where p.StatusTarefaId == 11
                                   select p;

            return View(TarefasReabertas.ToList());
        }

        public ActionResult Aberto30dias()
        {
            var mes = DateTime.Today.AddDays(-30);
            var Aberto30dias = from p in db.Tarefas
                               where p.data_abertura == mes && p.StatusTarefaId == 1
                               select p;

            return View(Aberto30dias.ToList());
        }

        public ActionResult TarefaCorrecoes()
        {
            var mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var correcoesMes = from p in db.Tarefas
                               where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                               where p.StatusTarefaId == 10
                               where p.MotivoId == 21
                               select p;
            return View(correcoesMes.ToList());
        }


        public ActionResult TarefaMelhorias()
        {
            var mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var Melhorias = from p in db.Tarefas
                            where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                            where p.StatusTarefaId == 10
                            where p.MotivoId == 26
                            select p;
            return View(Melhorias.ToList());
        }
    }
}
