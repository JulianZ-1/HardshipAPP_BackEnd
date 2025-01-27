namespace HardshipAPI.Models
{
    public class HardshipManagementInsert
    {
        public short HardshipTypeID { get; set; }
        public int DebtID { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public string? Comments { get; set; }
    }
    
    public class HardshipManagementUpdate
    {
        public string? Comments { get; set; }
        public short HardshipTypeID { get; set; }
        public string Name { get; set; }  // Debt fields
        public string DOB { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }

    }

    public class HardshipManagementView
    {
        public string HardshipTypeName { get; set; }
        public int HardshipID { get; set; }
        public int DebtID { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public decimal? Income { get; set; }
        public decimal? Expenses { get; set; }
        public string? Comments { get; set; }

    }
}
