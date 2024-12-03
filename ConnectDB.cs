using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuroApp
{
    public class ConnectDB
    {
        private readonly static string serverName = "127.0.0.1";
        private readonly static string port = "5432";
        private readonly static string userName = "postgres";
        private readonly static string password = "Sivec@20";
        private readonly static string databaseName = "NeuroApp";
        public NpgsqlConnection npgsqlConnection = null;
        public string connString = null;

        public ConnectDB()
        {
            connString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                                            serverName, port, userName, password, databaseName);
        }
    }

    public class DatabaseActions : ConnectDB
    {
        public bool userLogin(User user)
        {
            bool connected = false;

            try
            {
                using (NpgsqlConnection npgsqlConnection = new(connString))
                {
                    npgsqlConnection.Open();

                    using (var cmd = new NpgsqlCommand($"SELECT userName, passkey FROM users WHERE userName = @name AND passkey = @passkey", npgsqlConnection))
                    {
                        cmd.Parameters.AddWithValue("name", user.UserName);
                        cmd.Parameters.AddWithValue("passkey",user.Password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            connected = reader.Read();
                        }
                    }
                }

                return connected;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }

            return false;
        }

        public bool BuildOS(ServiceOrder serviceOrder)
        {
            try
            {
                using NpgsqlConnection npgsqlConnection = new(connString);
                npgsqlConnection.Open();

                using var cmd = new NpgsqlCommand(
                    $"INSERT INTO ServiceOrder(customer,numeroos,arrivaldate,isguarantee,status,observation) VALUES (@cliente,@numeroOs,@dataChegada,@garantia,@status,@observacao)",
                    npgsqlConnection
                );

                serviceOrder.Status_ = serviceOrder.IsGuarantee
                    ? NeuroApp.ServiceOrder.Status.budgetApproved
                    : NeuroApp.ServiceOrder.Status.waitingBudget;

                cmd.Parameters.AddWithValue("cliente", serviceOrder.Customer);
                cmd.Parameters.AddWithValue("numeroOs", serviceOrder.OsNumber);
                cmd.Parameters.AddWithValue("dataChegada", serviceOrder.ArrivalDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("observacao", (object?)serviceOrder.Observation ?? DBNull.Value);
                cmd.Parameters.AddWithValue("garantia", NpgsqlTypes.NpgsqlDbType.Boolean).Value = serviceOrder.IsGuarantee;
                cmd.Parameters.AddWithValue("status", (int)serviceOrder.Status_);

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return false;
            }
        }

        public DataTable SearchOs()
        {
            DataTable dt = new();

            try
            {
                using (npgsqlConnection = new NpgsqlConnection(connString))
                    npgsqlConnection.Open();

                    string cmdSelect = "SELECT * FROM serviceorder ORDER BY numeroos DESC ";

                    using (NpgsqlDataAdapter Adpt = new NpgsqlDataAdapter(cmdSelect, npgsqlConnection))
                        Adpt.Fill(dt);

                    npgsqlConnection.Close();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return dt;
        }

       /* public DataTable SearchOsByFilter(string customer, int? osNumber)
        {
            DataTable dt = new();

            try
            {
                using (npgsqlConnection = new NpgsqlConnection(connString))
                {
                    npgsqlConnection.Open();

                    string cmdSelect = "SELECT * FROM serviceorder WHERE 1=1";

                    if (!string.IsNullOrEmpty(customer))
                        cmdSelect += " AND customer = @customer";
                    if (osNumber.HasValue)
                        

                }


            }
            catch (NpgsqlException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }*/


    }
}
