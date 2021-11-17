using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StatusTarefa")]
    public class StatusTarefa
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

        public virtual ICollection<Tarefa> Tarefas { get; set; }
        public virtual ICollection<TarefaHistorico> TarefaHistoricos { get; set; }
    }
}