using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Protocolo.Models
{
    [Table("Tarefa_historico")]
    public class TarefaHistorico
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("Tarefaid")]
        [Display(Name = "Tarefa_id")]
        public int TarefaId { get; set; }
        public virtual Tarefa Tarefa { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy hh:mm}")]
        [Column("data_historico")]
        [Display(Name = "Data ")]
        public DateTime Datahistorico { get; set; }

        [Required(ErrorMessage = "Insira uma descrição, para gravar o historico.")]
        [AllowHtml]
        [StringLength(1500)]
        [Column("descricao")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Column("usuarioid")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Required(ErrorMessage = "Insira o historico.")]
        [Column("StatusTarefaid")]
        [Display(Name = "Status")]
        public int StatusTarefaId { get; set; }
        public virtual StatusTarefa StatusTarefa { get; set; }

        [Required]
        [Column("PessoaId")]
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }
        public virtual Usuario Pessoa { get; set; }

    }
}