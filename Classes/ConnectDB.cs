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

namespace NeuroApp.Classes
{
    public class ConnectDB
    {
        private readonly static string ServerName = "127.0.0.1";
        private readonly static string Port = "5432";
        private readonly static string UserName = "postgres";
        private readonly static string Password = "Sivec@20";
        private readonly static string DatabaseName = "NeuroApp";

        public string ConnectionString { get; }

        public ConnectDB()
        {
            ConnectionString = new NpgsqlConnectionStringBuilder
            {
                Host = ServerName,
                Port = int.Parse(Port),
                Username = UserName,
                Password = Password,
                Database = DatabaseName,
                SslMode = SslMode.Disable,
            }.ConnectionString;
        }
    }

    public class DatabaseActions : ConnectDB
    {
        public async Task<(bool IsAuthenticated, string UserRole)> UserLoginAsync(string username, string password)
        {
            try
            {
                await using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string hashedPassword = HashPassword(password);

                await using var cmd = new NpgsqlCommand(
                    @"SELECT role_ FROM users WHERE username = @username AND password_ = @password", connection);

                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", hashedPassword);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return (true, reader["role_"].ToString() ?? "generic");
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
                return (false, "Erro de conexão");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return (false, "Erro interno");
            }

            return (false, "generic");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        //public bool BuildOS(ServiceOrder serviceOrder)
        //{
        //    try
        //    {
        //        using NpgsqlConnection npgsqlConnection = new(ConnectionString);
        //        npgsqlConnection.Open();

        //        using var cmd = new NpgsqlCommand(
        //            $"INSERT INTO ServiceOrder(customer,numeroos,arrivaldate,status,observation) VALUES (@cliente,@numeroOs,@dataChegada,@status,@observacao)",
        //            npgsqlConnection
        //        );

        //        serviceOrder.Status_ = serviceOrder.IsGuarantee
        //            ? NeuroApp.ServiceOrder.Status.budgetApproved
        //            : NeuroApp.ServiceOrder.Status.waitingBudget;

        //        cmd.Parameters.AddWithValue("cliente", serviceOrder.Customer);
        //        cmd.Parameters.AddWithValue("numeroOs", serviceOrder.OsNumber);
        //        cmd.Parameters.AddWithValue("dataChegada", serviceOrder.ArrivalDate ?? (object)DBNull.Value);
        //        cmd.Parameters.AddWithValue("observacao", (object?)serviceOrder.Observation ?? DBNull.Value);
        //        //cmd.Parameters.AddWithValue("garantia", NpgsqlTypes.NpgsqlDbType.Boolean).Value = serviceOrder.IsGuarantee;
        //        cmd.Parameters.AddWithValue("status", (int)serviceOrder.Status_);

        //        cmd.ExecuteNonQuery();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro inesperado: {ex.Message}");
        //        return false;
        //    }
        //}

        private string GetEnumJsonValue<T>(T enumValue) where T : Enum
        {
            var type = typeof(T);
            var field = type.GetField(enumValue.ToString());

            if (field == null)
                throw new ArgumentException($"O valor '{enumValue}' não é um membro válido do enum '{typeof(T).Name}'.", nameof(enumValue));
            
            var attribute = field.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
                                  .FirstOrDefault() as JsonPropertyNameAttribute;

            return attribute?.Name ?? enumValue.ToString();
        }

        public async Task VerifyAndSave(Sales sale)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    await connection.OpenAsync();

                    var (currentStatus, currentApprovedAt, currentPriority) = await GetCurrentStatusAsync(connection, sale.Code);

                    DateTime? approvedAt = currentApprovedAt ?? (sale.Status == Status.Aprovado ? DateTime.Now : null);

                    DateTime? deadline = sale.Status == Status.Aprovado
                        ? BusinessDayCalculator.CalculateDeadline(approvedAt.Value)
                        : null;

                    int priority = CalculatePriority(sale.Status.ToString());

                    await InsertOrUpdateServiceOrdersAsync(connection, sale, approvedAt, deadline, priority);

                    foreach (var tag in sale.Tags)
                    {
                        await SaveTagForOSAsync(sale.Code, tag.TagId);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CheckIfOsExistsAsync(string osCode)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM serviceorders WHERE numos = @osCode";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@osCode", osCode);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar existência da OS {osCode}: {ex.Message}");
                return false;
            }
        }

