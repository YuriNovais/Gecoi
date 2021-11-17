namespace Protocolo.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ge_secretaria")]
    public class Secretaria
    {
        public Secretaria()
        {
            Setores = new HashSet<Setor>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Nome Abreviado")]
        [StringLength(30)]
        [Column("nome_abreviado")]
        public string NomeAbreviado { get; set; }

        [StringLength(3)]
        [Column("sigla")]
        public string Sigla { get; set; }

        public virtual ICollection<Setor> Setores { get; set; }
    }
}
