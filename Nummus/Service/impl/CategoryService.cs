using System.Collections.Generic;
using System.Linq;
using Nummus.Data;
using Nummus.Exception;

namespace Nummus.Service {
    public class CategoryService : ICategoryService {
        private readonly NummusDbContext _nummusDbContext;
        private readonly NummusUserService _nummusUserService;

        public CategoryService(NummusDbContext nummusDbContext, NummusUserService nummusUserService) {
            _nummusDbContext = nummusDbContext;
            _nummusUserService = nummusUserService;
        }
        
        public Category[] GetAllCategories() {
            return _nummusDbContext.Categories
                .Where(it => it.NummusUser.Id == _nummusUserService.CurrentNummusUser.Id)
                .ToArray();
        }

        public void CreateCategory(Category category) {
            if (_nummusDbContext.Categories
                .Where(it => it.NummusUser.Id == _nummusUserService.CurrentNummusUser.Id)
                .Any(it => it.Description == category.Description)) {
                throw new NummusCategoryAlreadyExistsException();
            }

            category.NummusUser = _nummusUserService.CurrentNummusUser;
            _nummusDbContext.Categories.Add(category);
            _nummusDbContext.SaveChanges();
        }

        public void UpdateCategory(Category category) {
            var existingCategory = _nummusDbContext.Categories
                .Where(it => it.NummusUser == _nummusUserService.CurrentNummusUser)
                .FirstOrDefault(it => it.Description == category.Description);

            if (existingCategory != null && existingCategory.Id != category.Id) {
                throw new NummusCategoryAlreadyExistsException();
            }
            
            _nummusDbContext.Categories.Update(category);
            _nummusDbContext.SaveChanges();
        }

        public void DeleteCategory(Category category) {
            if (_nummusDbContext.BookingLines.
                Any(bookingLine => bookingLine.Category.Id == category.Id)) {
                throw new NummusCategoryContainsBookingLinesException();
            }

            _nummusDbContext.Categories.Remove(category);
            _nummusDbContext.SaveChanges();
        }

        public bool IsDeletable(Category category) {
            return !_nummusDbContext.BookingLines.Any(bookingLine => bookingLine.Category.Id == category.Id);
        }
    }
}