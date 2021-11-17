using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace Protocolo.Controllers
{
    [Authorize]
    public class UsuarioController : BaseController
    {
        public UsuarioController() : base(new ProtocoloEntities())
        {

        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Home", "Protocolo");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            PasswordHasher passwordHasher = new PasswordHasher();

            string HashedPassword = passwordHasher.HashPassword(passwordHasher.CreateSalt(model.Logon), model.Senha);

            var usuario = db.Usuarios
                .SingleOrDefault(u => u.Logon == model.Logon && u.Senha == HashedPassword && u.Ativo);

            if (usuario == null)
            {
                ModelState.AddModelError("", "O Logon ou Senha informados estão incorretos.");

                return View(model);
            }

            var authTicket = new FormsAuthenticationTicket(
                1,
                usuario.Logon,
                DateTime.Now,
                DateTime.Now.AddMinutes(40),
                false,
                usuario.Perfis
            );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);


            Session["Instituicao"] = db.Intituicoes.SingleOrDefault();

            // Se o usuário não mudou a senha padrão
            if (usuario.Logon == model.Senha)
            {
                Warning("Por favor, atualize sua senha.");

                return RedirectToAction("AlterarSenha", "Usuario", new { logon = usuario.Logon });
            }
            else if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Home", "Protocolo");
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Home", "Protocolo");
        }

        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index()
        {
            return View(db.Usuarios.OrderBy(u => u.Nome).ToList());
        }

        [ActionName("Visualizar")]
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuarios
                .SingleOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            //ViewBag.SecretariaId = new SelectList(new List<Secretaria> { usuario.Secretaria }, "Id", "Nome", usuario.SecretariaId);

            return View("Details", usuario);
        }

        [ActionName("Cadastrar")]
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Create()
        {
            ViewBag.SecretariaId = new SelectList(db.Secretarias.OrderBy(s => s.Nome), "Id", "Nome");

            return View("Create", new UsuarioModel());
        }

        [HttpPost, ActionName("Cadastrar")]
        [Authorize(Roles = "Administrador,Gestor")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioModel usuarioModel)
        {

            if (!this.ModelState.IsValid)
            {
                Danger("Existem campos de valores obrigatórios não preenchidos.");
                return View("Create", usuarioModel);
            }

            if (ExisteUsuarioLogonInformado(usuarioModel))
            {
                return View("Create", usuarioModel);
            }

            if (PerfisInvalidos(usuarioModel))
            {
                return View("Create", usuarioModel);
            }

            if (Existeemail(usuarioModel))
            {
                return View("Create", usuarioModel);
            }


            //usuario.Perfis = string.Join(";", usuarioModel.PerfisList);
            //usuario.SetorPadraoId = setoresPadrao.First();

            PasswordHasher passwordHasher = new PasswordHasher();

            // A senha inicial é igual ao login do usuário
            usuarioModel.Senha = passwordHasher.HashPassword(passwordHasher.CreateSalt(usuarioModel.Logon), usuarioModel.Logon);

            var usuario = new Usuario();

            usuario.Logon = usuarioModel.Logon;
            usuario.Nome = usuarioModel.Nome;
            usuario.Perfis = usuarioModel.Perfil;
            usuario.Telefone = usuarioModel.Telefone;
            usuario.Senha = usuarioModel.Senha;

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            //SaveSetoresUsuario(setoresSelecionados, usuario);

            Success("Usuário cadastrado com sucesso.");

            return RedirectToAction("Index");
        }

        [ActionName("Editar")]
        [Authorize(Roles = "Administrador,Gestor,Comum,Consulta")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (IsAlteracaoInvalida(id))
            {
                return RedirectToAction("Home", "Protocolo");
            }

            Usuario usuario = db.Usuarios
                .SingleOrDefault(u => u.Id == id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            //ViewBag.SecretariaId = new SelectList(db.Secretarias.OrderBy(s => s.Nome), "Id", "Nome", usuario.SecretariaId);

            var setores = db.Setores
                .ToList();

            // armazena a página que chamou o Editar
            ViewBag.returnUrl = Request.UrlReferrer.AbsolutePath;

            return View("Edit", new EditUsuarioModel(usuario, setores));
        }

        [ActionName("EditarPerfil")]
        [Authorize(Roles = "Administrador,Gestor,Comum,Consulta")]
        public ActionResult Edit(string logon)
        {
            Usuario usuario = db.Usuarios
                .SingleOrDefault(u => u.Logon == logon);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            if (IsAlteracaoInvalida(usuario.Id))
            {
                return RedirectToAction("Home", "Protocolo");
            }

            //ViewBag.SecretariaId = new SelectList(db.Secretarias.OrderBy(s => s.Nome), "Id", "Nome", usuario.SecretariaId);

            var setores = db.Setores
                .ToList();

            // armazena a página que chamou o Editar
            ViewBag.returnUrl = Request.UrlReferrer.AbsolutePath;

            return View("Edit", new EditUsuarioModel(usuario, setores));
        }

        [HttpPost, ActionName("Editar")]
        [Authorize(Roles = "Administrador,Gestor,Comum,Consulta")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUsuarioModel usuarioModel, string returnUrl)
        {
            if (IsAlteracaoInvalida(usuarioModel.Id))
            {
                return RedirectToAction("Home", "Protocolo");
            }

            if (!this.ModelState.IsValid)
            {
                return View("Edit", usuarioModel);
            }

            if (ExisteUsuarioLogonInformado(usuarioModel))
            {
                return View("Edit", usuarioModel);
            }

            if (PerfisInvalidos(usuarioModel))
            {
                return View("Edit", usuarioModel);
            }


            var usuario = db.Usuarios
                .SingleOrDefault(u => u.Id == usuarioModel.Id);

            usuario.Logon = usuarioModel.Logon;
            usuario.Nome = usuarioModel.Nome;
            usuario.Email = usuarioModel.Email;
            usuario.Telefone = usuarioModel.Telefone;
            usuario.Celular = usuarioModel.Celular;
            usuario.Ativo = usuarioModel.Ativo;
            //usuario.SecretariaId = usuarioModel.SecretariaId;
            usuario.Perfis = string.Join(";", usuarioModel.Perfil);
            //usuario.SetorPadraoId = setoresPadrao.First();

            //db.UsuariosSetor.RemoveRange(usuario.Setores);

            //usuario.Setores.Clear();

            db.Entry(usuario).State = EntityState.Modified;

            db.SaveChanges();

            //SaveSetoresUsuario(setoresSelecionados, usuario);

            /*
             * Altera o setor do usuário bem como a lista de setores para o usuário selecionar
             * caso o usuário alterado seja o usuário logado
             */
            if (usuario.Id == GetUsuarioLogado().Id)
            {
                /*Session["SetorUsuario"] = usuario.SetorPadrao;
                Session["SetoresSelecionar"] = usuario
                    //.Select(us => us.Setor)
                    //.Where(s => s.Id != usuario.SetorPadrao.Id)
                    .OrderBy(s => s.Nome)
                    .ToList(); */
            }

            Success("Usuário alterado com sucesso.");

            return Redirect(returnUrl);
        }

        [ActionName("AlterarSenha")]
        [Authorize(Roles = "Administrador,Gestor,Comum,Consulta")]
        public ActionResult UpdatePassword(string logon)
        {
            Usuario usuario = db.Usuarios.SingleOrDefault(u => u.Logon == logon);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            if (IsAlteracaoInvalida(usuario.Id))
            {
                return RedirectToAction("Home", "Protocolo");
            }

            // armazena a página que chamou o Alterar Senha
            ViewBag.returnUrl = Request.UrlReferrer.AbsolutePath;

            return View("UpdatePassword", new UpdatePasswordModel(usuario.Id, usuario.Logon, usuario.Nome));
        }

        [HttpPost, ActionName("AlterarSenha")]
        [Authorize(Roles = "Administrador,Gestor,Comum,Consulta")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(UpdatePasswordModel updatePasswordModel, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return View("UpdatePassword", updatePasswordModel);
            }

            Usuario usuario = db.Usuarios.Find(updatePasswordModel.Id);

            PasswordHasher passwordHasher = new PasswordHasher();

            usuario.Senha = passwordHasher.HashPassword(passwordHasher.CreateSalt(updatePasswordModel.Logon), updatePasswordModel.Senha);

            db.Entry(usuario).State = EntityState.Modified;

            db.SaveChanges();

            Success("Senha alterada com sucesso.");

            return Redirect(returnUrl);
        }

        [HttpPost, ActionName("ResetarSenha")]
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult ResetPassword(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuarios.Find(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            if (IsAlteracaoInvalida(usuario.Id))
            {
                return RedirectToAction("Home", "Protocolo");
            }

            PasswordHasher passwordHasher = new PasswordHasher();

            // A senha padrão é igual ao login do usuário
            usuario.Senha = passwordHasher.HashPassword(passwordHasher.CreateSalt(usuario.Logon), usuario.Logon);

            db.Entry(usuario).State = EntityState.Modified;

            db.SaveChanges();

            Success("Senha resetada com sucesso.");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetSetores(UsuarioModel usuarioModel)
        {
            /*var setores = db.Setores
                .Where(s => s.SecretariaId == usuarioModel.SecretariaId)
                .OrderBy(s => s.Nome)
                .ToList();

            var setorUsuarioList = new List<SetorUsuarioModel>();

            setores.ForEach(s =>
            {
                setorUsuarioList.Add(SetorUsuarioModel.FromSetor(s));
            });

            usuarioModel.SetorUsuarioList = setorUsuarioList;

            return Json(RenderRazorViewToString("_SetoresView", usuarioModel)); */
            return null;
        }

        [HttpGet, ActionName("SelecionarSetor")]
        public ActionResult SelectSetor(int? id, string returnUrl)
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

            var setorAtual = Session["SetorUsuario"] as Setor;
            var setoresSelecionar = Session["SetoresSelecionar"] as List<Setor>;

            setoresSelecionar.Add(setorAtual);

            setoresSelecionar = setoresSelecionar.Where(s => s.Id != setor.Id).OrderBy(s => s.Nome).ToList();

            Session["SetorUsuario"] = setor;
            Session["SetoresSelecionar"] = setoresSelecionar;

            return Redirect(returnUrl);
        }

        private bool IsAlteracaoInvalida(int? usuarioId)
        {
            return (!IsAdministrador() && !IsGestor() && GetUsuarioLogado().Id != usuarioId);
        }

        private bool ExisteUsuarioLogonInformado(UsuarioModel _usuarioModel)
        {
            if (db.Usuarios.Any(u => u.Logon == _usuarioModel.Logon && u.Id != _usuarioModel.Id))
            {
                Danger("Já existe um Usuário com o Logon informado.");

                return true;
            }

            return false;
        }

        private bool PerfisInvalidos(UsuarioModel usuarioModel)
        {
            return !usuarioModel.Perfis.Any();
        }

        private bool SetoresInvalidos(List<int> setoresSelecionados, List<int> setoresPadrao)
        {
            if (setoresSelecionados.Count() == 0)
            {
                Danger("Selecione pelo menos um Setor para ser vinculado ao usuário.");

                return true;
            }

            if (setoresPadrao.Count() == 0)
            {
                Danger("Selecione um Setor para ser o setor padrão do usuário.");

                return true;
            }

            if (setoresPadrao.Count() > 1)
            {
                Danger("Apenas um Setor pode ser selecionado como padrão do usuário.");

                return true;
            }

            if (!setoresSelecionados.Contains(setoresPadrao.First()))
            {
                Danger("O Setor padrão deve ser selecionado também como setor a ser vinculado ao usuário.");

                return true;
            }

            return false;
        }

        private void GetSetores(UsuarioModel usuarioModel, out List<int> setoresSelecionados, out List<int> setoresPadrao)
        {
            setoresSelecionados = new List<int>();
            setoresPadrao = new List<int>();
            /*
            foreach(var setorUsuario in usuarioModel.SetorUsuarioList)
            {
                if (setorUsuario.Selecionado)
                {
                    setoresSelecionados.Add(setorUsuario.Id);
                }
                if (setorUsuario.Padrao)
                {
                    setoresPadrao.Add(setorUsuario.Id);
                }
            } */
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Existeemail(UsuarioModel _UsuarioModel)
        {
            if (db.Usuarios.Any(u => u.Email == _UsuarioModel.Email && u.Id != _UsuarioModel.Id))
            {
                Danger("Já existe Usuarios cadastrado com esse Email.");

                return true;
            }
            return false;
        }
    }
}
