using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KoiShipping.API
{
    [Authorize] // Đảm bảo rằng chỉ những người dùng có xác thực mới có thể truy cập
    public class OrderHub : Hub
    {
        // Phương thức này sẽ được gọi khi có đơn hàng mới
        public async Task NotifyNewOrderDetail(string message)
        {
            // Gửi thông báo đến tất cả các client đã kết nối với vai trò Manager và Sale Staff
            await Clients.Users("Manager", "Sale Staff").SendAsync("ReceiveOrderNotification", message);
        }
    }
}
