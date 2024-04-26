namespace Capstone.Models.ViewModels.SubModels
{
    public class SingleStat
    {
        public int Score { get; set; }
        public int Amount { get; set; }
        public string? Status { get; set; }
    }


    public class Stats
    {
        public List<SingleStat> Score { get; set; } = new List<SingleStat>();
        public List<SingleStat> Status { get; set; } = new List<SingleStat>();
    }
}
