using Protocolo.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize(Roles = "Administrador,Gestor")]
    public class AssuntoController : BaseController
    {
        public AssuntoController() 
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            return View(db.Assuntos.OrderBy(a => a.Descricao).ToList());
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Assunto assunto = db.Assuntos
                .Include(a => a.DocumentosAssunto.Select(d => d.Documento))
                .SingleOrDefault(a => a.Id == id);

            if (assunto == null)
            {
                return HttpNotFound();
            }

            return View("Details", assunto);
        }

        [ActionName("Cadastrar")]
        public ActionResult Create()
        {
            return View("Create", new CreateAssuntoModel(db.Documentos.OrderBy(d => d.Descricao).ToList()));
        }

        [HttpPost, ActionName("Cadastrar")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateAssuntoModel assuntoModel)
        {
            if (!this.ModelState.IsValid)
            {
                return View("Create", assuntoModel);
            }

            if (PrazoInvalido(assuntoModel))
            {
                return View("Create", assuntoModel);
            }

            var assunto = assuntoModel.AsAssunto();

            db.Assuntos.Add(assunto);
            db.SaveChanges();

            SaveDocumentosAssunto(assuntoModel.AssuntoDocumentoList, assunto);

            Success("Assunto cadastrado com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Editar")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Assunto assunto = db.Assuntos
                .Include(a => a.DocumentosAssunto.Select(da => da.Documento))
                .SingleOrDefault(a => a.Id == id);

            if (assunto == null)
            {
                return HttpNotFound();
            }

            return View("Edit", new EditAssuntoModel(assunto, db.Documentos.OrderBy(d => d.Descricao).ToList()));
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditAssuntoModel assuntoModel)
        {
            if (!this.ModelState.IsValid)
            {
                return View("Edit", assuntoModel);
            }

            if (PrazoInvalido(assuntoModel))
            {
                return View("Edit", assuntoModel);
            }

            var assunto = db.Assuntos
                .Include(a => a.DocumentosAssunto)
                .SingleOrDefault(a => a.Id == assuntoModel.Id);

            assunto.Descricao = assuntoModel.Descricao;
            assunto.Prazo = assuntoModel.Prazo;

            db.AssuntosDocumento.RemoveRange(assunto.DocumentosAssunto);

            assunto.DocumentosAssunto.Clear();

            db.Entry(assunto).State = EntityState.Modified;

            db.SaveChanges();

            SaveDocumentosAssunto(assuntoModel.AssuntoDocumentoList, assunto);

            Success("Assunto alterado com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Excluir")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Assunto assunto = db.Assuntos.Include(a => a.DocumentosAssunto)
                .SingleOrDefault(a => a.Id == id);

            if (assunto == null)
            {
                return HttpNotFound();
            }

            return View("Delete", assunto);
        }

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Assunto assunto = db.Assuntos.Include(a => a.DocumentosAssunto)
                .SingleOrDefault(a => a.Id == id);

            if (AssuntoHasDependencies(assunto))
            {
                return View("Delete", assunto);
            }

            db.Assuntos.Remove(assunto);
            db.SaveChanges();

            Success("Assunto excluído com sucesso.");

            return RedirectToAction("Index");
        }

        private bool PrazoInvalido(AssuntoModel model)
        {
            if (model.Prazo == 0)
            {
                Danger("O Prazo deve conter um valor maior do que 0");

                return true;
            }

            return false;
        }

        private bool AssuntoHasDependencies(Assunto assunto)
        {
            if (db.Protocolos.Any(p => p.AssuntoId == assunto.Id))
            {
                Danger("Não é possível excluir o Assunto pois o mesmo possui Protocolo(s) vinculado(s).");

                return true;
            }

            return false;
        }

        private void SaveDocumentosAssunto(List<AssuntoDocumentoModel> _assuntoDocumentoList, Assunto _assunto)
        {
            _assuntoDocumentoList.ForEach(ad =>
            {
                if (ad.Selecionado)
                {
                    db.AssuntosDocumento.Add(new DocumentoAssunto
                    {
                        AssuntoId = _assunto.Id,
                        DocumentoId = ad.Id,
                        Obrigatorio = ad.Obrigatorio
                    });
                }
            });

            db.SaveChanges();
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
