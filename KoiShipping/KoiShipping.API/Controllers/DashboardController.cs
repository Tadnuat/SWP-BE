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

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardResponse>> GetDashboardData(DateTime? fromDate, DateTime? toDate)
        {
            // Đặt fromDate thành đầu ngày
            if (fromDate.HasValue)
            {
                fromDate = fromDate.Value.Date; // Đầu ngày: 00:00:00
            }

            // Đặt toDate thành cuối ngày
            if (toDate.HasValue)
            {
                toDate = toDate.Value.Date.AddDays(1).AddSeconds(-1); // Cuối ngày: 23:59:59
            }
            var totalOrders =  _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false
                                                                               && od.Status.ToLower() != "canceled"
                                                                               && (!fromDate.HasValue || od.CreatedDate >= fromDate)
                                                                               && (!toDate.HasValue || od.CreatedDate <= toDate))
                                                                      .Count();

            var inTransitOrders =  _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false
                                                                                   && od.Status.ToLower() == "delivering"
                                                                                   && (!fromDate.HasValue || od.CreatedDate >= fromDate)
                                                                                   && (!toDate.HasValue || od.CreatedDate <= toDate))
                                                                        .Count();

            var pendingOrders =  _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false
                                                                                 && od.Status.ToLower() == "pending"
                                                                                 && (!fromDate.HasValue || od.CreatedDate >= fromDate)
                                                                                 && (!toDate.HasValue || od.CreatedDate <= toDate))
                                                                      .Count();

            var totalRevenue =  _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false
                                                                                && od.Status.ToLower() == "finish"
                                                                                && (!fromDate.HasValue || od.CreatedDate >= fromDate)
                                                                                && (!toDate.HasValue || od.CreatedDate <= toDate))
                                                                     .Sum(od => od.Price);

            var totalOrdersWithRating =  _unitOfWork.OrderDetailRepository.Get(od => od.Rating.HasValue
                                                                                        && od.DeleteStatus == false
                                                                                        && od.Status == "finish"
                                                                                        && (!fromDate.HasValue || od.CreatedDate >= fromDate)
                                                                                        && (!toDate.HasValue || od.CreatedDate <= toDate))
                                                                              .Count();

            var satisfiedOrders =  _unitOfWork.OrderDetailRepository.Get(od => od.Rating >= 4
                                                                                   && od.DeleteStatus == false
                                                                                   && (!fromDate.HasValue || od.CreatedDate >= fromDate)
                                                                                   && (!toDate.HasValue || od.CreatedDate <= toDate))
                                                                         .Count();

            var satisfactionRate = totalOrdersWithRating > 0
                ? (decimal)satisfiedOrders / totalOrdersWithRating * 100
                : 0;

            return Ok(new DashboardResponse
            {
                TotalOrders = totalOrders,
                InTransitOrders = inTransitOrders,
                PendingOrders = pendingOrders,
                TotalRevenue = totalRevenue,
                SatisfactionRate = satisfactionRate
            });
        }
        // 6. Tính doanh thu theo từng tháng trong năm cho phép nhập năm
        [HttpGet("monthly-revenue/{year}")]
        public async Task<ActionResult<List<MonthlyRevenueItem>>> GetMonthlyRevenue(int year)
        {
            decimal[] monthlyRevenue = new decimal[12];

            var orderDetails = await Task.Run(() =>
                _unitOfWork.OrderDetailRepository.Get(od => od.DeleteStatus == false && od.CreatedDate.Year == year && od.Status == "finish"));

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
