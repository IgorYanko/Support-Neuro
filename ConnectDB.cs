using NeuroApp.Classes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json.Serialization;
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
        public (bool isAuthenticated, string userRole) userLogin(User user)
        {
            string userRole = null;

            try
            {
                using (NpgsqlConnection npgsqlConnection = new(connString))
                {
                    npgsqlConnection.Open();

                    using (var cmd = new NpgsqlCommand(
                        @"SELECT username, password_, role_ 
                          FROM users 
                          WHERE username = @username AND password_ = @password",
                        npgsqlConnection))
                    {

                        cmd.Parameters.AddWithValue("username", user.UserName);
                        cmd.Parameters.AddWithValue("password", user.Password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userRole = reader["role_"].ToString();
                                return (true, userRole);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
            }

            return (false, null);
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

        private string GetEnumJsonValue<T>(T enumValue) where T : Enum
        {
            var type = typeof(T);
            var field = type.GetField(enumValue.ToString());
            var attribute = field?.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                                  .FirstOrDefault() as JsonPropertyNameAttribute;

            return attribute?.Name ?? enumValue.ToString();
        }

        public async Task VerifyAndSave(Sales sale)
        {
            try
            {
                var db = new ConnectDB();
                using (var npgsqlConnection = new NpgsqlConnection(db.connString))
                {
                    await npgsqlConnection.OpenAsync();

                    string selectQuery = "SELECT approved, approvedat FROM serviceorders WHERE numos = @numeroOs";
                    bool? isApproved = false;
                    DateTime? currentApprovedAt = null;

                    using (var selectCommand = new NpgsqlCommand(selectQuery, npgsqlConnection))
                    {
                        selectCommand.Parameters.AddWithValue("numeroOs", sale.code);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                isApproved = reader["approved"] as bool?;
                                currentApprovedAt = reader["approvedat"] as DateTime?;
                            }
                        }
                    }

                    DateTime? approvedAt = (sale.approved && isApproved != true)
                        ? DateTime.UtcNow
                        : currentApprovedAt;

                    string insertQuery = @"
                INSERT INTO serviceorders (customer, numos, arrivaldate, ostype, observations, status, approved, approvedat) 
                VALUES (@cliente, @numeroOs, @dataChegada, @ostype, @observacao, @status, @approved, @approvedAt)
                ON CONFLICT (numos) DO UPDATE
                SET approved = EXCLUDED.approved,
                    approvedat = CASE
                        WHEN EXCLUDED.approved = true THEN EXCLUDED.approvedat
                        ELSE serviceorders.approvedat
                    END";

                    using (var insertCommand = new NpgsqlCommand(insertQuery, npgsqlConnection))
                    {
                        insertCommand.Parameters.AddWithValue("cliente",  sale.personRazao ?? sale.personName);
                        insertCommand.Parameters.AddWithValue("numeroOs", sale.code);
                        insertCommand.Parameters.AddWithValue("dataChegada", sale.DateCreated);
                        insertCommand.Parameters.AddWithValue("ostype", GetEnumJsonValue(sale.Type));
                        insertCommand.Parameters.AddWithValue("observacao", (object?)sale.Observation ?? DBNull.Value);
                        insertCommand.Parameters.AddWithValue("status", sale.status.ToString());
                        insertCommand.Parameters.AddWithValue("approved", sale.approved);
                        insertCommand.Parameters.AddWithValue("approvedAt", (object?)approvedAt ?? DBNull.Value);

                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new NpgsqlException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddObservations(string obsText, string numeroOs)
        {
            var db = new ConnectDB();
            using (var npgsqlConnection = new NpgsqlConnection(db.connString))
            {
                npgsqlConnection.Open();

                string obsQuery = "UPDATE serviceorders SET observations = @observation WHERE numos = @numeroOS";

                using (var updateCommand = new NpgsqlCommand(obsQuery, npgsqlConnection))
                {
                    updateCommand.Parameters.AddWithValue("observation", obsText);
                    updateCommand.Parameters.AddWithValue("numeroOs", numeroOs);

                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public static string MapToSaleType(string value)
        {
            value = value.Trim().ToUpper().Normalize();

            switch (value)
            {
                case "ASSISTÊNCIA TÉCNICA":
                    return "Assistência Técnica";

                case "ORÇAMENTO":
                    return "Orçamento";

                case "VENDA":
                    return "Venda";

                case "COMPRA":
                    return "Compra";

                case "VENDA CONSIGNADA":
                    return "Venda Consignada";

                case "VENDA REPRESENTAÇÃO":
                    return "Venda Representação";

                case "BONIFICAÇÃO/REMESSA":
                    return "Bonificação/Remessa";

                case "ORDEM DE SERVIÇO":
                    return "Ordem de Serviço";

                case "TRANSFERÊNCIA":
                    return "Transferência";

                default:
                    return "Unknown";
            }
        }

        public static string MapToSaleStatus(string value)
        {
            switch (value)
            {
                case "Emaberto":
                    return "Em aberto";

                case "Emorçamento":
                    return "Em orçamento";

                case "Aprovado":
                    return "Aprovado";

                case "Faturado":
                    return "Faturado";

                case "OrçamentoRecusado":
                    return "Orçamento Recusado";

                case "Cancelado":
                    return "Cancelado";

                case "Recusado":
                    return "Recusado";

                default:
                    return "Unknown";
            }
        }

        public async Task<List<Sales>> GetSalesFromDatabaseAsync()
        {
            List<Sales> sales = new();

            try
            {
                var db = new ConnectDB();
                using (var connection = new NpgsqlConnection(db.connString))
                {
                    await connection.OpenAsync();

                    var query = "SELECT * FROM serviceorders ORDER BY ismanual DESC, priority ASC, approvedat DESC";

                    using (var command = new NpgsqlCommand(query, connection)) 
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var sale = new Sales
                            {
                                code = reader["numos"].ToString(),
                                DateCreated = Convert.ToDateTime(reader["arrivaldate"]),
                                Observation = reader["observations"]?.ToString(),
                                personName = reader["customer"]?.ToString() ?? string.Empty,
                                personRazao = reader["customer"]?.ToString() ?? string.Empty,
                                Tags = new List<Tag>(),

                                displayType = reader["ostype"] != DBNull.Value
                                    ? MapToSaleType(reader["ostype"].ToString())
                                    : "Unknown",

                                displayStatus = reader["status"] != DBNull.Value
                                    ? MapToSaleStatus(reader["status"].ToString())
                                    : "Unknown"
                            };

                            if (reader["tagid"] != DBNull.Value)
                            {
                                var tagId = reader["tagid"].ToString();
                                sale.Tags.Add(new Tag
                                {
                                    TagId = tagId
                                });
                            }

                            sales.Add(sale);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar dados: {ex.Message}");
            }

            return sales;
        }

        public async Task SaveTagForOSAsync(string osCode, string TagId)
        {
            try
            {
                var db = new ConnectDB();
                using (var connection = new NpgsqlConnection(db.connString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        INSERT INTO salestags (oscode, tagid)
                        VALUES (@OSCode, @TagId)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OSCode", osCode);
                        command.Parameters.AddWithValue("@TagId", TagId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar Tag para OS: {ex.Message}");
            }
        }

        public async Task LinkTagToSaleAsync(string osCode, string tagId)
        {
            try
            {
                var db = new ConnectDB();
                using (var connection = new NpgsqlConnection(db.connString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        UPDATE serviceorders
                        SET tagid = @TagId
                        WHERE numos = @OSCode";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TagId", tagId);
                        command.Parameters.AddWithValue("@OSCode", osCode);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao vincular Tag ao Sale: {ex.Message}");
            }
        }

        public void RemoveOs(string OsCode)
        {
            var db = new ConnectDB();
            using (var connection = new NpgsqlConnection(db.connString))
            {
                connection.Open();

                string removeQuery = "DELETE FROM serviceorders WHERE numos = @oscode";

                using (var removeCommand = new NpgsqlCommand(removeQuery, connection))
                {
                    removeCommand.Parameters.AddWithValue("oscode", OsCode);

                    removeCommand.ExecuteNonQuery();
                }
            }
        }

        public void PauseOs(string OsCode)
        {
            var db = new ConnectDB();
            using (var connection = new NpgsqlConnection(db.connString))
            {
                connection.Open();

                string pauseQuery = "";
            }
        }

        public void UpdatePriority(string OsCode, int priority)
        {
            var db = new ConnectDB();
            using (var connection = new NpgsqlConnection(db.connString))
            {
                connection.Open();

                string updateQuery = "UPDATE serviceorders SET ismanual = true, priority = @priority WHERE numos = @oscode";

                using (var updateCommand = new NpgsqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("priority", priority);
                    updateCommand.Parameters.AddWithValue("oscode", OsCode);

                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public async Task UpdateApprovedAtAsync(string code, DateTime approvedAt)
        {
            try
            {
                var db = new ConnectDB();
                using (var connection = new NpgsqlConnection(db.connString))
                {
                    var updateQuery = "UPDATE serviceorders SET approvedat = @ApprovedAt WHERE Code = @Code";

                    using (var command = new NpgsqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ApprovedAt", approvedAt);
                        command.Parameters.AddWithValue("Code", code);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar ApprovedAt: {ex.Message}");
            }
        }

        /*public async Task Update()
        {
            var query = @"UPDATE serviceorders
                          SET situation = CASE
                              WHEN NOW() > deadline THEN 'atrasado'
                              WHEN NOW() + INTERVAL '1 days' > deadline THEN 'critico'
                              WHEN NOW() + INTERVAL '3 days' > deadline THEN 'aviso'
                              ELSE 'normal'
                          END";

            var db = new ConnectDB();
            using (var connection = new NpgsqlConnection(db.connString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }*/

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
