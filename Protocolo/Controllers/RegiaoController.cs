using Protocolo.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize(Roles = "Administrador,Gestor")]
    public class RegiaoController : BaseController
    {

        public RegiaoController()
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            return View(db.Regioes.OrderBy(s => s.Descricao).ToList());
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Regiao regiao = db.Regioes.Find(id);

            if (regiao == null)
            {
                return HttpNotFound();
            }

            return View("Details", regiao);
        }

        [ActionName("Cadastrar")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost, ActionName("Cadastrar")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Descricao")] Regiao regiao)
        {
            if (!this.ModelState.IsValid)
            {
                return View("Create", regiao);
            }

            db.Regioes.Add(regiao);
            db.SaveChanges();

            Success("Região cadastrada com sucesso.");

            return RedirectToAction("Index");

        }

        [ActionName("Editar")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Regiao regiao = db.Regioes.Find(id);

            if (regiao == null)
            {
                return HttpNotFound();
            }

            return View("Edit", regiao);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Descricao")] Regiao regiao)
        {
            if (!this.ModelState.IsValid)
            {
                return View("Edit", regiao);
            }

            db.Entry(regiao).State = EntityState.Modified;
            db.SaveChanges();

            Success("regiçao alterada com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Excluir")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Regiao regiao = db.Regioes.Find(id);

            if (regiao == null)
            {
                return HttpNotFound();
            }

            return View("Delete", regiao);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Regiao regiao = db.Regioes.Find(id);

            db.Regioes.Remove(regiao);
            db.SaveChanges();

            Success("Região excluída com sucesso.");

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
