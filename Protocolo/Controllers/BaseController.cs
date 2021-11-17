using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    public class BaseController : Controller
    {
        protected ProtocoloEntities db { get; private set; }

        public BaseController(ProtocoloEntities _db)
        {
            db = _db;
        }

        protected Usuario GetUsuarioLogado()
        {
            return db.Usuarios
                .SingleOrDefault(u => u.Logon == this.HttpContext.User.Identity.Name);
        }

        protected Setor GetSetorUsuario()
        {/*
            if (Session["SetorUsuario"] == null)
            {
                Session["SetorUsuario"] = GetUsuarioLogado().SetorPadrao;
            }

            return Session["SetorUsuario"] as Setor; */
            return null;
        }

        protected bool IsAdministrador()
        {
            return IsUserInRole(Perfil.Administrador);
        }

        protected bool IsGestor()
        {
            return IsUserInRole(Perfil.Gestor);
        }

        protected bool IsComum()
        {
            return IsUserInRole(Perfil.Comum);
        }

        protected bool IsConsulta()
        {
            return IsUserInRole(Perfil.Consulta);
        }

        private bool IsUserInRole(string role)
        {
            var usuarioLogado = GetUsuarioLogado();

            List<string> roles = usuarioLogado.Perfis
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            return roles.Contains(role);
        }

        protected void Success(string message)
        {
            AddAlert(AlertStyles.Success, IconStyles.Success, message);
        }

        protected void Information(string message)
        {
            AddAlert(AlertStyles.Information, IconStyles.Info, message);
        }

        protected void Warning(string message)
        {
            AddAlert(AlertStyles.Warning, IconStyles.Warning, message);
        }

        protected void Danger(string message)
        {
            AddAlert(AlertStyles.Danger, IconStyles.Danger, message);
        }

        private void AddAlert(string alertStyle, string icon, string message)
        {
            var alert = new Alert
            {
                AlertStyle = alertStyle,
                Icon = icon,
                Message = message
            };

            TempData[Alert.TempDataKey] = alert;
        }

        protected string RenderRazorViewToString(string viewName, object model)
        {
            if (model != null)
            {
                ViewData.Model = model;
            }

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}