namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_protocolo_documento")]
    public class DocumentoProtocolo
    {
        [Key]
        [Display(Name = "Protocolo")]
        [Column("pr_protocolo_id", Order = 1)]
        public int ProtocoloId { get; set; }

        [Key]
        [Display(Name = "Documento")]
        [Column("ge_documento_id", Order = 2)]
        public int DocumentoId { get; set; }

        [Column("entregue")]
        public bool Entregue { get; set; }

        public virtual Documento Documento { get; set; }

        public virtual Protocolo Protocolo { get; set; }
    }
}
