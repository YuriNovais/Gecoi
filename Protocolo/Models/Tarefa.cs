using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Protocolo.Models
{
    [Table("Tarefa")]
    public class Tarefa
    {

        public Tarefa()
        {
            TarefasHistorico = new HashSet<TarefaHistorico>();
            TarefaAnexo = new HashSet<TarefaAnexo>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key]
        [Display(Name = "N° Tarefa")]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_abertura")]
        [Display(Name = "Dt.Abertura")]
        public DateTime data_abertura { get; set; }

        [Required]
        [Column("usuarioid")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }


        [Column("atendimentoid")]
        [Display(Name = "Atendimento")]
        public int? AtendimentoId { get; set; }
        public virtual Atendimento Atendimento { get; set; }

        [Required(ErrorMessage = "Insira o motivo")]
        [Column("Motivoid")]
        [Display(Name = "Motivo")]
        public int MotivoId { get; set; }
        public virtual Motivo Motivo { get; set; }

        [Required(ErrorMessage = "Insira a prioridade")]
        [Column("prioridadeid")]
        [Display(Name = "Prioridade")]
        public int PrioridadeId { get; set; }
        public virtual prioridade prioridade { get; set; }
        /*public virtual PrioridadeRequisicao PrioridadeRequisicao { get; set; }*/

        [Required]
        [AllowHtml]
        [StringLength(1500)]
        [Column("descricao_tarefa")]
        [Display(Name = "Descrição")]
        public string Descrição { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_prevista")]
        [Display(Name = "Dt.Prevista")]
        public DateTime data_prevista { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_conclusao")]
        [Display(Name = "Dt.conclusao")]
        public DateTime data_conclusao { get; set; }

        [NotMapped]
        [StringLength(1500)]
        [Display(Name = "Histórico/Observação")]
        public string HistoricoObservacao { get; set; }

        [Required]
        [Column("telaid")]
        [Display(Name = "Tela")]
        public int TelaId { get; set; }
        public virtual Tela Tela { get; set; }

        [Required]
        [Column("funcionarioclienteid")]
        [Display(Name = "Solicitante Id")]
        public int FuncionarioClienteId { get; set; }
        public virtual FuncionarioCliente FuncionarioCliente { get; set; }

        [Required]
        [Column("StatusTarefaid")]
        [Display(Name = "Status")]
        public int StatusTarefaId { get; set; }
        public virtual StatusTarefa StatusTarefa { get; set; }

        [Column("PessoaId")]
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }
        public virtual Usuario Pessoa { get; set; }

        public virtual ICollection<TarefaHistorico> TarefasHistorico { get; set; }
        public virtual ICollection<TarefaAnexo> TarefaAnexo { get; set; }

        public object TarefaHistorico { get; internal set; }


    }
}