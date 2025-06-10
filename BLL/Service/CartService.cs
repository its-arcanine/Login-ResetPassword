using AutoMapper;
using BLL.DTOs;
using DAL.Entities;
using DAL.Reposistories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class CartService
    {
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IMapper _mapper;

        public CartService(IGenericRepository<Cart> cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<Cart> GetCartByIdAsync(string cartId)
        {
            if (string.IsNullOrEmpty(cartId))
            {
                throw new ArgumentNullException(nameof(cartId));
            }

            var cart = _cartRepository.GetSingle(c => c.CartId == cartId);
            if (cart == null)
            {
                return null;
            }

            return cart;
        }

        public async Task<ResponseDTO> CreateCartAsync(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            try
            {
                var cart = new Cart
                {
                    CartId = Guid.NewGuid().ToString(),
                    AccountId = accountId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _cartRepository.Create(cart);
                return new ResponseDTO { Success = true, Message = "Cart created successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseDTO> AddProduct(string cartId, string productId, int quantity = 1 )
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException("Cart ID and Product ID cannot be null or empty.");
            }

            try
            {
                var cart = _cartRepository.GetSingle(c => c.CartId == cartId);
                if (cart == null)
                {
                    return new ResponseDTO { Success = false, Message = "Cart not found." };
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        CartItemId = Guid.NewGuid().ToString(),
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = quantity
                    });
                }

                _cartRepository.Update(cart);
                return new ResponseDTO { Success = true, Message = "Product added to cart successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseDTO> RemoveProduct(string cartId, string productId)
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException("Cart ID and Product ID cannot be null or empty.");
            }

            try
            {
                var cart = _cartRepository.GetSingle(c => c.CartId == cartId);
                if (cart == null)
                {
                    return new ResponseDTO { Success = false, Message = "Cart not found." };
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cart.CartItems.Remove(cartItem);
                    _cartRepository.Update(cart);
                    return new ResponseDTO { Success = true, Message = "Product removed from cart successfully." };
                }
                else
                {
                    return new ResponseDTO { Success = false, Message = "Product not found in cart." };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResponseDTO> ClearCartAsync(string cartId)
        {
            if (string.IsNullOrEmpty(cartId))
            {
                throw new ArgumentNullException(nameof(cartId));
            }

            try
            {
                var cart = _cartRepository.GetSingle(c => c.CartId == cartId);
                if (cart == null)
                {
                    return new ResponseDTO { Success = false, Message = "Cart not found." };
                }

                cart.CartItems.Clear();
                _cartRepository.Update(cart);
                return new ResponseDTO { Success = true, Message = "Cart cleared successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }
        
        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartId)
        {
            if (string.IsNullOrEmpty(cartId))
            {
                throw new ArgumentNullException(nameof(cartId));
            }

            var cart = _cartRepository.GetSingle(c => c.CartId == cartId);
            if (cart == null)
            {
                return Enumerable.Empty<CartItem>();
            }

            return cart.CartItems;
        }

        public async Task<ResponseDTO> UpdateCartItemQuantityAsync(string cartId, string productId, int quantity)
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException("Cart ID and Product ID cannot be null or empty.");
            }

            try
            {
                var cart = _cartRepository.GetSingle(c => c.CartId == cartId);
                if (cart == null)
                {
                    return new ResponseDTO { Success = false, Message = "Cart not found." };
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    _cartRepository.Update(cart);
                    return new ResponseDTO { Success = true, Message = "Cart item quantity updated successfully." };
                }
                else
                {
                    return new ResponseDTO { Success = false, Message = "Product not found in cart." };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
        }
    }
}
