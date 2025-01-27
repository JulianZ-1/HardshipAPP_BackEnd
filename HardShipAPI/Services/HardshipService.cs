using HardshipAPI.Models;
using System.Data.SQLite;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HardshipAPI.Services
{
    public interface IHardshipService
    {
        Task<long> AddHardship(HardshipInsert hardship);
        Task<long> UpdateHardship(HardshipUpdate hardship);
        Task<HardshipManagementView[]> ViewAllHardShips();
        Hardship? GetHardship(long hardshipId);
        Task<bool> DoesDebtHaveHardshipAsync(long debtId); // Add this line
        Task<HardshipManagementView?> GetHardshipByDebtIdAsync(long debtId); // Add this line

    }
    public class HardshipService : IHardshipService
    {
        private readonly ISQLiteService _sqliteService;

        public HardshipService(ISQLiteService sqliteService)
        {
            _sqliteService = sqliteService;
        }
        public async Task<long> AddHardship(HardshipInsert hardship)
        {
            await using var connection = _sqliteService.GetConnection();
            await connection.OpenAsync();

            const string insertHardshipQuery = @"
            INSERT INTO Hardship 
            (HardshipTypeID, DebtID, Comments)
            VALUES
            (@TypeID, @DebtID, @Comments);";

            using var cmd = new SQLiteCommand(insertHardshipQuery, connection);
            cmd.Parameters.AddWithValue("@TypeID", hardship.HardshipTypeID);
            cmd.Parameters.AddWithValue("@DebtID", hardship.DebtID);
            cmd.Parameters.AddWithValue("@Comments", hardship.Comments ?? (object)DBNull.Value);
            return await cmd.ExecuteNonQueryAsync();
        }
        public async Task<long> UpdateHardship(HardshipUpdate hardship)
        {
            await using var connection = _sqliteService.GetConnection();
            await connection.OpenAsync();

            const string updateHardshipQuery = @"
            UPDATE Hardship 
            SET
            HardshipTypeID = @HardshipTypeID,
            Comments = @Comments
            WHERE
            HardshipID = @HardshipID";

            using var cmd = new SQLiteCommand(updateHardshipQuery, connection);
            cmd.Parameters.AddWithValue("@HardshipTypeID", hardship.HardshipTypeID);
            cmd.Parameters.AddWithValue("@HardshipID", hardship.HardshipID);
            cmd.Parameters.AddWithValue("@Comments", hardship.Comments ?? (object)DBNull.Value);
            return await cmd.ExecuteNonQueryAsync();
        }

        public Hardship? GetHardship(long hardshipId)
        {
            using var connection = _sqliteService.GetConnection();
            connection.Open();
            const string query = "SELECT * FROM Hardship WHERE HardshipID = @HardshipID";

            using var cmd = new SQLiteCommand(query, connection);
            
            cmd.Parameters.AddWithValue("@HardshipID", hardshipId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Hardship
                {
                    HardshipID = reader.GetInt64(0),
                    HardshipTypeID = reader.GetInt16(1),
                    DebtID = reader.GetInt64(2),
                    Comments = reader.IsDBNull(3) ? null : reader.GetString(3)
                };
            }

            return null;
        }
        public async Task<HardshipManagementView[]> ViewAllHardShips()
        {
            await using var connection = _sqliteService.GetConnection();
            await connection.OpenAsync();
            var results = new List<HardshipManagementView>();

            const string viewAllHardShipsQuery = @"
            SELECT d.Name, d.DOB, d.Income, d.Expenses, h.Comments, h.DebtID, h.HardshipID, ht.Name AS HardshipTypeName
            FROM Hardship AS h
            INNER JOIN Debt AS d ON d.DebtID = h.DebtID
            INNER JOIN HardshipTypes AS ht ON ht.HardshipTypeID = h.HardshipTypeID
            ORDER BY h.HardshipID
            "
            ;

            using var cmd = new SQLiteCommand(viewAllHardShipsQuery, connection);
            using var reader = cmd.ExecuteReader();

            while (await reader.ReadAsync())
            {
                results.Add(new HardshipManagementView
                {
                    Name = reader.GetString(0),
                    DOB = reader.GetString(1),
                    Income = reader.GetDecimal(2),
                    Expenses = reader.GetDecimal(3),
                    Comments = reader.IsDBNull(4) ? null : reader.GetString(4),
                    DebtID = reader.GetInt32(5),
                    HardshipID = reader.GetInt32(6),
                    HardshipTypeName = reader.GetString(7)
                });
            }
            return results.ToArray();
        }
        public async Task<bool> DoesDebtHaveHardshipAsync(long debtId)
        {
            await using var connection = _sqliteService.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT 1 FROM Hardship WHERE DebtID = @DebtID LIMIT 1";
            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@DebtID", debtId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<HardshipManagementView?> GetHardshipByDebtIdAsync(long debtId)
        {
            await using var connection = _sqliteService.GetConnection();
            await connection.OpenAsync();

            const string query = @"
            SELECT d.Name, d.DOB, d.Income, d.Expenses, h.Comments, h.DebtID, h.HardshipID, ht.Name AS HardshipTypeName
            FROM Hardship AS h
            INNER JOIN Debt AS d ON d.DebtID = h.DebtID
            INNER JOIN HardshipTypes AS ht ON ht.HardshipTypeID = h.HardshipTypeID
            WHERE h.DebtID = @DebtID
            LIMIT 1"; // DebtID is unique, so only 1 result

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@DebtID", debtId);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new HardshipManagementView
                {
                    Name = reader.GetString(0),
                    DOB = reader.GetString(1),
                    Income = reader.GetDecimal(2),
                    Expenses = reader.GetDecimal(3),
                    Comments = reader.IsDBNull(4) ? null : reader.GetString(4),
                    DebtID = reader.GetInt32(5),
                    HardshipID = reader.GetInt32(6),
                    HardshipTypeName = reader.GetString(7)
                };
            }

            return null; // No matching record
        }
    }
}
