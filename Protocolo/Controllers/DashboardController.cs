using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Protocolo.Models.Dashboard;
using System.Web.Mvc;
using Protocolo.SQL;
using Protocolo.Services;

namespace Protocolo.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {

        //private readonly SQLService sqlService;
        public ActionResult Index()
        {
            var statement = SQLHelper.LoadSQLStatement("Dashboard.SelectDadosDashboard.sql");
            var parametros = new SqlParameter[0];

            //var model = SQLService.ExecuteSQLStatement<DashboardViewModel>(statement, parametros);

            //return View(model);
            return View();
        }


        
    }
}