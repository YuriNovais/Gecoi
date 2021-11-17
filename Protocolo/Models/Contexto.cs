using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace Protocolo.Models
{
    public class Contexto: DbContext
    {
        public DbSet<Sistema> Sistemas { get; set; }
    }
}