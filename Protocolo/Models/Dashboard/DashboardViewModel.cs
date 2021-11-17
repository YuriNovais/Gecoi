using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Protocolo.Models.Dashboard
{
    public class DashboardViewModel
    {

        public DashboardViewModel()
        {

        }

        public DashboardViewModel(int _AtendimentosAbertoDia, 
                                  int _AtendimentosFinalizadosDia, 
                                  int _AtendimentosMes,
                                  int _AtendimentoAbertos,
                                  int _AtendimentoErros,
                                  int _AtendimentoDuvidas,
                                  int _TarefasAbertaMes,
                                  int _TarefasResolvidasMes,
                                  int _TarefasReaberto,
                                  int _TarefasAbertaMais30,
                                  int _TarefasErrosResolvidos,
                                  int _TarefasMelhoriasResolvidas)
        {
            AtendimentosAbertoDia = _AtendimentosAbertoDia;
            AtendimentosFinalizadosDia = _AtendimentosFinalizadosDia;
            AtendimentosMes = _AtendimentosMes;
            AtendimentoAbertos = _AtendimentoAbertos;
            AtendimentoErros = _AtendimentoErros;
            AtendimentoDuvidas = _AtendimentoDuvidas;
            TarefasAbertaMes = _TarefasAbertaMes;
            TarefasResolvidasMes = _TarefasResolvidasMes;
            TarefasReaberto = _TarefasReaberto;
            TarefasReaberto = _TarefasReaberto;
            TarefasAbertaMais30 = _TarefasAbertaMais30;
            TarefasErrosResolvidos = _TarefasErrosResolvidos;
            TarefasMelhoriasResolvidas = _TarefasMelhoriasResolvidas;

        }

        public int AtendimentosAbertoDia { get; set; }
        public int AtendimentosFinalizadosDia { get; set; }
        public int AtendimentosMes { get; set; }
        public int AtendimentoAbertos { get; set; }
        public int AtendimentoErros { get; set; }
        public int AtendimentoDuvidas { get; set; }
        public int TarefasAbertaMes { get; set; }
        public int TarefasResolvidasMes { get; set; }
        public int TarefasReaberto { get; set; }
        public int TarefasAbertaMais30 { get; set; }
        public int TarefasErrosResolvidos { get; set; }
        public int TarefasMelhoriasResolvidas { get; set; }
        
    }
}