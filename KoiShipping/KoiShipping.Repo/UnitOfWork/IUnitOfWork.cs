using KoiShipping.Repo.Entities;
using KoiShipping.Repo.Repository;
using System;
using System.Threading.Tasks;

namespace KoiShipping.Repo.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        Task<int> SaveAsync();

        // Repository properties
        public GenericRepository<AdvancedService> AdvancedServiceRepository { get; }
        public GenericRepository<AserviceOrderD> AserviceOrderDRepository { get; }
        public GenericRepository<Customer> CustomerRepository { get; }
        public GenericRepository<Order> OrderRepository { get; }
        public GenericRepository<OrderDetail> OrderDetailRepository { get; }
        public GenericRepository<OrderStaff> OrderStaffRepository { get; }
        public GenericRepository<Service> ServiceRepository { get; }
        public GenericRepository<Staff> StaffRepository { get; }
    }
}
