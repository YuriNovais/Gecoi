namespace Protocolo.Models
{
    using Helpers;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_protocolo_historico")]
    public class ProtocoloHistorico
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("pr_protocolo_id")]
        public int ProtocoloId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Column("data_movimento", TypeName = "datetime2")]
        public DateTime DataMovimento { get; set; }

        [Required]
        [StringLength(100)]
        [Column("historico")]
        public string Historico { get; set; }

        [Column("se_usuario_id")]
        public int UsuarioId { get; set; }

        [Column("ge_setor_id")]
        public int SetorId { get; set; }

        [Column("ge_setor_destino_id")]
        public int? SetorDestinoId { get; set; }

        [Column("pr_lote_id")]
        public int? LoteId { get; set; }

        [Required]
        [StringLength(2)]
        [Column("status")]
        public string Status { get; set; }

        [NotMapped]
        public string DescricaoStatus
        {
            get
            {
                return StatusHistorico.DescricaoFromCodigo(Status);
            }
        }

        public virtual Protocolo Protocolo { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual Setor Setor { get; set; }

        public virtual Setor SetorDestino { get; set; }

        public virtual Lote Lote { get; set; }
    }
}
