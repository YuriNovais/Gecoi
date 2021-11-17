using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Protocolo.Models
{
    [Table("pr_protocolo_anexo")]
    public class ProtocoloAnexo
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("pr_protocolo_id")]
        public int ProtocoloId { get; set; }

        [StringLength(255)]
        [Column("nome_arquivo")]
        public string NomeArquivo { get; set; }

        [StringLength(100)]
        [Column("tipo_arquivo")]
        public string TipoArquivo { get; set; }

        [Column("arquivo")]
        public byte[] Arquivo { get; set; }

        public virtual Protocolo Protocolo { get; set; }
    }
}