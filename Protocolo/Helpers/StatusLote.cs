namespace Protocolo.Helpers
{
    public sealed class StatusLote
    {
        public static readonly StatusLote Aberto = new StatusLote("AB", "Aberto");

        public static readonly StatusLote Enviado = new StatusLote("EN", "Enviado");

        public static readonly StatusLote Recebido = new StatusLote("RE", "Recebido");

        public static readonly StatusLote Devolvido = new StatusLote("DV", "Devolvido");

        public static readonly StatusLote Pendente = new StatusLote("PD", "Pendente");

        public static readonly StatusLote Cancelado = new StatusLote("CA", "Cancelado");

        private StatusLote(string codigo, string descricao)
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
            else if (Enviado.Codigo.Equals(codigo))
            {
                return Enviado.Descricao;
            }
            else if (Recebido.Codigo.Equals(codigo))
            {
                return Recebido.Descricao;
            }
            else if (Devolvido.Codigo.Equals(codigo))
            {
                return Devolvido.Descricao;
            }
            else if (Pendente.Codigo.Equals(codigo))
            {
                return Pendente.Descricao;
            }
            else if (Cancelado.Codigo.Equals(codigo))
            {
                return Cancelado.Descricao;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}