namespace Protocolo.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_numero_protocolo")]
    public class NumeroProtocolo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ano")]
        public int Ano { get; set; }

        [Column("numero")]
        public int Numero { get; set; }
    }
}