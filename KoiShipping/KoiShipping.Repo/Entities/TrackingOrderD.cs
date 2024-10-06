using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShipping.Repo.Entities
{
    public partial class TrackingOrderD
    {
        public int TrackingOrderDId { get; set; } // Khóa chính
        public int OrderDetailId { get; set; } // Khóa ngoại
        public int TrackingId { get; set; } // Khóa ngoại
        public DateTime Date { get; set; } // Ngày theo dõi

        // Mối quan hệ với OrderDetail
        public OrderDetail OrderDetail { get; set; }

        // Mối quan hệ với Tracking
        public Tracking Tracking { get; set; }
    }

}
