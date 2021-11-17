namespace Protocolo.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("pr_protocolo_fluxo")]
    public class ProtocoloFluxo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("pr_protocolo_id")]
        public int ProtocoloId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column("data_entrada", TypeName = "datetime2")]
        public DateTime DataEntrada { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Column("data_saida", TypeName = "datetime2")]
        public DateTime? DataSaida { get; set; }

        [Column("ge_setor_id")]
        public int SetorId { get; set; }

        [Column("se_usuario_id")]
        public int? UsuarioRecebimentoId { get; set; }

        public int TempoMedio
        {
            get
            {
                int tempoMedio;

                if (DataSaida == null)
                {
                    tempoMedio  = DateTime.Now.Date.Subtract(DataEntrada.Date).Days;
                }
                else
                {
                    tempoMedio = DataSaida.Value.Date.Subtract(DataEntrada.Date).Days;
                }

                return tempoMedio;
            }
        }

        public virtual Protocolo Protocolo { get; set; }

        public virtual Setor Setor { get; set; }

        public virtual Usuario UsuarioRecebimento { get; set; }
    }
}