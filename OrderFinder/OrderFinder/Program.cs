using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace OrderFinder
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("\n******************** O R D E R    C O D E    F I N D E R ********************\n\n");

            while (true)
            {
                Console.Write("Enter the GERITM Ticket Number : ");
                var input = Console.ReadLine();
                input = RemoveSpecialCharacters(input);
                var orderCode = GetOrderCode(input.ToUpper().Trim());
                if (!string.IsNullOrEmpty(orderCode))
                {
                    var output = "\nOrderCode = " + orderCode + " for TicketNumber " + input + "\n";
                    Console.WriteLine(output);
                    LogResult(output);
                }
                else
                {
                    Console.WriteLine("\nOrderCode not found.\n");
                }
            }
        }

        private static string GetOrderCode(string ticketNumber)
        {
            var ordercode = "";
            if (string.IsNullOrEmpty(ticketNumber))
                return ordercode;

            var connectionString =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["OrderFinder"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("RetrieveOrderCode", connection)
                    {
                        CommandText =
                            "SELECT TOP 1 [OrderCode] FROM  [ORDERUPLOAD_HEADERCUSTOMFIELDS] WHERE [CustomFieldValue] = \'" +
                            ticketNumber + "\' ORDER BY [CustomFieldOrderAssignmentID] DESC",
                        CommandType = CommandType.Text
                    };
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ordercode = reader.GetString(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex = ex;
                    throw;
                }

                return ordercode;
            }
        }

        private static void LogResult(string output)
        {
            var systemPath = System.Environment.
                GetFolderPath(
                    Environment.SpecialFolder.CommonApplicationData
                );
            //var completePath = Path.Combine(systemPath, "OrderCodesByTicketNumber.txt");
            var completePath = Environment.CurrentDirectory + "\\OrderCodesByTicketNumber.txt";
            if (!File.Exists(completePath))
            {
                // Create a file to write to.
                File.WriteAllText(completePath, output);
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            File.AppendAllText(completePath, output + Environment.NewLine);

        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') )
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}