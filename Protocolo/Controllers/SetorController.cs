using Protocolo.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize(Roles = "Administrador,Gestor")]
    public class SetorController : BaseController
    {
        public SetorController()
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            var setores = db.Setores.Include(s => s.Secretaria).OrderBy(s => s.Nome);

            return View(setores.ToList());
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Setor setor = db.Setores.Find(id);

            if (setor == null)
            {
                return HttpNotFound();
            }

            ViewBag.SecretariaId = new SelectList(new List<Secretaria> { setor.Secretaria }, "Id", "Nome", setor.SecretariaId);

            return View("Details", setor);
        }

        [ActionName("Cadastrar")]
        public ActionResult Create()
        {
            ViewBag.SecretariaId = new SelectList(db.Secretarias.OrderBy(s => s.Nome), "Id", "Nome");

            return View("Create");
        }

        [HttpPost, ActionName("Cadastrar")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SecretariaId,Nome")] Setor setor)
        {
            if (!this.ModelState.IsValid)
            {
                ViewBag.SecretariaId = new SelectList(db.Secretarias.OrderBy(s => s.Nome), "Id", "Nome", setor.SecretariaId);

                return View("Create", setor);
            }

            db.Setores.Add(setor);
            db.SaveChanges();

            Success("Setor cadastrado com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Editar")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Setor setor = db.Setores.Find(id);

            if (setor == null)
            {
                return HttpNotFound();
            }

            ViewBag.SecretariaId = new SelectList(db.Secretarias.OrderBy(s => s.Nome), "Id", "Nome", setor.SecretariaId);

            return View("Edit", setor);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SecretariaId,Nome")] Setor setor)
        {
            if (!this.ModelState.IsValid)
            {
                ViewBag.SecretariaId = new SelectList(db.Secretarias.OrderBy(s => s.Nome), "Id", "Nome", setor.SecretariaId);
                return View("Edit", setor);
            }

            db.Entry(setor).State = EntityState.Modified;
            db.SaveChanges();

            Success("Setor alterado com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Excluir")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Setor setor = db.Setores.Include(s => s.Secretaria)
                .SingleOrDefault(s => s.Id == id);

            if (setor == null)
            {
                return HttpNotFound();
            }

            ViewBag.SecretariaId = new SelectList(new List<Secretaria> { setor.Secretaria }, "Id", "Nome", setor.SecretariaId);

            return View("Delete", setor);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Setor setor = db.Setores.Include(s => s.Secretaria)
                .SingleOrDefault(s => s.Id == id);

            if (SetorHasDependencies(setor))
            {
                ViewBag.SecretariaId = new SelectList(new List<Secretaria> { setor.Secretaria }, "Id", "Nome", setor.SecretariaId);

                return View("Delete", setor);
            }

            db.Setores.Remove(setor);
            db.SaveChanges();

            Success("Setor excluído com sucesso.");

            return RedirectToAction("Index");
        }

        private bool SetorHasDependencies(Setor setor)
        {
            /* if (db.UsuariosSetor.Any(us => us.SetorId == setor.Id))
             {
                 Danger("Não é possível excluir o Setor pois o mesmo possui Usuário(s) vinculado(s).");

                 return true;
             }

             if (db.Protocolos.Any(p => p.SetorAtualId == setor.Id))
             {
                 Danger("Não é possível excluir o Setor pois o mesmo possui Protocolo(s) vinculado(s).");

                 return true;
             }

             if (db.Lotes.Any(l => l.SetorOrigemId == setor.Id || l.SetorDestinoId == setor.Id))
             {
                 Danger("Não é possível excluir o Setor pois o mesmo possui Lote(s) vinculado(s).");

                 return true;
             }
             */
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
