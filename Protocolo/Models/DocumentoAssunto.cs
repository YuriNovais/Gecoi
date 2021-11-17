namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_assunto_documento")]
    public class DocumentoAssunto
    {
        [Key]
        [Display(Name = "Assunto")]
        [Column("pr_assunto_id", Order = 1)]
        public int AssuntoId { get; set; }

        [Key]
        [Display(Name = "Documento")]
        [Column("ge_documento_id", Order = 2)]
        public int DocumentoId { get; set; }

        [Display(Name = "Obrigatório")]
        [Column("obrigatorio")]
        public bool Obrigatorio { get; set; }

        public virtual Documento Documento { get; set; }

        public virtual Assunto Assunto { get; set; }
    }
}
