using Protocolo.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Protocolo.Models
{
    [Table("funcionariocliente")]
    public class FuncionarioCliente
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        [Key]
        [Display(Name = "Id")]
        public Int32 Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Nome")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [StringLength(50)]
        [Column("email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [Column("ClienteId")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Column("ativo")]
        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }

        public virtual Cliente Cliente { get; set; }


    }       


}