namespace FoxOffice.Admin.Models
{
    using FluentAssertions;
    using FoxOffice.ReadModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ScreeningViewModel_specs
    {
        [TestMethod, AutoData]
        public void Translate_sets_simple_type_properties_correctly(
            ScreeningDto source)
        {
            // Act
            var actual = ScreeningViewModel.Translate(source);

            // Assert
            actual.Should().BeEquivalentTo(new
            {
                source.Id,
                source.TheaterId,
                source.TheaterName,
                source.ScreeningTime,
                source.DefaultFee,
                source.ChildrenFee,
                source.CreatedAt,
            });
        }

        [TestMethod]
        public void given_source_is_null_then_Translate_returns_null()
        {
            ScreeningDto source = default;
            var actual = ScreeningViewModel.Translate(source);
            actual.Should().BeNull();
        }
    }
}
