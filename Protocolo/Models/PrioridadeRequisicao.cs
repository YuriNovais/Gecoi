using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{
    public class PrioridadeRequisicao
    {

        public int Codigo { get; set; }
        public string descricao { get; set; }

        public List<PrioridadeRequisicao> ListaSituacao()
        {
            return new List<PrioridadeRequisicao>
            {
                new PrioridadeRequisicao { Codigo = 1, descricao="Alta" },
                new PrioridadeRequisicao { Codigo = 2, descricao="Media" },
                new PrioridadeRequisicao { Codigo = 3, descricao="Baixa" },
               

            };

        }
    }
}