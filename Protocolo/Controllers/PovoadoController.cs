using Protocolo.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize(Roles = "Administrador,Gestor")]
    public class PovoadoController : BaseController
    {
        public PovoadoController()
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            var povoados = db.Povoados.Include(s => s.Regiao).OrderBy(s => s.Descricao);

            return View(povoados.ToList());
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Povoado povoado = db.Povoados.Find(id);

            if (povoado == null)
            {
                return HttpNotFound();
            }

            ViewBag.RegiaoId = new SelectList(new List<Regiao> { povoado.Regiao }, "Id", "Descricao", povoado.RegiaoId);

            return View("Details", povoado);
        }

        [ActionName("Cadastrar")]
        public ActionResult Create()
        {
            ViewBag.RegiaoId = new SelectList(db.Regioes.OrderBy(s => s.Descricao), "Id", "Descricao");

            return View("Create");
        }

        [HttpPost, ActionName("Cadastrar")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RegiaoId,Descricao")] Povoado povoado)
        {
            if (!this.ModelState.IsValid)
            {
                ViewBag.RegiaoId = new SelectList(db.Regioes.OrderBy(s => s.Descricao), "Id", "Descricao", povoado.RegiaoId);

                return View("Create", povoado);
            }

            db.Povoados.Add(povoado);
            db.SaveChanges();

            Success("Loalidade cadastrada com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Editar")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Povoado povoado = db.Povoados.Find(id);

            if (povoado == null)
            {
                return HttpNotFound();
            }

            ViewBag.RegiaoId = new SelectList(db.Regioes.OrderBy(s => s.Descricao), "Id", "Descricao", povoado.RegiaoId);

            return View("Edit", povoado);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RegiaoId,Descricao")] Povoado povoado)
        {
            if (!this.ModelState.IsValid)
            {
                ViewBag.RegiaoId = new SelectList(db.Regioes.OrderBy(s => s.Descricao), "Id", "Descricao", povoado.RegiaoId);
                return View("Edit", povoado);
            }

            db.Entry(povoado).State = EntityState.Modified;
            db.SaveChanges();

            Success("Localidade alterada com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Excluir")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Povoado povoado = db.Povoados.Include(s => s.Regiao)
                .SingleOrDefault(s => s.Id == id);

            if (povoado == null)
            {
                return HttpNotFound();
            }

            ViewBag.RegiaoId = new SelectList(new List<Regiao> { povoado.Regiao }, "Id", "Descricao", povoado.RegiaoId);

            return View("Delete", povoado);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Povoado povoado = db.Povoados.Include(s => s.Regiao)
                .SingleOrDefault(s => s.Id == id);

            
              //  ViewBag.RegiaoId = new SelectList(new List<Regiao> { povoado.Regiao }, "Id", "Descricao", povoado.RegiaoId);

              //  return View("Delete", povoado);
            

            db.Povoados.Remove(povoado);
            db.SaveChanges();

            Success("Localidade excluído com sucesso.");

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
    }
}
