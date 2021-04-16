using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;

using TinyBank.Core.Model;
using TinyBank.Core.Implementation.Data;

using Xunit;
using TinyBank.Core.Services;
using TinyBank.Core.Constants;

namespace TinyBank.Core.Tests
{
    public class CardServiceTests : IClassFixture<TinyBankFixture>
    {
        private readonly TinyBankDbContext _dbContext;
        private readonly ICardService _cards;

        public CardServiceTests(TinyBankFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _cards = fixture.GetService<ICardService>();
        }



        [Fact]
        public void Card_Checkout_Success()
        {
            var card = _dbContext.Set<Card>()
                .Where(c => c.Active && c.Expiration > DateTime.Now && c.Accounts.Where(a => a.State == AccountState.Active && a.Balance > 100).Any())
                .FirstOrDefault();
            Assert.NotNull(card);

            //var account = card.Accounts.Where(a => a.State == Constants.AccountState.Active && a.Balance > 100).SingleOrDefault();
            //Assert.NotNull(account);

            var res = _cards.CheckOut(
                new Services.Options.CheckoutCardOptions {
                    CardNumber = card.CardNumber,
                    ExpirationMonth = card.Expiration.Month.ToString(),
                    ExpirationYear = card.Expiration.Year.ToString(),
                    Amount = 100
                }
            );

            Assert.Equal(res.Code, ApiResultCode.Success);




        }
    }
}
