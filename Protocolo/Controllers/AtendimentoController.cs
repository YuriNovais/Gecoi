using Newtonsoft.Json;
using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize]
    public class AtendimentoController : BaseController
    {
        public AtendimentoController() : base(new ProtocoloEntities())
        {

        }

        [Authorize(Roles = "Administrador,Gestor")]
        [HttpPost, ActionName("Filtrar")]
        public ActionResult Filtrar(SearchGeneral FiltrosDaTela)
        {



            var q = db.Atendimentos.AsQueryable();
            if (FiltrosDaTela.ClienteId > 0)
                q = q.Where(c => c.FuncionarioCliente.ClienteId == FiltrosDaTela.ClienteId);

            if (FiltrosDaTela.SolicitanteId > 0)
                q = q.Where(c => c.FuncionarioCliente.Id == FiltrosDaTela.SolicitanteId);

            if (FiltrosDaTela.StatusId > 0)
                q = q.Where(c => c.StatusAtendimento.Id == FiltrosDaTela.StatusId);

            if (FiltrosDaTela.MotivoId > 0)
                q = q.Where(c => c.Motivo.id == FiltrosDaTela.MotivoId);

            if (FiltrosDaTela.SistemaId > 0)
                q = q.Where(c => c.Tela.Sistema.Id == FiltrosDaTela.SistemaId);

            if (FiltrosDaTela.UsuarioId > 0)
                q = q.Where(c => c.Usuario.Id == FiltrosDaTela.UsuarioId);

            if (FiltrosDaTela.DataInicio != null)
            {
                q = q.Where(c => c.data_abertura >= FiltrosDaTela.DataInicio);
            }

            if (FiltrosDaTela.DataFim != null)
            {
                FiltrosDaTela.DataFim = AdjustDataFim(FiltrosDaTela.DataFim);
                PeriodoInvalido(FiltrosDaTela);
                q = q.Where(c => c.data_abertura <= FiltrosDaTela.DataFim);

            }

            //-- se foi chamado pelo botão, carrega a view na tela com o grid
            if (Request.IsAjaxRequest())
                return PartialView("_IndexGrid", q.ToList());

            PreencherViewBag();


            SearchGeneral Filtros = new SearchGeneral();

            q = q.Where(c => c.StatusAtendimentoId == 1);
                               

            Filtros.Atendimentos = q.ToList();


            //-- se não foi chamado pelo ajax (botão do indexsearch) traz apenas os filtros

            return View("Index", Filtros);

        }


        // GET: Atendimento
        [Authorize(Roles = "Administrador,Gestor")]
        public ActionResult Index(SearchGeneral FiltrosDaTela)
        {

            var q = db.Atendimentos.AsQueryable();
            if (FiltrosDaTela.ClienteId > 0)
                q = q.Where(c => c.FuncionarioCliente.ClienteId == FiltrosDaTela.ClienteId);

            if (FiltrosDaTela.SolicitanteId > 0)
                q = q.Where(c => c.FuncionarioCliente.Id == FiltrosDaTela.SolicitanteId);

            if (FiltrosDaTela.StatusId > 0)
                q = q.Where(c => c.StatusAtendimento.Id == FiltrosDaTela.StatusId);

            if (FiltrosDaTela.MotivoId > 0)
                q = q.Where(c => c.Motivo.id == FiltrosDaTela.MotivoId);

            if (FiltrosDaTela.SistemaId > 0)
                q = q.Where(c => c.Tela.Sistema.Id == FiltrosDaTela.SistemaId);

            if (FiltrosDaTela.UsuarioId > 0)
                q = q.Where(c => c.Usuario.Id == FiltrosDaTela.UsuarioId);

            if (FiltrosDaTela.DataInicio != null)
            {
                q = q.Where(c => c.data_abertura >= FiltrosDaTela.DataInicio);//  <= c.data_abertura.CompareTo(FiltrosDaTela.DataFim) );
            }

            if (FiltrosDaTela.DataFim != null)
            {
                FiltrosDaTela.DataFim = AdjustDataFim(FiltrosDaTela.DataFim);
                PeriodoInvalido(FiltrosDaTela);
                q = q.Where(c => c.data_abertura <= FiltrosDaTela.DataFim);

            }
            //-- se foi chamado pelo botão, carrega a view na tela com o grid
            if (Request.IsAjaxRequest())
                return PartialView("_IndexGrid", q.ToList());

            PreencherViewBag();


            SearchGeneral Filtros = new SearchGeneral();

            Usuario USUARIOLOGADO = GetUsuarioLogado();

            q = q.Where(c => c.Usuario.Id == USUARIOLOGADO.Id);
            q = q.Where(c => c.StatusAtendimentoId == 1);
            q = q.OrderByDescending(c => c.data_abertura);

            Filtros.Atendimentos = q.ToList();


            //-- se não foi chamado pelo ajax (botão do indexsearch) traz apenas os filtros

            return View("Index", Filtros);

        }

                

        private bool PeriodoInvalido(SearchGenericReportModel searchModel)
        {
            if (searchModel.DataInicio != null && searchModel.DataFim != null && searchModel.DataInicio > searchModel.DataFim)
            {
                Danger("Data Inícial deve ser inferior a Data Final.");

                PreencherViewBag();

                return true;
            }

            return false;
        }

        private DateTime? AdjustDataFim(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : dateTime;
        }



        private void PreencherViewBag()
        {
            ViewBag.SistemaId = new SelectList(db.Sistemas.OrderBy(t => t.Nome), "Id", "Nome");
            ViewBag.ClienteId = new SelectList(db.Clientes.OrderBy(a => a.RazaoSocial), "Id", "RazaoSocial");
            ViewBag.SolicitanteId = new SelectList(db.FuncionarioClientes.OrderBy(s => s.Nome), "Id", "Nome");
            ViewBag.Motivoid = new SelectList(db.Motivos.OrderBy(s => s.descricao), "Id", "Descricao");
            ViewBag.Usuarioid = new SelectList(db.Usuarios.OrderBy(s => s.Logon), "Id", "Logon");
            ViewBag.StatusId = new SelectList(db.StatusAtendimentos.OrderBy(s => s.descricao), "Id", "descricao");
            ViewBag.SituacaoId = new SelectList(db.StatusTarefas.OrderBy(s => s.descricao), "Id", "descricao");
        }

        // GET: Atendimento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atendimento atendimento = db.Atendimentos.Find(id);
            if (atendimento == null)
            {
                return HttpNotFound();
            }
            return View(atendimento);
        }

        // GET: Atendimento/Create
        public ActionResult Create()
        {

            // pega o nome de outra tabela 
            var solicitante = db.FuncionarioClientes
               .Select(u => new
               {
                   Nome = u.Cliente.RazaoSocial + " - " + u.Nome,
                   identificador = u.Id
               }).ToList();


            /*  var solicitante = db.Clientes
                  .Select(u => new
                  {
                      Nome = u.RazaoSocial + " - " + u.FuncionarioCliente.Nome,
                      identificador = u.Id
                  }).ToList();*/



            var telas = db.Telas
              .Select(u => new
              {
                  Nome = u.Sistema.Nome + " - " + u.Descricao,
                  ident = u.Id
              }).ToList();

            ViewBag.FuncionarioClienteId = new SelectList(solicitante.OrderBy(s => s.Nome), "identificador", "Nome");
            ViewBag.TelaId = new SelectList(telas.OrderBy(s => s.Nome), "ident", "Nome");
            ViewBag.MotivoId = new SelectList(db.Motivos.OrderBy(s => s.descricao), "Id", "Descricao");


            return View();
        }

        // POST: Atendimento/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,funcionarioclienteid,TelaId,MotivoId,Problema,UsuarioId,statusid")] Atendimento atendimento, HttpPostedFileBase anexo)
        {
            //-- pegando o usuário logado no sistema

            atendimento.Usuario = GetUsuarioLogado();
            atendimento.data_abertura = DateTime.Now;
            atendimento.data_fechamento = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Atendimentos.Add(atendimento);
                db.SaveChanges();

                var historico = new AtendimentoHistorico
                {



                    AtendimentoId = atendimento.Id,
                    Datahistorico = DateTime.Now,
                    Solucao = "Abertura do Atendimento",
                    Usuario = GetUsuarioLogado(),
                    StatusAtendimentoId = 1
                };
                db.AtendimentosHistorico.Add(historico);

                var atendimentoAnexo = CreateAnexo(anexo);

                if (atendimentoAnexo != null)
                {
                    atendimentoAnexo.AtendimentoId = atendimento.Id;

                    db.AtendimentosAnexo.Add(atendimentoAnexo);
                }

                db.SaveChanges();
                Success("Atendimento Registrado com sucesso.");
                //-- Após Gravar, para não retornar ao Grid, Reabre a View de Edição
                var atendimentoPersisted = db.Atendimentos.Find(atendimento.Id);
                //Atendimento atendimentoNovo = db.Atendimentos.Find(novoId);
                //return View("Edit",atendimentoNovo);
                return RedirectToAction("Edit", atendimentoPersisted);
            }

            /* ViewBag.MotivoId = new SelectList(db.Motivos, "id", "descricao", atendimento.MotivoId);
             ViewBag.TelaId = new SelectList(db.Telas, "Id", "descricao", atendimento.TelaId);
             ViewBag.FuncionarioClienteId = new SelectList(db.FuncionarioClientes, "Id", "Nome", atendimento.FuncionarioClienteId);
             ViewBag.UsuarioId = new SelectList(db.Usuarios, "Id", "Logon", atendimento.UsuarioId);*/
            return View(atendimento);
        }

        // GET: Atendimento/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return Redirect("home");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atendimento atendimento = db.Atendimentos.Find(id);
            if (atendimento == null)
            {
                return HttpNotFound();
            }
            ViewBag.MotivoId = new SelectList(db.Motivos.OrderBy(s => s.descricao), "id", "descricao", atendimento.MotivoId);
            ViewBag.TelaId = new SelectList(db.Telas.OrderBy(s => s.Sistema), "Id", "descricao", atendimento.TelaId);
            ViewBag.FuncionarioClienteId = new SelectList(db.FuncionarioClientes.OrderBy(s => s.Cliente), "Id", "Nome", atendimento.FuncionarioClienteId);
            ViewBag.UsuarioId = new SelectList(db.Usuarios.OrderBy(s => s.Logon), "Id", "Logon", atendimento.UsuarioId);
            ViewBag.StatusAtendimentoId = new SelectList(db.StatusAtendimentos.OrderBy(s => s.descricao), "Id", "descricao", atendimento.StatusAtendimentoId);





            var minutoInicial = from x in db.AtendimentosHistorico
                                where x.StatusAtendimentoId == 1
                                /*where x.Datahistorico.Minute == minA*/
                                where x.AtendimentoId == id
                                select x.Datahistorico.Minute;


            var minutoFinal = from p in db.AtendimentosHistorico
                              where p.StatusAtendimentoId == 5
                              /* where p.Datahistorico.Minute == minB*/
                              where p.AtendimentoId == id
                              select p.Datahistorico.Minute;
            //var mi = minutoInicial.ToList().First();
            //var mf = minutoFinal.ToList().LastOrDefault();

          // int minutos = minutoFinal.ToList().LastOrDefault() - minutoInicial.ToList().First();
            /*

            int minInicial = db.AtendimentosHistorico.First(p => p.Datahistorico.Minute &*

            var minInicial = from t in db.AtendimentosHistorico
                         select new
                         {
                             coluna1 = t.Datahistorico,
                             coluna2 = t.StatusAtendimentoId == 1
                         };

            var minFinal = from t in db.AtendimentosHistorico
                             select new
                             {
                                 coluna1 = t.Datahistorico,
                                 coluna2 = t.StatusAtendimentoId == 5
                             };

            int minutos = minInicial - minFinal;
            */
        //    ViewBag.minutos = minutos;

            return View("Edit", atendimento);
        }

        // POST: Atendimento/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TelaId,MotivoId,Problema,HistoricoObservacao,StatusAtendimentoId")] Atendimento atendimento, int StatusAtendimentoId, HttpPostedFileBase anexo)
        {

            var usuarioLogado = GetUsuarioLogado();

            var atendimentoPersisted = db.Atendimentos.Find(atendimento.Id);

            atendimentoPersisted.Problema = atendimento.Problema;
            atendimentoPersisted.MotivoId = atendimento.MotivoId;
          //  atendimentoPersisted.data_abertura = DateTime.Now;
            atendimentoPersisted.StatusAtendimentoId = atendimento.StatusAtendimentoId;


            /*atendimentoPersisted.data_fechamento = DateTime.Now;*/



            db.Entry(atendimentoPersisted).State = EntityState.Modified;
            if (!string.IsNullOrEmpty(atendimento.HistoricoObservacao))
            {
                var ultimoHistorico = atendimento.AtendimentosHistorico.LastOrDefault();

                db.AtendimentosHistorico.Add(new AtendimentoHistorico
                {
                    AtendimentoId = atendimento.Id,
                    Datahistorico = DateTime.Now,
                    Solucao = atendimento.HistoricoObservacao,
                    Usuario = GetUsuarioLogado(),
                    StatusAtendimentoId = StatusAtendimentoId

                });
            }
            if (anexo != null)
            {
                var atendimentoAnexo = CreateAnexo(anexo);

                if (atendimentoAnexo != null)
                {
                    atendimentoAnexo.AtendimentoId = atendimento.Id;

                    db.AtendimentosAnexo.Add(atendimentoAnexo);
                }

            }

            db.SaveChanges();

            Success("Dados alterados com sucesso.");

            return RedirectToAction("Edit");
        }

        // GET: Atendimento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atendimento atendimento = db.Atendimentos.Find(id);
            if (atendimento == null)
            {
                return HttpNotFound();
            }
            return View(atendimento);
        }

        // POST: Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Atendimento atendimento = db.Atendimentos.Find(id);
            db.Atendimentos.Remove(atendimento);
            db.SaveChanges();
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

        public FileResult DownloadAnexo(int id)
        {
            var anexo = db.AtendimentosAnexo.Find(id);

            return File(anexo.Arquivo, anexo.TipoArquivo, anexo.NomeArquivo);
        }

        private AtendimentoAnexo CreateAnexo(HttpPostedFileBase anexo)
        {
            if (anexo == null || anexo.ContentLength == 0)
            {
                return null;
            }

            var atendimentoAnexo = new AtendimentoAnexo
            {
                NomeArquivo = Path.GetFileName(anexo.FileName),
                TipoArquivo = anexo.ContentType,
            };

            using (var reader = new BinaryReader(anexo.InputStream))
            {
                atendimentoAnexo.Arquivo = reader.ReadBytes(anexo.ContentLength);
            }

            return atendimentoAnexo;
        }

        [HttpPost]
        [ActionName("AdicionarAnexo")]
        public ActionResult AddAnexo(int id, HttpPostedFileBase anexo, string returnUrl)
        {
            var atendimentoAnexo = CreateAnexo(anexo);

            if (atendimentoAnexo == null)
            {
                ModelState.AddModelError("Arquivo", "Por favor selecione um arquivo");
            }
            else
            {
                atendimentoAnexo.AtendimentoId = id;

                db.AtendimentosAnexo.Add(atendimentoAnexo);

                var ultimoHistorico = db.Atendimentos.Find(id).AtendimentosHistorico.LastOrDefault();

                db.AtendimentosHistorico.Add(new AtendimentoHistorico
                {
                    AtendimentoId = id,
                    Datahistorico = DateTime.Now,
                    Solucao = "Anexo \"" + atendimentoAnexo.NomeArquivo + "\" adicionado",
                    Usuario = GetUsuarioLogado(),
                    StatusAtendimentoId = 1
                });

                db.SaveChanges();

                Success("Anexo adicionado com sucesso.");
            }

            var result = Details(id);

            ViewBag.returnUrl = returnUrl;

            return result;
        }

        [HttpPost]
        [ActionName("DeletarAnexo")]
        public ActionResult DelAnexo(int id, string returnUrl, string Situacao)
        {
            var atendimentoAnexo = db.AtendimentosAnexo.Find(id);
            var ultimoHistorico = db.Atendimentos.Find(atendimentoAnexo.AtendimentoId).AtendimentosHistorico.LastOrDefault();

            db.AtendimentosAnexo.Remove(atendimentoAnexo);

            db.AtendimentosHistorico.Add(new AtendimentoHistorico
            {
                AtendimentoId = atendimentoAnexo.AtendimentoId,
                Datahistorico = DateTime.Now,
                Solucao = "Anexo \"" + atendimentoAnexo.NomeArquivo + "\" deletado",
                UsuarioId = GetUsuarioLogado().Id,
                StatusAtendimentoId = ultimoHistorico.StatusAtendimentoId
            });

            db.SaveChanges();

            Success("Anexo deletado com sucesso.");

            return RedirectToAction("Edit", new { id = atendimentoAnexo.AtendimentoId, returnUrl = returnUrl });
        }

        //Dashboard 
        public ActionResult Pendentedia()
        {

            var dia = DateTime.Today.AddDays(1);
            var resultado = from e in db.Atendimentos
                            where e.data_abertura >= DateTime.Today && e.data_abertura < dia
                            && e.StatusAtendimentoId == 1
                            select e;

            return View(resultado.ToList());

        }

        public ActionResult Resolvido()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var AtendResolvido = from p in db.Atendimentos
                            where p.data_fechamento.Month == mes && p.data_fechamento.Year == ano
                            where p.StatusAtendimentoId == 5
                            select p;

            return View(AtendResolvido.ToList());
        }

        public ActionResult atendimentoMes()
        {

            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;

            var AtendGeral = from p in db.Atendimentos
                             where p.data_fechamento.Month == mes && p.data_abertura.Year == ano
                             select p;
            return View(AtendGeral.ToList());
        }

        public ActionResult Pendente()
        {
            var AtendPendente = db.Atendimentos.Where(p => p.StatusAtendimentoId == 1);
            return View(AtendPendente.ToList());
        }

        public ActionResult DuvidaMes()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var duvidames = from p in db.Atendimentos
                            where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                            where p.MotivoId == 39
                            select p;

            return View(duvidames.ToList());
        }

        public ActionResult ErroMes()
        {
            int mes = DateTime.Now.Month;
            int ano = DateTime.Now.Year;
            var erromes = from p in db.Atendimentos
                          where p.data_abertura.Month == mes && p.data_abertura.Year == ano
                          where p.MotivoId == 19
                          select p;

            return View(erromes.ToList());
        }

       


        public ActionResult Time()
        {
            int minA = DateTime.Now.Minute;
            int minB = DateTime.Now.Minute;

            var data_inicio = from x in db.AtendimentosHistorico
                              where x.StatusAtendimentoId == 1
                              where x.Datahistorico.Minute == minA
                              select x;

            var data_final = from p in db.AtendimentosHistorico
                             where p.StatusAtendimentoId == 5
                             where p.Datahistorico.Minute == minB
                             select p;

            var minutos = minA + minB;


            return View(minutos);


            /*


            var tempo = "select Id, DateDiff(MINUTE, (select top 1 datahistorico from Atendimento_Historico where Atendimento.id = Atendimento_historico.Atendimentoid" +
                          " and Atendimento_historico.StatusAtendimentoid = 1)," +
                                                   " (select top 1 datahistorico from Atendimento_Historico where Atendimento.id = Atendimento_historico.Atendimentoid" +
                          " and Atendimento_historico.StatusAtendimentoid = 5 order by id desc )) AS Minutos  from atendimento";

                SqlDataReader reader = null;
                SqlConnection conn = null;

            try
            {

                conn = new
                SqlConnection(db.Database.Connection.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand(tempo, conn);

                reader = cmd.ExecuteReader();
                reader.Read();
                ViewBag.Data_inicio = reader["Data_inicio"].ToString();
                ViewBag.Data_final = reader["Data_final"].ToString();
            }
            finally
            {
                // Fecha o datareader
                if (reader != null)
                {
                    reader.Close();
                }

                // Fecha a conexão
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return View();  */
        }


    }
}



