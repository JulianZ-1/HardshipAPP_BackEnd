using System.Data.SQLite;

namespace HardshipAPI.Services
{
    public interface ISQLiteService
    {
        SQLiteConnection GetConnection();
        void InitializeDatabase();
    }

    public class SQLiteService : ISQLiteService
    {
        private readonly string _connectionString;
        private readonly string _dbPath;

        public SQLiteService(IConfiguration configuration, IHostEnvironment env)
        {
            _dbPath = Path.Combine(env.ContentRootPath, "HardshipData.db");
            _connectionString = configuration.GetConnectionString("SQLite");
            InitializeDatabase();
        }

        public SQLiteConnection GetConnection() => new SQLiteConnection(_connectionString);

        public void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
                using var connection = GetConnection();
                connection.Open();

                CreateTables(connection);
            }
        }

        private static void CreateTables(SQLiteConnection connection)
        {
            // Create Debt table
            ExecuteNonQuery(connection, @"
            CREATE TABLE IF NOT EXISTS Debt (
                DebtID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                DOB TEXT NOT NULL,
                Income DECIMAL,
                Expenses DECIMAL
            )");

            // Create HardshipTypes table
            ExecuteNonQuery(connection, @"
            CREATE TABLE IF NOT EXISTS HardshipTypes (
                HardshipTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            )");

            // Create Hardship table
            ExecuteNonQuery(connection, @"
            CREATE TABLE IF NOT EXISTS Hardship (
                HardshipID INTEGER PRIMARY KEY AUTOINCREMENT,
                HardshipTypeID SMALLINT NOT NULL,
                DebtID INTEGER NOT NULL,
                Comments TEXT,
                FOREIGN KEY(HardshipTypeID) REFERENCES HardshipTypes(HardshipTypeID),
                FOREIGN KEY(DebtID) REFERENCES Debt(DebtID)
            )"
            );

            // Insert default hardship types
            ExecuteNonQuery(connection, @"
            INSERT INTO HardshipTypes (Name)
            VALUES
                ('Financial'),
                ('Medical'),
                ('Economic');
            
            ");

            ExecuteNonQuery(connection, @"
            INSERT INTO Debt (Name, DOB, Income, Expenses)
            VALUES
                 ('Test Acount1', '1998-03-31',NULL,NULL),
                 ('Test Acount2', '1998-03-31',NULL,NULL),
                 ('Test Acount3', '1998-03-31',NULL,NULL),
                 ('Test Acount4', '1998-03-31',NULL,NULL),
                 ('Test Acount5', '1998-03-31',NULL,NULL),
                 ('Test Acount6', '1998-03-31',NULL,NULL),
                 ('Test Acount7', '1998-03-31',NULL,NULL),
                 ('Test Acount8', '1998-03-31',NULL,NULL),
                 ('Test Acount9', '1998-03-31',NULL,NULL);
            ");

        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string query)
        {
            using var cmd = new SQLiteCommand(query, connection);
            cmd.ExecuteNonQuery();
        }
    }
}
