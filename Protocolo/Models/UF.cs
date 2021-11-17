namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_uf")]
    public class UF
    {
        [StringLength(2)]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [StringLength(60)]
        [Column("nome")]
        public string Nome { get; set; }
    }
}
