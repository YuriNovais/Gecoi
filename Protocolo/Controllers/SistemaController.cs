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
    public class SistemaController : BaseController
    {
        public SistemaController()
            : base(new ProtocoloEntities())
        {

        }

        // GET: Sistema
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index()
        {
            return View(db.Sistemas.ToList());
        }

        // GET: Sistema/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sistema sistema = db.Sistemas.Find(id);
            if (sistema == null)
            {
                return HttpNotFound();
            }
            return View(sistema);
        }

        // GET: Sistema/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sistema/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Sigla")] Sistema sistema)
        {
           

           /* if (ModelState.IsValid)
            {
                db.Sistemas.Add(sistema);
                db.SaveChanges();
                Success("Sistema cadastrada com sucesso.");
                return RedirectToAction("Index");
            }

            return View(sistema); */
           if  (Existesigla(sistema))
            {
                return RedirectToAction("Create");
            }

            if (Existenome(sistema))
            {
                return RedirectToAction("Create");
            }

            if (ModelState.IsValid)
            {
                db.Sistemas.Add(sistema);
                db.SaveChanges();
                Success("Sistema cadastrada com sucesso.");
                return RedirectToAction("Index");
            }
            return View(sistema);
        }

        // GET: Sistema/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sistema sistema = db.Sistemas.Find(id);
            if (sistema == null)
            {
                return HttpNotFound();
            }
            return View(sistema);
        }

        // POST: Sistema/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Sigla")] Sistema sistema)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sistema).State = EntityState.Modified;
                db.SaveChanges();
                Success("Sistema alterada com sucesso.");
                return RedirectToAction("Index");
            }
            return View(sistema);
        }

        // GET: Sistema/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sistema sistema = db.Sistemas.Find(id);
            if (sistema == null)
            {
                return HttpNotFound();
            }
            return View(sistema);
        }

        // POST: Sistema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            Sistema sistema = db.Sistemas.Find(id);

            if (db.Telas.Any(s => s.SistemaId  == sistema.Id))
            {
                Danger("O sistema não pode ser excluído, pois existe atendimentos vinculados.");

                return View("Delete", sistema);
            }
            db.Sistemas.Remove(sistema);
            db.SaveChanges();
            Success("Sistema excluído com sucesso.");
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

        private bool Existesigla(Sistema _sistema)
        {
            if (db.Sistemas.Any(u => u.Sigla == _sistema.Sigla && u.Id != _sistema.Id))
            {
                Danger("Já existe um sistema cadastrado com está sigla.");

                return true;
            }
            return false;
        }

        private bool Existenome(Sistema _sistema)
        {
            if (db.Sistemas.Any(u => u.Nome == _sistema.Nome && u.Id != _sistema.Id))
            {
                Danger("Já existe um sistema cadastrado com essa descrição.");

                return true;
            }
            return false;
        }
    }
}
