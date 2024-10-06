using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShipping.Repo.Entities
{
    public partial class Tracking
    {
        public int TrackingId { get; set; } // Khóa chính
        public string TrackingName { get; set; } // Tên theo dõi

        // Mối quan hệ với TrackingOrderD
        public ICollection<TrackingOrderD> TrackingOrderDs { get; set; }
    }
}
