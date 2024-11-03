namespace KoiShipping.API.Models.DashboardModel
{
    public class DashboardResponse
    {
        public int TotalOrders { get; set; }
        public int InTransitOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal SatisfactionRate { get; set; }
    }

    public class MonthlyRevenueItem
    {
        public int Month { get; set; } // Month number (1-12)
        public decimal Revenue { get; set; } // Revenue for that month
    }
}
