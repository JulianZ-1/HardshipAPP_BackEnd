using HardshipAPI.Models;
using System.Data.SQLite;

namespace HardshipAPI.Services
{
    public interface IDebtService
    {
        Task<long> UpdateDebt(Debt debt);
        Debt? GetDebt(int debtId);
    }
    public class DebtService : IDebtService
    {
        private readonly ISQLiteService _sqliteService;

        public DebtService(ISQLiteService sqliteService)
        {
            _sqliteService = sqliteService;
        }
        public async Task<long> UpdateDebt(Debt debt)
        {
            await using var connection = _sqliteService.GetConnection();
            await connection.OpenAsync();

            const string updateDebtQuery = @"
            UPDATE Debt 
            SET
            Name = @Name,
            DOB = @DOB,
            Income = @Income,
            Expenses = @Expenses
            WHERE 
            DebtID = @DebtID";

            using var cmd = new SQLiteCommand(updateDebtQuery, connection);
            cmd.Parameters.AddWithValue("@Name", debt.Name);
            cmd.Parameters.AddWithValue("@DOB", debt.DOB);
            cmd.Parameters.AddWithValue("@Income", debt.Income ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Expenses", debt.Expenses ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DebtID", debt.DebtID);
            return await cmd.ExecuteNonQueryAsync();
        }

        public Debt? GetDebt(int debtId)
        {
            using var connection = _sqliteService.GetConnection();
            connection.Open();
            const string query = "SELECT * FROM Debt WHERE DebtID = @DebtID";

            using var cmd = new SQLiteCommand(query, connection);

            cmd.Parameters.AddWithValue("@DebtID", debtId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Debt
                {
                    DebtID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DOB = reader.GetString(2),
                    Income = reader.GetDecimal(3),
                    Expenses = reader.GetDecimal(4),
                };
            }

            return null;
        }

    }
}
