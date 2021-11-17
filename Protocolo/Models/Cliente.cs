using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{

    [Table("cliente")]
    public class Cliente
    {
        

       /* public Cliente()
        {
            funcionarioclientes = new HashSet<FuncionarioCliente>();
            Atendimentos = new HashSet<Atendimento>();

        } */

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [Key()]
        [Display(Name = "Id")]
        public Int32 Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("razao_social")]
        [Display(Name = "Razão Social")]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(50)]
        [Column("fantasia")]
        [Display(Name = "Fantasia")]
        public string Fantasia { get; set; }

        [StringLength(1)]
        [Column("tipo_pessoa")]
        [Display(Name = "Tipo Pessoa")]
        public string TipoPessoa { get; set; }

        [Required]
        [StringLength(20)]
        [Column("cnpj")]
        [Display(Name = "CNPJ")]
        public string Cnpj { get; set; }

        [Required]
        [StringLength(50)]
        [Column("endereco")]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [StringLength(30)]
        [Column("complemento")]
        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        [StringLength(40)]
        [Column("bairro")]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        [StringLength(9)]
        [Column("cep")]
        [Display(Name = "Cep")]
        public string Cep { get; set; }

        [StringLength(40)]
        [Column("cidade")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }


        [StringLength(2)]
        [Column("uf")]
        [Display(Name = "Uf")]
        public string Uf { get; set; }

        [StringLength(15)]
        [Column("telefone")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

        [StringLength(50)]
        [Column("email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        
        }
    }
