using Newtonsoft.Json;
using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Net;
using System.Web;
using Protocolo.Models.Dashboard;
using System.Web.Mvc;
using Protocolo.SQL;
using Protocolo.Services;

namespace Protocolo.Controllers
{
    [Authorize]
    public class ProtocoController : BaseController
    {
        public ProtocoController() : base(new ProtocoloEntities())
        {

        }

        [AllowAnonymous]
        [ActionName("Pesquisar")]
        public ActionResult Pesquisar()
        {
            return View("Search", new SearchProtocoloModel());
        }

        [AllowAnonymous]
        [HttpPost, ActionName("Pesquisar")]
        public ActionResult Pesquisar(SearchProtocoloModel searchModel)
        {
            Models.Protocolo protocolo = db.Protocolos
                .Include(p => p.TipoDocumento)
                .Include(p => p.Assunto)
                .Include(p => p.Pessoa)
                .Include(p => p.HistoricoProtocolo)
                .SingleOrDefault(p => p.Numero == searchModel.Numero && p.Ano == searchModel.Ano && p.Chave == searchModel.Chave);

            if (protocolo == null)
            {
                searchModel.ResultModel = null;

                Danger("Nenhum protocolo encontrado com os parâmetros informados.");
            }
            else
            {
                ResultProtocoloModel resultModel = new ResultProtocoloModel
                {
                    NumeroAno = protocolo.NumeroAno,
                    TipoDocumento = protocolo.TipoDocumento.Descricao,
                    Assunto = protocolo.Assunto.Descricao,
                    Requerente = protocolo.Pessoa.NomeRazao,
                    Descricao = protocolo.Descricao,
                    SetorAtual = protocolo.SetorAtual.Nome,
                    Status = protocolo.DescricaoStatus,
                    Historico = protocolo.HistoricoProtocolo.ToList()
                };

                searchModel.ResultModel = resultModel;
            }

            return View("Search", searchModel);
        }

