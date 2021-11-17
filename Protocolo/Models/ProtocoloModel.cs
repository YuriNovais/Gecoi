using Protocolo.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Protocolo.Models
{
    public class SearchProtocoloModel
    {
        [Required]
        public int? Numero { get; set; }

        [Required]
        public int? Ano { get; set; }

        [Required]
        public string Chave { get; set; }

        public ResultProtocoloModel ResultModel { get; set; }
    }

    public class ResultProtocoloModel
    {
        [Display(Name = "N° Protocolo")]
        public string NumeroAno { get; set; }

        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; }

        public string Assunto { get; set; }

        public string Requerente { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Setor Atual")]
        public string SetorAtual { get; set; }

        public string Status { get; set; }

        public List<ProtocoloHistorico> Historico { get; set; }
    }

    public class CreateProtocoloModel
    {
        public CreateProtocoloModel()
        {
            ProtocoloSalvo = false;
            DocumentosList = new List<DocumentoAssuntoProtocolo>();
        }

        [Display(Name = "Tipo de Documento")]
        public int TipoDocumentoId { get; set; }

        [Required]
        [Display(Name = "Assunto")]
        public int AssuntoId { get; set; }

        [Display(Name = "Setor Inicial")]
        public string SetorInicial { get; set; }

        [Display(Name = "Fornecedor")]
        public int? FornecedorId { get; set; }

        public string Fornecedor { get; set; }

        [Display(Name = "Povoado")]
        public int? PovoadoId { get; set; }

        public string Povoado { get; set; }

        [Display(Name = "Requerente")]
        public int RequerenteId { get; set; }

        [Required]
        public string Requerente { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(300)]
        public string Descricao { get; set; }

        public bool ProtocoloSalvo { get; set; }

        public int? ProtocoloId { get; set; }

        public string NumeroProtocolo { get; set; }

        public List<DocumentoAssuntoProtocolo> DocumentosList { get; set; }
    }

    public class DocumentoAssuntoProtocolo
    {
        public DocumentoAssuntoProtocolo()
        {

        }

        public DocumentoAssuntoProtocolo(int _id, string _descricao, bool _obrigatorio)
        {
            Id = _id;
            Descricao = _descricao;
            Obrigatorio = _obrigatorio;
        }

        public int Id { get; set; }

        public string Descricao { get; set; }

        public bool Obrigatorio { get; set; }

        public bool Entregue { get; set; }

    }

    public class SendProtocoloModel
    {
        public SendProtocoloModel()
        {

        }

        public SendProtocoloModel(DateTime _dataEnvio, List<Protocolo> _protocolos)
        {
            DataEnvio = _dataEnvio;
            Protocolos = _protocolos;
        }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Data de Envio")]
        public DateTime DataEnvio { get; set; }

        [Required]
        [Display(Name = "Destino")]
        public int SetorDestinoId { get; set; }

        public List<Protocolo> Protocolos { get; set; }
    }

    // Finalizar, Cancelar e Despachar
    public class ActionProtocoloModel
    {
        public ActionProtocoloModel()
        {

        }

        public ActionProtocoloModel(List<Protocolo> _protocolos)
        {
            Protocolos = _protocolos;
        }

        [Required]
        [Display(Name = "Justificativa")]
        public string DescricaoJustificativa { get; set; }

        public List<Protocolo> Protocolos { get; set; }
    }

    public class DeclineLoteModel
    {
        public DeclineLoteModel()
        {

        }

        public DeclineLoteModel(List<Lote> _lotes)
        {
            Lotes = _lotes;
        }

        [Required]
        [Display(Name = "Justificativa")]
        public string DescricaoJustificativa { get; set; }

        public List<Lote> Lotes { get; set; }
    }

    public class ReceivingProtocoloModel : Selecionavel
    {
        public ReceivingProtocoloModel()
        {

        }

        public ReceivingProtocoloModel(int _numeroLote, int _protocoloId, string _numeroProtocolo, string _secretariaSetorOrigem, DateTime _dataEnvio)
        {
            NumeroLote = _numeroLote;
            ProtocoloId = _protocoloId;
            NumeroProtocolo = _numeroProtocolo;
            SecretariaSetorOrigem = _secretariaSetorOrigem;
            DataEnvio = _dataEnvio;
        }

        [Display(Name = "Número do Lote")]
        public int NumeroLote { get; set; }

        public int ProtocoloId { get; set; }

        [Display(Name = "Número do Protocolo")]
        public string NumeroProtocolo { get; set; }

        [Display(Name = "Secretaria/Setor de Origem")]
        public string SecretariaSetorOrigem { get; private set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Data de Envio")]
        public DateTime DataEnvio { get; private set; }
    }

    public class ReceivedProtocoloModel : Selecionavel
    {
        public ReceivedProtocoloModel()
        {

        }

        public ReceivedProtocoloModel(int _protocoloId, int _numero, int _ano, string _chave, string _assunto, DateTime _dataRecebimento, 
            string _responsavelRecebimento, bool _atrasado)
        {
            ProtocoloId = _protocoloId;
            Numero = _numero;
            Ano = _ano;
            Chave = _chave;
            Assunto = _assunto;
            DataRecebimento = _dataRecebimento;
            ResponsavelRecebimento = _responsavelRecebimento;
            Atrasado = _atrasado;
        }

        public int ProtocoloId { get; set; }

        public int Numero { get; private set; }

        public int Ano { get; private set; }

        public string Chave { get; set; }

        [Display(Name = "Número do Protocolo")]
        public string NumeroAno
        {
            get
            { 
                return Numero + "/" + Ano + "/" + Chave;
            }
        }

        public string Assunto { get; private set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Data de Recebimento")]
        public DateTime DataRecebimento { get; private set; }

        [Display(Name = "Responsável pelo Recebimento")]
        public string ResponsavelRecebimento { get; set; }

        public bool Atrasado { get; set; }

        public int? DiasAtraso { get; set; }

        public int? Prazo { get; set; }

        public string Status { get; set; }
    }

    public class SentProtocoloModal : Selecionavel
    {
        public SentProtocoloModal()
        {

        }

        public SentProtocoloModal(int _numeroLote, string _numeroProtocolo, string _secretariaSetorDestino, string _responsavelEnvio, DateTime _dataEnvio)
        {
            NumeroLote = _numeroLote;
            NumeroProtocolo = _numeroProtocolo;
            SecretariaSetorDestino = _secretariaSetorDestino;
            ResponsavelEnvio = _responsavelEnvio;
            DataEnvio = _dataEnvio;
        }

        [Display(Name = "Número do Lote")]
        public int NumeroLote { get; set; }

        [Display(Name = "Número do Protocolo")]
        public string NumeroProtocolo { get; set; }

        [Display(Name = "Secretaria/Setor de Destino")]
        public string SecretariaSetorDestino { get; private set; }

        [Display(Name = "Responsável pelo Envio")]
        public string ResponsavelEnvio { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Data de Envio")]
        public DateTime DataEnvio { get; private set; }
    }

    public class FinishedProtocoloModel
    {
        public FinishedProtocoloModel()
        {

        }

        public FinishedProtocoloModel(int _protocoloId, int _numero, int _ano, string _chave, string _assunto, DateTime _dataFinalizacao)
        {
            ProtocoloId = _protocoloId;
            Numero = _numero;
            Ano = _ano;
            Chave = _chave;
            Assunto = _assunto;
            DataFinalizacao = _dataFinalizacao;
        }

        public int ProtocoloId { get; set; }

        public int Numero { get; private set; }

        public int Ano { get; private set; }

        public string Chave { get; set; }

        [Display(Name = "Número do Protocolo")]
        public string NumeroAno
        {
            get
            {
                return Numero + "/" + Ano + "/" + Chave;
            }
        }

        public string Assunto { get; private set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Data da Finalização")]
        public DateTime DataFinalizacao { get; private set; }
    }

}