using Protocolo.Helpers;
using Protocolo.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Protocolo.Controllers
{
    [Authorize]
    public class GraficoController : BaseController
    {
        public GraficoController() 
            : base(new ProtocoloEntities())
        {

        }

        [HttpGet]
        public JsonResult FluxoProcesso(int id)
        {
            var fluxoProtocolo = db.FluxoProtocolo
                .Include(f => f.Protocolo)
                .Include(f => f.Protocolo.TipoDocumento)
                .Include(f => f.Protocolo.Pessoa)
                .Include(f => f.Setor)
                .Include(f => f.UsuarioRecebimento)
                .Where(f =>
                    f.Protocolo.Id == id && f.Protocolo.Status != StatusProtocolo.Cancelado.Codigo
                 )
                .OrderBy(f => f.DataEntrada)
                .ToList();

            // Guarda o protocolo para futura referência
            var protocolo = fluxoProtocolo.First().Protocolo;

            var searchModel = new SearchFlow
            {
                NumeroAno = protocolo.NumeroAno,
                TipoDocumento = protocolo.TipoDocumento.Descricao,
                Assunto = protocolo.Assunto.Descricao,
                Requerente = protocolo.Pessoa.NomeRazao,
                FluxoProtocolo = fluxoProtocolo
            };

            // Pega o único registro de instituição do banco caso exista
            if (db.Intituicoes.Any())
            {
                searchModel.Instituicao = db.Intituicoes.Single();
            }

            return Json(RenderRazorViewToString("_ProcessFlow", searchModel), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Dashboard()
        {
            var setorLogadoId = GetSetorUsuario().Id;

            /*
             * Protocolos no setor (usado para a contagem de protocolos cadastrados no setor e protocolos atrasados)
             */
            var protocolosSetor = db.Protocolos
                .Where(p => (p.Status == StatusProtocolo.Aberto.Codigo || p.Status == StatusProtocolo.Recebido.Codigo) && p.SetorAtualId == setorLogadoId);

            /*
             * Protocolos no setor
             */
            var TotalSetor = protocolosSetor.Count();

            /*
             * Protocolos recebidos (usado para a contagem de protocolos no setor e protocolos atrasados)
             */
            var protocolosRecebidos = db.Protocolos
                .Where(p => p.Status == StatusProtocolo.Recebido.Codigo && p.SetorAtualId == setorLogadoId);

            /*
             * Protocolos recebidos
             */
            var TotalRecebidos = protocolosRecebidos.Count();

            /*
             * Protocolos a receber
             */
            var TotalReceber = db.Lotes
                .Where(l => l.SetorDestinoId == setorLogadoId && l.Status == StatusLote.Enviado.Codigo)
                .Count();

            /*
             * Protocolos enviados
             */
            var TotalEnviados = db.Lotes
                .Where(l => l.SetorOrigemId == setorLogadoId && l.Status == StatusLote.Enviado.Codigo)
                .Count();

            /*
             * Protocolos atrasados
             */
            var TotalAtrasadosSetor = 0;

            foreach (var prot in protocolosSetor)
            {
                if (IsAtrasado(prot))
                {
                    TotalAtrasadosSetor++;
                }
            }

            foreach (var prot in protocolosRecebidos)
            {
                if (IsAtrasado(prot))
                {
                    TotalAtrasadosSetor++;
                }
            }

            var TotalFinalizados = db.Protocolos
                .Where(p => (p.Status == StatusProtocolo.Finalizado.Codigo) && p.SetorAtualId == setorLogadoId)
                .Count();

            return View(new DashboardModel(TotalSetor, TotalRecebidos, TotalReceber, TotalEnviados, TotalAtrasadosSetor, TotalFinalizados));
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
    }
}