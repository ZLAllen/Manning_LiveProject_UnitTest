using AutoMapper;
using FluentAssertions;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Mapping;
using ShoppingCartService.Models;

namespace ShoppingCartServiceTests.BusinessLogic
{
    public class CheckOutEngineTest
    {
        public class ShippingCalculatorStub : IShippingCalculator
        {
            public double CalculateShippingCost(Cart cart) => ShippingCost;
        }

        private const double ShippingCost = 1.0d;
        private readonly IMapper _mapper;

        public CheckOutEngineTest()
        {
            MapperConfiguration config = new(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            
            _mapper = config.CreateMapper();
        }

        private CheckOutEngine CreateCheckoutEngine()
        {
            IShippingCalculator shippingCalculator = new ShippingCalculatorStub();
            return new(shippingCalculator, _mapper);
        }

        private Cart GenerateCartData(CustomerType type)
        {
            return new()
            {
                CustomerType = type,
                Items = new List<Item>
                {
                    new() { Price = 4.0d, Quantity = 1 },
                    new() { Price = 5.0d, Quantity = 1 },
                    new() { Price = 10.0d, Quantity = 1 }
                }
            };
        }

        [Fact]
        public void Premium_customer_pays_discounted_price()
        {
            CheckOutEngine sut = CreateCheckoutEngine();
            Cart cart = GenerateCartData(CustomerType.Premium);

            CheckoutDto result = sut.CalculateTotals(cart);
            CheckoutDto expect = new(_mapper.Map<ShoppingCartDto>(cart), ShippingCost, 10.0d, 18.0d);

            result.Should().BeEquivalentTo(expect);
        }

        [Fact]
        public void Standard_customer_pays_full_price()
        {
            CheckOutEngine sut = CreateCheckoutEngine();
            Cart cart = GenerateCartData(CustomerType.Standard);

            CheckoutDto result = sut.CalculateTotals(cart);
            CheckoutDto expect = new(_mapper.Map<ShoppingCartDto>(cart), ShippingCost, 0.0d, 20.0d);

            result.Should().BeEquivalentTo(expect);
        }
    }
}
