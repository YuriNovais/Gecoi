using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{
    [Table("Tarefa_anexo")]
    public class TarefaAnexo
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("tarefaId")]
        public int TarefaId { get; set; }

        [StringLength(255)]
        [Column("nome_arquivo")]
        public string NomeArquivo { get; set; }

        [StringLength(100)]
        [Column("tipo_arquivo")]
        public string TipoArquivo { get; set; }

        [Column("Arquivo")]
        public byte[] Arquivo { get; set; }




        public virtual Tarefa Tarefa { get; set; }
    }
}