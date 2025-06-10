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
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<CartItem> _cartItemRepository;
        private readonly IMapper _mapper;

        public CartService(IGenericRepository<Cart> cartRepository, IMapper mapper, IGenericRepository<Product> productRepository, IGenericRepository<CartItem> cartItemRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<ResponseDTO> GetCartItemByCartIdAsync(string cartId)
        {
            if (string.IsNullOrEmpty(cartId))
            {
                throw new ArgumentNullException(nameof(cartId));
            }

            try
            {
                var cartItem = _cartItemRepository.Get(c => c.CartId == cartId, c => c.Product);
                if (cartItem == null || !cartItem.Any())
                {
                    return new ResponseDTO { Success = false, Message = "No items found in the cart." };
                }
                var cartItemDTOs = cartItem.Select(ci => new CartItemDTO
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Price, // This is the calculated price from CartItem
                    ProductName = ci.Product.ProductName, // Use null-conditional operator in case Product wasn't included or is null
                    ProductImageUrl = ci.Product.ProductImageUrl

                }).ToList();
                return new ResponseDTO { Success = true, Result = cartItemDTOs };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = ex.Message };
            }
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

        public async Task<ResponseDTO> AddProduct(string cartId, string productId, int quantity )
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException("Cart ID and Product ID cannot be null or empty.");
            }

            try
            {
                var cart = _cartRepository.GetSingle(c => c.CartId == cartId, c => c.CartItems);
                if (cart == null)
                {
                    return new ResponseDTO { Success = false, Message = "Cart not found." };
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                var product = _productRepository.GetSingle(p => p.ProductId == productId);
               
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                    cartItem.Price = product.ProductPrice * cartItem.Quantity; // Update price based on new quantity
                }
                
                else
                {
                   
                    cart.CartItems.Add(new CartItem
                    {
                        CartItemId = Guid.NewGuid().ToString(),
                        CartId = cartId,
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.ProductPrice,
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

        public async Task<ResponseDTO> RemoveProduct(string cartId, string cartItemid)
        {
            if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(cartItemid))
            {
                throw new ArgumentNullException("Cart ID and Product ID cannot be null or empty.");
            }

            try
            {
                var cart = _cartRepository.GetSingle(c => c.CartId == cartId, c => c.CartItems);
                if (cart == null)
                {
                    return new ResponseDTO { Success = false, Message = "Cart not found." };
                }

                var cartItem = cart.CartItems.FirstOrDefault(c => c.CartItemId == cartItemid);
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
