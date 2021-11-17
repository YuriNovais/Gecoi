namespace Protocolo.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_instituicao")]
    public class Instituicao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Razão Social")]
        [StringLength(100)]
        [Column("razao_social")]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(18)]
        [Column("cnpj")]
        public string CNPJ { get; set; }

        [Display(Name = "Inscrição Estadual")]
        [StringLength(14)]
        [Column("inscricao_estadual")]
        public string InscricaoEstadual { get; set; }

        [Required]
        [Display(Name = "Endereço")]
        [StringLength(100)]
        [Column("endereco")]
        public string Endereco { get; set; }

        [StringLength(60)]
        [Column("complemento")]
        public string Complemento { get; set; }

        [StringLength(50)]
        [Column("bairro")]
        public string Bairro { get; set; }

        [StringLength(9)]
        [Column("cep")]
        public string CEP { get; set; }

        [StringLength(14)]
        [Column("telefone")]
        public string Telefone { get; set; }

        [StringLength(14)]
        [Column("fax")]
        public string Fax { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Column("email")]
        public string Email { get; set; }

        [MaxLength(8000)]
        [Column("logomarca")]
        public byte[] Logomarca { get; set; }

        [StringLength(100)]
        [Column("nome_logomarca")]
        public string NomeLogomarca { get; set; }

        [StringLength(100)]
        [Column("tipo_logomarca")]
        public string TipoLogomarca { get; set; }

        [Column("data_inclusao", TypeName = "datetime2")]
        public DateTime DataInclusao { get; set; }

        [Column("data_edicao", TypeName = "datetime2")]
        public DateTime? DataEdicao { get; set; }

        [Column("se_usuario_edicao_id")]
        public int? UsuarioEdicaoId { get; set; }

        [Display(Name = "Município")]
        [Column("ge_municipio_id")]
        public int? MunicipioId { get; set; }

        [Display(Name = "UF")]
        [StringLength(2)]
        [Column("ge_uf_id")]
        public string UFId { get; set; }

        public virtual Usuario UsuarioEdicao { get; set; }

        public virtual UF UF { get; set; }

        public virtual Municipio Municipio { get; set; }
    }
}
