using Newtonsoft.Json;
using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize]
    public class AtendimentoController : BaseController
    {
        public AtendimentoController() : base(new ProtocoloEntities())
        {

        }



        // GET: Atendimento
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index()
        {
            var atendimentos = db.Atendimentos.Include(a => a.Motivo).Include(a => a.Tela).Include(a => a.FuncionarioCliente).Include(a => a.Usuario);
            return View(atendimentos.ToList());
        }

        // GET: Atendimento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atendimento atendimento = db.Atendimentos.Find(id);
            if (atendimento == null)
            {
                return HttpNotFound();
            }
            return View(atendimento);
        }

        // GET: Atendimento/Create
        public ActionResult Create()
        {

            // pega o nome de outra tabela 
            var solicitante = db.FuncionarioClientes
               .Select(u => new
               {
                   Nome = u.Cliente.RazaoSocial + " - " + u.Nome,
                   identificador = u.Id
               }).ToList();


            /*  var solicitante = db.Clientes
                  .Select(u => new
                  {
                      Nome = u.RazaoSocial + " - " + u.FuncionarioCliente.Nome,
                      identificador = u.Id
                  }).ToList();*/



            var telas = db.Telas
              .Select(u => new
              {
                  Nome = u.Sistema.Nome + " - " + u.Descricao,
                  ident = u.Id
              }).ToList();

            ViewBag.FuncionarioClienteId = new SelectList(solicitante.OrderBy(s => s.Nome), "identificador", "Nome");
            ViewBag.TelaId = new SelectList(telas.OrderBy(s => s.Nome), "ident", "Nome");
            ViewBag.MotivoId = new SelectList(db.Motivos.OrderBy(s => s.descricao), "Id", "Descricao");


            return View();
        }

        // POST: Atendimento/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,funcionarioclienteid,TelaId,MotivoId,Problema,UsuarioId,statusid")] Atendimento atendimento, HttpPostedFileBase anexo)
        {
            //-- pegando o usuário logado no sistema

            atendimento.Usuario = GetUsuarioLogado();
            atendimento.data_abertura = DateTime.Now;
            atendimento.data_fechamento = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Atendimentos.Add(atendimento);
                db.SaveChanges();

                var historico = new AtendimentoHistorico
                {
                    AtendimentoId = atendimento.Id,
                    Datahistorico = DateTime.Now,
                    Solucao = "Abertura do Atendimento",
                    Usuario = GetUsuarioLogado(),
                    StatusAtendimentoId = 1
                };
                db.AtendimentosHistorico.Add(historico);

                var atendimentoAnexo = CreateAnexo(anexo);

                if (atendimentoAnexo != null)
                {
                    atendimentoAnexo.AtendimentoId = atendimento.Id;

                    db.AtendimentosAnexo.Add(atendimentoAnexo);
                }

                db.SaveChanges();
                Success("Atendimento Registrado com sucesso.");
                //-- Após Gravar, para não retornar ao Grid, Reabre a View de Edição
                var atendimentoPersisted = db.Atendimentos.Find(atendimento.Id);
                //Atendimento atendimentoNovo = db.Atendimentos.Find(novoId);
                //return View("Edit",atendimentoNovo);
                return RedirectToAction("Edit", atendimentoPersisted);
            }

            ViewBag.MotivoId = new SelectList(db.Motivos, "id", "descricao", atendimento.MotivoId);
            ViewBag.TelaId = new SelectList(db.Telas, "Id", "descricao", atendimento.TelaId);
            ViewBag.FuncionarioClienteId = new SelectList(db.FuncionarioClientes, "Id", "Nome", atendimento.FuncionarioClienteId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Logon", atendimento.UsuarioId);
            return View(atendimento);
        }

        // GET: Atendimento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return Redirect("home");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atendimento atendimento = db.Atendimentos.Find(id);
            if (atendimento == null)
            {
                return HttpNotFound();
            }
            ViewBag.MotivoId = new SelectList(db.Motivos, "id", "descricao", atendimento.MotivoId);
            ViewBag.TelaId = new SelectList(db.Telas, "Id", "descricao", atendimento.TelaId);
            ViewBag.FuncionarioClienteId = new SelectList(db.FuncionarioClientes, "Id", "Nome", atendimento.FuncionarioClienteId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Logon", atendimento.UsuarioId);
            ViewBag.StatusAtendimentoId = new SelectList(db.StatusAtendimentos, "Id", "descricao", atendimento.StatusAtendimentoId);


            return View("Edit", atendimento);
        }

        // POST: Atendimento/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TelaId,MotivoId,Problema,HistoricoObservacao,StatusAtendimentoId")] Atendimento atendimento, int StatusAtendimentoId, HttpPostedFileBase anexo)
        {

            var usuarioLogado = GetUsuarioLogado();

            var atendimentoPersisted = db.Atendimentos.Find(atendimento.Id);

            atendimentoPersisted.Problema = atendimento.Problema;
            atendimentoPersisted.MotivoId = atendimento.MotivoId;
            atendimentoPersisted.data_abertura = DateTime.Now;
            atendimentoPersisted.StatusAtendimentoId = atendimento.StatusAtendimentoId;

            /*atendimentoPersisted.data_fechamento = DateTime.Now;*/



            db.Entry(atendimentoPersisted).State = EntityState.Modified;
            if (!string.IsNullOrEmpty(atendimento.HistoricoObservacao))
            {
                var ultimoHistorico = atendimento.AtendimentosHistorico.LastOrDefault();

                db.AtendimentosHistorico.Add(new AtendimentoHistorico
                {
                    AtendimentoId = atendimento.Id,
                    Datahistorico = DateTime.Now,
                    Solucao = atendimento.HistoricoObservacao,
                    Usuario = GetUsuarioLogado(),
                    StatusAtendimentoId = StatusAtendimentoId

                });
            }
            if (anexo != null)
            {
                var atendimentoAnexo = CreateAnexo(anexo);

                if (atendimentoAnexo != null)
                {
                    atendimentoAnexo.AtendimentoId = atendimento.Id;

                    db.AtendimentosAnexo.Add(atendimentoAnexo);
                }

            }

            db.SaveChanges();

            Success("Dados alterados com sucesso.");

            return RedirectToAction("Edit");
        }

        // GET: Atendimento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atendimento atendimento = db.Atendimentos.Find(id);
            if (atendimento == null)
            {
                return HttpNotFound();
            }
            return View(atendimento);
        }

        // POST: Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Atendimento atendimento = db.Atendimentos.Find(id);
            db.Atendimentos.Remove(atendimento);
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
            var anexo = db.AtendimentosAnexo.Find(id);

            return File(anexo.Arquivo, anexo.TipoArquivo, anexo.NomeArquivo);
        }

        private AtendimentoAnexo CreateAnexo(HttpPostedFileBase anexo)
        {
            if (anexo == null || anexo.ContentLength == 0)
            {
                return null;
            }

            var atendimentoAnexo = new AtendimentoAnexo
            {
                NomeArquivo = Path.GetFileName(anexo.FileName),
                TipoArquivo = anexo.ContentType,
            };

            using (var reader = new BinaryReader(anexo.InputStream))
            {
                atendimentoAnexo.Arquivo = reader.ReadBytes(anexo.ContentLength);
            }

            return atendimentoAnexo;
        }

        [HttpPost]
        [ActionName("AdicionarAnexo")]
        public ActionResult AddAnexo(int id, HttpPostedFileBase anexo, string returnUrl)
        {
            var atendimentoAnexo = CreateAnexo(anexo);

            if (atendimentoAnexo == null)
            {
                ModelState.AddModelError("Arquivo", "Por favor selecione um arquivo");
            }
            else
            {
                atendimentoAnexo.AtendimentoId = id;

                db.AtendimentosAnexo.Add(atendimentoAnexo);

                var ultimoHistorico = db.Atendimentos.Find(id).AtendimentosHistorico.LastOrDefault();

                db.AtendimentosHistorico.Add(new AtendimentoHistorico
                {
                    AtendimentoId = id,
                    Datahistorico = DateTime.Now,
                    Solucao = "Anexo \"" + atendimentoAnexo.NomeArquivo + "\" adicionado",
                    Usuario = GetUsuarioLogado(),
                    StatusAtendimentoId = 1
                });

                db.SaveChanges();

                Success("Anexo adicionado com sucesso.");
            }

            var result = Details(id);

            ViewBag.returnUrl = returnUrl;

            return result;
        }

        [HttpPost]
        [ActionName("DeletarAnexo")]
        public ActionResult DelAnexo(int id, string returnUrl, string Situacao)
        {
            var atendimentoAnexo = db.AtendimentosAnexo.Find(id);
            var ultimoHistorico = db.Atendimentos.Find(atendimentoAnexo.AtendimentoId).AtendimentosHistorico.LastOrDefault();

            db.AtendimentosAnexo.Remove(atendimentoAnexo);

            db.AtendimentosHistorico.Add(new AtendimentoHistorico
            {
                AtendimentoId = atendimentoAnexo.AtendimentoId,
                Datahistorico = DateTime.Now,
                Solucao = "Anexo \"" + atendimentoAnexo.NomeArquivo + "\" deletado",
                UsuarioId = GetUsuarioLogado().Id,
                StatusAtendimentoId = ultimoHistorico.StatusAtendimentoId
            });

            db.SaveChanges();

            Success("Anexo deletado com sucesso.");

            return RedirectToAction("Edit", new { id = atendimentoAnexo.AtendimentoId, returnUrl = returnUrl });
        }

        //Dashboard 
        public ActionResult Pendentedia()
        {

            var dia = DateTime.Today.AddDays(1);
            var resultado = from e in db.Atendimentos
                            where e.data_abertura >= DateTime.Today && e.data_abertura < dia
                            && e.StatusAtendimentoId == 1
                            select e;

            return View(resultado.ToList());

        }

        public ActionResult Resolvido()
        {
            var mes = DateTime.Today.AddDays(-30);
            var AtendResolvido = from p in db.Atendimentos
                                 where p.data_fechamento >= mes && p.StatusAtendimentoId == 5
                                 select p;

            return View(AtendResolvido.ToList());
        }

        public ActionResult atendimentoMes()
        {

            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;

            var AtendGeral = from p in db.Atendimentos
                             where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                             select p;
            return View(AtendGeral.ToList());
        }

        public ActionResult Pendente()
        {
            var AtendPendente = db.Atendimentos.Where(p => p.StatusAtendimentoId == 1);
            return View(AtendPendente.ToList());
        }

        public ActionResult DuvidaMes()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var duvidames = from p in db.Atendimentos
                            where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                            where p.MotivoId == 39
                            select p;

            return View(duvidames.ToList());
        }

        public ActionResult ErroMes()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var erromes = from p in db.Atendimentos
                          where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                          where p.MotivoId == 19
                          select p;

            return View(erromes.ToList());
        }


        


    }
}



