namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_tipo_protocolo")]
    public class TipoDocumento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(80)]
        [Column("descricao")]
        public string Descricao { get; set; }
    }
}