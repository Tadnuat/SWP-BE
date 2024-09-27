namespace KoiShipping.API.Models.DashboardModel
{
    public class TotalOrdersResponse
    {
        public int TotalOrders { get; set; }
    }

    public class InTransitOrdersResponse
    {
        public int InTransitOrders { get; set; }
    }

    public class PendingOrdersResponse
    {
        public int PendingOrders { get; set; }
    }

    public class TotalRevenueResponse
    {
        public decimal TotalRevenue { get; set; }
    }

    public class SatisfactionRateResponse
    {
        public decimal SatisfactionRate { get; set; } // Percentage
    }

    public class MonthlyRevenueItem
    {
        public int Month { get; set; } // Month number (1-12)
        public decimal Revenue { get; set; } // Revenue for that month
    }
}
