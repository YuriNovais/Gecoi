using System;
using System.ComponentModel.DataAnnotations;

namespace Protocolo.Models
{
    public class InstituicaoModel
    {
        public InstituicaoModel()
        {
            
        }

        public InstituicaoModel(Instituicao _instituicao)
        {
            Id = _instituicao.Id;
            Nome = _instituicao.Nome;
            RazaoSocial = _instituicao.RazaoSocial;
            CNPJ = _instituicao.CNPJ;
            InscricaoEstadual = _instituicao.InscricaoEstadual;
            Endereco = _instituicao.Endereco;
            Complemento = _instituicao.Complemento;
            Bairro = _instituicao.Bairro;
            CEP = _instituicao.CEP;
            Telefone = _instituicao.Telefone;
            UFId = _instituicao.UFId;
            MunicipioId = _instituicao.MunicipioId;
            Fax = _instituicao.Fax;
            Email = _instituicao.Email;
            Logomarca = _instituicao.Logomarca;
            NomeLogomarca = _instituicao.NomeLogomarca;
            TipoLogomarca = _instituicao.TipoLogomarca;
            DataInclusao = _instituicao.DataInclusao;
            DataEdicao = _instituicao.DataEdicao;
            UsuarioEdicaoId = _instituicao.UsuarioEdicaoId;
        }

        public int? Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Razão Social")]
        [StringLength(100)]
        public string RazaoSocial { get; set; }

        [Required]
        [StringLength(18)]
        public string CNPJ { get; set; }

        [Display(Name = "Inscrição Estadual")]
        [StringLength(14)]
        public string InscricaoEstadual { get; set; }

        [Required]
        [Display(Name = "Endereço")]
        [StringLength(100)]
        public string Endereco { get; set; }

        [StringLength(60)]
        public string Complemento { get; set; }

        [StringLength(50)]
        public string Bairro { get; set; }

        [StringLength(9)]
        public string CEP { get; set; }

        [Display(Name = "UF")]
        [StringLength(2)]
        public string UFId { get; set; }

        [Display(Name = "Município")]
        public int? MunicipioId { get; set; }

        [StringLength(14)]
        public string Telefone { get; set; }

        [StringLength(14)]
        public string Fax { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [MaxLength(8000)]
        public byte[] Logomarca { get; set; }

        [StringLength(100)]
        public string NomeLogomarca { get; set; }

        [StringLength(100)]
        public string TipoLogomarca { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInclusao { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataEdicao { get; set; }

        public int? UsuarioEdicaoId { get; set; }

        public Instituicao NewInstituicao()
        {
            return new Instituicao
            {
                Nome = this.Nome,
                RazaoSocial = this.RazaoSocial,
                CNPJ = this.CNPJ,
                InscricaoEstadual = this.InscricaoEstadual,
                Endereco = this.Endereco,
                Complemento = this.Complemento,
                Bairro = this.Bairro,
                CEP = this.CEP,
                Telefone = this.Telefone,
                UFId = this.UFId,
                MunicipioId = this.MunicipioId,
                Fax = this.Fax,
                Email = this.Email,
                Logomarca = this.Logomarca,
                NomeLogomarca = this.NomeLogomarca,
                TipoLogomarca = this.TipoLogomarca,
                DataInclusao = this.DataInclusao,
                UsuarioEdicaoId = this.UsuarioEdicaoId
            };
        }

        public Instituicao UpdateInstituicao(Instituicao _instituicao)
        {
            _instituicao.Nome = this.Nome;
            _instituicao.RazaoSocial = this.RazaoSocial;
            _instituicao.CNPJ = this.CNPJ;
            _instituicao.InscricaoEstadual = this.InscricaoEstadual;
            _instituicao.Endereco = this.Endereco;
            _instituicao.Complemento = this.Complemento;
            _instituicao.Bairro = this.Bairro;
            _instituicao.CEP = this.CEP;
            _instituicao.Telefone = this.Telefone;
            _instituicao.UFId = this.UFId;
            _instituicao.MunicipioId = this.MunicipioId;
            _instituicao.Fax = this.Fax;
            _instituicao.Email = this.Email;
            _instituicao.Logomarca = this.Logomarca;
            _instituicao.NomeLogomarca = this.NomeLogomarca;
            _instituicao.TipoLogomarca = this.TipoLogomarca;
            _instituicao.DataInclusao = this.DataInclusao;
            _instituicao.DataEdicao = this.DataEdicao;
            _instituicao.UsuarioEdicaoId = this.UsuarioEdicaoId;

            return _instituicao;
        }
    }
}