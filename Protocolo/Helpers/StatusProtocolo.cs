using System.Collections.Generic;

namespace Protocolo.Helpers
{
    public sealed class StatusProtocolo
    {
        public static readonly StatusProtocolo Aberto = new StatusProtocolo("AB", "Aberto");

        public static readonly StatusProtocolo Encaminhado = new StatusProtocolo("EC", "Encaminhado");

        public static readonly StatusProtocolo Recebido = new StatusProtocolo("RE", "Recebido");

        public static readonly StatusProtocolo Cancelado = new StatusProtocolo("CA", "Cancelado");

        public static readonly StatusProtocolo Finalizado = new StatusProtocolo("FI", "Finalizado");

        private StatusProtocolo(string codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }

        public string Codigo { get; private set; }

        public string Descricao { get; private set; }

        public static string DescricaoFromCodigo(string codigo)
        {
            if (Aberto.Codigo.Equals(codigo))
            {
                return Aberto.Descricao;
            }
            else if (Encaminhado.Codigo.Equals(codigo))
            {
                return Encaminhado.Descricao;
            }
            else if (Recebido.Codigo.Equals(codigo))
            {
                return Recebido.Descricao;
            }
            else if (Cancelado.Codigo.Equals(codigo))
            {
                return Cancelado.Descricao;
            }
            else if (Finalizado.Codigo.Equals(codigo))
            {
                return Finalizado.Descricao;
            }
            else
            {
                return string.Empty;
            }
        }

        public static List<StatusProtocolo> ToOrderedList()
        {
            return new List<StatusProtocolo> {
                Aberto, Cancelado, Encaminhado, Finalizado, Recebido
            };
        }
    }
}