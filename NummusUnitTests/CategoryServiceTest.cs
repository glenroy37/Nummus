using System.Collections.Generic;
using System.Linq;
using Nummus.Data;
using Nummus.Exception;
using Nummus.Service;
using NUnit.Framework;

namespace NummusUnitTests {
    public class CategoryServiceTest {
        private CategoryService _categoryService;
        private NummusDbContext _nummusDbContext;
        private NummusUserService _nummusUserService;

        [SetUp]
        public void SetUp() {
            _nummusDbContext = TestDbContextBuilder.InMemoryContext();
            _nummusUserService = TestUserInitialiser.InitialiseTestUserService(_nummusDbContext);
            _categoryService = new CategoryService(_nummusDbContext, _nummusUserService);
        }

        [Test]
        public void TestGetAllCategories() {
            SetUpCategories();
            var categories = _categoryService.GetAllCategories();
            Assert.AreEqual(2, categories.Length);
            Assert.AreEqual("cat2", categories[0].Description);
            Assert.AreEqual("cat1", categories[1].Description);
        }

        [Test]
        public void TestCantCreateCategoryTwice() {
            SetUpCategories();
            var newCategory = new Category {
                Description = "cat1",
                CategoryType = CategoryType.INCOME
            };
            Assert.Throws<NummusCategoryAlreadyExistsException>(() => {
                _categoryService.CreateCategory(newCategory);
            });
        }

        [Test]
        public void TestCanCreateCategoryWhichAnotherUserHas() {
            SetUpCategories();
            _categoryService.CreateCategory(new Category {
                Description = "cat3",
                CategoryType = CategoryType.INCOME
            });
            var createdCategory = _categoryService.GetAllCategories()[2];
            Assert.AreEqual("cat3", createdCategory.Description);
        }

        [Test]
        public void TestChangeCategoryName() {
            SetUpCategories();
            var category = _nummusDbContext.Categories
                .First(it => it.Description == "cat1");
            category.Description = "cat4711";
            _categoryService.UpdateCategory(category);
            var updatedCategory = _nummusDbContext.Categories
                .First(it => it.Description == "cat4711");
            Assert.AreEqual(category.Id, updatedCategory.Id);
        }

        [Test]
        public void TestChangeCategoryNameToExistingName() {
            SetUpCategories();
            var category = _nummusDbContext.Categories
                .First(it => it.Description == "cat1");
            category.Description = "cat2";
            Assert.Throws<NummusCategoryAlreadyExistsException>(() => {
                _categoryService.UpdateCategory(category);
            });
        }

        [Test]
        public void TestChangeCategoryNameToExistingNameOfOtherUser() {
            SetUpCategories();
            var category = _nummusDbContext.Categories
                .First(it => it.Description == "cat1");
            category.Description = "cat3";
            _categoryService.UpdateCategory(category);
            var updatedCategory = _nummusDbContext.Categories
                .Where(it => it.NummusUser == _nummusUserService.CurrentNummusUser)
                .First(it => it.Description == "cat3");
            Assert.AreEqual(category.Id, updatedCategory.Id);
        }

        [Test]
        public void TestCanDeleteEmptyCategory() {
            SetUpCategories();
            var category = _nummusDbContext.Categories
                .First(it => it.Description == "cat1");
            _categoryService.DeleteCategory(category);
            Assert.IsFalse(_nummusDbContext.Categories
                .Any(it => it.Description == "cat1"));
        }

        [Test]
        public void TestCantDeleteCategoryWithBookingLines() {
            SetUpCategories();
            var category = _nummusDbContext.Categories
                .First(it => it.Description == "cat1");
            SetUpBookingLine(category);
            Assert.Throws<NummusCategoryContainsBookingLinesException>(() => {
                _categoryService.DeleteCategory(category);
            });
        }

        [Test]
        public void TestIsDeletable() {
            SetUpCategories();
            var category = _nummusDbContext.Categories
                .First(it => it.Description == "cat1");
            Assert.True(_categoryService.IsDeletable(category));
            SetUpBookingLine(category);
            Assert.False(_categoryService.IsDeletable(category));
        }
        
        private void SetUpCategories() {
            var category1 = new Category {
                Description = "cat1",
                CategoryType = CategoryType.INCOME,
                NummusUser = _nummusUserService.CurrentNummusUser
            };
            var category2 = new Category {
                Description = "cat2",
                CategoryType = CategoryType.EXPENSE,
                NummusUser = _nummusUserService.CurrentNummusUser
            };
            var category3 = new Category() {
                Description = "cat3",
                CategoryType = CategoryType.CARRY
            };
            _nummusDbContext.AddRange(new List<Category> { category1, category2, category3 });
            _nummusDbContext.SaveChanges();
        }

        private void SetUpBookingLine(Category category) {
            var account = new Account("test", _nummusUserService.CurrentNummusUser);
            _nummusDbContext.Accounts.Add(account);
            var bookingLine = new BookingLine {
                Category = category,
                BookingText = "test",
                Account = account
            };
            _nummusDbContext.BookingLines.Add(bookingLine);
            _nummusDbContext.SaveChanges();
        }
    }
}