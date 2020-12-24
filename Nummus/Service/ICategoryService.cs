using System.Collections.Generic;
using Nummus.Data;

namespace Nummus.Service {
    public interface ICategoryService {
        public Category[] GetAllCategories();
        public void CreateCategory(Category categoryName);
        public void UpdateCategory(Category category);
        public void DeleteCategory(Category category);

        public bool IsDeletable(Category category);
    }
}