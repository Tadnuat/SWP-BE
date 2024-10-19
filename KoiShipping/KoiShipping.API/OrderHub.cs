using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KoiShipping.API
{
    public class OrderHub : Hub
    {
        // Phương thức này sẽ được gọi khi có đơn hàng mới
        public async Task NotifyNewOrderDetail(string message)
        {
            // Gửi thông báo đến tất cả các client đã kết nối với vai trò Manager và Sale Staff
            await Clients.All.SendAsync("ReceiveOrderNotification", message);
        }
    }
}
