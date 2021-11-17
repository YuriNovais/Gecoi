namespace Protocolo.Models
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_protocolo")]
    public class Protocolo
    {
        public Protocolo()
        {
            DocumentosProtocolo = new HashSet<DocumentoProtocolo>();
            HistoricoProtocolo = new HashSet<ProtocoloHistorico>();
            AnexosProtocolo = new HashSet<ProtocoloAnexo>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("ano")]
        public int Ano { get; set; }

        [Display(Name = "Número")]
        [Column("numero")]
        public int Numero { get; set; }

        [NotMapped]
        [Display(Name = "Número/Ano/Chave")]
        public string NumeroAno
        {
            get { return Numero + "/" + Ano + "/" + Chave; }
        }

        [Display(Name = "Secretaria")]
        [Column("ge_secretaria_id")]
        public int SecretariaId { get; set; }

        [Display(Name = "Fornecedor")]
        [Column("pr_fornecedor_id")]
        public int? FornecedorId { get; set; }

        [NotMapped]
        [Display(Name = "Fornecedor")]
        public string NomeFornecedor { get; set; }

        [Required]
        [Display(Name = "Assunto")]
        [Column("pr_assunto_id")]
        public int AssuntoId { get; set; }

        [Required]
        [Display(Name = "Requerente")]
        [Column("ge_pessoa_id")]
        public int PessoaId { get; set; }

        [Display(Name = "Data de Abertura")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_abertura", TypeName = "datetime2")]
        public DateTime DataAbertura { get; set; }

        [Display(Name = "Usuário de Abertura")]
        [Column("se_usuario_abertura_id")]
        public int UsuarioAberturaId { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(300)]
        [Column("descricao")]
        public string Descricao { get; set; }

        [StringLength(2)]
        [Column("status")]
        public string Status { get; set; }

        [Display(Name = "Data de Alteração de Status")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_status", TypeName = "datetime2")]
        public DateTime DataStatus { get; set; }

        [Display(Name = "Data de Edição")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_edicao", TypeName = "datetime2")]
        public DateTime? DataEdicao { get; set; }

        [Display(Name = "Usuário Edição")]
        [Column("se_usuario_edicao_id")]
        public int? UsuarioEdicaoId { get; set; }

        [Display(Name = "Setor de Abertura")]
        [Column("ge_setor_atual_id")]
        public int SetorAtualId { get; set; }

        [NotMapped]
        [Display(Name = "Povoado")]
        [Column("ge_povoado_id")]
        public int PovoadoId { get; set; }

        [Required]
        [Display(Name = "Tipo de Documento")]
        [Column("pr_tipo_protocolo_id")]
        public int TipoDocumentoId { get; set; }

        [StringLength(4)]
        [Column("chave")]
        public string Chave { get; set; }

        [Display(Name = "Requerente")]
        public virtual Pessoa Pessoa { get; set; }

        [Display(Name = "Secretaria")]
        public virtual Secretaria Secretaria { get; set; }

        [Display(Name = "Fornecedor")]
        public virtual Pessoa Fornecedor { get; set; }

        [Display(Name = "Assunto")]
        public virtual Assunto Assunto { get; set; }

        [Display(Name = "Usuário Abertura")]
        public virtual Usuario UsuarioAbertura { get; set; }

        [Display(Name = "Usuário Edição")]
        public virtual Usuario UsuarioEdicao { get; set; }

        [Display(Name = "Setor")]
        public virtual Setor SetorAtual { get; set; }

        [NotMapped]
        [Display(Name = "Povoado")]
        public virtual Povoado Povoado { get; set; }

        [Display(Name = "Tipo de Documento")]
        public virtual TipoDocumento TipoDocumento { get; set; }

        [NotMapped]
        [StringLength(100)]
        [Display(Name = "Histórico/Observação")]
        public string HistoricoObservacao { get; set; }

        [NotMapped]
        [Display(Name = "Secretaria/Setor")]
        public string SecretariaSetor
        {
            get
            {
                return Secretaria.Nome + "/" + SetorAtual.Nome;
            }
        }

        [NotMapped]
        public string DescricaoStatus
        {
            get
            {
                return StatusProtocolo.DescricaoFromCodigo(Status);
            }
        }

        [NotMapped]
        public Instituicao Instituicao { get; set; }

        public virtual ICollection<DocumentoProtocolo> DocumentosProtocolo { get; set; }

        public virtual ICollection<ProtocoloHistorico> HistoricoProtocolo { get; set; }

        public virtual ICollection<ProtocoloAnexo> AnexosProtocolo { get; set; }
    }
}
