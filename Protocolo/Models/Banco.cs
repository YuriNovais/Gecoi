namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_banco")]
    public class Banco
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public short Id { get; set; }

        [Required]
        [StringLength(60)]
        [Column("nome")]
        public string Nome { get; set; }
    }
}