        private async Task<(string status, DateTime? approvedAt, int priority)> GetCurrentStatusAsync(NpgsqlConnection connection, string code)
        {
            const string selectQuery = @"
                SELECT status, approvedat, priority
                FROM serviceorders
                WHERE numos = @numeroOs";

            await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("numeroOs", code);

            await using var reader = await selectCommand.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                string status = GetValueOrDefault(reader["status"], "Unknown");
                DateTime? approvedAt = reader["approvedat"] as DateTime?;
                int priority = GetValueOrDefault(reader["priority"], 6);

                return (status, approvedAt, priority);
            }

            return ("Unknown", null, 6);
        }

        private async Task InsertOrUpdateServiceOrdersAsync(NpgsqlConnection connection, Sales sale, DateTime? approvedAt, DateTime? deadline, int priority)
        {
            string insertQuery = @"
                INSERT INTO serviceorders (customer, numos, arrivaldate, ostype, observations, status, approvedat, deadline, priority, ismanual)
                VALUES (@cliente, @numeroOs, @dataChegada, @ostype, @observacao, @status, @approvedAt, @deadline, @priority, @isManual)
                ON CONFLICT (numos) DO UPDATE
                SET status = EXCLUDED.status,
                    approvedat = CASE
                        WHEN serviceorders.status != 'Aprovado' AND EXCLUDED.status = 'Aprovado' THEN EXCLUDED.approvedat
                        ELSE serviceorders.approvedat
                    END,
                    deadline = CASE
                        WHEN serviceorders.status != 'Aprovado' AND EXCLUDED.status = 'Aprovado' THEN EXCLUDED.deadline
                        ELSE serviceorders.deadline
                    END,
                    priority = CASE
                        WHEN serviceorders.ismanual = true THEN serviceorders.priority
                        ELSE EXCLUDED.priority
                    END;";

            using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddRange(new[]
                {
                    new NpgsqlParameter("cliente", sale.PersonRazao ?? sale.PersonName),
                    new NpgsqlParameter("numeroOs", sale.Code),
                    new NpgsqlParameter("dataChegada", sale.DateCreated),
                    new NpgsqlParameter("ostype", GetEnumJsonValue(sale.Type)),
                    new NpgsqlParameter("observacao", sale.Observation ?? (object)DBNull.Value),
                    new NpgsqlParameter("status", sale.Status.ToString()),
                    new NpgsqlParameter("approvedAt", approvedAt ?? (object)DBNull.Value),
                    new NpgsqlParameter("deadline", deadline ?? (object)DBNull.Value),
                    new NpgsqlParameter("priority", priority),
                    new NpgsqlParameter("isManual", sale.IsManual)
                });

                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Sales>> GetSalesFromDatabaseAsync()
        {
            var sales = new List<Sales>();

            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                var query = @"
                              SELECT *,
                                  COALESCE(approvedat, '1970-01-01') AS approvedat_fallback,
                                  COALESCE(arrivaldate, '1970-01-01') AS arrivaldate_fallback,
                                  COALESCE(pauseddate, '1970-01-01') AS pauseddate_fallback,
                                  COALESCE(quotationdate, NULL) AS quotationdate_fallback
                              FROM serviceorders
                              WHERE 
                                   status != 'Faturado'
                                   OR (status = 'Faturado' AND ostype = 'Venda' AND arrivaldate >= NOW() - INTERVAL '7 days')
                              ORDER BY
                                    CASE
                                        WHEN ismanual = true THEN priority
                                        ELSE
                                            CASE 
                                                WHEN ostype = 'Venda' THEN 1 -- Adicionando regra para 'Faturado' e 'Venda' ter prioridade 1
                                                WHEN status IN ('Emexecução', 'ControledeQualidade', 'ReprovadoQualidade', 'AprovadoQualidade', 'EsperandoColeta') THEN 0
                                                WHEN status = 'Aprovado' THEN 1
                                                WHEN status = 'Emorçamento' THEN 2
                                                WHEN status = 'Emaberto' THEN 4
                                                WHEN status = 'Faturado' THEN 5
                                                ELSE 6 
                                            END
                                    END,
                                    CASE 
                                        WHEN status IN ('Emexecução', 'Controledequalidade', 'ReprovadoQualidade', 'AprovadoQualidade', 'EsperandoColeta') THEN approvedat
                                        WHEN status = 'Aprovado' THEN approvedat
                                        WHEN status = 'Emorçamento' THEN arrivaldate
                                        WHEN status = 'Pausada' THEN pauseddate
                                        WHEN status = 'Emaberto' THEN COALESCE(arrivaldate, '1970-01-01')
                                        ELSE NULL
                                    END;";

                using var command = new NpgsqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var sale = MapToSale(reader);
                    sales.Add(sale);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar dados: {ex.Message}");
            }

            return sales;
        }

        private int CalculatePriority(string status)
        {
            return status switch
            {
                "Emexecução" or
                "Controledequalidade" or
                "ReprovadoQualidade" or
                "AprovadoQualidade" or
                "EsperandoColeta" => 0,
                
                "Aprovado" => 1,
                "Emorçamento" => 2,
                "Pausada" => 3,
                "Emaberto" => 4,
                "Faturado" => 5,
                _ => 6
            };
        }

        public async Task AddObservationsAsync(string obsText, string numeroOs)
        {
            var db = new ConnectDB();
            await using var npgsqlConnection = new NpgsqlConnection(db.ConnectionString);

            try
            {
                await npgsqlConnection.OpenAsync();

                string selectQuery = "SELECT observations FROM serviceorders WHERE numos = @numeroOs";
                await using var selectCommand = new NpgsqlCommand(selectQuery, npgsqlConnection);
                selectCommand.Parameters.AddWithValue("numeroOs", numeroOs);

                string? currentObservation = (await selectCommand.ExecuteScalarAsync()) as string;

                if (currentObservation == obsText)
                {
                    Console.WriteLine($"A observação da OS {numeroOs} já está atualizada.");
                    return;
                }

                string obsQuery = "UPDATE serviceorders SET observations = @observation WHERE numos = @numeroOS";
                await using var updateCommand = new NpgsqlCommand(obsQuery, npgsqlConnection);
                updateCommand.Parameters.AddWithValue("observation", (object?)obsText ?? DBNull.Value);
                updateCommand.Parameters.AddWithValue("numeroOs", numeroOs);

                await updateCommand.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static readonly Dictionary<string, string> SaleTypeMappings = new()
        {
            { "ASSISTÊNCIA TÉCNICA", "Assistência Técnica" },
            { "ORÇAMENTO", "Orçamento" },
            { "VENDA", "Venda" },
            { "COMPRA", "Compra" },
            { "VENDA CONSIGNADA", "Venda Consignada" },
            { "VENDA REPRESENTAÇÃO", "Venda Representação" },
            { "BONIFICAÇÃO/REMESSA", "Bonificação/Remessa" },
            { "ORDEM DE SERVIÇO", "Ordem de Serviço" },
            { "TRANSFERÊNCIA", "Transferência" },
            { "LOCAÇÃO", "Locação" }
        };

        public static string MapToSaleType(string value)
        {
            value = value.Trim().ToUpper().Normalize();
            return SaleTypeMappings.TryGetValue(value, out var mappedValue) ? mappedValue : "Unknown";
        }

        private static readonly Dictionary<string, string> SaleStatusMappings = new()
        {
            { "Emaberto", "Em Aberto" },
            { "Emorçamento", "Em Orçamento" },
            { "Aprovado", "Aprovado" },
            { "Faturado", "Faturado" },
            { "OrçamentoRecusado", "Orçamento Recusado" },
            { "Cancelado", "Cancelado" },
            { "Recusado", "Recusado" },
            { "Emexecução", "Em Execução" },
            { "Pausada", "Pausada" },
            { "ControledeQualidade", "Controle de Qualidade" },
            { "AprovadoQualidade", "Aprovado na Qualidade" },
            { "ReprovadoQualidade", "Reprovado na Qualidade" },
            { "EsperandoColeta", "Esperando Coleta" }
        };

        public static string MapToSaleStatus(string value)
        {
            return SaleStatusMappings.TryGetValue(value, out var mappedValue) ? mappedValue : "Unknown";
        }

        private T GetValueOrDefault<T>(object value, T defaultValue = default)
        {
            return value == DBNull.Value || value == null
                ? defaultValue
                : (T)Convert.ChangeType(value, typeof(T));
        }

        private DateTime? GetNullableDateTime(object value)
        {
            return value == DBNull.Value ? null : (DateTime?)Convert.ChangeType(value, typeof(DateTime));
        }

        private bool? GetNulableBool(object value)
        {
            return value == DBNull.Value ? null : value is bool b ? b : (bool?)Convert.ChangeType(value, typeof(bool));
        }

        private Sales MapToSale(NpgsqlDataReader reader)
        {
            var sale = new Sales
            {
                Code = GetValueOrDefault(reader["numos"], string.Empty),
                DateCreated = GetNullableDateTime(reader["arrivaldate"]) ?? DateTime.MinValue,
                Observation = GetValueOrDefault(reader["observations"], string.Empty),
                PersonName = GetValueOrDefault(reader["customer"], string.Empty),
                PersonRazao = GetValueOrDefault(reader["customer"], string.Empty),
                Tags = new List<Tag>(),
                DisplayType = MapToSaleType(GetValueOrDefault(reader["ostype"], "Unknown")),
                DisplayStatus = MapToSaleStatus(GetValueOrDefault(reader["status"], "Unknown")),
                IsPaused = reader["pausedDate"] != DBNull.Value,
                IsManual = GetNulableBool(reader["ismanual"]) ?? false,
                Priority = CalculatePriority(GetValueOrDefault(reader["status"], "Unknown")),
                ApprovedAt = GetNullableDateTime(reader["approvedat"]),
                QuotationDate = GetNullableDateTime(reader["quotationdate_fallback"]),
                Excluded = reader["excluded"] != DBNull.Value && (bool)reader["excluded"]
            };

            if (sale.DisplayStatus == "Aprovado" && sale.ApprovedAt.HasValue)
            {
                sale.Deadline = BusinessDayCalculator.CalculateDeadline(sale.ApprovedAt.Value);
            }

            if (reader["tagid"] != DBNull.Value && !string.IsNullOrEmpty(reader["tagid"].ToString()))
            {
                sale.Tags.Add(new Tag
                {
                    TagId = reader["tagid"].ToString()
                });
            }

            return sale;
        }

        public async Task SaveTagForOSAsync(string osCode, string tagId)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                var checkQuery = "SELECT COUNT(*) FROM salestags WHERE oscode = @OSCode AND tagid = @TagId";
                using var checkCommand = new NpgsqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@OSCode", osCode);
                checkCommand.Parameters.AddWithValue("@TagId", tagId);

                int tagExists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                
                if (tagExists > 0)
                {
                    Console.WriteLine($"Tag {tagId} já está associada à OS {osCode}, não será inserida novamente.");
                    return;
                }
                
                var query = @"
                        INSERT INTO salestags (oscode, tagid)
                        VALUES (@OSCode, @tagId)
                        ON CONFLICT DO NOTHING";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@OSCode", osCode);
                command.Parameters.AddWithValue("@tagId", tagId);
                
                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Tag {tagId} associada à OS {osCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar Tag para OS: {ex.Message}");
            }
        }

        public async Task<List<Tag>> GetTagsForOsAsync(string osCode)
        {
            var tags = new List<Tag>();

            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string query = "SELECT tagid FROM salestags WHERE oscode = @OsCode";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@OsCode", osCode);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tags.Add(new Tag
                    {
                        TagId = reader["tagid"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar tags da OS {osCode}: {ex.Message}");
            }

            return tags;
        }

        public async Task RemoveOs(string osCode)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string removeTagsQuery = "UPDATE salestags SET excluded = true WHERE oscode = @oscode";
                using var removeTagsCommand = new NpgsqlCommand(removeTagsQuery, connection);
                removeTagsCommand.Parameters.AddWithValue("oscode", osCode);
                await removeTagsCommand.ExecuteNonQueryAsync();

                string removeQuery = "UPDATE serviceorders SET excluded = true WHERE numos = @oscode";
                using var removeOsCommand = new NpgsqlCommand(removeQuery, connection);
                removeOsCommand.Parameters.AddWithValue("oscode", osCode);
                await removeOsCommand.ExecuteNonQueryAsync();

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Erro ao remover OS {osCode}: {ex}");
            }
        }

        public async Task<bool> PauseOsAsync(string osCode, Status status)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string pauseQuery = @"
                    UPDATE serviceorders
                    SET pausedDate = @pausedDate
                    WHERE numos = @OsCode
                    AND pausedDate IS NULL 
                    AND (approvedat IS NOT NULL OR deadline IS NOT NULL)
                    RETURNING pausedDate";

                using var command = new NpgsqlCommand(pauseQuery, connection);
                command.Parameters.AddWithValue("@pausedDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@OsCode", osCode);

                var result = await command.ExecuteScalarAsync();

                if (status.ToString() == "Aprovado"|| Sales.IsLocalStatus(status.ToString()))
                {
                    if (result != null)
                    {
                        MessageBox.Show($"OS {osCode} pausada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"A OS {osCode} não pode ser pausada. Verifique se já está aprovada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao pausar OS {osCode}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnpauseOsAsync(string osCode)
        {
            try
            {
                  using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string selectQuery = "SELECT pausedDate, deadline, status FROM serviceorders WHERE numos = @OsCode";
                string updateQuery = "UPDATE serviceorders SET pausedDate = NULL, deadline = @newDeadline WHERE numos = @OsCode";

                DateTime? pausedDate = null;
                DateTime? deadline = null;
                string status = null;

                using (var selectCommand = new NpgsqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@OsCode", osCode);

                    using var reader = await selectCommand.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        pausedDate = reader["pausedDate"] as DateTime?;
                        deadline = reader["deadline"] as DateTime?;
                        status = reader["status"]?.ToString();
                    }
                }

                List<string> localStatuses = new() { "Aprovado", "Em Execução", "Controle de Qualidade", "Aprovado na Qualidade", "Reprovado na Qualidade", "Esperando Coleta" };
                string normalizedStatus = status.Replace(" ", "").ToLower();

                bool isLocal = localStatuses.Any(s => s.Replace(" ", "").ToLower() == normalizedStatus);

                if (pausedDate.HasValue && isLocal)
                {
                    if (!(pausedDate.Value == DateTime.UtcNow))
                    {
                        using var updateCommand = new NpgsqlCommand("UPDATE serviceorders SET pausedDate = NULL WHERE numos = @OsCode", connection);
                        updateCommand.Parameters.AddWithValue("@OsCode", osCode);
                        await updateCommand.ExecuteNonQueryAsync();

                        MessageBox.Show($"Os {osCode} reativada sem alteração no prazo.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }

                    int diasPausados = BusinessDayCalculator.CalculateBusinessDays(pausedDate.Value, DateTime.UtcNow);

                    DateTime newDeadline = (deadline ?? DateTime.UtcNow).AddDays(diasPausados);


                    using var updateCommandFinal = new NpgsqlCommand(updateQuery, connection);
                    updateCommandFinal.Parameters.AddWithValue("@newDeadline", newDeadline);
                    updateCommandFinal.Parameters.AddWithValue("@OsCode", osCode);
                    await updateCommandFinal.ExecuteNonQueryAsync();

                    MessageBox.Show($"OS {osCode} reativada. Novo prazo: {newDeadline}", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show($"A OS {osCode} não pode ser reativada. Verifique o status e a data de pausa.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao reativar a OS {osCode}: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdatePriorityAsync(string osCode, int priority, bool isManual)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string updateQuery = @"
                    UPDATE serviceorders
                    SET priority = @priority,
                        ismanual = COALESCE(ismanual, @isManual)
                    WHERE numos = @oscode";

                using var updateCommand = new NpgsqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("priority", priority);
                updateCommand.Parameters.AddWithValue("oscode", osCode);
                updateCommand.Parameters.AddWithValue("ismanual", isManual);

                int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar prioridade da OS {osCode}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateStatusOnDatabaseAsync(string code, string status)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                    await connection.OpenAsync();

                    var updateQuery = "UPDATE serviceorders SET status = @status WHERE numos = @code";

                using var command = new NpgsqlCommand(updateQuery, connection);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@code", code);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar status da OS {ex.Message}");
                return false;
            }
        }


    }
}