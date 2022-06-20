using RedisApplication.IServices;
using RedisApplication.Model;

namespace RedisApplication.Services
{
    public class CategoryService : ICategoryService
    {
        static List<CategoryModel> categories => new()
        {
            new CategoryModel { Id = 1, Name = "Electronic" },
            new CategoryModel { Id = 2, Name = "Clothes" }
        };

        public ICacheService CacheService { get; }

        public CategoryService(ICacheService cacheService)
        {
            CacheService = cacheService;
        }
        public List<CategoryModel> GetAllCategory()
        {
            return GetCategoriesFromCache();
        }
        private List<CategoryModel> GetCategoriesFromCache()
        {
            return CacheService.GetOrAdd("allcategories", () => { return categories; });

            //GetOrAdd fonksiyonunda iki parametre var ilki keyin ismi (string) diğeri de bir delegate (Func<T>)
        }

        //Verilerde tutarlılığı sağlamak için cache’de tutulan veri değiştiğinde ya direkt güncellemeliyiz ya da keyi silmeliyiz örneğin kategorilere bir ekleme yaptığımız zaman cache’deki veriyi güncellemezsek bir veri tutarsızlığı ortaya çıkar.
    }
}
