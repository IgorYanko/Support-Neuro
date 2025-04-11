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
using Microsoft.Extensions.Configuration;

namespace NeuroApp.Classes
{
    public class ConnectDB
    {
        private readonly string _connectionString;

        public ConnectDB(IConfiguration configuration)
        {
            var serverName = configuration.GetValue<string>("Database:Server") ?? "127.0.0.1";
            var port = configuration.GetValue<string>("Database:Port") ?? "5432";
            var userName = configuration.GetValue<string>("Database:Username") ?? "postgres";
            var password = configuration.GetValue<string>("Database:Password") ?? "Sivec@20";
            var databaseName = configuration.GetValue<string>("Database:Name") ?? "NeuroApp";

            _connectionString = new NpgsqlConnectionStringBuilder
            {
                Host = serverName,
                Port = int.Parse(port),
                Username = userName,
                Password = password,
                Database = databaseName,
                SslMode = SslMode.Disable,
            }.ConnectionString;
        }

        public string ConnectionString => _connectionString;
    }

    public class DatabaseActions : ConnectDB
    {
        public DatabaseActions(IConfiguration configuration) : base(configuration)
        {
        }

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
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var (currentStatus, currentApprovedAt) = await GetCurrentStatusAsync(connection, sale.Code);
                        
                        DateTime? approvedAt = currentApprovedAt ?? (sale.Status == Status.Aprovado ? DateTime.Now : null);
                        
                        DateTime? deadline = sale.Status == Status.Aprovado
                            ? BusinessDayCalculator.CalculateDeadline(approvedAt.Value)
                            : null;

                        await InsertOrUpdateServiceOrdersAsync(connection, sale, approvedAt, deadline);

                        await DeleteExistingTagsForSaleAsync(connection, transaction, sale.Code);

                        foreach (var tag in sale.Tags)
                        {
                            await SaveTagForOSAsync(connection, transaction, sale.Code, tag.TagId);
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Erro ao salvar venda e tags: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        private async Task DeleteExistingTagsForSaleAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string osCode)
        {
            var command = new NpgsqlCommand("DELETE FROM salestags WHERE oscode = @osCode", connection, transaction);
            command.Parameters.AddWithValue("@osCode", osCode);
            await command.ExecuteNonQueryAsync();
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

        private async Task<(string status, DateTime? approvedAt)> GetCurrentStatusAsync(NpgsqlConnection connection, string code)
        {
            const string selectQuery = @"
                SELECT status, approvedat
                FROM serviceorders
                WHERE numos = @numeroOs";

            await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("numeroOs", code);

            await using var reader = await selectCommand.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                string status = GetValueOrDefault(reader["status"], "Unknown");
                DateTime? approvedAt = reader["approvedat"] as DateTime?;

                return (status, approvedAt);
            }

            return ("Unknown", null);
        }

