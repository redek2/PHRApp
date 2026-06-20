using PHRApp.Models.DTOs;
using PHRApp.Services.Implementations;
using PHRApp.Tests.TestHelpers;

namespace PHRApp.Tests.Services
{
    public class CategoryServiceTests : IDisposable
    {
        private readonly TestDbContextFactory _db;
        private readonly CategoryService _sut;

        public CategoryServiceTests()
        {
            _db = new TestDbContextFactory();
            _sut = new CategoryService(_db.Context);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithEmptyName_ThrowsArgumentException()
        {
            var dto = new CreateCategoryDto { Name = "   " };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateCategoryAsync(dto));
        }

        [Fact]
        public async Task CreateCategoryAsync_WithValidName_ReturnsNewId()
        {
            var dto = new CreateCategoryDto { Name = "Wizyty kontrolne" };

            var id = await _sut.CreateCategoryAsync(dto);

            Assert.True(id > 0);
        }

        [Fact]
        public async Task CreateCategoryAsync_WithDuplicateNameDifferentCase_ThrowsInvalidOperationException()
        {
            await _sut.CreateCategoryAsync(new CreateCategoryDto { Name = "Szczepienia" });

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.CreateCategoryAsync(new CreateCategoryDto { Name = "SZCZEPIENIA" }));
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsCategoriesOrderedByName()
        {
            await _sut.CreateCategoryAsync(new CreateCategoryDto { Name = "Zabiegi" });
            await _sut.CreateCategoryAsync(new CreateCategoryDto { Name = "Badania" });

            var result = await _sut.GetAllCategoriesAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("Badania", result[0].Name);
            Assert.Equal("Zabiegi", result[1].Name);
        }

        public void Dispose() => _db.Dispose();
    }
}