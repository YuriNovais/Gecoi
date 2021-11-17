namespace Protocolo.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_povoado")]
    public class Povoado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Regiao")]
        [Column("ge_regiao_id")]
        public int RegiaoId { get; set; }

        [Required]
        [StringLength(60)]
        [Column("descricao")]
        public string Descricao { get; set; }

        public virtual Regiao Regiao { get; set; }

    }
}
