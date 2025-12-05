using System;
using MySql.Data.MySqlClient;

class TestConnection
{
    static void Main()
    {
        // Test connection string exactly as used in FinalPOS
        string connectionString = "Server=localhost;Port=3307;Database=POS_NEXA_ERP;Uid=root;Pwd=Shivaanica;AllowPublicKeyRetrieval=True;";
        
        Console.WriteLine("Testing MySQL Connection...");
        Console.WriteLine($"Connection String: {connectionString}");
        Console.WriteLine();
        
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                Console.WriteLine("Attempting to connect...");
                connection.Open();
                Console.WriteLine("✅ SUCCESS! Connected to MySQL!");
                
                using (var command = new MySqlCommand("SELECT VERSION()", connection))
                {
                    var version = command.ExecuteScalar();
                    Console.WriteLine($"MySQL Version: {version}");
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"❌ MySQL Error #{ex.Number}: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

