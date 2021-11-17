using System.Collections.Generic;
using System.Linq;
using Protocolo.Models;
using System.Data.SqlClient;

namespace Protocolo.Services
{
    public class SQLService
    {

        public List<T> ExecuteSQLStatement<T>(string statement, params SqlParameter[] parameters)
        {
            using (var context = new BaseContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                return context.Database.SqlQuery<T>(statement, parameters).ToList();
            }
        }

        public List<T> ExecuteSQLStatementSP<T>(string statement)
        {
            using (var context = new BaseContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                return context.Database.SqlQuery<T>(statement).ToList();
            }
        }
        public T ExecuteSingleSQLStatement<T>(string statement, params SqlParameter[] parameters)
        {
            using (var context = new BaseContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                return context.Database.SqlQuery<T>(statement, parameters).SingleOrDefault();
            }
        }
    }
}
