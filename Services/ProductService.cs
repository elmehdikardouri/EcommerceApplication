using EcomApplication.Data;
using EcomApplication.Interfaces;
using EcomApplication.Models;

namespace EcomApplication.Services
{
    public class ProductService : IProductService
    {
        private readonly EcommerceDbContext _dbContext;

        public ProductService(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Product GetProductById(int productId)
        {
            return _dbContext.Products.FirstOrDefault(p => p.Id == productId);
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _dbContext.Products.ToList();
        }
    }
}
