using System.ComponentModel.DataAnnotations;

namespace Protocolo.Models
{
    public class DashboardModel
    {
        public DashboardModel()
        {

        }

        public DashboardModel(int _totalSetor, int _totalRecebidos, int _totalReceber, int _totalEnviados, int _totalAtrasadosSetor, int _totalFinalizados)
        {
            TotalSetor = _totalSetor;
            TotalRecebidos = _totalRecebidos;
            TotalReceber = _totalReceber;
            TotalEnviados = _totalEnviados;
            TotalAtrasadosSetor = _totalAtrasadosSetor;
            TotalFinalizados = _totalFinalizados;
        }

        [Display(Name = "Processos no Setor")]
        public int TotalSetor { get; set; }

        [Display(Name = "Processos Recebidos")]
        public int TotalRecebidos { get; set; }

        [Display(Name = "Lotes a Receber")]
        public int TotalReceber { get; set; }

        [Display(Name = "Lotes Enviados")]
        public int TotalEnviados { get; set; }

        [Display(Name = "Processos Atrasados no Setor")]
        public int TotalAtrasadosSetor { get; set; }

        [Display(Name = "Processos Finalizados")]
        public int TotalFinalizados { get; set; }

        public int TotalGeral {
            get
            {
                return TotalSetor + TotalReceber + TotalRecebidos + TotalEnviados + TotalAtrasadosSetor + TotalFinalizados;
            }
        }
    }
}