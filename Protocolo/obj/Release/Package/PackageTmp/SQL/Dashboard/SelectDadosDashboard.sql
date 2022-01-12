select * from
(select COUNT(*) as Atendimento_Abertos_dia FROM Atendimento where DAY(data_abertura) = DAY(GETDATE()) and StatusAtendimentoid in (1)) q1,
(select COUNT(*) as Atendimentos_Finalizados_dia from Atendimento where MONTH(data_fechamento) = MONTH(GETDATE()) and statusatendimentoid in (5)) q2,
(select COUNT(*) as Atendimentos_mes from Atendimento  where MONTH(data_fechamento) = MONTH(GETDATE()) ) q3,
(select COUNT(*) as Atendimento_Abertos FROM Atendimento where StatusAtendimentoid in (1)) q4,
(select COUNT(*) as Atendimento_Erros FROM Atendimento where Motivoid in (19) and MONTH(data_abertura) = MONTH(GETDATE())) q5,
(select COUNT(*) as Atendimento_Duvidas FROM Atendimento where Motivoid in (39) and MONTH(data_abertura) = MONTH(GETDATE())) q6,
(select COUNT(*) as Tarefas_Aberta_mes from Tarefa where MONTH(data_abertura) = MONTH(GETDATE()) and StatusTarefaid in (1)) q7,
(select COUNT(*) as Tarefas_Resolvidas_mes from Tarefa where MONTH(data_conclusao) = MONTH(GETDATE()) and StatusTarefaid in (10)) q8,
(select COUNT(*) as Tarefas_Reaberto from Tarefa where StatusTarefaid in (11)) q9,
(select COUNT(*) as Tarefas_Aberta_Mais30 from Tarefa where data_abertura <= GETDATE()-30 and StatusTarefaid in (1)) q10,
(select COUNT(*) as Tarefas_Erros_Resolvidos from Tarefa where motivoid in (3) and month(data_conclusao) = Month(GETDATE()) and StatusTarefaid in (10)) q11,
(select COUNT(*) as Tarefas_Melhorias_Resolvidas from Tarefa where motivoid in (12) and month(data_conclusao) = Month(GETDATE()) and StatusTarefaid in (10)) q12
