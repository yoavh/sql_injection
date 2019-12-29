using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace sql_injection
{
    class Program
    {
        static void Main(string[] args)
        {
            RunSafeInjectionUsingStringBuilder();
            RunUnSafeInjectionUsingStringBuilder();
            RunUnSafeInjectionUsingSqlParameter();
            RunSafeInjectionUsingSqlParameter();
            Console.Read();
        }

        static void RunSafeInjectionUsingStringBuilder()
        {
            RunQueryUsingStringBuilder("'%spt%'");
        }
        static void RunSafeInjectionUsingSqlParameter()
        {
            RunQueryUsingSqlParameter("%spt%");
        }

        static void RunUnSafeInjectionUsingStringBuilder()
        {
            var dangerInjection = "'%spt%';CREATE TABLE spt_MyNewTable ([Number] int NOT NULL IDENTITY(1,1)); SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%spt%';DROP TABLE spt_MyNewTable;";
            RunQueryUsingStringBuilder(dangerInjection);
        }
        static void RunUnSafeInjectionUsingSqlParameter()
        {
            var dangerInjection = "'%spt%';CREATE TABLE spt_MyNewTable ([Number] int NOT NULL IDENTITY(1,1)); SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE '%spt%';DROP TABLE spt_MyNewTable;";
            RunQueryUsingSqlParameter(dangerInjection);
        }

        static void RunQueryUsingStringBuilder(string input)
        {
            var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE {input}";
            var localDb = new BaseDataAccess($"Data Source=;Initial Catalog=;Persist Security Info=True;User ID=sa;Password=W!nni3_P00h;");
            var results = localDb.ExecuteCommand(query, CommandType.Text, new List<SqlParameter>());
            PrintDataSet(results);
        }

        static void RunQueryUsingSqlParameter(string input)
        {
            var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE @@table_name";
            var localDb = new BaseDataAccess($"Data Source=;Initial Catalog=;Persist Security Info=True;User ID=sa;Password=W!nni3_P00h;");
            var sqlParameter = localDb.GetParameter("@@table_name", input);
            var results = localDb.ExecuteCommand(query, CommandType.Text, new[] { sqlParameter });
            PrintDataSet(results);
        }
        static void PrintDataSet(DataSet dataSet)
        {
            for (int j = 0; j < dataSet.Tables.Count; j++)
            {
                Console.WriteLine($"{j}. {dataSet.Tables[j].TableName}");
                DataTable table = dataSet.Tables[j];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Console.WriteLine($"\t{j}.{i}. {table.Rows[i]["TABLE_NAME"].ToString()}");
                }
            }
        }
    }

}
