using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Protocolo.Helpers;
using Protocolo.Models;

namespace Protocolo.Controllers
{
    [Authorize]
    public class ClienteController : BaseController
    {
        public ClienteController()
            : base(new ProtocoloEntities())
        {

        }


        private void AddAlert(string danger1, string danger2, string message)
        {
            throw new NotImplementedException();
        }



        // GET: Cliente
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index()
        {
            return View(db.Clientes.ToList());
        }

        // GET: Cliente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Cliente/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cliente/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RazaoSocial,Fantasia,TipoPessoa,Cnpj,Endereco,Complemento,Bairro,Cep,Cidade,Uf,Telefone,Email")] Cliente cliente)
        {

            if (Existecnpj(cliente))
            {
                return RedirectToAction("Create");
            }

            if (ModelState.IsValid)
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
                Success("Cliente cadastrada com sucesso.");

                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RazaoSocial,Fantasia,TipoPessoa,Cnpj,Endereco,Complemento,Bairro,Cep,Cidade,Uf,Telefone,Email")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                Success("Sistema alterada com sucesso.");
                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Cliente cliente = db.Clientes.Find(id);

            if (db.FuncionarioClientes.Any(s => s.ClienteId == cliente.Id))
            {
                Danger("O Cliente ( Prefeitura) não pode ser excluído, pois existem funcionarios vinculados.");

                return View("Delete", cliente);
            }

            db.Clientes.Remove(cliente);
            db.SaveChanges();

            Success("Cliente ( Prefeitura) excluída com sucesso.");

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

        private bool Existecnpj(Cliente cliente)
        {
            if (db.Clientes.Any(u => u.Cnpj == cliente.Cnpj && u.Id != cliente.Id))
            {
                Danger("Já existe um cliente cadastrado com esse CNPJ.");

                return true;
            }
            return false;
        }
    }
}

