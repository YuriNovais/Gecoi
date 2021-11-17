namespace Protocolo.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_setor")]
    public class Setor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Secretaria")]
        [Column("ge_secretaria_id")]
        public int SecretariaId { get; set; }

        [Required]
        [StringLength(60)]
        [Column("nome")]
        public string Nome { get; set; }

        public virtual Secretaria Secretaria { get; set; }

    }
}
