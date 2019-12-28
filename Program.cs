using System;
using System.Data;
using System.Data.SqlClient;

namespace sql_injection
{
    class Program
    {
        static void Main(string[] args)
        {
            RunSafeInjection();
            RunUnSafeInjection();
            Console.Read();
        }

        static void RunSafeInjection()
        {
            RunQueryWithStringBuilder("%spt%");
        }

        static void RunUnSafeInjection()
        {
            var dangerInjection = "%spt%';CREATE TABLE spt_MyNewTable ([Number] int NOT NULL IDENTITY(1,1)); SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%spt%";
            RunQueryWithStringBuilder(dangerInjection);
        }

        static void RunQueryWithStringBuilder(string input)
        {
            var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '{input}'";
            var localDb = new BaseDataAccess($"Data Source=;Initial Catalog=;Persist Security Info=True;User ID=sa;Password=W!nni3_P00h;");
            var results = localDb.ExecuteCommand(query, CommandType.Text, null);
            for (int j = 0; j < results.Tables.Count; j++)
            {
                Console.WriteLine($"{j} table names");
                DataTable table = results.Tables[j];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Console.WriteLine($"{i}. {table.Rows[i]["TABLE_NAME"].ToString()}");
                }
            }
        }
    }

}
