using System.IO;
using System.Reflection;

namespace Protocolo.SQL
{
    public static class SQLHelper
    {
        public static string LoadSQLStatement(string statementName)
        {
            var sqlStatement = string.Empty;

            var resourceName = typeof(SQLHelper).Namespace + '.' + statementName;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    sqlStatement = new StreamReader(stream).ReadToEnd();
                }
            }

            return sqlStatement;
        }
    }
}