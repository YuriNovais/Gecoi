using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Protocolo.Models

{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("sistema")]
    public class Sistema
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key]
        public Int32 Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [StringLength(5)]
        [Column("sigla")]
        public string Sigla { get; set; }


    }
}