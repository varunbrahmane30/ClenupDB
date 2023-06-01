using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace PerodicallyDBClean
{
    class Program
    {
        static string ConnectionString { get; set; }
        static void Main(string[] args)
        {
            try
            {
                ConnectionString = GetConnectionStringByName("FLCADDB");
                Run();
            }
            finally
            {
                Console.WriteLine("stopped ...");

            }
        }

        private static void Run()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();

                    var cmd = new SqlCommand(ConfigurationManager.AppSettings["FLCEventScript"], conn)
                    {
                        CommandType = CommandType.Text
                    };

                    var reader = cmd.ExecuteReader();

                    //Console.WriteLine("Query Executed Sucessfully, " + reader.Read());
              
                    var cmd2 = new SqlCommand(ConfigurationManager.AppSettings["FLCModuleInstanceScript"], conn)
                    {
                        CommandType = CommandType.Text
                    };

                    var reader2 = cmd2.ExecuteReader();

                    var cmd3 = new SqlCommand(ConfigurationManager.AppSettings["FLCLogfilesScript"], conn)
                    {
                        CommandType = CommandType.Text
                    };

                    var reader3 = cmd3.ExecuteReader();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    LogException(ex);
                }
            }

        }

        public static string GetConnectionStringByName(string dbName)
        {
            string returnValue = null;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[dbName];

            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }

        private static void LogException(Exception ex)
        {
            string filePath = ConfigurationManager.AppSettings["ErrorLogPath"];

            if (File.Exists(filePath) == false)
            {
                FileStream read = System.IO.File.Create(filePath);

                read.Close();
            }

            using (StreamWriter writer = new StreamWriter(File.Open(filePath, FileMode.Append)))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString() + "\n");

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    ex = ex.InnerException;
                }
            }
        }

    }
}

