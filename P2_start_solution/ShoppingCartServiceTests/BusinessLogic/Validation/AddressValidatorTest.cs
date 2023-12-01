using FluentAssertions;
using ShoppingCartService.BusinessLogic.Validation;
using ShoppingCartService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartServiceTests.BusinessLogic.Validation
{
    public class TestDataGenerator
    {
        public static List<object?[]> GetInvalidAddressTestData() => new()
        {
            new object?[] {null},
            new object?[] {new Address {Country = string.Empty, City = "abc", Street = "abc"}},
            new object?[] {new Address {Country = "abc", City = string.Empty, Street = "abc"}},
            new object?[] {new Address {Country = "abc", City = "abc", Street = string.Empty}},
            new object?[] {new Address {Country = null, City = "abc", Street = "abc"}},
            new object?[] {new Address {Country = "abc", City = null, Street = "abc"}},
            new object?[] {new Address {Country = "abc", City = "abc", Street = null}},
        };
    }

    public class AddressValidatorTest
    {
        public AddressValidator CreateAddressValidator() => new AddressValidator();


        [Theory]
        [MemberData(nameof(TestDataGenerator.GetInvalidAddressTestData), MemberType = typeof(TestDataGenerator))]
        public void Can_detect_an_invalid_address(Address addr)
        {
            AddressValidator sut = CreateAddressValidator();

            bool result = sut.IsValid(addr);

            result.Should().BeFalse();
        }

        [Fact]
        public void Can_confirm_a_valid_address()
        {
            AddressValidator sut = CreateAddressValidator();
            Address addr = new() { Country = "abc", City = "abc", Street = "abc" };

            bool result = sut.IsValid(addr);

            result.Should().BeTrue();
        }
    }
}
