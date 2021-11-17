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
    [Authorize(Roles = "Administrador,Gestor")]
    public class TipoDocumentoController : BaseController
    {
        public TipoDocumentoController()
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            return View(db.TiposDocumento.OrderBy(t => t.Descricao).ToList());
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoDocumento tipoDocumento = db.TiposDocumento.Find(id);

            if (tipoDocumento == null)
            {
                return HttpNotFound();
            }

            return View("Details", tipoDocumento);
        }

        [ActionName("Cadastrar")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost, ActionName("Cadastrar")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao")] TipoDocumento tipoDocumento)
        {
            if (!this.ModelState.IsValid)
            {
                return View("Create", tipoDocumento);
            }

            db.TiposDocumento.Add(tipoDocumento);
            db.SaveChanges();

            Success("Tipo de Documento cadastrado com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Editar")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoDocumento tipoDocumento = db.TiposDocumento.Find(id);

            if (tipoDocumento == null)
            {
                return HttpNotFound();
            }

            return View("Edit", tipoDocumento);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao")] TipoDocumento tipoDocumento)
        {
            if (!this.ModelState.IsValid)
            {
                return View(tipoDocumento);
            }

            db.Entry(tipoDocumento).State = EntityState.Modified;
            db.SaveChanges();

            Success("Tipo de Documento alterado com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Excluir")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TipoDocumento tipoDocumento = db.TiposDocumento.Find(id);

            if (tipoDocumento == null)
            {
                return HttpNotFound();
            }

            return View("Delete", tipoDocumento);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoDocumento tipoDocumento = db.TiposDocumento.Find(id);

            if (db.Protocolos.Any(p => p.TipoDocumentoId == tipoDocumento.Id))
            {
                Danger("Não é possível excluir o Tipo de Documento pois o mesmo possui Protocolo(s) vinculado(s).");

                return View("Delete", tipoDocumento);
            }

            db.TiposDocumento.Remove(tipoDocumento);
            db.SaveChanges();

            Success("Tipo de Documento excluído com sucesso.");

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
