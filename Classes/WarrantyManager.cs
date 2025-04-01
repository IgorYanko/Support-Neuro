using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using System.Threading.Tasks;
using NeuroApp.Services;

namespace NeuroApp.Classes
{
    public class WarrantyManager
    {
        private readonly string _connectionString;

        public WarrantyManager(IConfiguration configuration)
        {
            LoggingService.LogInformation("WarrantyManager sendo inicializado");
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            LoggingService.LogInformation($"String de conexão: {_connectionString}");
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoggingService.LogError("String de conexão não foi inicializada no WarrantyManager");
                throw new InvalidOperationException("String de conexão não foi inicializada");
            }
        }

        public async Task UpdateWarrantyAsync(Warranty warranty)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(@"
                UPDATE warranties SET
                    ClientName = @ClientName,
                    Device = @Device,
                    ServiceDate = @ServiceDate,
                    WarrantyEndDate = @WarrantyEndDate,
                    WarrantyMonths = @WarrantyMonths,
                    Observation = @Observation,
                    SerialNumber
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
            using var connection = new NpgsqlConnection(_connectionString);
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

            using var connection = new NpgsqlConnection(_connectionString);
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

        public async Task<Warranty> GetWarrantyByOsCodeAsync(string osCode)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM Warranties WHERE OsCode = @OsCode", connection);
            command.Parameters.AddWithValue("@OsCode", osCode);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Warranty
                {
                    OsCode = reader["OsCode"].ToString(),
                    ClientName = reader["ClientName"].ToString(),
                    SerialNumber = reader["SerialNumber"]?.ToString(),
                    Device = reader["DeviceDescription"]?.ToString(),
                    ServiceDate = reader.GetDateTime(reader.GetOrdinal("ServiceDate")),
                    WarrantyEndDate = reader.GetDateTime(reader.GetOrdinal("WarrantyEndDate")),
                    WarrantyMonths = reader.GetInt32(reader.GetOrdinal("WarrantyMonths")),
                    Observation = reader["Observation"]?.ToString()
                };
            }

            return null;
        }

        public async Task DeleteWarrantyByOsCodeAsync(string osCode)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM Warranties WHERE OsCode = @OsCode", connection);
            command.Parameters.AddWithValue("@OsCode", osCode);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteWarrantyAsync(string serialNumber)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("DELETE FROM Warranties WHERE serialnumber = @SerialNumber", connection);
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);

            await command.ExecuteNonQueryAsync();
        }
    }
} 