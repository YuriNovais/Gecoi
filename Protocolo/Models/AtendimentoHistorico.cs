using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Protocolo.Helpers;
using System.Web.Mvc;

namespace Protocolo.Models
{
    [Table("Atendimento_historico")]
    public class AtendimentoHistorico
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("AtendimentoId")]
        [Display(Name = "AtendimentoId")]
        public int AtendimentoId { get; set; }
        public virtual Atendimento Atendimento { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy hh:mm:ss}")]
        [Column("datahistorico")]
        [Display(Name = "Data ")]
        public DateTime Datahistorico { get; set; }

        [Required]
        [AllowHtml]
        [StringLength(500)]
        [Column("solucao")]
        [Display(Name = "Descrição")]
        public string Solucao { get; set; }

        [Required]
        [Column("usuarioid")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Required]
        [Column("StatusAtendimentoId")]
        [Display(Name = "Status")]
        public int StatusAtendimentoId { get; set; }
        public virtual StatusAtendimento StatusAtendimento { get; set; }

      //  public virtual ICollection<StatusAtendimento> StatusAtendimentos { get; set; }






    }

}