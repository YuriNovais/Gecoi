using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Protocolo.Models
{

    [Table("prioridade")]
    public class prioridade
    {
        
       
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")]
            [Key()]
            [Display(Name = "Id")]
            public Int32 Id { get; set; }

            [StringLength(50)]
            [AllowHtml]
            [Column("descricao")]
            [Display(Name = "Descrição")]
            public string Descricao { get; set; }

        
    }
}