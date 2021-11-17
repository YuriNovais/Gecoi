namespace Protocolo.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_regiao")]
    public class Regiao
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        [Column("descricao")]
        public string Descricao { get; set; }
                
    }
}
