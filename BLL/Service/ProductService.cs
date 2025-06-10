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
    public class ProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IGenericRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return  _productRepository.GetAll();
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException(nameof(productId));
            }

            var product = _productRepository.GetSingle(p => p.ProductId == productId);


            return product;
        }

        public async Task<ResponseDTO> CreateProductAsync(ProductDTO productRequest)
        {
           
            if (productRequest == null)
            {
                throw new ArgumentNullException(nameof(productRequest));
            }

            try
            { 
                var product = _mapper.Map<Product>(productRequest);
                product.CreatedAt = DateTime.UtcNow;
                product.IsActive = true;
                product.ProductId = Guid.NewGuid().ToString();
                _productRepository.Create(product);
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = $"An error occurred while creating the product: {ex.Message}" };
            }

            return new ResponseDTO { Success = true, Message = "Product created successfully." };
        }

        public async Task<ResponseDTO> UpdateProductAsync(string productId, ProductDTO productRequest)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException(nameof(productId));
            }

            if (productRequest == null)
            {
                throw new ArgumentNullException(nameof(productRequest));
            }

            var existingProduct = _productRepository.GetSingle(p => p.ProductId == productId);
            if (existingProduct == null)
            {
                return new ResponseDTO { Success = false, Message = "Product not found." };
            }

            try
            {
                existingProduct.ProductName = productRequest.ProductName;
                existingProduct.ProductDescription = productRequest.ProductDescription;
                existingProduct.ProductPrice = productRequest.ProductPrice;
                existingProduct.CategoryId = productRequest.CategoryId;
                existingProduct.ProductQuantity = productRequest.ProductQuantity;
                existingProduct.ProductImageUrl = productRequest.ProductImageUrl;
                existingProduct.CreatedAt = DateTime.UtcNow;

                _productRepository.Update(existingProduct);
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = $"An error occurred while updating the product: {ex.Message}" };
            }

            return new ResponseDTO { Success = true, Message = "Product updated successfully." };
        }


        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                throw new ArgumentNullException(nameof(categoryId));
            }

            return _productRepository.Get(p => p.CategoryId == categoryId);
        }


        public async Task<ResponseDTO> DeleteProductAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException(nameof(productId));
            }

            var existingProduct = _productRepository.GetSingle(p => p.ProductId == productId);
            if (existingProduct == null)
            {
                return new ResponseDTO { Success = false, Message = "Product not found." };
            }

            try
            {
                _productRepository.Delete(existingProduct);
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = $"An error occurred while deleting the product: {ex.Message}" };
            }

            return new ResponseDTO { Success = true, Message = "Product deleted successfully." };
        }

    }
}
