using Protocolo.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class InstituicaoController : BaseController
    {

        private const string JPG = "image/jpeg";

        private const string PNG = "image/png";

        public InstituicaoController() 
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            InstituicaoModel model;

            if (db.Intituicoes.Any())
            {
                // Retorna o único elemento do banco, pois só existe uma instituição por cliente.
                model = new InstituicaoModel(db.Intituicoes.Single());
            }
            else
            {
                model = new InstituicaoModel();
            }

            return BaseView(model);
        }

        // Nome mantido como Index para evitar problemas com a mudança de URL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InstituicaoModel model, HttpPostedFileBase Logo)
        {
            if (!this.ModelState.IsValid)
            {
                return BaseView(model);
            }

            model.UsuarioEdicaoId = GetUsuarioLogado().Id;

            if (InvalidImage(model, Logo))
            {
                return BaseView(model);
            }

            if (model.Id == null)
            {
                model.DataInclusao = DateTime.Now;

                db.Intituicoes.Add(model.NewInstituicao());
            }
            else
            {
                // Retorna o único elemento do banco, pois só existe uma instituição por cliente.
                var instituicao = db.Intituicoes.Single();

                model.DataEdicao = DateTime.Now;

                model.UpdateInstituicao(instituicao);

                db.Entry(instituicao).State = EntityState.Modified;
            }

            db.SaveChanges();

            Success("Instituição salva com sucesso.");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetMunicipios(string UFId)
        {
            var municipios = db.Municipios.Where(m => m.UFId == UFId).OrderBy(m => m.Nome).ToList();

            return Json(municipios, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public FileResult ViewLogomarca()
        {
            if (db.Intituicoes.Any())
            {
                var instituicao = db.Intituicoes.Single();

                return File(instituicao.Logomarca, instituicao.TipoLogomarca);
            }

            return null;
        }

        [AllowAnonymous]
        public string ViewInstituicao()
        {
            if (db.Intituicoes.Any())
            {
                return db.Intituicoes.Single().Nome;
            }

            return string.Empty;
        }

        private ViewResult BaseView(InstituicaoModel model)
        {
            ViewBag.UFId = new SelectList(db.UFs.OrderBy(uf => uf.Nome), "Id", "Nome", model.UFId);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(m => m.UFId == model.UFId).OrderBy(m => m.Nome), "Id", "Nome", model.MunicipioId);

            return View(model);
        }

        // Valida o arquivo e adiciona ao model caso o mesmo seja válido (até 8000 e do tipo imagem)
        private bool InvalidImage(InstituicaoModel model, HttpPostedFileBase Logo)
        {
            if (Logo == null || Logo.ContentLength == 0)
            {
                return false;
            }
            else if (Logo.ContentLength > 8000)
            {
                Danger("O arquivo não pode ultrapassar o tamanho máximo de 6 KB.");

                return true;
            }
            else if (Logo.ContentType != JPG && Logo.ContentType != PNG)
            {
                Danger("O arquivo tem que ser uma imagem do tipo .jpg ou .png");

                return true;
            }
            else
            {
                using (var reader = new BinaryReader(Logo.InputStream))
                {
                    model.Logomarca = reader.ReadBytes(Logo.ContentLength);
                    model.NomeLogomarca = Logo.FileName;
                    model.TipoLogomarca = Logo.ContentType;
                }
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
