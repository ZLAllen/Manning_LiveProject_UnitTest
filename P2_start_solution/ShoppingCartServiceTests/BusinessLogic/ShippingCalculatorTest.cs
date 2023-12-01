using FluentAssertions;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using CartTuple = System.Tuple<ShoppingCartServiceTests.BusinessLogic.ShippingCalculatorTest.AddressType, ShoppingCartService.Models.CustomerType, ShoppingCartService.Models.ShippingMethod>;

namespace ShoppingCartServiceTests.BusinessLogic
{

    public class ShippingCalculatorTest
    {

        public enum AddressType
        {
            SameCity,
            SameCountry,
            International
        }

        private static readonly Address SameCityAddress = new()
        {
            Country = "USA",
            City = "New York City"
        };

        private static readonly Address SameCountryAddress = new()
        {
            Country = "USA",
            City = "Chicago"
        };

        private static readonly Address InternationAddress = new()
        {
            Country = "Germany"
        };

        public static readonly Dictionary<AddressType, Address> Addresses = new()
        {
            {AddressType.SameCity,  SameCityAddress},
            {AddressType.SameCountry, SameCountryAddress},
            {AddressType.International, InternationAddress}
        };

        private readonly Dictionary<Tuple<AddressType, CustomerType, ShippingMethod>, double> ShippingCosts = new()
        {
            {new CartTuple(AddressType.SameCity, CustomerType.Standard, ShippingMethod.Standard),  6.0d },
            {new CartTuple(AddressType.SameCity, CustomerType.Standard, ShippingMethod.Expedited),  7.2d },
            {new CartTuple(AddressType.SameCity, CustomerType.Standard, ShippingMethod.Priority),  12.0d },
            {new CartTuple(AddressType.SameCity, CustomerType.Standard, ShippingMethod.Express),  15.0d },
            {new CartTuple(AddressType.SameCity, CustomerType.Premium, ShippingMethod.Standard),  6.0d },
            {new CartTuple(AddressType.SameCity, CustomerType.Premium, ShippingMethod.Expedited),  6.0d },
            {new CartTuple(AddressType.SameCity, CustomerType.Premium, ShippingMethod.Priority),  6.0d },
            {new CartTuple(AddressType.SameCity, CustomerType.Premium, ShippingMethod.Express),  15.0d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Standard, ShippingMethod.Standard),  12.0d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Standard, ShippingMethod.Expedited),  14.4d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Standard, ShippingMethod.Priority),  24.0d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Standard, ShippingMethod.Express),  30.0d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Premium, ShippingMethod.Standard),  12.0d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Premium, ShippingMethod.Expedited),  12.0d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Premium, ShippingMethod.Priority),  12.0d },
            {new CartTuple(AddressType.SameCountry, CustomerType.Premium, ShippingMethod.Express),  30.0d },
            {new CartTuple(AddressType.International, CustomerType.Standard, ShippingMethod.Standard),  90.0d },
            {new CartTuple(AddressType.International, CustomerType.Standard, ShippingMethod.Expedited),  108.0d },
            {new CartTuple(AddressType.International, CustomerType.Standard, ShippingMethod.Priority),  180.0d },
            {new CartTuple(AddressType.International, CustomerType.Standard, ShippingMethod.Express),  225.0d },
            {new CartTuple(AddressType.International, CustomerType.Premium, ShippingMethod.Standard),  90.0d },
            {new CartTuple(AddressType.International, CustomerType.Premium, ShippingMethod.Expedited),  90.0d },
            {new CartTuple(AddressType.International, CustomerType.Premium, ShippingMethod.Priority),  90.0d },
            {new CartTuple(AddressType.International, CustomerType.Premium, ShippingMethod.Express),  225.0d },
        };

        public class CartTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var allAddressTypes = Enum.GetValues(typeof(AddressType)).Cast<AddressType>();
                var allCustomerTypes = Enum.GetValues(typeof(CustomerType)).Cast<CustomerType>();
                var allShippingMethods = Enum.GetValues(typeof(ShippingMethod)).Cast<ShippingMethod>();
                var objs = from addressType in allAddressTypes
                            from customerType in allCustomerTypes
                            from shippingMethod in allShippingMethods
                            select new object[] { new Cart { ShippingAddress = Addresses[addressType], CustomerType = customerType, ShippingMethod = shippingMethod, Items = CreateCartItems()},
                            new CartTuple(addressType, customerType, shippingMethod)};

                foreach (var obj in objs)
                {
                    yield return obj;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public List<Item> CreateCartItems() => new()
            {
                new() {Quantity = 1},
                new() {Quantity = 2},
                new() {Quantity = 3},
            };
        }

        public ShippingCalculator CreateShippingCalculator(Address address)
        {
            return new ShippingCalculator(address);
        }

        [Theory]
        [ClassData(typeof(CartTestData))]
        public void Can_calculate_shipping_cost(Cart cart, CartTuple cartTuple)
        {
            ShippingCalculator sut = CreateShippingCalculator(SameCityAddress);

            double result = sut.CalculateShippingCost(cart);

            result.Should().BeApproximately(ShippingCosts[cartTuple], 0.01d);
        }

    }
}
