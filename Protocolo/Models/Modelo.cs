using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Protocolo.Models
{
    [Table("Modelo")]
    public class Modelo
    {
        [Key()]
        public int Id { get; set; }
        public string Nome { get; set; }

        public void Salvar()
        {
            var db = new BaseContext();
            db.modelos.Add(this);
            db.SaveChanges();
        }

        public List<Modelo> SelecionarTodos()
        {
            var db = new BaseContext();
            db.modelos.Add(this);
            return db.modelos.ToList();
        }
    }
}