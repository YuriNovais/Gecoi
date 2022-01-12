using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Protocolo.Models
{
    [Table("Atendimento")]
    public class Atendimento
    {

        public Atendimento()
        {
            AtendimentosHistorico = new HashSet<AtendimentoHistorico>();
            AtendimentoAnexo = new HashSet<AtendimentoAnexo>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key]

        [Display(Name = "N° Atendimento")]
        public int Id { get; set; }

        [Required]
        [Column("funcionarioclienteid")]
        [Display(Name = "Solicitante Id")]
        public int FuncionarioClienteId { get; set; }
        public virtual FuncionarioCliente FuncionarioCliente { get; set; }

        [Required]
        [Column("telaid")]
        [Display(Name = "Tela")]
        public int TelaId { get; set; }
        public virtual Tela Tela { get; set; }

        [Required]
        [Column("Motivoid")]
        [Display(Name = "Motivo")]
        public int MotivoId { get; set; }
        public virtual Motivo Motivo { get; set; }


        [Required]
        [AllowHtml]
        [StringLength(1500)]
        [Column("problema")]
        [Display(Name = "Descrição")]
        public string Problema { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_abertura")]
        [Display(Name = "Dt.Abertura")]
        public DateTime data_abertura { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_fechamento")]
        [Display(Name = "Dt.Encerramento")]
        public DateTime data_fechamento { get; set; }

        [NotMapped]
        [AllowHtml]
        [StringLength(1500)]
        [Display(Name = "Histórico/Observação")]
        public string HistoricoObservacao { get; set; }


        [Required]
        [Column("usuarioid")]
        [Display(Name = "Usuário")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Required]
        [Column("StatusAtendimentoId")]
        [Display(Name = "Status")]
        public int StatusAtendimentoId { get; set; }
        public virtual StatusAtendimento StatusAtendimento { get; set; }



        public virtual ICollection<AtendimentoHistorico> AtendimentosHistorico { get; set; }

        public virtual ICollection<AtendimentoAnexo> AtendimentoAnexo { get; set; }
        //public virtual ICollection<Tarefa> Tarefas { get; set; }
       public object AtendimentoHistorico { get; internal set; }

        //DropDowList da index do atendimento
        /*
        
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
        public int? MotivosId { get; set; }
        public string Motivos { get; set; }

        [Display(Name = "Usuario")]
        public int? UsuariosId { get; set; }
        public string Usuarios { get; set; }

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
        */
    }

    public class SearchAtend : SearchGenericReportModel
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


}