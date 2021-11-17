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


    public class FuncionarioClienteController : BaseController
    {
        public FuncionarioClienteController()
             : base(new ProtocoloEntities())
        {

        }

        // GET: FuncionarioCliente
        public ActionResult Index()
        {
            var funcionarioclientes = db.FuncionarioClientes.Include(s => s.ClienteId).OrderBy(s => s.Nome);

            return View(db.FuncionarioClientes.ToList());
        }

        // GET: FuncionarioCliente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FuncionarioCliente funcionarioCliente = db.FuncionarioClientes.Find(id);
            if (funcionarioCliente == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClienteId = new SelectList(new List<Cliente> { funcionarioCliente.Cliente }, "Id", "RazaoSocial", funcionarioCliente.ClienteId);

            return View("details", funcionarioCliente);
        }

        // GET: FuncionarioCliente/Create
        public ActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(db.Clientes.OrderBy(s => s.RazaoSocial), "Id", "RazaoSocial");
            return View("Create");
        }

        // POST: FuncionarioCliente/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nome,Email,ClienteId")] FuncionarioCliente funcionarioCliente)
        {
            if (!this.ModelState.IsValid)
            {
                ViewBag.ClienteId = new SelectList(db.Clientes.OrderBy(s => s.RazaoSocial), "Id", "RazaoSocial", funcionarioCliente.ClienteId);
                return View("Create", funcionarioCliente);
            }

            db.FuncionarioClientes.Add(funcionarioCliente);
            db.SaveChanges();

            Success("Funcionario cadastrada com sucesso.");

            return RedirectToAction("Index");
        }

        // GET: FuncionarioCliente/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FuncionarioCliente funcionarioCliente = db.FuncionarioClientes.Find(id);
            if (funcionarioCliente == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClienteId = new SelectList(db.Clientes.OrderBy(s => s.RazaoSocial), "Id", "RazaoSocial", funcionarioCliente.ClienteId); ;

            return View(funcionarioCliente);
        }

        // POST: FuncionarioCliente/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nome,Email,ClienteId")] FuncionarioCliente funcionarioCliente)
        {
            if (!this.ModelState.IsValid)
            {
                ViewBag.ClienteId = new SelectList(db.Clientes.OrderBy(s => s.RazaoSocial), "Id", "RazaoSocial", "ClienteId", funcionarioCliente.ClienteId);
                return View("Edit", funcionarioCliente);
            }

            db.Entry(funcionarioCliente).State = EntityState.Modified;
            db.SaveChanges();

            Success("Funcionario alterado com sucesso.");

            return RedirectToAction("Index"); ;
        }

        // GET: FuncionarioCliente/Delete/5
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FuncionarioCliente funcionarioCliente = db.FuncionarioClientes.Include(s => s.Cliente)
                .SingleOrDefault(s => s.Id == Id);

            if (funcionarioCliente == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClienteId = new SelectList(new List<Cliente> { funcionarioCliente.Cliente }, "Id", "RazaoSocial", funcionarioCliente.ClienteId);

            return View("Delete", funcionarioCliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            FuncionarioCliente funcionarioCliente = db.FuncionarioClientes.Include(s => s.Cliente)
                .SingleOrDefault(s => s.Id == Id);

            if (FuncionarioClienteHasDependencie(funcionarioCliente))
            {
                ViewBag.ClienteId = new SelectList(new List<Cliente> { funcionarioCliente.Cliente }, "Id", "RazaoSocial", funcionarioCliente.ClienteId);

                return View("Delete", funcionarioCliente);
            }

            if (db.Atendimentos.Any(s => s.FuncionarioClienteId == funcionarioCliente.Id))
            {
                Danger("O funcionario não pode ser excluído, pois existem atendimentos vinculados.");

                return View("Delete", funcionarioCliente);
            }
            db.FuncionarioClientes.Remove(funcionarioCliente);
            db.SaveChanges();

            Success("Funcionario excluido com sucesso.");

            return RedirectToAction("Index");
        }

        private bool FuncionarioClienteHasDependencie(FuncionarioCliente funcionarioCliente)
        {
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
