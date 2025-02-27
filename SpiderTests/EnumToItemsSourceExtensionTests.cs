using Xunit;
using SpiderGame; // <- Директива using
using System;
using System.Windows.Markup;
using System.Collections.Generic;

namespace SpiderTests
{
    public class EnumToItemsSourceExtensionTests
    {
        [Fact]
        public void ProvideValue_WithValidEnum_ReturnsFilteredItems()
        {
            // Arrange
            var extension = new EnumToItemsSourceExtension(typeof(WeaponType));

            // Act
            var result = extension.ProvideValue(null);

            // Assert
            var items = Assert.IsType<List<string>>(result);
            Assert.DoesNotContain("None", items);
            Assert.Equal(3, items.Count); // Knife, Web, Net
        }

        [Fact]
        public void ProvideValue_WithNonEnumType_ThrowsException()
        {
            // Arrange
            var extension = new EnumToItemsSourceExtension(typeof(string));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => extension.ProvideValue(null));
        }

        [Fact]
        public void ProvideValue_WithNullType_ThrowsException()
        {
            // Arrange
            var extension = new EnumToItemsSourceExtension(null);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => extension.ProvideValue(null));
        }
    }
}