using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Protocolo.Models

{
    public class BaseContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public BaseContext() : base("BD_GECOIWEB") { }
        public DbSet<Modelo> modelos { get; set; }
        public DbSet<Carro> carros { get; set; }
    }
}