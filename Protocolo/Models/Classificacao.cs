namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_classificacao")]
    public class Classificacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(50)]
        [Column("descricao")]
        public string Descricao { get; set; }
    }
}
