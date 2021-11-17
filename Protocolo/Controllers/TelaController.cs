using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Protocolo.Models;

namespace Protocolo.Controllers
{
    [Authorize]
    public class TelaController : BaseController
    {
        public TelaController()
            : base(new ProtocoloEntities())
        {

        }

        // GET: Tela
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index()
        {
            var telasSistema = db.Telas.Include(s => s.SistemaId).OrderBy(s => s.Descricao);

            return View(db.Telas.ToList());
        }

        // GET: Tela/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tela tela = db.Telas.Find(id);
            if (tela == null)
            {
                return HttpNotFound();
            }
            ViewBag.SistemaId = new SelectList(new List<Sistema> { tela.Sistema }, "Id", "Nome", tela.Sistema);
            return View("details",tela);
        }

        // GET: Tela/Create
        public ActionResult Create()
        {
            ViewBag.SistemaId = new SelectList(db.Sistemas.OrderBy(s => s.Nome), "Id", "Nome");
            return View("Create");
        }

        // POST: Tela/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao,SistemaId")] Tela tela)
        {
            if (Duplicidade(tela))
            {
                return RedirectToAction("Create");
            }

            if (!this.ModelState.IsValid)
            {
                ViewBag.SistemaId = new SelectList(db.Sistemas.OrderBy(s => s.Nome), "Id", "Nome", tela.SistemaId);
                return View("Create", tela);
            }
                db.Telas.Add(tela);
                db.SaveChanges();

            Success("Tela cadastrada com sucesso !");

                return RedirectToAction("Index");
        }

        // GET: Tela/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tela tela = db.Telas.Find(id);
            if (tela == null)
            {
                return HttpNotFound();
            }
            ViewBag.SistemaId = new SelectList(db.Sistemas.OrderBy(s => s.Nome), "Id", "Nome", tela.SistemaId);

            return View(tela);

        }

        // POST: Tela/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais bdeseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao")] Tela tela)
        {
            var telapersisted = db.Telas.Find(tela.Id);

            telapersisted.Descricao = tela.Descricao;

            if (!this.ModelState.IsValid)
            {
                ViewBag.SistemaId = new SelectList(db.Sistemas.OrderBy(s => s.Nome), "Id", "Descricao", tela.SistemaId);
                return View("Edit", tela);
            }

                db.Entry(telapersisted).State = EntityState.Modified;
                db.SaveChanges();

                 Success("Tela alterada com sucesso.");

            return RedirectToAction("Index");

        }

        // GET: Tela/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tela tela = db.Telas.Find(id);
            if (tela == null)
            {
                return HttpNotFound();
            }
            ViewBag.SistemaId = new SelectList(new List<Sistema> { tela.Sistema }, "Id", "Nome", tela.Sistema);

            return View("Delete",tela);
        }

        // POST: Tela/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            Tela tela = db.Telas.Include(s => s.Sistema)
                .SingleOrDefault(s => s.Id == Id);

            if (TelaHasDependencie(tela))
            {
                ViewBag.ClienteId = new SelectList(new List<Sistema> { tela.Sistema }, "Id", "Nome", tela.SistemaId);

                return View("Delete", tela);
            }

            if (db.Telas.Any(s => s.SistemaId == tela.Id))
            {
                Danger("A tela do sistema não pode ser excluída, pois existe atendimentos vinculados.");

                return View("Delete", tela);
            }
            db.Telas.Remove(tela);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool TelaHasDependencie(Tela tela)
        {
            return false;
        }

        private bool Duplicidade(Tela _tela)
        {
            if (db.Telas.Any(u => u.Descricao == _tela.Descricao && u.Id != _tela.Id))
            {
                Danger("Já existe tela cadastrada com essa descrição.");

                return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
