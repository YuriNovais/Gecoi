namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_cnae")]
    public class Cnae
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        [StringLength(10)]
        [Column("codigo")]
        public string Codigo { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(200)]
        [Column("descricao")]
        public string Descricao { get; set; }
    }
}
