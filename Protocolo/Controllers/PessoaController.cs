using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize(Roles = "Administrador,Gestor,Comum")]
    public class PessoaController : BaseController
    {
        public PessoaController() 
            : base(new ProtocoloEntities())
        {

        }

        public ActionResult Index()
        {
            return View(DbPessoaFetch().OrderBy(p => p.NomeRazao).ToList());
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Pessoa pessoa = DbPessoaFetch()
                .Include(p => p.ClassificacoesPessoa.Select(c => c.Classificacao))
                .SingleOrDefault(p => p.Id == id);

            if (pessoa == null)
            {
                return HttpNotFound();
            }

            if (pessoa.TipoPessoa.Equals("F"))
            {
                pessoa.PessoaFisica = db.PessoasFisicas.Find(id);
            }
            else
            {
                pessoa.PessoaJuridica = db.PessoasJuridicas.Find(id);
            }

            PopulateCombos(pessoa);

            pessoa.ClassificacoesPessoa.Sort((x, y) => string.Compare(x.Classificacao.Descricao, y.Classificacao.Descricao));

            return View("Details", pessoa);
        }

        [ActionName("Cadastrar")]
        public ActionResult Create()
        {
            PopulateCombos(null);

            var pessoa = new Pessoa
            {
                Ativo = true
            };

            pessoa.ClassificaoSelectList = new SelectList(db.Classificacoes.OrderBy(c => c.Descricao), "Id", "Descricao");

            return View("Create", pessoa);
        }

        [HttpPost, ActionName("Cadastrar")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Id")] Pessoa pessoa)
        {
            if (!this.ModelState.IsValid)
            {
                PopulateCombos(pessoa);

                return View("Create", pessoa);
            }

            if (PessoaJaCadastrada(pessoa))
            {
                PopulateCombos(pessoa);

                return View("Create", pessoa);
            }

            pessoa.UsuarioCadastroId = GetUsuarioLogado().Id;
            pessoa.DataCadastro = DateTime.Now;

            pessoa.ClassificacoesPessoa.Clear();

            db.Pessoas.Add(pessoa);
            db.SaveChanges();

            if (pessoa.TipoPessoa.Equals("F"))
            {
                pessoa.PessoaFisica.Id = pessoa.Id;
                db.PessoasFisicas.Add(pessoa.PessoaFisica);
            }
            else
            {
                pessoa.PessoaJuridica.Id = pessoa.Id;
                db.PessoasJuridicas.Add(pessoa.PessoaJuridica);
            }

            SaveClassificacoesPessoa(pessoa);

            Success("Pessoa cadastrada com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Editar")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Pessoa pessoa = DbPessoaFetch()
                .Include(p => p.ClassificacoesPessoa.Select(c => c.Classificacao))
                .SingleOrDefault(p => p.Id == id);

            if (pessoa == null)
            {
                return HttpNotFound();
            }

            if (pessoa.TipoPessoa.Equals("F"))
            {
                pessoa.PessoaFisica = db.PessoasFisicas.Find(pessoa.Id);
            }
            else
            {
                pessoa.PessoaJuridica = db.PessoasJuridicas.Find(pessoa.Id);
            }

            PopulateCombos(pessoa);

            List<Classificacao> classificacoes = db.Classificacoes.OrderBy(c => c.Descricao).ToList();

            classificacoes.ForEach(c => 
            {
                if (pessoa.ClassificacoesPessoa.Exists(cp => cp.ClassificacaoId == c.Id))
                {
                    pessoa.Classificacoes.Add(c.Id.ToString());
                }
            });

            return View("Edit", pessoa);
        }

        [HttpPost, ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pessoa pessoa)
        {
            if (!this.ModelState.IsValid)
            {
                PopulateCombos(pessoa);

                return View("Edit", pessoa);
            }

            pessoa.UsuarioEdicaoId = GetUsuarioLogado().Id;
            pessoa.DataEdicao = DateTime.Now;

            List<ClassificacaoPessoa> classificacoesPessoa = db.ClassificacoesPessoa
                .Where(c => c.PessoaId == pessoa.Id)
                .ToList();

            classificacoesPessoa.ForEach(c =>
            {
                db.Entry(c).State = EntityState.Deleted;
            });

            pessoa.ClassificacoesPessoa.Clear();

            db.Entry(pessoa).State = EntityState.Modified;

            if (pessoa.TipoPessoa.Equals("F"))
            {
                db.Entry(pessoa.PessoaFisica).State = EntityState.Modified;
            }
            else
            {
                db.Entry(pessoa.PessoaJuridica).State = EntityState.Modified;
            }

            db.SaveChanges();

            SaveClassificacoesPessoa(pessoa);

            Success("Pessoa alterada com sucesso.");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetMunicipios(string UFId)
        {
            var municipios = db.Municipios.Where(m => m.UFId == UFId).OrderBy(m => m.Nome).ToList();

            return Json(municipios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PessoaFisicaView()
        {
            return Json(RenderRazorViewToString("_PessoaFisicaView", null), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PessoaJuridicaView()
        {
            return Json(RenderRazorViewToString("_PessoaJuridicaView", null), JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Pessoa> DbPessoaFetch()
        {
            return db.Pessoas.Include(p => p.Banco).Include(p => p.UF).Include(p => p.Municipio);
        }

        private void PopulateCombos(Pessoa _pessoa)
        {
            short? _bancoSelecionado = null;
            string _UFSelecionado = null;
            int? _municipioSelecionado = null;

            if (_pessoa != null)
            {
                _bancoSelecionado = _pessoa.BancoId;
                _UFSelecionado = _pessoa.UFId;
                _municipioSelecionado = _pessoa.MunicipioId;
                _pessoa.ClassificaoSelectList = new SelectList(db.Classificacoes.OrderBy(c => c.Descricao), "Id", "Descricao");
            }

            ViewBag.BancoId = new SelectList(db.Bancos.OrderBy(b => b.Nome), "Id", "Nome", _bancoSelecionado);
            ViewBag.UFId = new SelectList(db.UFs.OrderBy(uf => uf.Nome), "Id", "Nome", _UFSelecionado);
            ViewBag.MunicipioId = new SelectList(db.Municipios.Where(m => m.UFId == _UFSelecionado).OrderBy(m => m.Nome), "Id", "Nome", _municipioSelecionado);
        }

        private bool PessoaJaCadastrada(Pessoa _pessoa)
        {
            if (_pessoa.TipoPessoa.Equals("F") && db.Pessoas.Any(p => p.CPF.Equals(_pessoa.CPF) && p.Id != _pessoa.Id))
            {
                Danger("Já existe uma Pessoa com o CPF informado.");

                return true;
            }

            if (_pessoa.TipoPessoa.Equals("J") && db.Pessoas.Any(p => p.CNPJ.Equals(_pessoa.CNPJ) && p.Id != _pessoa.Id))
            {
                Danger("Já existe uma Pessoa com o CNPJ informado.");

                return true;
            }

            return false;
        }

        private void GetClassificacoes(Pessoa _pessoa, out List<int> classificacoesSelecionadas)
        {
            classificacoesSelecionadas = new List<int>();

            foreach (var c in _pessoa.ClassificacoesPessoa)
            {
                classificacoesSelecionadas.Add(c.ClassificacaoId);
            }
        }

        private void SaveClassificacoesPessoa(Pessoa _pessoa)
        {
            _pessoa.Classificacoes.ForEach(c =>
            {
                db.ClassificacoesPessoa.Add(new ClassificacaoPessoa
                {
                    PessoaId = _pessoa.Id,
                    ClassificacaoId = Int32.Parse(c)
                });
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
