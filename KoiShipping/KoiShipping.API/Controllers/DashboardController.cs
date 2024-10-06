using KoiShipping.API.Models.DashboardModel;
using KoiShipping.Repo.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KoiShipping.API.Controllers
{
    [Authorize(Roles = "Manager")]
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. Tổng số đơn hàng với điều kiện status không phải "Canceled"
        [HttpGet("total-orders")]
        public async Task<ActionResult<TotalOrdersResponse>> GetTotalOrders()
        {
            var totalOrders = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false && od.Status.ToLower() != "canceled").Count());

            return Ok(new TotalOrdersResponse { TotalOrders = totalOrders });
        }

        // 2. Số đơn hàng đang vận chuyển với status là "In Transit"
        [HttpGet("in-transit-orders")]
        public async Task<ActionResult<InTransitOrdersResponse>> GetInTransitOrders()
        {
            var inTransitOrders = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false && od.Status.ToLower() == "in transit").Count());

            return Ok(new InTransitOrdersResponse { InTransitOrders = inTransitOrders });
        }

        // 3. Số đơn hàng đang chờ xử lý với status là "Pending"
        [HttpGet("pending-orders")]
        public async Task<ActionResult<PendingOrdersResponse>> GetPendingOrders()
        {
            var pendingOrders = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false && od.Status.ToLower() == "pending").Count());

            return Ok(new PendingOrdersResponse { PendingOrders = pendingOrders });
        }

        // 4. Tổng doanh thu từ những đơn hàng có status là "Delivered"
        [HttpGet("total-revenue")]
        public async Task<ActionResult<TotalRevenueResponse>> GetTotalRevenue()
        {
            var totalRevenue = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false && od.Status.ToLower() == "delivered")
                    .Sum(od => od.Price));

            return Ok(new TotalRevenueResponse { TotalRevenue = totalRevenue });
        }

        // 5. Tính tỉ lệ hài lòng với rating từ 4 trở lên
        [HttpGet("satisfaction-rate")]
        public async Task<ActionResult<SatisfactionRateResponse>> GetSatisfactionRate()
        {
            var totalOrdersWithRating = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.Rating.HasValue && od.DeleteStatus == false).Count());

            var satisfiedOrders = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.Rating >= 4 && od.DeleteStatus == false).Count());

            var satisfactionRate = totalOrdersWithRating > 0
                ? (decimal)satisfiedOrders / totalOrdersWithRating * 100
                : 0;

            return Ok(new SatisfactionRateResponse { SatisfactionRate = satisfactionRate });
        }

        // 6. Tính doanh thu theo từng tháng trong năm hiện tại
        // 6. Tính doanh thu theo từng tháng trong năm cho phép nhập năm
        [HttpGet("monthly-revenue/{year}")]
        public async Task<ActionResult<List<MonthlyRevenueItem>>> GetMonthlyRevenue(int year)
        {
            decimal[] monthlyRevenue = new decimal[12];

            var orderDetails = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false && od.CreatedDate.Year == year));

            for (int month = 1; month <= 12; month++)
            {
                monthlyRevenue[month - 1] = orderDetails
                    .Where(od => od.CreatedDate.Month == month)
                    .Sum(od => od.Price);
            }

            // Create a list of monthly revenue items for response
            var response = monthlyRevenue.Select((revenue, index) => new MonthlyRevenueItem
            {
                Month = index + 1,
                Revenue = revenue
            }).ToList();

            return Ok(response);
        }

    }
}
