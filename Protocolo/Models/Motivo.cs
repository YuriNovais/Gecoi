using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("motivo")]
    public class Motivo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key]
        public Int32 id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("descricao")]
        [Display(Name ="Descrição")]
        public string descricao { get; set; }

        public virtual ICollection<Atendimento> Atendimentos { get; set; }

    }
}