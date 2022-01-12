using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{
    public class AtendimentoModelView
    {
        public Atendimento Atendimento { get; set; }
        public SearchGeneral Filtros { get; set; }
    }
}