using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{
    [Table("tela")]
    public class Tela
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key()]
        [Display(Name = "Id")]
        public Int32 Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("descricao")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Column("SistemaId")]
        [Display(Name = "Sistema")]
        public int SistemaId { get; set; }
        public virtual Sistema Sistema { get; set; }

       // public virtual Atendimento Atendimentos { get; set; }

    }
}