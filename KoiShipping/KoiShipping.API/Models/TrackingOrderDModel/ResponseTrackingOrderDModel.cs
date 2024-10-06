namespace KoiShipping.API.Models.TrackingOrderDModel
{
    public class ResponseTrackingOrderDModel
    {
        public int TrackingOrderDId { get; set; }
        public int OrderDetailId { get; set; }
        public int TrackingId { get; set; }
        public DateTime Date { get; set; }
    }
}
