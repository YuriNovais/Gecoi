namespace Protocolo.Helpers
{
    public sealed class StatusHistorico
    {
        public static readonly StatusHistorico Aberto = new StatusHistorico("AB", "Aberto");

        public static readonly StatusHistorico Enviado = new StatusHistorico("EN", "Enviado");

        public static readonly StatusHistorico Recebido = new StatusHistorico("RE", "Recebido");

        public static readonly StatusHistorico Devolvido = new StatusHistorico("DV", "Devolvido");

        public static readonly StatusHistorico Pendente = new StatusHistorico("PD", "Pendente");

        public static readonly StatusHistorico Cancelado = new StatusHistorico("CA", "Cancelado");

        public static readonly StatusHistorico Finalizado = new StatusHistorico("FI", "Finalizado");

        public static readonly StatusHistorico Despachado = new StatusHistorico("DE", "Despachado");

        private StatusHistorico(string codigo, string descricao)
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
            else if (Finalizado.Codigo.Equals(codigo))
            {
                return Finalizado.Descricao;
            }
            else if (Despachado.Codigo.Equals(codigo))
            {
                return Despachado.Descricao;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}