namespace Protocolo.Models
{
    using System.Data.Entity;

    public partial class ProtocoloEntities : DbContext
    {
        public ProtocoloEntities() : base("name=ProtocoloEntities")
        {
        }

        public virtual DbSet<StatusTarefa> StatusTarefas { get; set; }
        public virtual DbSet<StatusAtendimento> StatusAtendimentos { get; set; }
        public virtual DbSet<TarefaAnexo> TarefasAnexo { get; set; }
        public virtual DbSet<TarefaHistorico> TarefasHistorico { get; set; }
        public virtual DbSet<Tarefa> Tarefas { get; set; }
        public virtual DbSet<AtendimentoAnexo> AtendimentosAnexo { get; set; }
        public virtual DbSet<Tela> Telas { get; set; }
        public virtual DbSet<AtendimentoHistorico> AtendimentosHistorico { get; set; }
        public virtual DbSet<Atendimento> Atendimentos { get; set; }
        public virtual DbSet<FuncionarioCliente> FuncionarioClientes { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Motivo> Motivos { get; set; }
        public virtual DbSet<Sistema> Sistemas { get; set; }
        public virtual DbSet<Banco> Bancos { get; set; }
        public virtual DbSet<Classificacao> Classificacoes { get; set; }
        public virtual DbSet<Cnae> Cnaes { get; set; }
        public virtual DbSet<Documento> Documentos { get; set; }
        public virtual DbSet<Instituicao> Intituicoes { get; set; }
        public virtual DbSet<Municipio> Municipios { get; set; }
        public virtual DbSet<Pessoa> Pessoas { get; set; }
        public virtual DbSet<ClassificacaoPessoa> ClassificacoesPessoa { get; set; }
        public virtual DbSet<PessoaFisica> PessoasFisicas { get; set; }
        public virtual DbSet<PessoaJuridica> PessoasJuridicas { get; set; }
        public virtual DbSet<PessoaJuridicaCnae> PessoasJuridicasCnae { get; set; }
        public virtual DbSet<Secretaria> Secretarias { get; set; }
        public virtual DbSet<Regiao> Regioes { get; set; }
        public virtual DbSet<Povoado> Povoados { get; set; }
        public virtual DbSet<Setor> Setores { get; set; }
        public virtual DbSet<UF> UFs { get; set; }
        public virtual DbSet<Assunto> Assuntos { get; set; }
        public virtual DbSet<DocumentoAssunto> AssuntosDocumento { get; set; }
        public virtual DbSet<Lote> Lotes { get; set; }
        public virtual DbSet<Protocolo> Protocolos { get; set; }
        public virtual DbSet<DocumentoProtocolo> DocumentosProtocolo { get; set; }
        public virtual DbSet<ProtocoloHistorico> HistoricoProtocolo { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        //public virtual DbSet<UsuarioSetor> UsuariosSetor { get; set; }
        public virtual DbSet<NumeroProtocolo> NumeroProtocolo { get; set; }
        public virtual DbSet<TipoDocumento> TiposDocumento { get; set; }
        public virtual DbSet<ProtocoloFluxo> FluxoProtocolo { get; set; }
        public virtual DbSet<ProtocoloAnexo> AnexoProtocolo { get; set; }
        public object FuncionarioCliente { get; internal set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instituicao>()
                .Property(e => e.CEP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Instituicao>()
                .Property(e => e.DataInclusao)
                .HasPrecision(6);

            modelBuilder.Entity<Instituicao>()
                .Property(e => e.DataEdicao)
                .HasPrecision(6);

            modelBuilder.Entity<Instituicao>()
                .Property(e => e.UFId)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Municipio>()
                .Property(e => e.UFId)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.TipoPessoa)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.Cep)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.UFId)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.DataCadastro)
                .HasPrecision(6);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.DataEdicao)
                .HasPrecision(6);

            modelBuilder.Entity<Pessoa>()
                .HasMany(p => p.ClassificacoesPessoa)
                .WithRequired(p => p.Pessoa)
                .HasForeignKey(p => p.PessoaId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.ClassificacoesPessoa)
                .WithRequired(e => e.Pessoa)
                .HasForeignKey(e => e.PessoaId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PessoaFisica>()
                .Property(e => e.Sexo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<PessoaJuridica>()
                .HasMany(e => e.PessoaJuridicaCnaeCollection)
                .WithRequired(e => e.PessoaJuridica)
                .HasForeignKey(e => e.PessoaJuridicaId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Secretaria>()
                .Property(e => e.Sigla)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Secretaria>()
                .HasMany(e => e.Setores)
                .WithRequired(e => e.Secretaria)
                .HasForeignKey(e => e.SecretariaId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UF>()
                .Property(e => e.Id)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Assunto>()
                .HasMany(e => e.DocumentosAssunto)
                .WithRequired(e => e.Assunto)
                .HasForeignKey(e => e.AssuntoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lote>()
                .Property(e => e.DataAbertura)
                .HasPrecision(6);

            modelBuilder.Entity<Lote>()
                .Property(e => e.DataRecepcao)
                .HasPrecision(6);

            modelBuilder.Entity<Lote>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Lote>()
                .HasMany(e => e.Protocolos)
                .WithMany()
                .Map(m =>
                {
                    m.MapLeftKey("pr_lote_id");
                    m.MapRightKey("pr_protocolo_id");
                    m.ToTable("pr_lote_protocolo");
                });

            modelBuilder.Entity<Protocolo>()
                .Property(e => e.DataAbertura)
                .HasPrecision(6);

            modelBuilder.Entity<Protocolo>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Protocolo>()
                .Property(e => e.DataStatus)
                .HasPrecision(6);

            modelBuilder.Entity<Protocolo>()
                .Property(e => e.DataEdicao)
                .HasPrecision(6);

            modelBuilder.Entity<Protocolo>()
                .HasMany(e => e.DocumentosProtocolo)
                .WithRequired(e => e.Protocolo)
                .HasForeignKey(e => e.ProtocoloId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Protocolo>()
                .HasMany(e => e.HistoricoProtocolo)
                .WithRequired(e => e.Protocolo)
                .HasForeignKey(e => e.ProtocoloId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProtocoloHistorico>()
                .Property(e => e.DataMovimento)
                .HasPrecision(6);

            modelBuilder.Entity<ProtocoloHistorico>()
                .Property(e => e.Status)
                .IsFixedLength()
                .IsUnicode(false);


            modelBuilder.Entity<ProtocoloFluxo>()
                .Property(e => e.DataEntrada)
                .HasPrecision(6);

            modelBuilder.Entity<ProtocoloFluxo>()
                .Property(e => e.DataSaida)
                .HasPrecision(6);

        }


    }
}