using Protocolo.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Protocolo.Models
{
    [Table("ge_pessoa")]
    public class Pessoa
    {
        public Pessoa()
        {
            PessoaFisica = new PessoaFisica();
            PessoaJuridica = new PessoaJuridica();
            Classificacoes = new List<string>();
            ClassificacoesPessoa = new List<ClassificacaoPessoa>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nome/Razão Social")]
        [StringLength(60)]
        [Column("nome_razao")]
        public string NomeRazao { get; set; }

        [Required]
        [Display(Name = "Tipo de Pessoa")]
        [StringLength(1)]
        [Column("tipo_pessoa")]
        public string TipoPessoa { get; set; }

        public string DescricaoTipoPessoa
        {
            get
            {
                return string.IsNullOrEmpty(TipoPessoa) ? string.Empty : TipoPessoa.Equals("F") ? "Física" : "Jurídica";
            }
        }

        [Cnpj]
        [StringLength(18)]
        [Column("cnpj")]
        public string CNPJ { get; set; }

        [Cpf]
        [StringLength(14)]
        [Column("cpf")]
        public string CPF { get; set; }

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
        public string Cep { get; set; }

        [Display(Name = "UF")]
        [StringLength(2)]
        [Column("ge_uf_id")]
        public string UFId { get; set; }

        [Display(Name = "Município")]
        [Column("ge_municipio_id")]
        public int? MunicipioId { get; set; }

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

        [Display(Name = "Banco")]
        [Column("ge_banco_id")]
        public short? BancoId { get; set; }

        [Display(Name = "Agencia Bancária")]
        [StringLength(10)]
        [Column("agencia_bancaria")]
        public string AgenciaBancaria { get; set; }

        [Display(Name = "Conta Bancária")]
        [StringLength(10)]
        [Column("conta_bancaria")]
        public string ContaBancaria { get; set; }

        [StringLength(50)]
        [Column("home_page")]
        public string HomePage { get; set; }

        [Display(Name = "Observação")]
        [StringLength(100)]
        [Column("observacao")]
        public string Observacao { get; set; }

        [Display(Name = "Ativo?")]
        [Column("ativo")]
        public bool Ativo { get; set; }

        [Column("data_cadastro", TypeName = "datetime2")]
        public DateTime DataCadastro { get; set; }

        [Column("se_usuario_cadastro_id")]
        public int UsuarioCadastroId { get; set; }

        [Column("data_edicao", TypeName = "datetime2")]
        public DateTime? DataEdicao { get; set; }

        [Column("se_usuario_edicao_id")]
        public int? UsuarioEdicaoId { get; set; }

        public virtual Banco Banco { get; set; }

        [Display(Name = "Município")]
        public virtual Municipio Municipio { get; set; }

        public virtual Usuario UsuarioCadastro { get; set; }

        public virtual Usuario UsuarioEdicao { get; set; }

        public virtual UF UF { get; set; }

        [NotMapped]
        public virtual PessoaFisica PessoaFisica { get; set; }

        [NotMapped]
        public virtual PessoaJuridica PessoaJuridica { get; set; }

        [NotMapped]
        [Display(Name = "Classificações")]
        public virtual List<string> Classificacoes { get; set; }

        [NotMapped]
        public MultiSelectList ClassificaoSelectList { get; set; }

        [Display(Name = "Classificações")]
        public virtual List<ClassificacaoPessoa> ClassificacoesPessoa { get; set; }
    }

    [Table("ge_pessoa_classificacao")]
    public class ClassificacaoPessoa
    {
        [Key]
        [Display(Name = "Pessoa")]
        [Column("ge_pessoa_id", Order = 1)]
        public int PessoaId { get; set; }

        [Key]
        [Display(Name = "Classificação")]
        [Column("ge_classificacao_id", Order = 2)]
        public int ClassificacaoId { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public virtual Classificacao Classificacao { get; set; }
    }

    [Table("ge_pessoa_fisica")]
    public class PessoaFisica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ge_pessoa_id")]
        public int Id { get; set; }

        [StringLength(20)]
        [Column("rg")]
        public string RG { get; set; }

        [Display(Name = "Órgão de Expedição")]
        [StringLength(20)]
        [Column("orgao_expedicao_rg")]
        public string OrgaoExpedicaoRG { get; set; }

        [Display(Name = "Data de Expedição")]
        [Column("data_expedicao_rg", TypeName = "date")]
        public DateTime? DataExpedicaoRG { get; set; }

        [Display(Name = "Registro Médico")]
        [StringLength(20)]
        [Column("registro_medico")]
        public string RegistroMedico { get; set; }

        [Display(Name = "PIS/PASEP")]
        [StringLength(20)]
        [Column("pis_pasep")]
        public string PisPasep { get; set; }

        [StringLength(20)]
        [Column("cnh")]
        public string CNH { get; set; }

        [Display(Name = "Categoria da CNH")]
        [StringLength(5)]
        [Column("categoria_cnh")]
        public string CategoriaCNH { get; set; }

        [StringLength(1)]
        [Column("sexo")]
        public string Sexo { get; set; }

        [Display(Name = "Data de Nascimento")]
        [Column("data_nascimento", TypeName = "date")]
        public DateTime? DataNascimento { get; set; }
    }

    [Table("ge_pessoa_juridica")]
    public class PessoaJuridica
    {
        public PessoaJuridica()
        {
            PessoaJuridicaCnaeCollection = new HashSet<PessoaJuridicaCnae>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ge_pessoa_id")]
        public int Id { get; set; }

        [Display(Name = "Nome Fantasia")]
        [StringLength(60)]
        [Column("fantasia")]
        public string Fantasia { get; set; }

        [Display(Name = "Inscrição Estadual")]
        [StringLength(14)]
        [Column("inscricao_estadual")]
        public string InscricaoEstadual { get; set; }

        [Display(Name = "Inscrição Municipal")]
        [StringLength(14)]
        [Column("inscricao_municipal")]
        public string InscricaoMunicipal { get; set; }

        [Display(Name = "Nome do Responsável")]
        [StringLength(60)]
        [Column("nome_responsavel")]
        public string NomeResponsavel { get; set; }

        [Display(Name = "CPF do Responsável")]
        [StringLength(14)]
        [Column("cpf_responsavel")]
        public string CPFResponsavel { get; set; }

        public virtual ICollection<PessoaJuridicaCnae> PessoaJuridicaCnaeCollection { get; set; }
    }

    [Table("ge_pessoa_juridica_cnae")]
    public class PessoaJuridicaCnae
    {
        [Key]
        [Column("ge_pessoa_juridica_id", Order = 1)]
        public int PessoaJuridicaId { get; set; }

        [Key]
        [Column("ge_cnae_id", Order = 2)]
        public int CnaeId { get; set; }

        [Column("principal")]
        public bool Principal { get; set; }

        public virtual Cnae Cnae { get; set; }

        public virtual PessoaJuridica PessoaJuridica { get; set; }
    }
}