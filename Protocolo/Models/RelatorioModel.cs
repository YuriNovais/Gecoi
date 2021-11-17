using Protocolo.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Protocolo.Models
{
    public class SearchGenericReportModel
    {
       /* [Display(Name = "Tipo")]
        public int? TipoDocumentoId { get; set; }

        public string TipoDocumento { get; set; }

        [Display(Name = "Setor")]
        public int? SetorId { get; set; }

        public string Setor { get; set; }

        [Display(Name = "Assunto")]
        public int? AssuntoId { get; set; }


        public string Assunto { get; set; }

        [Display(Name = "Situação")]
        public string Status { get; set; }

        public string DescricaoStatus { get; set; }*/

        [Display(Name = "Sistema")]
        public int? SistemaId { get; set; }
        public string Sistema { get; set; }


        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }
        public string Cliente { get; set; }


        [Display(Name = "Solicitante")]
        public int? SolicitanteId { get; set; }
        public string Solicitante { get; set; }

        [Display(Name = "Motivo")]
        public int? MotivoId { get; set; }
        public string Motivo { get; set; }

        [Display(Name = "Usuario")]
        public int? UsuarioId { get; set; }
        public string Usuario { get; set; }

        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        public string Status { get; set; }

        [Display(Name = "Situacao")]
        public int? SituacaoId { get; set; }
        public string Situacao { get; set; }

        [Display(Name = "Data inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataInicio { get; set; }

        [Display(Name = "Data Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataFim { get; set; }

        public bool Atrasados { get; set; }

        public Instituicao Instituicao { get; set; }

    }

    public class SearchSubject : SearchGenericReportModel
    {
        public List<Group<Cliente, Atendimento>> GroupedResult { get; set; }

        public int Total
        {
            get {
                if (GroupedResult == null)
                {
                    return 0;
                }
                else
                {
                    int total = 0;

                    foreach(var group in GroupedResult)
                    {
                        total += group.Quantidade;
                    }

                    return total;
                }
            }
        }
    }

    // Tarefa
    public class SearchSubjects : SearchGenericReportModel
    {
        public List<Group<Cliente, Tarefa>> GroupedResult { get; set; }

        public int Total
        {
            get
            {
                if (GroupedResult == null)
                {
                    return 0;
                }
                else
                {
                    int total = 0;

                    foreach (var group in GroupedResult)
                    {
                        total += group.Quantidade;
                    }

                    return total;
                }
            }
        }
    }

    // atendimento
    public class SearchGeneral : SearchGenericReportModel
    {
        public List<Atendimento> Result { get; set; }

        public int Total
        {
            get
            {
                if (Result == null)
                {
                    return 0;
                }
                else
                {
                    return Result.Count;
                }
            }
        }
    }

    public class SearchGeneralTarefa : SearchGenericReportModel
    {
        public List<Tarefa> Result { get; set; }

        public int Total
        {
            get
            {
                if (Result == null)
                {
                    return 0;
                }
                else
                {
                    return Result.Count;
                }
            }
        }
    }

    
    // atendimento
    public class SearchSistema : SearchGenericReportModel
    {
        public List<Group<Sistema, Atendimento>> GroupedResult { get; set; }


        public int Total
        {
            get
            {
                if (GroupedResult == null)
                {
                    return 0;
                }
                else
                {
                    int total = 0;

                    foreach (var group in GroupedResult)
                    {
                        total += group.Quantidade;
                    }

                    return total;
                }
            }
        }
    }

    // tarefa
    public class SearchSistemas: SearchGenericReportModel
    {
        public List<Group<Sistema, Tarefa>> GroupedResult { get; set; }


        public int Total
        {
            get
            {
                if (GroupedResult == null)
                {
                    return 0;
                }
                else
                {
                    int total = 0;

                    foreach (var group in GroupedResult)
                    {
                        total += group.Quantidade;
                    }

                    return total;
                }
            }
        }
    }


    public class SearchSectorYear : SearchGenericReportModel
    {
        [Required]
        public int Ano { get; set; }

        public Dictionary<FuncionarioCliente, Dictionary<int, int>> Result { get; set; }

        public Dictionary<FuncionarioCliente, int> TotalGeralPorSetor { get; set; }

        public Dictionary<int, int> TotalGeralPorMes { get; set; }

        public int TotalGeral { get; set; }
    }


    // Atendimento
    public class SearchFuncionarioCliente : SearchGenericReportModel
    {

        public List<Group<FuncionarioCliente, Atendimento>> GroupedResult { get; set; }

        public int Total
        {
            get
            {
                if (GroupedResult == null)
                {
                    return 0;
                }
                else
                {
                    int total = 0;

                    foreach (var group in GroupedResult)
                    {
                        total += group.Quantidade;
                    }

                    return total;
                }
            }
        }
    }

    // Tarefa
    public class SearchFuncionarioClientes : SearchGenericReportModel
    {

        public List<Group<FuncionarioCliente, Tarefa>> GroupedResult { get; set; }

        public int Total
        {
            get
            {
                if (GroupedResult == null)
                {
                    return 0;
                }
                else
                {
                    int total = 0;

                    foreach (var group in GroupedResult)
                    {
                        total += group.Quantidade;
                    }

                    return total;
                }
            }
        }
    }




    public class SearchFlow
    {
        [Required]
        public int? Numero { get; set; }

        [Required]
        public int? Ano { get; set; }

        [Display(Name = "N° Processo")]
        public string NumeroAno { get; set; }

        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; }

        public string Assunto { get; set; }

        public string Requerente { get; set; }

        public Instituicao Instituicao { get; set; }
        public List<ProtocoloFluxo> FluxoProtocolo { get; set; }
    }
}