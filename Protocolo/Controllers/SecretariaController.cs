using Protocolo.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize(Roles = "Administrador,Gestor")]
    public class SecretariaController : BaseController
    {
        public SecretariaController() 
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            return View(db.Secretarias.OrderBy(s => s.Nome).ToList());
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Secretaria secretaria = db.Secretarias.Find(id);

            if (secretaria == null)
            {
                return HttpNotFound();
            }

            return View("Details", secretaria);
        }

        [ActionName("Cadastrar")]
        public ActionResult Create()
        {
            return View("Create");
        }

        [HttpPost, ActionName("Cadastrar")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,NomeAbreviado,Sigla")] Secretaria secretaria)
        {
            if (!this.ModelState.IsValid)
            {
                return View("Create", secretaria);
            }

            db.Secretarias.Add(secretaria);
            db.SaveChanges();

            Success("Secretaria cadastrada com sucesso.");

            return RedirectToAction("Index");

        }

        [ActionName("Editar")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Secretaria secretaria = db.Secretarias.Find(id);

            if (secretaria == null)
            {
                return HttpNotFound();
            }

            return View("Edit", secretaria);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,NomeAbreviado,Sigla")] Secretaria secretaria)
        {
            if (!this.ModelState.IsValid)
            {
                return View("Edit", secretaria);
            }

            db.Entry(secretaria).State = EntityState.Modified;
            db.SaveChanges();

            Success("Secretaria alterada com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Excluir")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Secretaria secretaria = db.Secretarias.Find(id);

            if (secretaria == null)
            {
                return HttpNotFound();
            }

            return View("Delete", secretaria);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Secretaria secretaria = db.Secretarias.Find(id);

            if (db.Setores.Any(s => s.SecretariaId == secretaria.Id))
            {
                Danger("A Secretaria não pode ser excluída pois existem Setores vinculados.");

                return View("Delete", secretaria);
            }

            db.Secretarias.Remove(secretaria);
            db.SaveChanges();

            Success("Secretaria excluída com sucesso.");

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
