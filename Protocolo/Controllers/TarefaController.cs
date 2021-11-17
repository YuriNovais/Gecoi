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

        // GET: Tarefa
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index()
        {
            

            var tarefas = db.Tarefas.Include(t => t.FuncionarioCliente).Include(t => t.Motivo).Include(t => t.Tela).Include(t => t.Usuario);
            return View(tarefas.ToList());
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
                   Nome = u.Nome + " - " + u.Cliente.RazaoSocial,
                   identificador = u.Id
               }).ToList();

            var telas = db.Telas
              .Select(u => new
              {
                  Nome = u.Descricao + " - " + u.Sistema.Nome,
                  ident = u.Id
              }).ToList();

            ViewBag.FuncionarioClienteId = new SelectList(solicitante.OrderBy(s => s.Nome), "identificador", "Nome");
            ViewBag.TelaId = new SelectList(telas.OrderBy(s => s.Nome), "ident", "Nome");
            ViewBag.MotivoId = new SelectList(db.Motivos.OrderBy(s => s.descricao), "Id", "Descricao");
            return View();
        }

        // POST: Tarefa/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UsuarioId,AtendimentoId,MotivoId,Prioridade,Descrição,TelaId,FuncionarioClienteId")] Tarefa tarefa, HttpPostedFileBase anexo)
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
                    StatusTarefaId = 1
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

             ViewBag.FuncionarioClienteId = new SelectList(db.FuncionarioClientes, "Id", "Nome", tarefa.FuncionarioClienteId);
             ViewBag.MotivoId = new SelectList(db.Motivos, "id", "descricao", tarefa.MotivoId);
             ViewBag.TelaId = new SelectList(db.Telas, "Id", "Descricao", tarefa.TelaId);
             ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Logon", tarefa.UsuarioId);
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
            ViewBag.MotivoId = new SelectList(db.Motivos, "id", "descricao", tarefa.MotivoId);
            ViewBag.TelaId = new SelectList(db.Telas, "Id", "Descricao", tarefa.TelaId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Logon", tarefa.UsuarioId);
            ViewBag.StatusTarefaId = new SelectList(db.StatusTarefas, "Id", "descricao", tarefa.StatusTarefaId);

            return View("Edit", tarefa);
        }

        // POST: Tarefa/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UsuarioId,AtendimentoId,MotivoId,Prioridade,Descrição,TelaId,FuncionarioClienteId, HistoricoObservacao")] Tarefa tarefa, int StatusTarefaId, HttpPostedFileBase anexo)
        {

            var usuarioLogado = GetUsuarioLogado();

            var tarefaPersisted = db.Tarefas.Find(tarefa.Id);

            tarefaPersisted.Descrição = tarefa.Descrição;
            tarefaPersisted.MotivoId = tarefa.MotivoId;
            tarefaPersisted.data_abertura = DateTime.Now;
            tarefaPersisted.StatusTarefaId= tarefa.StatusTarefaId;

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
                    StatusTarefaId = StatusTarefaId
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
                               where p.StatusTarefaId == 10 && p.StatusTarefaId == 9
                               where p.MotivoId == 19 && p.MotivoId == 21
                               select p;
            return View(correcoesMes.ToList());
        }


        public ActionResult TarefaMelhorias()
        {
            var mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var Melhorias = from p in db.Tarefas
                            where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                            where p.StatusTarefaId == 10 && p.StatusTarefaId == 9
                            where p.MotivoId == 19 && p.MotivoId == 27 && p.MotivoId == 26
                            select p;
            return View(Melhorias.ToList());
        }
    }
}
