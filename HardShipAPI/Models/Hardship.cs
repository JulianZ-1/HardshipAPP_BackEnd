namespace HardshipAPI.Models
{
    public class HardshipInsert
    {
        public short HardshipTypeID { get; set; }
        public int DebtID { get; set; }
        public string? Comments { get; set; }
    }
    public class HardshipUpdate
    {
        public int HardshipID { get; set; }
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
