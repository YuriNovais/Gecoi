namespace Protocolo.Models
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_lote")]
    public class Lote
    {
        public Lote()
        {
            Protocolos = new HashSet<Protocolo>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_abertura", TypeName = "datetime2")]
        public DateTime DataAbertura { get; set; }

        [Required]
        [Column("se_usuario_abertura_id")]
        public int UsuarioAberturaId { get; set; }

        [Required]
        [Display(Name = "Setor Origem")]
        [Column("ge_setor_origem_id")]
        public int SetorOrigemId { get; set; }

        [Required]
        [Display(Name = "Setor Destino")]
        [Column("ge_setor_destino_id")]
        public int SetorDestinoId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_recepcao", TypeName = "datetime2")]
        public DateTime? DataRecepcao { get; set; }

        [Column("ge_usuario_recepcao_id")]
        public int? UsuarioRecepcaoId { get; set; }

        [Required]
        [StringLength(2)]
        [Column("status")]
        public string Status { get; set; }

        [NotMapped]
        public string DescricaoStatus
        {
            get
            {
                return StatusLote.DescricaoFromCodigo(Status);
            }
        }

        [NotMapped]
        public Instituicao Instituicao { get; set; }

        public virtual Setor SetorOrigem { get; set; }

        public virtual Setor SetorDestino { get; set; }

        public virtual Usuario UsuarioAbertura { get; set; }

        public virtual Usuario UsuarioRecepcao { get; set; }

        public virtual ICollection<Protocolo> Protocolos { get; set; }
    }
}
