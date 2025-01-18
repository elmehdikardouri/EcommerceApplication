using EcomApplication.Models;

namespace EcomApplication.Interfaces
{
    public interface IProductService
    {
        Product GetProductById(int productId);
        IEnumerable<Product> GetAllProducts();
    }
}
