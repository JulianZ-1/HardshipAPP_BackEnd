namespace HardshipAPI.Models
{
    public class HardshipInsert
    {
        public short HardshipTypeID { get; set; }
        public long DebtID { get; set; }
        public string? Comments { get; set; }
    }
    public class HardshipUpdate
    {
        public long HardshipID { get; set; }
        public short HardshipTypeID { get; set; }
        public string? Comments { get; set; }
    }
    public class Hardship
    {
        public long HardshipID { get; set; }
        public short HardshipTypeID { get; set; }
        public long DebtID { get; set; }
        public string? Comments { get; set; }
    }
}
