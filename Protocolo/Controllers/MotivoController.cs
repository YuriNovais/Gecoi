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
    public class MotivoController : BaseController
    {
        public MotivoController()
             : base(new ProtocoloEntities())
        {

        }

        // GET: Motivos
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index(string Pesquisa ="")
        {
            var q = db.Motivos.AsQueryable();

            if (!string.IsNullOrEmpty(Pesquisa))
                q = q.Where(c => c.descricao.Contains(Pesquisa));

            q = q.OrderBy(c => c.descricao);

            if (Request.IsAjaxRequest())
                return PartialView("_Motivo", q.ToList());

            return View(q.ToList());
        }

        // GET: Motivos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Motivo motivo = db.Motivos.Find(id);
            if (motivo == null)
            {
                return HttpNotFound();
            }
            return View(motivo);
        }

        // GET: Motivos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Motivos/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descricao")] Motivo motivo)
        {
            if (Duplicidade(motivo))
            {
                return RedirectToAction("Create");
            }

            if (ModelState.IsValid)
            {
                db.Motivos.Add(motivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(motivo);
        }

        // GET: Motivos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Motivo motivo = db.Motivos.Find(id);
            if (motivo == null)
            {
                return HttpNotFound();
            }
            return View(motivo);
        }

        // POST: Motivos/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descricao")] Motivo motivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(motivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(motivo);
        }

        // GET: Motivos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Motivo motivo = db.Motivos.Find(id);
            if (motivo == null)
            {
                return HttpNotFound();
            }
            return View(motivo);
        }

        // POST: Motivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Motivo motivo = db.Motivos.Find(id);

            if (db.Atendimentos.Any(s => s.MotivoId == motivo.id))
            {
                Danger("O Motivo não pode ser excluído, pois existem atendimentos vinculados.");

                return View("Delete", motivo);
            }
            db.Motivos.Remove(motivo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool Duplicidade(Motivo _motivo)
        {
            if (db.Motivos.Any(u => u.descricao == _motivo.descricao && u.id != _motivo.id))
            {
                Danger("Já existe um motivo cadastrado com essa descrição.");

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
