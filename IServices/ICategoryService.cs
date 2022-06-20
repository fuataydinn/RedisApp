using RedisApplication.Model;

namespace RedisApplication.IServices
{
    public interface ICategoryService
    {
        List<CategoryModel> GetAllCategory();
    }
}