        private async Task InsertOrUpdateServiceOrdersAsync(NpgsqlConnection connection, Sales sale, DateTime? approvedAt, DateTime? deadline)
        {
            string insertQuery = @"
                INSERT INTO serviceorders (customer, numos, arrivaldate, ostype, observations, status, approvedat, deadline, ismanual)
                VALUES (@cliente, @numeroOs, @dataChegada, @ostype, @observacao, @status, @approvedAt, @deadline, @isManual)
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
                    ismanual = EXCLUDED.ismanual;";

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
                                   OR (status = 'Faturado' AND ostype = 'Venda' );";

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

        public int CalculatePriority(string status)
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
            await using var npgsqlConnection = new NpgsqlConnection(ConnectionString);

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

        public async Task SaveTagForOSAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string osCode, string tagId)
        {

            var query = @"
                INSERT INTO salestags (oscode, tagid)
                VALUES (@OSCode, @tagId)
                ON CONFLICT DO NOTHING";

            var command = new NpgsqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@OSCode", osCode);
            command.Parameters.AddWithValue("@tagId", tagId);

            await command.ExecuteNonQueryAsync();
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

        public async Task<bool> PauseOsAsync(string osCode, Status status, string type)
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
                    RETURNING pausedDate";

                using var command = new NpgsqlCommand(pauseQuery, connection);
                command.Parameters.AddWithValue("@pausedDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@OsCode", osCode);

                var result = await command.ExecuteScalarAsync();

                if (status.ToString() == "Aprovado" || SalesUtils.IsLocalStatus(status.ToString()) || type == "Venda")
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

        private DateTime CalculateNewDeadline(DateTime? deadline, DateTime pausedDate)
        {
            int diasPausados = BusinessDayCalculator.CalculateBusinessDays(pausedDate, DateTime.Now);
            return (deadline ?? DateTime.Now).AddDays(diasPausados);
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
                string? status = null;

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

                if (pausedDate.HasValue)
                {
                    if (!(pausedDate.Value == DateTime.Now))
                    {
                        using var updateCommand = new NpgsqlCommand("UPDATE serviceorders SET pausedDate = NULL WHERE numos = @OsCode", connection);
                        updateCommand.Parameters.AddWithValue("@OsCode", osCode);
                        await updateCommand.ExecuteNonQueryAsync();

                        MessageBox.Show($"Os {osCode} reativada sem alteração no prazo.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        DateTime newDeadline = CalculateNewDeadline(deadline, pausedDate.Value);

                        using var updateCommandFinal = new NpgsqlCommand(updateQuery, connection);
                        updateCommandFinal.Parameters.AddWithValue("@newDeadline", newDeadline);
                        updateCommandFinal.Parameters.AddWithValue("@OsCode", osCode);
                        await updateCommandFinal.ExecuteNonQueryAsync();

                        MessageBox.Show($"OS {osCode} reativada. Novo prazo: {newDeadline}", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show($"A OS {osCode} não pode ser reativada. Verifique o status e a data de pausa.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao reativar a OS {osCode}: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdatePriorityAsync(string osCode, bool isManual)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string updateQuery = @"
                    UPDATE serviceorders
                    SET ismanual = @isManual
                    WHERE numos = @osCode AND ismanual <> @isManual";

                using var updateCommand = new NpgsqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("isManual", isManual);
                updateCommand.Parameters.AddWithValue("osCode", osCode);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="warranty"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// 

        public async Task UpdateWarrantyAsync(Warranty warranty)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(@"
                UPDATE warranties SET
                    ClientName = @ClientName,
                    Device = @Device,
                    ServiceDate = @ServiceDate,
                    WarrantyEndDate = @WarrantyEndDate,
                    WarrantyMonths = @WarrantyMonths,
                    Observation = @Observation,
                    SerialNumber = @SerialNumber
                    WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@ClientName", warranty.ClientName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Device", warranty.Device ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ServiceDate", warranty.ServiceDate.Date);
            command.Parameters.AddWithValue("@WarrantyEndDate", warranty.WarrantyEndDate.Date);
            command.Parameters.AddWithValue("@WarrantyMonths", warranty.WarrantyMonths);
            command.Parameters.AddWithValue("@Observation", warranty.Observation ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SerialNumber", warranty.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Id", warranty.Id);

            int affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                throw new Exception("Nenhuma garantia foi encontrada com esse número de série.");
            }
        }

        public async Task SaveWarrantyAsync(Warranty warranty)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(@"
                INSERT INTO warranties
                (ClientName, SerialNumber, Device, ServiceDate, WarrantyEndDate, WarrantyMonths, Observation)
                VALUES
                (@ClientName, @SerialNumber, @Device, @ServiceDate, @WarrantyEndDate, @WarrantyMonths, @Observation)
                ON CONFLICT (SerialNumber) DO UPDATE SET
                    ClientName = EXCLUDED.ClientName,
                    SerialNumber = EXCLUDED.SerialNumber,
                    Device = EXCLUDED.Device,
                    ServiceDate = EXCLUDED.ServiceDate,
                    WarrantyEndDate = EXCLUDED.WarrantyEndDate,
                    WarrantyMonths = EXCLUDED.WarrantyMonths,
                    Observation = EXCLUDED.Observation", connection);

            command.Parameters.AddWithValue("@ClientName", warranty.ClientName);
            command.Parameters.AddWithValue("@SerialNumber", warranty.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Device", warranty.Device ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ServiceDate", warranty.ServiceDate.Date);
            command.Parameters.AddWithValue("@WarrantyEndDate", warranty.WarrantyEndDate.Date);
            command.Parameters.AddWithValue("@WarrantyMonths", warranty.WarrantyMonths);
            command.Parameters.AddWithValue("@Observation", warranty.Observation ?? (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Warranty>> GetAllWarrantiesAsync()
        {
            var warranties = new List<Warranty>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM Warranties ORDER BY WarrantyEndDate ASC", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                warranties.Add(new Warranty
                {
                    Id = reader.GetInt32("id"),
                    OsCode = reader["OsCode"].ToString(),
                    ClientName = reader["ClientName"].ToString(),
                    SerialNumber = reader["SerialNumber"]?.ToString(),
                    Device = reader["Device"]?.ToString(),
                    ServiceDate = reader.GetDateTime(reader.GetOrdinal("ServiceDate")),
                    WarrantyEndDate = reader.GetDateTime(reader.GetOrdinal("WarrantyEndDate")),
                    WarrantyMonths = reader.GetInt32(reader.GetOrdinal("WarrantyMonths")),
                    Observation = reader["Observation"]?.ToString()
                });
            }

            return warranties;
        }

        public async Task<List<Warranty>> GetWarrantyBySearchAsync(string searchText)
        {
            var warranties = new List<Warranty>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(
                @"SELECT * FROM Warranties WHERE serialnumber LIKE '%' || :SearchText || '%'",
                connection
            );
            
            command.Parameters.AddWithValue(":SearchText", searchText);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                warranties.Add(new Warranty
                {
                    OsCode = reader["OsCode"].ToString(),
                    ClientName = reader["ClientName"].ToString(),
                    SerialNumber = reader["SerialNumber"]?.ToString(),
                    Device = reader["Device"]?.ToString(),
                    ServiceDate = reader.GetDateTime(reader.GetOrdinal("ServiceDate")),
                    WarrantyEndDate = reader.GetDateTime(reader.GetOrdinal("WarrantyEndDate")),
                    WarrantyMonths = reader.GetInt32(reader.GetOrdinal("WarrantyMonths")),
                    Observation = reader["Observation"]?.ToString()
                });
            }

            return warranties;
        }

        public async Task DeleteWarrantyByOsCodeAsync(string osCode)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM Warranties WHERE OsCode = @OsCode", connection);
            command.Parameters.AddWithValue("@OsCode", osCode);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteWarrantyAsync(string serialNumber)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM Warranties WHERE serialnumber = @SerialNumber", connection);
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);

            await command.ExecuteNonQueryAsync();
        }
    }
}