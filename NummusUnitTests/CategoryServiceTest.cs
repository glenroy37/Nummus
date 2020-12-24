using System.Collections.Generic;
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
        public void GetAllCategories() {
            SetUpCategories();
            var categories = _categoryService.GetAllCategories();
            Assert.AreEqual(2, categories.Length);
            Assert.AreEqual("cat2", categories[0].Description);
            Assert.AreEqual("cat1", categories[1].Description);
        }

        [Test]
        public void CantCreateCategoryTwice() {
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
        public void CanCreateCategoryWhichAnotherUserHas() {
            SetUpCategories();
            _categoryService.CreateCategory(new Category {
                Description = "cat3",
                CategoryType = CategoryType.INCOME
            });
            var createdCategory = _categoryService.GetAllCategories()[2];
            Assert.AreEqual("cat3", createdCategory.Description);
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
    }
}