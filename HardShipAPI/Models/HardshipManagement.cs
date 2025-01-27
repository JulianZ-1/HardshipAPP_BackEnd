namespace HardshipAPI.Models
{
    public class HardshipManagementInsert
    {
        public short HardshipTypeID { get; set; }
        public long DebtID { get; set; }
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
        public long HardshipID { get; set; }
        public long DebtID { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public decimal? Income { get; set; }
        public decimal? Expenses { get; set; }
        public string? Comments { get; set; }

    }
}
