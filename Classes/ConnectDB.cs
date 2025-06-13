using Npgsql;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using Microsoft.Extensions.Configuration;
using NeuroApp.Interfaces;

namespace NeuroApp.Classes
{
    public class ConnectDB
    {
        private readonly string _connectionString;

        public ConnectDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("A connection string 'DefaultConnection' não foi encontrada no appsettings.json.");
            }

        }

        public string ConnectionString => _connectionString;
    }

    public class DatabaseActions : ConnectDB, IDatabaseActions
    {
        private readonly IConfiguration _configuration;

        public DatabaseActions(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool IsAuthenticated, string UserRole)> UserLoginAsync(string username, string acess_code)
        {
            try
            {
                await using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                await using var cmd = new NpgsqlCommand(
                    @"SELECT permission_level FROM users WHERE username = @username AND acess_code = @acess_code", connection);

                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("acess_code", acess_code);

                await using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return (true, reader["permission_level"].ToString() ?? "medium");
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
            var command = new NpgsqlCommand("DELETE FROM sales_tags WHERE os_number = @os_number", connection, transaction);
            command.Parameters.AddWithValue("@os_number", osCode);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> CheckIfOsExistsAsync(string osCode)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM service_orders WHERE os_number = @os_number";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@os_number", osCode);

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
                SELECT status, approved_date
                FROM service_orders
                WHERE os_number = @numeroOs";

            await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("numeroOs", code);

            await using var reader = await selectCommand.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                string status = GetValueOrDefault(reader["status"], "Unknown");
                DateTime? approvedAt = reader["approved_date"] as DateTime?;

                return (status, approvedAt);
            }

            return ("Unknown", null);
        }

        private async Task InsertOrUpdateServiceOrdersAsync(NpgsqlConnection connection, Sales sale, DateTime? approvedAt, DateTime? deadline)
        {
            string insertQuery = @"
                INSERT INTO service_orders (customer, os_number, arrival_date, os_type, observations, status, approved_date, deadline, is_manual, excluded)
                VALUES (@customer, @os_number, @arrival_date, @os_type, @observations, @status, @approved_date, @deadline, @is_manual, @excluded)
                ON CONFLICT (os_number) DO UPDATE
                SET status = EXCLUDED.status,
                    approved_date = CASE
                        WHEN service_orders.status != 'Aprovado' AND EXCLUDED.status = 'Aprovado' THEN EXCLUDED.approved_date
                        ELSE service_orders.approved_date
                    END,
                    deadline = CASE
                        WHEN service_orders.status != 'Aprovado' AND EXCLUDED.status = 'Aprovado' THEN EXCLUDED.deadline
                        ELSE service_orders.deadline
                    END,
                    is_manual = EXCLUDED.is_manual;";

            using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddRange(new[]
                {
                    new NpgsqlParameter("customer", sale.PersonRazao ?? sale.PersonName),
                    new NpgsqlParameter("os_number", sale.Code),
                    new NpgsqlParameter("arrival_date", sale.DateCreated),
                    new NpgsqlParameter("os_type", GetEnumJsonValue(sale.Type)),
                    new NpgsqlParameter("observations", sale.Observation ?? (object)DBNull.Value),
                    new NpgsqlParameter("status", sale.Status.ToString()),
                    new NpgsqlParameter("approved_date", approvedAt ?? (object)DBNull.Value),
                    new NpgsqlParameter("deadline", deadline ?? (object)DBNull.Value),
                    new NpgsqlParameter("is_manual", sale.IsManual),
                    new NpgsqlParameter("excluded", sale.Excluded)
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
                                  COALESCE(approved_date, '1970-01-01') AS approved_date_fallback,
                                  COALESCE(arrival_date, '1970-01-01') AS arrival_date_fallback,
                                  COALESCE(paused_date, '1970-01-01') AS paused_date_fallback,
                                  COALESCE(quotation_date, NULL) AS quotation_date_fallback
                              FROM service_orders
                              WHERE 
                                   status != 'Faturado'
                                   OR (status = 'Faturado' AND os_type = 'Venda' );";

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
                
                "Faturado" => 1,
                "Aprovado" => 2,
                "Emorçamento" => 3,
                "Emaberto" => 4,
                _ => 5
            };
        }

        public async Task<bool>AddObservationsAsync(string obsText, string numeroOs)
        {
            await using var npgsqlConnection = new NpgsqlConnection(ConnectionString);

            try
            {
                await npgsqlConnection.OpenAsync();

                string selectQuery = "SELECT observations FROM service_orders WHERE os_number = @numeroOs";
                await using var selectCommand = new NpgsqlCommand(selectQuery, npgsqlConnection);
                selectCommand.Parameters.AddWithValue("numeroOs", numeroOs);

                string? currentObservation = (await selectCommand.ExecuteScalarAsync()) as string;

                if (currentObservation == obsText)
                {
                    return false;
                }

                string obsQuery = "UPDATE service_orders SET observations = @observation WHERE os_number = @numeroOS";
                await using var updateCommand = new NpgsqlCommand(obsQuery, npgsqlConnection);
                updateCommand.Parameters.AddWithValue("observation", (object?)obsText ?? DBNull.Value);
                updateCommand.Parameters.AddWithValue("numeroOs", numeroOs);

                await updateCommand.ExecuteNonQueryAsync();
                return true;
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
                Code = GetValueOrDefault(reader["os_number"], string.Empty),
                DateCreated = GetNullableDateTime(reader["arrival_date"]) ?? DateTime.MinValue,
                Observation = GetValueOrDefault(reader["observations"], string.Empty),
                PersonName = GetValueOrDefault(reader["customer"], string.Empty),
                PersonRazao = GetValueOrDefault(reader["customer"], string.Empty),
                Tags = new List<Tag>(),
                DisplayType = MapToSaleType(GetValueOrDefault(reader["os_type"], "Unknown")),
                DisplayStatus = MapToSaleStatus(GetValueOrDefault(reader["status"], "Unknown")),
                IsPaused = reader["paused_date"] != DBNull.Value,
                IsManual = GetNulableBool(reader["is_manual"]) ?? false,
                ApprovedAt = GetNullableDateTime(reader["approved_date"]),
                QuotationDate = GetNullableDateTime(reader["quotation_date_fallback"]),
                Excluded = reader["excluded"] != DBNull.Value && (bool)reader["excluded"]
            };

            if (sale.DisplayStatus == "Aprovado" && sale.ApprovedAt.HasValue)
            {
                sale.Deadline = BusinessDayCalculator.CalculateDeadline(sale.ApprovedAt.Value);
            }

            if (reader["tag_id"] != DBNull.Value && !string.IsNullOrEmpty(reader["tag_id"].ToString()))
            {
                sale.Tags.Add(new Tag
                {
                    TagId = reader["tag_id"].ToString()
                });
            }

            return sale;
        }

        public async Task SaveTagForOSAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string osCode, string tagId)
        {
            var query = @"
                INSERT INTO sales_tags (os_number, tag_id)
                VALUES (@os_number, @tagId)
                ON CONFLICT DO NOTHING";

            var command = new NpgsqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@os_number", osCode);
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

                string query = "SELECT tag_id FROM sales_tags WHERE os_number = @os_number";

                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@os_number", osCode);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    tags.Add(new Tag
                    {
                        TagId = reader["tag_id"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar tags da OS {osCode}: {ex.Message}");
            }

            return tags;
        }

        public async Task<bool> RemoveOsAsync(string osCode)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var registerDeletionQuery = @"INSERT INTO deleted_service_orders (os_number, deleted_at)
                                                VALUES (@os_number, @deleted_at)
                                                ON CONFLICT (os_number) DO NOTHING";

                await using var registerDeletionCommand = new NpgsqlCommand(registerDeletionQuery, connection, transaction);
                registerDeletionCommand.Parameters.AddWithValue("os_number", osCode);
                registerDeletionCommand.Parameters.AddWithValue("deleted_at", DateTime.UtcNow);
                await registerDeletionCommand.ExecuteNonQueryAsync();

                var removeTagsQuery = @"UPDATE sales_tags
                                          SET excluded = true
                                          WHERE os_number = @os_number";

                await using var removeTagsCommand = new NpgsqlCommand(removeTagsQuery, connection, transaction);
                removeTagsCommand.Parameters.AddWithValue("os_number", osCode);
                await removeTagsCommand.ExecuteNonQueryAsync();

                var removeQuery = @"UPDATE service_orders
                                      SET excluded = true
                                      WHERE os_number = @os_number";

                await using var removeOsCommand = new NpgsqlCommand(removeQuery, connection, transaction);
                removeOsCommand.Parameters.AddWithValue("os_number", osCode);

                var rowsAffected = await removeOsCommand.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                MessageBox.Show($"Erro ao remover OS {osCode}: {ex}");
                throw;
            }
        }

        public async Task<bool> PauseOsAsync(string osCode, Status status, string type)
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string pauseQuery = @"
                    UPDATE service_orders
                    SET paused_date = @pausedDate
                    WHERE os_number = @os_number
                    AND paused_date IS NULL
                    RETURNING paused_date";

                using var command = new NpgsqlCommand(pauseQuery, connection);
                command.Parameters.AddWithValue("@pausedDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@os_number", osCode);

                var result = await command.ExecuteScalarAsync();

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

                string selectQuery = "SELECT paused_date, deadline, status FROM service_orders WHERE os_number = @os_number";
                string updateQuery = "UPDATE service_orders SET paused_date = NULL, deadline = @newDeadline WHERE os_number = @os_number";

                DateTime? pausedDate = null;
                DateTime? deadline = null;
                string? status = null;

                using (var selectCommand = new NpgsqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@os_number", osCode);

                    using var reader = await selectCommand.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        pausedDate = reader["paused_date"] as DateTime?;
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
                        using var updateCommand = new NpgsqlCommand("UPDATE service_orders SET paused_date = NULL WHERE os_number = @os_number", connection);
                        updateCommand.Parameters.AddWithValue("@os_number", osCode);
                        await updateCommand.ExecuteNonQueryAsync();

                        MessageBox.Show($"Os {osCode} reativada sem alteração no prazo.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else
                    {
                        DateTime newDeadline = CalculateNewDeadline(deadline, pausedDate.Value);

                        using var updateCommandFinal = new NpgsqlCommand(updateQuery, connection);
                        updateCommandFinal.Parameters.AddWithValue("@newDeadline", newDeadline);
                        updateCommandFinal.Parameters.AddWithValue("@os_number", osCode);
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
                    UPDATE service_orders
                    SET is_manual = @isManual
                    WHERE os_number = @os_number AND is_manual <> @isManual";

                using var updateCommand = new NpgsqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("isManual", isManual);
                updateCommand.Parameters.AddWithValue("os_number", osCode);

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

                    var updateQuery = "UPDATE service_orders SET status = @status WHERE os_number = @code";

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

        public async Task<HashSet<string>> GetDeletedOsCodesAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();

                string query = "SELECT os_number FROM deleted_service_orders";
                using var command = new NpgsqlCommand(query, connection);

                var deletedCodes = new HashSet<string>();
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    deletedCodes.Add(reader.GetString(0));
                }

                return deletedCodes;
            }
            catch (Exception ex)
            {
                return new HashSet<string>();
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
                    costumer = @costumer,
                    Device = @Device,
                    service_date = @service_date,
                    warranty_end = @warranty_end,
                    warranty_length = @warranty_length,
                    description = @description,
                    serial_number = @serial_number
                    WHERE id = @Id", connection);

            command.Parameters.AddWithValue("@costumer", warranty.Customer ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Device", warranty.Device ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@service_date", warranty.ServiceDate.Date);
            command.Parameters.AddWithValue("@warranty_end", warranty.WarrantyEndDate.Date);
            command.Parameters.AddWithValue("@warranty_length", warranty.WarrantyMonths);
            command.Parameters.AddWithValue("@description", warranty.Observation ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@serial_number", warranty.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Id", warranty.Id);

            int affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                throw new Exception("Nenhuma garantia encontrada com esse número de série.");
            }
        }

        public async Task SaveWarrantyAsync(Warranty warranty)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(@"
                INSERT INTO warranties
                (os_number, customer, serial_number, device, service_date, warranty_end, warranty_length, description)
                VALUES
                (@os_number, @customer, @serial_number, @device, @service_date, @warranty_end, @warranty_length, @description)
                ON CONFLICT (serial_number) DO UPDATE SET
                    customer = EXCLUDED.customer,
                    serial_number = EXCLUDED.serial_number,
                    Device = EXCLUDED.Device,
                    service_date = EXCLUDED.service_date,
                    warranty_end = EXCLUDED.warranty_end,
                    warranty_length = EXCLUDED.warranty_length,
                    description = EXCLUDED.description", connection);

            command.Parameters.AddWithValue("@os_number", warranty.OsCode ?? string.Empty);
            command.Parameters.AddWithValue("@customer", warranty.Customer);
            command.Parameters.AddWithValue("@serial_number", warranty.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@device", warranty.Device ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@service_date", warranty.ServiceDate.Date);
            command.Parameters.AddWithValue("@warranty_end", warranty.WarrantyEndDate.Date);
            command.Parameters.AddWithValue("@warranty_length", warranty.WarrantyMonths);
            command.Parameters.AddWithValue("@description", warranty.Observation ?? (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Warranty>> GetAllWarrantiesAsync()
        {
            var warranties = new List<Warranty>();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM Warranties ORDER BY warranty_end ASC", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                warranties.Add(new Warranty
                {
                    Id = reader.GetInt32("id"),
                    OsCode = reader["os_number"].ToString(),
                    Customer = reader["customer"].ToString(),
                    SerialNumber = reader["serial_number"]?.ToString(),
                    Device = reader["Device"]?.ToString(),
                    ServiceDate = reader.GetDateTime(reader.GetOrdinal("service_date")),
                    WarrantyEndDate = reader.GetDateTime(reader.GetOrdinal("warranty_end")),
                    WarrantyMonths = reader.GetInt32(reader.GetOrdinal("warranty_length")),
                    Observation = reader["description"]?.ToString()
                });
            }

            return warranties;
        }

        public async Task<List<Warranty>> GetWarrantyBySearchAsync(string searchText, CancellationToken token = default)
        {
            var warranties = new List<Warranty>();

            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync(token);

                using var command = new NpgsqlCommand(
                @"SELECT * FROM Warranties
                  WHERE serial_number LIKE '%' || :SearchText || '%'
                     OR os_number LIKE '%' || :SearchText || '%'
                     OR customer LIKE '%' || :SearchText || '%'",
                connection);

                command.Parameters.AddWithValue(":SearchText", searchText);

                using var reader = await command.ExecuteReaderAsync(token);

                while (await reader.ReadAsync(token))
                {
                    token.ThrowIfCancellationRequested();

                    warranties.Add(new Warranty
                    {
                        OsCode = reader["os_number"].ToString(),
                        Customer = reader["customer"].ToString(),
                        SerialNumber = reader["serial_number"]?.ToString(),
                        Device = reader["Device"]?.ToString(),
                        ServiceDate = reader.GetDateTime(reader.GetOrdinal("service_date")),
                        WarrantyEndDate = reader.GetDateTime(reader.GetOrdinal("warranty_end")),
                        WarrantyMonths = reader.GetInt32(reader.GetOrdinal("warranty_length")),
                        Observation = reader["description"]?.ToString()
                    });
                }
            }
            catch (OperationCanceledException)
            {
                return new List<Warranty>();
            }
            catch (Exception ex)
            {
                throw;
            }

            return warranties;
        }

        public async Task DeleteWarrantyByOsCodeAsync(string osCode)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM Warranties WHERE os_number = @os_number", connection);
            command.Parameters.AddWithValue("@os_number", osCode);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteWarrantyAsync(string serialNumber)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM Warranties WHERE serial_number = @serial_number", connection);
            command.Parameters.AddWithValue("@serial_number", serialNumber);

            await command.ExecuteNonQueryAsync();
        }

        public async Task SaveProtocolAsync(Protocol protocol)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(@"
                INSERT INTO protocols
                (customer, title, serial_number, protocol_code, device, description)
                VALUES
                (@customer, @title, @serial_number, @protocol_code, @device, @description)", connection
            );

            command.Parameters.AddWithValue("@customer", protocol.Customer);
            command.Parameters.AddWithValue("@title", protocol.Title);
            command.Parameters.AddWithValue("@serial_number", protocol.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@protocol_code", protocol.ProtocolCode ?? string.Empty);
            command.Parameters.AddWithValue("@device", protocol.Device ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@description", protocol.Description ?? string.Empty);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Protocol>> GetProtocolBySearchAsync(string searchText, CancellationToken token = default)
        {
            var protocols = new List<Protocol>();

            try
            {
                using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync(token);

                using var command = new NpgsqlCommand(@"
                  SELECT * FROM protocols
                  WHERE protocol_code LIKE @search_text || '%'
                     OR LOWER(customer) LIKE @search_text || '%'",
                connection);

                command.Parameters.AddWithValue("@search_text", searchText);

                using var reader = await command.ExecuteReaderAsync(token);

                while (await reader.ReadAsync(token))
                {
                    token.ThrowIfCancellationRequested();

                    protocols.Add(new Protocol
                    {
                        Customer = reader["customer"].ToString(),
                        Title = reader["title"].ToString(),
                        SerialNumber = reader["serial_number"]?.ToString(),
                        ProtocolCode = reader["protocol_code"].ToString(),
                        Device = reader["device"]?.ToString(),
                        Description = reader["description"].ToString()
                    });
                }
            }
            catch (OperationCanceledException)
            {
                return new List<Protocol>();
            }
            catch (Exception ex)
            {
                throw;
            }

            return protocols;
        }

        public async Task DeleteProtocolAsync(string protocol_code)
        {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            string prefix = protocol_code.Substring(0, 4);

            using var command = new NpgsqlCommand("DELETE FROM protocols WHERE protocol_code LIKE @prefix || '%'", connection);
            command.Parameters.AddWithValue("@prefix", prefix);

            await command.ExecuteNonQueryAsync();
        }
    }
}