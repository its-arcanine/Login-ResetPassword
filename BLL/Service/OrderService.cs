using AutoMapper;
using BLL.DTOs;
using DAL.Entities;
using DAL.Reposistories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class OrderService
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IMapper mapper;
        public OrderService(IGenericRepository<Order> orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return _orderRepository.GetAll();
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            var order = _orderRepository.GetSingle(o => o.OrderId == orderId);

            return order;
        }

        public async Task<ResponseDTO> CreateOrderAsync(OrderDTO orderRequest)
        {
            if (orderRequest == null)
            {
                throw new ArgumentNullException(nameof(orderRequest));
            }
            try
            {
                var order = mapper.Map<Order>(orderRequest);
                order.OrderId = Guid.NewGuid().ToString();
                order.OrderDate = DateTime.UtcNow;
                order.Status = true;
                _orderRepository.Create(order);
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = $"An error occurred while creating the order: {ex.Message}" };
            }
            return new ResponseDTO { Success = true, Message = "Order created successfully." };
        }

        public async Task<ResponseDTO> GetOrderByAccountIdAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            try
            {
                var orders = _orderRepository.Get(o => o.AccountId == accountId);
                if (orders == null || !orders.Any())
                {
                    return new ResponseDTO { Success = false, Message = "No orders found for this account." };
                }
                var orderDTOs = orders.Select(o => mapper.Map<OrderDTO>(o)).ToList();
                return new ResponseDTO { Success = true, Result = orderDTOs };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseDTO> FilterDateOrder(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                throw new ArgumentNullException("Start date and end date cannot be empty.");
            }

            try
            {
                var orders = _orderRepository.Get(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
                if (orders == null || !orders.Any())
                {
                    return new ResponseDTO { Success = false, Message = "No orders found in the specified date range." };
                }
                var orderDTOs = orders.Select(o => mapper.Map<OrderDTO>(o)).ToList();
                return new ResponseDTO { Success = true, Result = orderDTOs };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseDTO> SumAllOrder(DateTime startDate, DateTime endDate)
        {
            if (startDate ==  default || endDate == default)
            {
                throw new ArgumentNullException("Start date and end date cannot be empty.");
            }

            try
            {
                var orders = _orderRepository.Get(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
                if (orders == null || !orders.Any())
                {
                    return new ResponseDTO { Success = false, Message = "No orders found in the specified date range." };
                }
                var totalAmount = orders.Sum(o => o.TotalAmount);
                return new ResponseDTO { Success = true, Result = totalAmount };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }


    }
}
