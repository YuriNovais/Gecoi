

namespace Protocolo.Models
{
    using Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Atendimento_anexo")]
    public class AtendimentoAnexo
    {
        [Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("atendimento_id")]
        public int AtendimentoId { get; set; }

        [StringLength(255)]
        [Column("nome_arquivo")]
        public string NomeArquivo { get; set; }

        [StringLength(100)]
        [Column("tipo_arquivo")]
        public string TipoArquivo { get; set; }

        [Column("arquivo")]
        public byte[] Arquivo { get; set; }

        public virtual Atendimento Atendimento  { get; set; }
    }
}