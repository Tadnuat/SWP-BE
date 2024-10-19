using Microsoft.EntityFrameworkCore;
using KoiShipping.Repo.Entities;
using System;
using System.Threading.Tasks;
using KoiShipping.Repo.Repository;

namespace KoiShipping.Repo.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private KoiShippingContext _context;

        private GenericRepository<AdvancedService> _advancedService;
        private GenericRepository<AserviceOrderD> _aserviceOrderD;
        private GenericRepository<Notification> _notification;
        private GenericRepository<Customer> _customer;
        private GenericRepository<Order> _order;
        private GenericRepository<OrderDetail> _orderDetail;
        private GenericRepository<OrderStaff> _orderStaff;
        private GenericRepository<Service> _service;
        private GenericRepository<Staff> _staff;

        // Thêm các biến riêng tư cho Tracking và TrackingOrderD
        private GenericRepository<Tracking> _tracking;
        private GenericRepository<TrackingOrderD> _trackingOrderD;

        public UnitOfWork(KoiShippingContext context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Repository properties
        public GenericRepository<AdvancedService> AdvancedServiceRepository
        {
            get
            {
                if (_advancedService == null)
                {
                    _advancedService = new GenericRepository<AdvancedService>(_context);
                }
                return _advancedService;
            }
        }

        public GenericRepository<AserviceOrderD> AserviceOrderDRepository
        {
            get
            {
                if (_aserviceOrderD == null)
                {
                    _aserviceOrderD = new GenericRepository<AserviceOrderD>(_context);
                }
                return _aserviceOrderD;
            }
        }

        public GenericRepository<Customer> CustomerRepository
        {
            get
            {
                if (_customer == null)
                {
                    _customer = new GenericRepository<Customer>(_context);
                }
                return _customer;
            }
        }

        public GenericRepository<Notification> NotificationRepository
        {
            get
            {
                if (_notification == null)
                {
                    _notification = new GenericRepository<Notification>(_context);
                }
                return _notification;
            }
        }

        public GenericRepository<Order> OrderRepository
        {
            get
            {
                if (_order == null)
                {
                    _order = new GenericRepository<Order>(_context);
                }
                return _order;
            }
        }

        public GenericRepository<OrderDetail> OrderDetailRepository
        {
            get
            {
                if (_orderDetail == null)
                {
                    _orderDetail = new GenericRepository<OrderDetail>(_context);
                }
                return _orderDetail;
            }
        }

        public GenericRepository<OrderStaff> OrderStaffRepository
        {
            get
            {
                if (_orderStaff == null)
                {
                    _orderStaff = new GenericRepository<OrderStaff>(_context);
                }
                return _orderStaff;
            }
        }

        public GenericRepository<Service> ServiceRepository
        {
            get
            {
                if (_service == null)
                {
                    _service = new GenericRepository<Service>(_context);
                }
                return _service;
            }
        }

        public GenericRepository<Staff> StaffRepository
        {
            get
            {
                if (_staff == null)
                {
                    _staff = new GenericRepository<Staff>(_context);
                }
                return _staff;
            }
        }

        // Thêm repository cho Tracking và TrackingOrderD
        public GenericRepository<Tracking> TrackingRepository
        {
            get
            {
                if (_tracking == null)
                {
                    _tracking = new GenericRepository<Tracking>(_context);
                }
                return _tracking;
            }
        }

        public GenericRepository<TrackingOrderD> TrackingOrderDRepository
        {
            get
            {
                if (_trackingOrderD == null)
                {
                    _trackingOrderD = new GenericRepository<TrackingOrderD>(_context);
                }
                return _trackingOrderD;
            }
        }
    }
}
