using Protocolo.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Protocolo.Models
{
    public class UsuarioModel
    {
        public UsuarioModel()
        {
            //PerfisList = new List<string>();
        }
        public int? Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Logon { get; set; }

        [Required]
        [StringLength(60)]
        public string Nome { get; set; }

        [StringLength(14)]
        public string Telefone { get; set; }

        [StringLength(14)]
        public string Celular { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; }

        [Display(Name = "Senhas")]
        public string Senha { get; set; }

        [Required]
        [Display(Name = "Perfis")]
        public string Perfil { get; set; }
        //public string PerfisList { get; set; }

        public SelectList Perfis
        {
            get
            {
                return new SelectList(new string[] { "Administrador", "Gestor", "Comum", "Consulta" });
            }/*
            get {
                return new SelectList(new[] {
                    new SelectListItem { Text = Perfil.Administrador, Value = Perfil.Administrador },
                    new SelectListItem { Text = Perfil.Gestor, Value = Perfil.Gestor },
                    new SelectListItem { Text = Perfil.Comum, Value = Perfil.Comum },
                    new SelectListItem { Text = Perfil.Consulta, Value = Perfil.Consulta }
                },
                "Text", 
                "Value"
                );
            }*/
        }

    }

    public class CreateUsuarioModel : UsuarioModel
    {/*
        public CreateUsuarioModel()
        {
            PerfisList = new List<string>();
            //PerfisList = new List( { "Administrador", "Gestor", "Comum", "Consulta" });
        }

        
        public SelectList Perfis
        {
            get
            {
                return new SelectList(new string[] { "Administrador", "Gestor", "Comum", "Consulta" });
            }
        }

        public CreateUsuarioModel(bool _ativo) : this()
        {
            Ativo = _ativo;
        }*/

        public Usuario AsUsuario()
        {
            return new Usuario
            {
                Logon = this.Logon,
                Nome = this.Nome,
                Telefone = this.Telefone,
                Celular = this.Celular,
                Email = this.Email,
                Perfis = this.Perfis.ToString(),
                Ativo = this.Ativo
            };
        }
    }

    public class EditUsuarioModel : UsuarioModel
    {
        public EditUsuarioModel()
        {
            //PerfisList = new List<string>();
        }

        public EditUsuarioModel(Usuario _usuario, List<Setor> _setores) : this()
        {
            Id = _usuario.Id;
            Logon = _usuario.Logon;
            Nome = _usuario.Nome;
            Telefone = _usuario.Telefone;
            Celular = _usuario.Celular;
            Email = _usuario.Email;
            Ativo = _usuario.Ativo;

            if (!string.IsNullOrEmpty(_usuario.Perfis))
            {
                //PerfisList.AddRange(_usuario.Perfis.Split(';'));
            }

        }


    }

    public class UpdatePasswordModel
    {
        public UpdatePasswordModel()
        {

        }

        public UpdatePasswordModel(int _id, string _logon, string _nome)
        {
            Id = _id;
            Logon = _logon;
            Nome = _nome;
        }

        public int Id { get; set; }

        public string Logon { get; set; }

        public string Nome { get; set; }

        [Required]
        [StringLength(32)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }

    public class SetorUsuarioModel : Selecionavel
    {
        public SetorUsuarioModel()
        {

        }

    }

    public class LoginModel
    {
        [Required]
        [StringLength(20)]
        public string Logon { get; set; }

        [Required]
        [StringLength(32)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }

    [Table("se_usuario")]
    public class Usuario
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Column("logon")]
        public string Logon { get; set; }

        [Required]
        [StringLength(60)]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        [Column("senha")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [StringLength(14)]
        [Column("telefone")]
        public string Telefone { get; set; }

        [StringLength(14)]
        [Column("celular")]
        public string Celular { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Column("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Ativo?")]
        [Column("ativo")]
        public bool Ativo { get; set; }

        [StringLength(100)]
        [Column("perfis")]
        [Required]
        public string Perfis { get; set; }

        [NotMapped]
        public SelectList Perfiss
        {
            get
            {
                return new SelectList(new string[] { "Administrador", "Gestor", "Comum", "Consulta" });
            }
        }

        public virtual ICollection<Atendimento> Atendimentos { get; set; }

    }

}