        // Redirect
        public ActionResult Home()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            var statement = " select Atendimento_Abertos_dia, Atendimentos_Finalizados_dia, Atendimentos_mes, " +
                " Atendimento_Abertos, Atendimento_Erros, Atendimento_Duvidas, Tarefas_Aberta_mes, Tarefas_Melhorias_Resolvidas, Tarefas_Resolvidas_mes, " +
                " Tarefas_Reaberto, Tarefas_Aberta_Mais30, Tarefas_Erros_Resolvidos  from " +
                "(select COUNT(*) as Atendimento_Abertos_dia FROM Atendimento where DAY(data_abertura) = DAY(GETDATE()) and StatusAtendimentoid in (1)) q1, " +
                "(select COUNT(*) as Atendimentos_Finalizados_dia from Atendimento where MONTH(data_fechamento) = MONTH(GETDATE()) and statusatendimentoid in (5)) q2, " +
                "(select COUNT(*) as Atendimentos_mes from Atendimento  where MONTH(data_fechamento) = MONTH(GETDATE()) ) q3, " +
                "(select COUNT(*) as Atendimento_Abertos FROM Atendimento where StatusAtendimentoid in (1)) q4, " +
                "(select COUNT(*) as Atendimento_Erros FROM Atendimento where Motivoid in (19) and MONTH(data_abertura) = MONTH(GETDATE())) q5, " +
                "(select COUNT(*) as Atendimento_Duvidas FROM Atendimento where Motivoid in (39) and MONTH(data_abertura) = MONTH(GETDATE())) q6, " +
                "(select COUNT(*) as Tarefas_Aberta_mes from Tarefa where MONTH(data_abertura) = MONTH(GETDATE())) q7, " +
                "(select COUNT(*) as Tarefas_Resolvidas_mes from Tarefa where MONTH(data_conclusao) = MONTH(GETDATE()) and StatusTarefaid in (10)) q8, " +
                "(select COUNT(*) as Tarefas_Reaberto from Tarefa where StatusTarefaid in (11)) q9, " +
                "(select COUNT(*) as Tarefas_Aberta_Mais30 from Tarefa where data_abertura <= GETDATE() - 30 and StatusTarefaid in (1)) q10, " +
                "(select COUNT(*) as Tarefas_Erros_Resolvidos from Tarefa where motivoid in (21) and month(data_conclusao) = Month(GETDATE()) and StatusTarefaid in (10)) q11, " +
                "(select COUNT(*) as Tarefas_Melhorias_Resolvidas from Tarefa where motivoid in (26) and month(data_conclusao) = Month(GETDATE()) and StatusTarefaid in (10)) q12 ";
            SqlDataReader reader = null;
            SqlConnection conn = null;
            try
            {

                conn = new
                SqlConnection(db.Database.Connection.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand(statement, conn);

                reader = cmd.ExecuteReader();
                reader.Read();
                ViewBag.AtendimentosAbertoDia = reader["Atendimento_Abertos_dia"].ToString();
                ViewBag.AtendimentosFinalizadosDia = reader["Atendimentos_Finalizados_dia"].ToString();
                ViewBag.AtendimentosMes = reader["Atendimentos_mes"].ToString();
                ViewBag.AtendimentoAbertos = reader["Atendimento_Abertos"].ToString();
                ViewBag.AtendimentoErros = reader["Atendimento_Erros"].ToString();
                ViewBag.AtendimentoDuvidas = reader["Atendimento_Duvidas"].ToString();
                ViewBag.TarefasAbertaMes = reader["Tarefas_Aberta_mes"].ToString();
                ViewBag.TarefasResolvidasMes = reader["Tarefas_Resolvidas_mes"].ToString();
                ViewBag.TarefasReaberto = reader["Tarefas_Reaberto"].ToString();
                ViewBag.TarefasAbertaMais30 = reader["Tarefas_Aberta_Mais30"].ToString();
                ViewBag.TarefasErrosResolvidos = reader["Tarefas_Erros_Resolvidos"].ToString();
                ViewBag.TarefasMelhoriasResolvidas = reader["Tarefas_Melhorias_Resolvidas"].ToString();
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

            return View();

        }

        [ActionName("AReceber")]
        public ActionResult Receiving()
        {
            var setorLogadoId = GetSetorUsuario().Id;

            var lotes = db.Lotes
                .Include(l => l.SetorOrigem)
                .Include(l => l.SetorOrigem.Secretaria)
                .Include(l => l.Protocolos)
                .Where(l => l.SetorDestinoId == setorLogadoId && l.Status == StatusLote.Enviado.Codigo)
                .ToList();

            var models = new List<ReceivingProtocoloModel>();

            // Construção dos view models
            foreach (var lote in lotes)
            {
                foreach (var protocolo in lote.Protocolos)
                {
                    models.Add(new ReceivingProtocoloModel(
                        lote.Id,
                        protocolo.Id,
                        protocolo.NumeroAno,
                        lote.SetorOrigem.Secretaria.Nome + "/" + lote.SetorOrigem.Nome,
                        lote.DataAbertura
                    ));
                }
            }

            return View("Receiving", models);
        }

        [HttpPost, ActionName("Receber")]
        public ActionResult Receive(List<ReceivingProtocoloModel> receivingList)
        {
            var usuarioLogado = GetUsuarioLogado();

            foreach (var receiving in receivingList)
            {
                if (receiving.Selecionado)
                {
                    Lote lote = db.Lotes
                        .Include(l => l.Protocolos)
                        .SingleOrDefault(l => l.Id == receiving.NumeroLote);

                    lote.UsuarioRecepcaoId = usuarioLogado.Id;
                    lote.DataRecepcao = DateTime.Now;
                    lote.Status = StatusLote.Recebido.Codigo;

                    foreach (var protocolo in lote.Protocolos)
                    {
                        protocolo.Status = StatusProtocolo.Recebido.Codigo;
                        protocolo.DataStatus = DateTime.Now;
                        protocolo.UsuarioEdicaoId = usuarioLogado.Id;
                        protocolo.SetorAtualId = lote.SetorDestinoId;

                        db.Entry(protocolo).State = EntityState.Modified;

                        db.HistoricoProtocolo.Add(new ProtocoloHistorico
                        {
                            ProtocoloId = protocolo.Id,
                            DataMovimento = DateTime.Now,
                            Historico = "Recebimento do Lote",
                            UsuarioId = usuarioLogado.Id,
                            SetorId = lote.SetorOrigemId,
                            SetorDestinoId = lote.SetorDestinoId,
                            LoteId = lote.Id,
                            Status = StatusHistorico.Recebido.Codigo
                        });

                        /*
                            Fluxo em aberto: sem data de saída
                        */
                        var fluxoAberto = db.FluxoProtocolo.FirstOrDefault(f => f.ProtocoloId == protocolo.Id && f.DataSaida == null);

                        // Só para garantir que não dê null pointer
                        if (fluxoAberto != null)
                        {
                            fluxoAberto.DataSaida = DateTime.Now;
                            fluxoAberto.UsuarioRecebimentoId = usuarioLogado.Id;

                            db.Entry(fluxoAberto).State = EntityState.Modified;
                        }

                        // Cria fluxo em aberto pois poderia não existir nenhum, ou o que existia estava fechado
                        db.FluxoProtocolo.Add(new ProtocoloFluxo
                        {
                            ProtocoloId = protocolo.Id,
                            SetorId = lote.SetorDestinoId,
                            DataEntrada = DateTime.Now
                        });
                    }

                    db.Entry(lote).State = EntityState.Modified;
                }
            }

            db.SaveChanges();

            Success("Lote(s) recebido(s) com sucesso.");

            return RedirectToAction("AReceber");
        }

        [HttpPost]
        public ActionResult OpenDeclineView(List<ReceivingProtocoloModel> receivingList)
        {
            return PartialView("_Decline", new DeclineLoteModel(
                GetSelectedLotes(receivingList)
            ));
        }

        [HttpPost, ActionName("Devolver")]
        public ActionResult Decline(DeclineLoteModel declineModel)
        {
            if (ModelState.IsValid)
            {
                var usuarioLogado = GetUsuarioLogado();
                var setorLogadoId = GetSetorUsuario().Id;

                foreach (var lote in declineModel.Lotes)
                {
                    var existingLote = db.Lotes
                        .Include(l => l.Protocolos)
                        .SingleOrDefault(l => l.Id == lote.Id);

                    existingLote.Status = StatusLote.Devolvido.Codigo;

                    db.Entry(existingLote).State = EntityState.Modified;

                    foreach (var protocolo in existingLote.Protocolos)
                    {
                        // Obtém o último status válido do Protocolo (Aberto ou Recebido)
                        var ultimoStatusValido = db.HistoricoProtocolo
                            .AsEnumerable()
                            .LastOrDefault(h => h.ProtocoloId == protocolo.Id
                                && (h.Status == StatusHistorico.Aberto.Codigo || h.Status == StatusHistorico.Recebido.Codigo))
                            .Status;

                        protocolo.Status = ultimoStatusValido;
                        protocolo.DataStatus = DateTime.Now;

                        db.Entry(protocolo).State = EntityState.Modified;

                        db.HistoricoProtocolo.Add(new ProtocoloHistorico
                        {
                            ProtocoloId = protocolo.Id,
                            DataMovimento = DateTime.Now,
                            Historico = declineModel.DescricaoJustificativa,
                            UsuarioId = usuarioLogado.Id,
                            SetorId = setorLogadoId,
                            SetorDestinoId = existingLote.SetorOrigemId,
                            LoteId = existingLote.Id,
                            Status = StatusHistorico.Devolvido.Codigo
                        });
                    }
                }

                db.SaveChanges();

                Success("Lote(s) devolvido(s) com sucesso.");

                return Json(new { Success = true });
            }

            return Json(new { Success = false, Page = RenderRazorViewToString("_Decline", declineModel) });
        }

        [ActionName("TodosSetores")]
        [Authorize(Roles = "Administrador")]
        public ActionResult AllRegistered()
        {
            var statuses = new List<string> { StatusProtocolo.Aberto.Codigo, StatusProtocolo.Recebido.Codigo, StatusProtocolo.Encaminhado.Codigo };

            var protocolos = db.Protocolos.Where(p => statuses.Contains(p.Status))
                                          .OrderByDescending(p => p.Ano)
                                          .ThenByDescending(p => p.Numero);

            return View("AllRegistered", protocolos.Any());
        }

        [HttpPost]
        public string AllRegistered(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            int initialPage = start.HasValue ? start.Value : 0;
            int pageSize = length ?? 20;

            var statuses = new List<string> { StatusProtocolo.Aberto.Codigo, StatusProtocolo.Recebido.Codigo, StatusProtocolo.Encaminhado.Codigo };

            var protocolos = db.Protocolos
                .Include(p => p.Assunto)
                .Include(p => p.UsuarioAbertura)
                .Where(p => statuses.Contains(p.Status));

            var recordsTotal = protocolos.Count();

            if (!string.IsNullOrEmpty(search))
            {
                var palavras = search.Split(' ');

                foreach (var palavra in palavras)
                {
                    if (!string.IsNullOrEmpty(palavra))
                    {
                        var searchPalavra = palavra.ToUpper();

                        protocolos = protocolos.Where(p => SqlFunctions.StringConvert((double)p.Numero).Contains(searchPalavra)
                                || SqlFunctions.StringConvert((double)p.Ano).Contains(searchPalavra)
                                || p.Chave.ToUpper().Contains(searchPalavra)
                                || p.Assunto.Descricao.ToUpper().Contains(searchPalavra)
                                || p.UsuarioAbertura.Logon.ToUpper().Contains(searchPalavra));
                    }
                }
            }

            var recordsFiltered = protocolos.Count();

            var dataList = protocolos
                .OrderBy(p => p.Ano)
                .ThenBy(p => p.Numero)
                .Skip(initialPage)
                .Take(pageSize)
                .ToList();

            var data = dataList.Select(p => new {
                ProtocoloId = p.Id,
                NumeroAno = p.NumeroAno,
                Assunto = p.Assunto.Descricao,
                DataRecebimento = p.DataAbertura.ToString("dd/MM/yyyy HH:mm"),
                ResponsavelRecebimento = p.UsuarioAbertura.Logon,
                Status = StatusProtocolo.DescricaoFromCodigo(p.Status),
                Index = dataList.IndexOf(p)
            });

            return JsonConvert.SerializeObject(new { draw, recordsTotal, recordsFiltered, data });
        }

        [ActionName("NoSetor")]
        public ActionResult Registered()
        {
            var setorLogadoId = GetSetorUsuario().Id;

            var protocolos = db.Protocolos
                .Where(p => (p.Status == StatusProtocolo.Aberto.Codigo || p.Status == StatusProtocolo.Recebido.Codigo) && p.SetorAtualId == setorLogadoId);

            return View("Registered", protocolos.Any());
        }

        [HttpPost]
        public string Registered(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            int initialPage = start.HasValue ? start.Value : 0;
            int pageSize = length ?? 20;

            var setorLogadoId = GetSetorUsuario().Id;

            var protocolos = db.Protocolos
                .Include(p => p.Assunto)
                .Include(p => p.UsuarioAbertura)
                .Where(p => (p.Status == StatusProtocolo.Aberto.Codigo || p.Status == StatusProtocolo.Recebido.Codigo) && p.SetorAtualId == setorLogadoId);

            var recordsTotal = protocolos.Count();

            if (!string.IsNullOrEmpty(search))
            {
                var palavras = search.Split(' ');

                foreach (var palavra in palavras)
                {
                    if (!string.IsNullOrEmpty(palavra))
                    {
                        var searchPalavra = palavra.ToUpper();

                        protocolos = protocolos.Where(p => SqlFunctions.StringConvert((double)p.Numero).Contains(searchPalavra)
                                || SqlFunctions.StringConvert((double)p.Ano).Contains(searchPalavra)
                                || p.Chave.ToUpper().Contains(searchPalavra)
                                || p.Assunto.Descricao.ToUpper().Contains(searchPalavra)
                                || p.UsuarioAbertura.Logon.ToUpper().Contains(searchPalavra));
                    }
                }
            }

            var recordsFiltered = protocolos.Count();

            var dataList = protocolos
                .OrderByDescending(p => p.Ano)
                .ThenByDescending(p => p.Numero)
                .Skip(initialPage)
                .Take(pageSize)
                .ToList();

            var data = dataList.Select(p => new {
                ProtocoloId = p.Id,
                NumeroAno = p.NumeroAno,
                Assunto = p.Assunto.Descricao,
                DataRecebimento = p.DataAbertura.ToString("dd/MM/yyyy HH:mm"),
                ResponsavelRecebimento = p.UsuarioAbertura.Logon,
                Index = dataList.IndexOf(p)
            });

            return JsonConvert.SerializeObject(new { draw, recordsTotal, recordsFiltered, data });
        }

        [ActionName("Recebidos")]
        public ActionResult Received()
        {
            var setorLogadoId = GetSetorUsuario().Id;

            var protocolos = db.Protocolos
                .Include(p => p.Assunto)
                .Include(p => p.UsuarioEdicao)
                .Where(p => p.Status == StatusProtocolo.Recebido.Codigo && p.SetorAtualId == setorLogadoId)
                .ToList();

            var models = new List<ReceivedProtocoloModel>();

            // Construção dos view models
            foreach (var protocolo in protocolos)
            {
                models.Add(new ReceivedProtocoloModel(
                    protocolo.Id,
                    protocolo.Numero,
                    protocolo.Ano,
                    protocolo.Chave,
                    protocolo.Assunto.Descricao,
                    protocolo.DataStatus,
                    protocolo.UsuarioEdicao.Logon,
                    IsAtrasado(protocolo)
                ));
            }

            return View("Received", models);
        }

        [HttpPost]
        public ActionResult OpenSendView(List<ReceivedProtocoloModel> receivedList)
        {
            var setorLogadoId = GetSetorUsuario().Id;

            ViewBag.SetorDestinoId = new SelectList(db.Setores.Where(s => s.Id != setorLogadoId).OrderBy(s => s.Nome), "Id", "Nome");

            return PartialView("_Send", new SendProtocoloModel(
                DateTime.Now,
                GetSelectedProtocolos(receivedList)
            ));
        }

        [HttpPost]
        public ActionResult OpenFinishView(List<ReceivedProtocoloModel> receivedList)
        {
            return PartialView("_Finish", new ActionProtocoloModel(
                GetSelectedProtocolos(receivedList)
            ));
        }

        [HttpPost]
        public ActionResult OpenCancelView(List<ReceivedProtocoloModel> receivedList)
        {
            return PartialView("_Cancel", new ActionProtocoloModel(
                GetSelectedProtocolos(receivedList)
            ));
        }

        [HttpPost]
        public ActionResult OpenDispatchView(List<ReceivedProtocoloModel> receivedList)
        {
            return PartialView("_Dispatch", new ActionProtocoloModel(
                GetSelectedProtocolos(receivedList)
            ));
        }

        private List<Lote> GetSelectedLotes(List<ReceivingProtocoloModel> receivingList)
        {
            List<int> Ids = new List<int>();

            foreach (var receiving in receivingList)
            {
                if (receiving.Selecionado)
                {
                    Ids.Add(receiving.NumeroLote);
                }
            }

            return db.Lotes.Where(p => Ids.Contains(p.Id)).ToList();
        }

        private List<Models.Protocolo> GetSelectedProtocolos(List<ReceivedProtocoloModel> receivedList)
        {
            List<int> Ids = new List<int>();

            foreach (var received in receivedList)
            {
                if (received.Selecionado)
                {
                    Ids.Add(received.ProtocoloId);
                }
            }

            return db.Protocolos.Where(p => Ids.Contains(p.Id)).ToList();
        }

        [HttpPost, ActionName("Enviar")]
        public ActionResult Send(SendProtocoloModel sendModel)
        {
            if (ModelState.IsValid)
            {
                var usuarioLogado = GetUsuarioLogado();
                var setorOrigemId = GetSetorUsuario().Id;

                var loteProtocolo = new Dictionary<int, Lote>(sendModel.Protocolos.Count);

                foreach (var prot in sendModel.Protocolos)
                {
                    var lote = new Lote
                    {
                        DataAbertura = sendModel.DataEnvio,
                        UsuarioAberturaId = usuarioLogado.Id,
                        SetorOrigemId = setorOrigemId,
                        SetorDestinoId = sendModel.SetorDestinoId,
                        Status = StatusLote.Enviado.Codigo,
                    };

                    var protocolo = db.Protocolos.Find(prot.Id);

                    protocolo.Status = StatusProtocolo.Encaminhado.Codigo;

                    db.Entry(protocolo).State = EntityState.Modified;

                    lote.Protocolos.Add(protocolo);

                    loteProtocolo.Add(prot.Id, lote);
                }

                db.Lotes.AddRange(loteProtocolo.Values);

                db.SaveChanges();

                foreach (var prot in sendModel.Protocolos)
                {
                    var historico = new ProtocoloHistorico
                    {
                        ProtocoloId = prot.Id,
                        DataMovimento = DateTime.Now,
                        Historico = "Envio do Protocolo",
                        UsuarioId = usuarioLogado.Id,
                        SetorId = setorOrigemId,
                        SetorDestinoId = sendModel.SetorDestinoId,
                        LoteId = loteProtocolo[prot.Id].Id,
                        Status = StatusHistorico.Enviado.Codigo
                    };

                    db.HistoricoProtocolo.Add(historico);
                }

                db.SaveChanges();

                Success("Protocolo(s) enviado(s) com sucesso.");

                return Json(new { Success = true });
            }

            //var setorLogadoId = GetUsuarioLogado().SetorPadraoId;

            //ViewBag.SetorDestinoId = new SelectList(db.Setores.Where(s => s.Id != setorLogadoId).OrderBy(s => s.Nome), "Id", "Nome", sendModel.SetorDestinoId);

            return Json(new { Success = false, Page = RenderRazorViewToString("_Send", sendModel) });
        }

        [HttpPost, ActionName("Finalizar")]
        public ActionResult Finish(ActionProtocoloModel actionModel)
        {
            if (ModelState.IsValid)
            {
                ExecuteGenericAction(actionModel, StatusProtocolo.Finalizado, StatusHistorico.Finalizado);

                foreach (var protocolo in actionModel.Protocolos)
                {
                    /*
                        Fluxo em aberto: sem data de saída
                    */
                    var fluxoAberto = db.FluxoProtocolo.FirstOrDefault(f => f.ProtocoloId == protocolo.Id && f.DataSaida == null);

                    // Só para garantir que não dê null pointer
                    if (fluxoAberto != null)
                    {
                        fluxoAberto.DataSaida = DateTime.Now;

                        db.Entry(fluxoAberto).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                Success("Protocolo(s) finalizado(s) com sucesso.");

                return Json(new { Success = true });
            }

            return Json(new { Success = false, Page = RenderRazorViewToString("_Finish", actionModel) });
        }

        [HttpPost, ActionName("CancelarProtocolo")]
        public ActionResult ActionCancel(ActionProtocoloModel actionModel)
        {
            if (ModelState.IsValid)
            {
                ExecuteGenericAction(actionModel, StatusProtocolo.Cancelado, StatusHistorico.Cancelado);

                Success("Protocolo(s) cancelados(s) com sucesso.");

                return Json(new { Success = true });
            }

            return Json(new { Success = false, Page = RenderRazorViewToString("_Cancel", actionModel) });
        }

        [HttpPost, ActionName("SalvarObservacao")]
        public ActionResult Dispatch(ActionProtocoloModel actionModel)
        {
            if (ModelState.IsValid)
            {
                ExecuteGenericAction(actionModel, null, StatusHistorico.Despachado);

                Success("Protocolo(s) despachado(s) com sucesso.");

                return Json(new { Success = true });
            }

            return Json(new { Success = false, Page = RenderRazorViewToString("_Dispatch", actionModel) });
        }

        private void ExecuteGenericAction(ActionProtocoloModel actionModel, StatusProtocolo statusProtocolo, StatusHistorico statusHistorito)
        {
            var usuarioLogado = GetUsuarioLogado();
            var setorOrigemId = GetSetorUsuario().Id;

            foreach (var prot in actionModel.Protocolos)
            {
                if (statusProtocolo != null)
                {
                    var protocolo = db.Protocolos.Find(prot.Id);

                    protocolo.Status = statusProtocolo.Codigo;
                    protocolo.DataStatus = DateTime.Now;
                    db.Entry(protocolo).State = EntityState.Modified;
                }

                var historico = new ProtocoloHistorico
                {
                    ProtocoloId = prot.Id,
                    DataMovimento = DateTime.Now,
                    Historico = actionModel.DescricaoJustificativa,
                    UsuarioId = usuarioLogado.Id,
                    SetorId = setorOrigemId,
                    Status = statusHistorito.Codigo
                };

                db.HistoricoProtocolo.Add(historico);
            }

            db.SaveChanges();
        }

        [ActionName("Enviados")]
        public ActionResult Sent()
        {
            var setorLogadoId = GetSetorUsuario().Id;

            var lotes = db.Lotes.Where(lote => lote.SetorOrigemId == setorLogadoId && lote.Status == StatusLote.Enviado.Codigo);

            return View("Sent", lotes.Any());
        }

        [HttpPost]
        public string Sent(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            int initialPage = start.HasValue ? start.Value : 0;
            int pageSize = length ?? 20;

            var setorLogadoId = GetSetorUsuario().Id;

            var lotes = db.Lotes
                .Include(l => l.SetorDestino)
                .Include(l => l.SetorDestino.Secretaria)
                .Include(l => l.UsuarioAbertura)
                .Include(l => l.Protocolos.Select(p => p.Assunto))
                .Where(l => l.SetorOrigemId == setorLogadoId && l.Status == StatusLote.Enviado.Codigo);

            var recordsTotal = lotes.Count();

            if (!string.IsNullOrEmpty(search))
            {
                var palavras = search.Split(' ');

                foreach (var palavra in palavras)
                {
                    if (!string.IsNullOrEmpty(palavra))
                    {
                        var searchPalavra = palavra.ToUpper();

                        lotes = lotes.Where(l => SqlFunctions.StringConvert((double)l.Id).Contains(searchPalavra)
                                || l.SetorDestino.Nome.ToUpper().Contains(searchPalavra)
                                || l.SetorDestino.Secretaria.Nome.ToUpper().Contains(searchPalavra)
                                || l.UsuarioAbertura.Logon.ToUpper().Contains(searchPalavra)
                                || l.Protocolos.Any(p => SqlFunctions.StringConvert((double)p.Numero).Contains(searchPalavra)
                                    || SqlFunctions.StringConvert((double)p.Ano).Contains(searchPalavra)
                                    || p.Chave.ToUpper().Contains(searchPalavra)
                                    || p.Assunto.Descricao.ToUpper().Contains(searchPalavra)
                                )
                        );
                    }
                }
            }

            var recordsFiltered = lotes.Count();

            var dataList = lotes
                .OrderBy(lote => lote.Id)
                .Skip(initialPage)
                .Take(pageSize)
                .ToList();

            var data = dataList.Select(lote => new {
                NumeroLote = lote.Id,
                NumeroAno = lote.Protocolos.ElementAt(0).NumeroAno,
                SecretariaSetorDestino = lote.SetorDestino.Secretaria.Nome + "/" + lote.SetorDestino.Nome,
                ResponsavelEnvio = lote.UsuarioAbertura.Logon,
                DataEnvio = lote.DataAbertura.ToString("dd/MM/yyyy HH:mm"),
                Index = dataList.IndexOf(lote)
            });

            return JsonConvert.SerializeObject(new { draw, recordsTotal, recordsFiltered, data });
        }

        [HttpGet]
        public JsonResult GetLoteImprimir(int id)
        {
            var lote = db.Lotes
                .Include(l => l.SetorOrigem)
                .Include(l => l.SetorDestino)
                .Include(l => l.UsuarioAbertura)
                .Include(l => l.Protocolos.Select(p => p.Assunto))
                .Include(l => l.Protocolos.Select(p => p.Pessoa))
                .SingleOrDefault(l => l.Id == id);

            // Pega o único registro de instituição do banco caso exista
            if (db.Intituicoes.Any())
            {
                lote.Instituicao = db.Intituicoes.Single();
            }

            return Json(RenderRazorViewToString("_ReceivingReportView", lote), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CapaProtocolo(int id)
        {
            var protocolo = db.Protocolos
                .Include(p => p.Assunto)
                .Include(p => p.Pessoa)
                .Include(p => p.UsuarioAbertura)
                .SingleOrDefault(p => p.Id == id);

            // Pega o único registro de instituição do banco caso exista
            if (db.Intituicoes.Any())
            {
                protocolo.Instituicao = db.Intituicoes.Single();
            }

            return Json(RenderRazorViewToString("_CoverReportView", protocolo), JsonRequestBehavior.AllowGet);
        }

        [ActionName("Atrasados")]
        public ActionResult Late()
        {
            var setorLogadoId = GetSetorUsuario().Id;

            var protocolos = db.Protocolos
                .Include(p => p.Assunto)
                .Include(p => p.UsuarioAbertura)
                .Include(p => p.UsuarioEdicao)
                .Where(p => (p.Status == StatusProtocolo.Aberto.Codigo || p.Status == StatusProtocolo.Recebido.Codigo)
                    && p.SetorAtualId == setorLogadoId)
                .ToList();

            List<ReceivedProtocoloModel> models = new List<ReceivedProtocoloModel>();

            // Construção dos view models
            foreach (var protocolo in protocolos)
            {
                if (IsAtrasado(protocolo))
                {
                    DateTime dataRecebimento;
                    Usuario responsavelRecebimento;

                    if (StatusProtocolo.Recebido.Codigo.Equals(protocolo.Status))
                    {
                        dataRecebimento = protocolo.DataStatus;
                        responsavelRecebimento = protocolo.UsuarioEdicao;
                    }
                    else
                    {
                        dataRecebimento = protocolo.DataAbertura;
                        responsavelRecebimento = protocolo.UsuarioAbertura;
                    }

                    models.Add(new ReceivedProtocoloModel(
                        protocolo.Id,
                        protocolo.Numero,
                        protocolo.Ano,
                        protocolo.Chave,
                        protocolo.Assunto.Descricao,
                        dataRecebimento,
                        responsavelRecebimento.Logon,
                        IsAtrasado(protocolo)
                    )
                    {
                        DiasAtraso = DiasAtraso(protocolo),
                        Prazo = protocolo.Assunto.Prazo
                    });
                }
            }

            models.Sort((x, y) => DateTime.Compare(y.DataRecebimento, x.DataRecebimento));

            return View("Late", models);
        }

        [HttpPost, ActionName("Cancelar")]
        public ActionResult Cancel(List<SentProtocoloModal> sentList)
        {
            var usuarioLogado = GetUsuarioLogado();
            var setorLogadoId = GetSetorUsuario().Id;

            foreach (var sent in sentList)
            {
                if (sent.Selecionado)
                {
                    Lote lote = db.Lotes
                        .Include(l => l.Protocolos)
                        .SingleOrDefault(l => l.Id == sent.NumeroLote);

                    lote.Status = StatusLote.Cancelado.Codigo;

                    db.Entry(lote).State = EntityState.Modified;

                    foreach (var protocolo in lote.Protocolos)
                    {
                        protocolo.Status = StatusProtocolo.Aberto.Codigo;
                        protocolo.DataStatus = DateTime.Now;

                        db.Entry(protocolo).State = EntityState.Modified;

                        db.HistoricoProtocolo.Add(new ProtocoloHistorico
                        {
                            ProtocoloId = protocolo.Id,
                            DataMovimento = DateTime.Now,
                            Historico = "Cancelamento de envio do Lote",
                            UsuarioId = usuarioLogado.Id,
                            SetorId = setorLogadoId,
                            SetorDestinoId = lote.SetorDestinoId,
                            LoteId = lote.Id,
                            Status = StatusHistorico.Cancelado.Codigo
                        });
                    }
                }
            }

            db.SaveChanges();

            Success("Lote(s) cancelado(s) com sucesso.");

            return RedirectToAction("Enviados");
        }

        public ActionResult ShowProtocolos(int id)
        {
            var protocolos = db.Lotes
                .Include(l => l.Protocolos.Select(p => p.Assunto))
                .SingleOrDefault(l => l.Id == id)
                .Protocolos
                .ToList();

            return PartialView("_ProtocolosDetails", protocolos);
        }

        [ActionName("Finalizados")]
        public ActionResult Finished()
        {
            var setorLogadoId = GetSetorUsuario().Id;

            var protocolos = db.Protocolos
                .Include(p => p.Assunto)
                .Include(p => p.UsuarioAbertura)
                .Where(p => (p.Status == StatusProtocolo.Finalizado.Codigo) && p.SetorAtualId == setorLogadoId)
                .ToList();

            List<FinishedProtocoloModel> models = new List<FinishedProtocoloModel>();

            // Construção dos view models
            foreach (var protocolo in protocolos)
            {
                models.Add(new FinishedProtocoloModel(
                    protocolo.Id,
                    protocolo.Numero,
                    protocolo.Ano,
                    protocolo.Chave,
                    protocolo.Assunto.Descricao,
                    protocolo.DataStatus
                ));
            }

            return View("Finished", models);
        }

        public ActionResult Index()
        {
            var protocolos = db.Protocolos.AsQueryable();

            if (IsConsulta())
            {
                protocolos = protocolos.Where(p => p.SetorAtualId == GetSetorUsuario().Id && p.Ano == 2020)
                                       .OrderBy(p => p.Numero);
            }

            return View(protocolos.Any());
        }

        [HttpPost]
        public string Index(int? draw, int? start, int? length)
        {
            var search = Request["search[value]"];
            int initialPage = start.HasValue ? start.Value : 0;
            int pageSize = length ?? 20;

            var protocolos = db.Protocolos
                .Include(p => p.Assunto)
                .Include(p => p.Pessoa)
                .Include(p => p.Secretaria)
                .Include(p => p.Fornecedor)
                .Include(p => p.SetorAtual)
                .Include(p => p.UsuarioAbertura)
                .Include(p => p.UsuarioEdicao);

            var recordsTotal = protocolos.Count();

            if (!string.IsNullOrEmpty(search))
            {
                string[] palavras;

                if (search.Contains('/'))
                {
                    palavras = search.Split('/');
                }
                else
                {
                    palavras = search.Split(' ');
                }

                foreach (var palavra in palavras)
                {
                    if (!string.IsNullOrEmpty(palavra))
                    {
                        var searchPalavra = palavra.Trim().ToUpper();

                        protocolos = protocolos.Where(p => SqlFunctions.StringConvert((double)p.Numero).Contains(searchPalavra)
                                || SqlFunctions.StringConvert((double)p.Ano).Contains(searchPalavra)
                                || p.Chave.ToUpper().Contains(searchPalavra)
                                || p.Assunto.Descricao.ToUpper().Contains(searchPalavra)
                                || p.Pessoa.NomeRazao.ToUpper().Contains(searchPalavra)
                                || p.Secretaria.Nome.ToUpper().Contains(searchPalavra)
                                || p.SetorAtual.Nome.ToUpper().Contains(searchPalavra)
                                || p.Fornecedor.NomeRazao.ToUpper().Contains(searchPalavra))
                              .OrderByDescending(p => p.Ano)
                              .ThenByDescending(p => p.Numero);
                    }
                }
            }

            var recordsFiltered = protocolos.Count();

            var data = protocolos
                .OrderByDescending(s => s.Ano)
                .ThenByDescending(s => s.Numero)
                .Skip(initialPage)
                .Take(pageSize)
                .ToList()
                .Select(p => new {
                    NumeroAno = p.NumeroAno,
                    Assunto = p.Assunto.Descricao,
                    Requerente = p.Pessoa.NomeRazao,
                    SecretariaSetor = p.SecretariaSetor,
                    Fornecedor = p.Fornecedor == null ? string.Empty : p.Fornecedor.NomeRazao,
                    Abertura = p.DataAbertura.ToString("dd/MM/yyyy"),
                    Status = StatusProtocolo.DescricaoFromCodigo(p.Status),
                    Id = p.Id,
                    p.Ano,
                    p.Numero
                });

            return JsonConvert.SerializeObject(new { draw, recordsTotal, recordsFiltered, data });
        }

        [ActionName("Visualizar")]
        public ActionResult Details(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Protocolo protocolo = db.Protocolos
                .Include(p => p.TipoDocumento)
                .Include(p => p.Assunto)
                .Include(p => p.Pessoa)
                .Include(p => p.Pessoa.Municipio)
                .Include(p => p.Fornecedor)
                .Include(p => p.DocumentosProtocolo.Select(d => d.Documento))
                .Include(p => p.HistoricoProtocolo.Select(h => h.Usuario))
                .Include(p => p.HistoricoProtocolo.Select(h => h.Setor))
                .Include(p => p.HistoricoProtocolo.Select(h => h.SetorDestino))
                .Include(p => p.AnexosProtocolo)
                .SingleOrDefault(p => p.Id == id);

            if (protocolo == null)
            {
                return HttpNotFound();
            }

            // armazena a página que chamou o Visualizar
            if (returnUrl == null)
            {
                ViewBag.returnUrl = Request.UrlReferrer.AbsolutePath;
            }
            else
            {
                ViewBag.returnUrl = returnUrl;
            }

            ViewBag.TipoDocumentoId = new SelectList(new List<TipoDocumento> { protocolo.TipoDocumento }, "Id", "Descricao", protocolo.TipoDocumentoId);
            ViewBag.AssuntoId = new SelectList(new List<Assunto> { protocolo.Assunto }, "Id", "Descricao", protocolo.AssuntoId);
            ViewBag.PovoadoId = new SelectList(db.Povoados.OrderBy(a => a.Descricao), "Id", "Descricao", protocolo.PovoadoId);

            return View("Details", protocolo);
        }

        [HttpPost]
        [ActionName("AdicionarAnexo")]
        public ActionResult AddAnexo(int id, HttpPostedFileBase anexo, string returnUrl)
        {
            var protocoloAnexo = CreateAnexo(anexo);

            if (protocoloAnexo == null)
            {
                ModelState.AddModelError("Arquivo", "Por favor selecione um arquivo");
            }
            else
            {
                protocoloAnexo.ProtocoloId = id;

                db.AnexoProtocolo.Add(protocoloAnexo);

                var ultimoHistorico = db.Protocolos.Find(id).HistoricoProtocolo.LastOrDefault();

                db.HistoricoProtocolo.Add(new ProtocoloHistorico
                {
                    ProtocoloId = id,
                    DataMovimento = DateTime.Now,
                    Historico = "Anexo \"" + protocoloAnexo.NomeArquivo + "\" adicionado",
                    UsuarioId = GetUsuarioLogado().Id,
                    SetorId = ultimoHistorico.SetorId,
                    SetorDestinoId = ultimoHistorico.SetorDestinoId,
                    LoteId = ultimoHistorico.LoteId,
                    Status = ultimoHistorico.Status
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
        public ActionResult DelAnexo(int id, string returnUrl)
        {
            var protocoloAnexo = db.AnexoProtocolo.Find(id);
            var ultimoHistorico = db.Protocolos.Find(protocoloAnexo.ProtocoloId).HistoricoProtocolo.LastOrDefault();

            db.AnexoProtocolo.Remove(protocoloAnexo);

            db.HistoricoProtocolo.Add(new ProtocoloHistorico
            {
                ProtocoloId = protocoloAnexo.ProtocoloId,
                DataMovimento = DateTime.Now,
                Historico = "Anexo \"" + protocoloAnexo.NomeArquivo + "\" deletado",
                UsuarioId = GetUsuarioLogado().Id,
                SetorId = ultimoHistorico.SetorId,
                SetorDestinoId = ultimoHistorico.SetorDestinoId,
                LoteId = ultimoHistorico.LoteId,
                Status = ultimoHistorico.Status
            });

            db.SaveChanges();

            Success("Anexo deletado com sucesso.");

            return RedirectToAction("Visualizar", new { id = protocoloAnexo.ProtocoloId, returnUrl = returnUrl });
        }

        [ActionName("Cadastrar")]
        [Authorize(Roles = "Administrador,Gestor,Comum")]
        public ActionResult Create()
        {
            ViewBag.TipoDocumentoId = new SelectList(db.TiposDocumento.OrderBy(t => t.Descricao), "Id", "Descricao");
            ViewBag.AssuntoId = new SelectList(db.Assuntos.OrderBy(a => a.Descricao), "Id", "Descricao");
            ViewBag.PovoadoId = new SelectList(db.Povoados.OrderBy(a => a.Descricao), "Id", "Descricao");
            return View("Create", new CreateProtocoloModel());
            //return View("Index", null, null);

        }

        [HttpGet]
        public JsonResult AutocompletePessoas(string term)
        {
            var result = (from p in db.Pessoas where p.NomeRazao.ToLower().Contains(term.ToLower()) select new { p.NomeRazao, p.Id })
                .OrderBy(p => p.NomeRazao)
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDocuments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var documentos = db.AssuntosDocumento
                .Include(d => d.Documento)
                .Where(d => d.AssuntoId == id)
                .Select(d => new DocumentoAssuntoProtocolo
                {
                    Id = d.DocumentoId,
                    Descricao = d.Documento.Descricao,
                    Obrigatorio = d.Obrigatorio,
                    Entregue = false
                }).ToList();

            return Json(documentos, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("Cadastrar")]
        [Authorize(Roles = "Administrador,Gestor,Comum")]
        public ActionResult Create(CreateProtocoloModel createModel, HttpPostedFileBase anexo)
        {
            ViewBag.TipoDocumentoId = new SelectList(db.TiposDocumento.OrderBy(t => t.Descricao), "Id", "Descricao", createModel.TipoDocumentoId);
            ViewBag.AssuntoId = new SelectList(db.Assuntos.OrderBy(a => a.Descricao), "Id", "Descricao", createModel.AssuntoId);
            ViewBag.PovoadoId = new SelectList(db.Povoados.OrderBy(a => a.Descricao), "Id", "Descricao", createModel.PovoadoId);

            if (!ModelState.IsValid)
            {
                return View("Create", createModel);
            }

            foreach (var documento in createModel.DocumentosList)
            {
                if (documento.Obrigatorio && !documento.Entregue)
                {
                    Danger("Todos os documentos obrigatórios precisam ser entregues para a criação do protocolo.");

                    return View("Create", createModel);
                }
            }

            var usuarioLogado = GetUsuarioLogado();
            var setorLogado = GetSetorUsuario();

            int numero = GetNextNumeroProtocolo();

            var protocolo = new Models.Protocolo
            {
                Numero = numero,
                Ano = DateTime.Now.Year,
                TipoDocumentoId = createModel.TipoDocumentoId,
                AssuntoId = createModel.AssuntoId,
                PessoaId = createModel.RequerenteId,
                SecretariaId = setorLogado.SecretariaId,
                SetorAtualId = setorLogado.Id,
                FornecedorId = createModel.FornecedorId,
                Descricao = createModel.Descricao,
                DataAbertura = DateTime.Now,
                UsuarioAberturaId = usuarioLogado.Id,
                Status = StatusProtocolo.Aberto.Codigo,
                DataStatus = DateTime.Now,
                Chave = ChaveAleatoia()
            };

            db.Protocolos.Add(protocolo);

            db.SaveChanges();

            var documentosProtocolo = new List<DocumentoProtocolo>();

            createModel.DocumentosList.ForEach(d =>
            {
                documentosProtocolo.Add(new DocumentoProtocolo
                {
                    ProtocoloId = protocolo.Id,
                    DocumentoId = d.Id,
                    Entregue = d.Entregue
                });
            });

            db.DocumentosProtocolo.AddRange(documentosProtocolo);

            var historico = new ProtocoloHistorico
            {
                ProtocoloId = protocolo.Id,
                DataMovimento = DateTime.Now,
                Historico = "Criação do Protocolo",
                UsuarioId = protocolo.UsuarioAberturaId,
                SetorId = setorLogado.Id,
                Status = StatusHistorico.Aberto.Codigo
            };

            db.HistoricoProtocolo.Add(historico);

            var fluxo = new ProtocoloFluxo
            {
                ProtocoloId = protocolo.Id,
                SetorId = setorLogado.Id,
                DataEntrada = DateTime.Now
            };

            db.FluxoProtocolo.Add(fluxo);

            var protocoloAnexo = CreateAnexo(anexo);

            if (protocoloAnexo != null)
            {
                protocoloAnexo.ProtocoloId = protocolo.Id;

                db.AnexoProtocolo.Add(protocoloAnexo);
            }

            db.SaveChanges();

            UpdateNumeroProtocolo();

            createModel.ProtocoloSalvo = true;
            createModel.ProtocoloId = protocolo.Id;
            createModel.NumeroProtocolo = protocolo.NumeroAno;

            return View("Create", createModel);
        }

        [ActionName("Editar")]
        [Authorize(Roles = "Administrador,Gestor,Comum")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Protocolo protocolo = db.Protocolos.Find(id);

            if (protocolo == null)
            {
                return HttpNotFound();
            }

            if (protocolo.Status == StatusProtocolo.Cancelado.Codigo || protocolo.Status == StatusProtocolo.Finalizado.Codigo)
            {
                Danger("Não é possível alterar pois o Protocolo foi " + StatusProtocolo.DescricaoFromCodigo(protocolo.Status) + ".");

                return RedirectToAction("Index");
            }

            var setorLogadoId = GetSetorUsuario().Id;

            if (!IsAdministrador() && !IsGestor() && protocolo.SetorAtualId != setorLogadoId)
            {
                Danger("Não é possível alterar pois o Protocolo não se encontra no seu setor.");

                return RedirectToAction("Index");
            }

            protocolo.NomeFornecedor = protocolo.Fornecedor == null ? string.Empty : protocolo.Fornecedor.NomeRazao;

            ViewBag.TipoDocumentoId = new SelectList(new List<TipoDocumento> { protocolo.TipoDocumento }, "Id", "Descricao", protocolo.TipoDocumentoId);
            ViewBag.AssuntoId = new SelectList(new List<Assunto> { protocolo.Assunto }, "Id", "Descricao", protocolo.AssuntoId);
            ViewBag.PovoadoId = new SelectList(db.Povoados.OrderBy(a => a.Descricao), "Id", "Descricao", protocolo.PovoadoId);
            ViewBag.Historico = new SelectList(db.HistoricoProtocolo.OrderBy(d => d.Historico).ToList());
            return View("Edit", protocolo);
        }

        [HttpPost, ActionName("Editar")]
        [Authorize(Roles = "Administrador,Gestor,Comum")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Ano,Numero,AssuntoId,TipoDocumentoId,PessoaId,FornecedorId,Descricao,HistoricoObservacao")] Models.Protocolo protocolo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TipoDocumentoId = new SelectList(new List<TipoDocumento> { protocolo.TipoDocumento }, "Id", "Descricao", protocolo.TipoDocumentoId);
                ViewBag.AssuntoId = new SelectList(new List<Assunto> { protocolo.Assunto }, "Id", "Descricao", protocolo.AssuntoId);

                return View("Edit", protocolo);
            }

            var usuarioLogado = GetUsuarioLogado();

            var protocoloPersisted = db.Protocolos.Find(protocolo.Id);

            protocoloPersisted.TipoDocumentoId = protocolo.TipoDocumentoId;
            protocoloPersisted.PessoaId = protocolo.PessoaId;
            protocoloPersisted.FornecedorId = protocolo.FornecedorId;
            protocoloPersisted.Descricao = protocolo.Descricao;
            protocoloPersisted.UsuarioEdicaoId = usuarioLogado.Id;
            protocoloPersisted.DataEdicao = DateTime.Now;

            db.Entry(protocoloPersisted).State = EntityState.Modified;

            if (!string.IsNullOrEmpty(protocolo.HistoricoObservacao))
            {
                var ultimoHistorico = protocoloPersisted.HistoricoProtocolo.LastOrDefault();

                db.HistoricoProtocolo.Add(new ProtocoloHistorico
                {
                    ProtocoloId = protocolo.Id,
                    DataMovimento = DateTime.Now,
                    Historico = protocolo.HistoricoObservacao,
                    UsuarioId = usuarioLogado.Id,
                    SetorId = ultimoHistorico.SetorId,
                    SetorDestinoId = ultimoHistorico.SetorDestinoId,
                    LoteId = ultimoHistorico.LoteId,
                    Status = ultimoHistorico.Status
                });
            }

            db.SaveChanges();

            Success("Protocolo alterado com sucesso.");

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrador,Gestor,Comum")]
        public ActionResult RedirectToIndex()
        {
            Success("Protocolo cadastrado com sucesso.");

            return RedirectToAction("Index");
        }

        [HttpGet, ActionName("EnviarProtocolo")]
        [Authorize(Roles = "Administrador,Gestor,Comum")]
        public ActionResult RedirectToEnviar(int id)
        {
            var setorLogadoId = GetSetorUsuario().Id;

            ViewBag.SetorDestinoId = new SelectList(db.Setores.Where(s => s.Id != setorLogadoId).OrderBy(s => s.Nome), "Id", "Nome");

            return View("Send", new SendProtocoloModel(DateTime.Now, db.Protocolos.Where(p => p.Id == id).ToList()));
        }

        public FileResult DownloadAnexo(int id)
        {
            var anexo = db.AnexoProtocolo.Find(id);

            return File(anexo.Arquivo, anexo.TipoArquivo, anexo.NomeArquivo);
        }

        private ProtocoloAnexo CreateAnexo(HttpPostedFileBase anexo)
        {
            if (anexo == null || anexo.ContentLength == 0)
            {
                return null;
            }

            var protocoloAnexo = new ProtocoloAnexo
            {
                NomeArquivo = Path.GetFileName(anexo.FileName),
                TipoArquivo = anexo.ContentType,
            };

            using (var reader = new BinaryReader(anexo.InputStream))
            {
                protocoloAnexo.Arquivo = reader.ReadBytes(anexo.ContentLength);
            }

            return protocoloAnexo;
        }

        private int GetNextNumeroProtocolo()
        {
            NumeroProtocolo numeroProtocolo = db.NumeroProtocolo.FirstOrDefault(np => np.Ano == DateTime.Now.Year);

            if (numeroProtocolo == null)
            {
                numeroProtocolo = new NumeroProtocolo
                {
                    Ano = DateTime.Now.Year,
                    Numero = 1
                };

                db.NumeroProtocolo.Add(numeroProtocolo);
                db.SaveChanges();
            }

            return numeroProtocolo.Numero;
        }

        private void UpdateNumeroProtocolo()
        {
            NumeroProtocolo numeroProtocolo = db.NumeroProtocolo.FirstOrDefault(np => np.Ano == DateTime.Now.Year);

            numeroProtocolo.Numero = ++numeroProtocolo.Numero;

            db.Entry(numeroProtocolo).State = EntityState.Modified;

            db.SaveChanges();
        }

        private bool IsAtrasado(Models.Protocolo protocolo)
        {
            if (protocolo.Assunto == null)
            {
                return false;
            }

            var prazo = protocolo.Assunto.Prazo;

            var diasCriacao = DateTime.Now.Date.Subtract(protocolo.DataAbertura.Date).Days;

            return (diasCriacao > prazo);
        }

        private int DiasAtraso(Models.Protocolo protocolo)
        {
            if (protocolo.Assunto == null)
            {
                return 0;
            }

            var prazo = protocolo.Assunto.Prazo;

            var diasAtraso = DateTime.Now.Date.Subtract(protocolo.DataAbertura.Date.AddDays((double)prazo)).Days;

            return diasAtraso;
        }

        // Geração de 4 LETRAS aleatóreas usadas na busca do protocolo por chave específica
        private static string ChaveAleatoia()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            var random = new Random();

            return new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
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