using System.ComponentModel.DataAnnotations.Schema;

namespace Protocolo.Helpers
{
    public abstract class Selecionavel
    {
        [NotMapped]
        public bool Selecionado { get; set; }
    }
}