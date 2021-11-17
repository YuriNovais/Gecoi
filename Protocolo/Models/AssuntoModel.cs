using Protocolo.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Protocolo.Models
{
    public class AssuntoModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(100)]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Prazo (em dias)")]
        public short? Prazo { get; set; }

        public List<AssuntoDocumentoModel> AssuntoDocumentoList { get; set; }
    }

    public class CreateAssuntoModel : AssuntoModel
    {
        public CreateAssuntoModel()
        {
            AssuntoDocumentoList = new List<AssuntoDocumentoModel>();
        }

        public CreateAssuntoModel(List<Documento> _documentos) : this()
        {
            _documentos.ForEach(d => AssuntoDocumentoList.Add(AssuntoDocumentoModel.FromDocumento(d)));
        }

        public Assunto AsAssunto()
        {
            return new Assunto
            {
                Descricao = this.Descricao,
                Prazo = this.Prazo
            };
        }
    }

    public class EditAssuntoModel : AssuntoModel
    {
        public EditAssuntoModel()
        {
            AssuntoDocumentoList = new List<AssuntoDocumentoModel>();
        }

        public EditAssuntoModel(Assunto _assunto, List<Documento> _documentos) : this()
        {
            Id = _assunto.Id;
            Descricao = _assunto.Descricao;
            Prazo = _assunto.Prazo;

            FillAssuntoDocumentoList(_assunto.DocumentosAssunto, _documentos);
        }

        private void FillAssuntoDocumentoList(ICollection<DocumentoAssunto> _documentoAssuntoList, List<Documento> _documentos)
        {
            foreach(var documentoAssunto in _documentoAssuntoList)
            {
                AssuntoDocumentoList.Add(new AssuntoDocumentoModel
                {
                    Id = documentoAssunto.DocumentoId,
                    Descricao = documentoAssunto.Documento.Descricao,
                    Obrigatorio = documentoAssunto.Obrigatorio,
                    Selecionado = true
                });
            }

            _documentos.ForEach(d =>
            {
                if (!AssuntoDocumentoList.Exists(ad => ad.Id == d.Id))
                {
                    AssuntoDocumentoList.Add(AssuntoDocumentoModel.FromDocumento(d));
                }
            });

            AssuntoDocumentoList.Sort((x, y) => string.Compare(x.Descricao, y.Descricao));
        }
    }

    public class AssuntoDocumentoModel : Selecionavel
    {
        public AssuntoDocumentoModel()
        {

        }

        private AssuntoDocumentoModel(int _id, string _descricao)
        {
            Id = _id;
            Descricao = _descricao;
        }

        public int Id { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Obrigatório")]
        public bool Obrigatorio { get; set; }

        public static AssuntoDocumentoModel FromDocumento(Documento _documento)
        {
            return new AssuntoDocumentoModel(_documento.Id, _documento.Descricao);
        }
    }

    [Table("pr_assunto")]
    public class Assunto
    {
        public Assunto()
        {
            DocumentosAssunto = new HashSet<DocumentoAssunto>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        [StringLength(100)]
        [Column("descricao")]
        public string Descricao { get; set; }

        [Display(Name = "Prazo (em dias)")]
        [Column("prazo")]
        public short? Prazo { get; set; }

        public virtual ICollection<DocumentoAssunto> DocumentosAssunto { get; set; }
    }
}