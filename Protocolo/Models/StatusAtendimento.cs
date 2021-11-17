using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StatusAtendimento")]
    public class StatusAtendimento
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        [Column("descricao")]
        [Display(Name = "Descrição")]
        public string descricao { get; set; }

        [Required]
        [StringLength(2)]
        [Column("sigla")]
        [Display(Name = "sigla")]
        public string sigla { get; set; }

        public virtual ICollection<Atendimento> Atendimentos { get; set; }
        public virtual ICollection<AtendimentoHistorico> AtendimentoHistoricos { get; set; }


    }
 
}