using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBank.Core.Constants;
using TinyBank.Core.Model;
using TinyBank.Core.Services;
using TinyBank.Core.Services.Options;


namespace TinyBank.Core.Implementation.Services
{
    class CardService : ICardService
    {

        private readonly IAccountService _accounts;
        private readonly Data.TinyBankDbContext _dbContext;

        public CardService(
            IAccountService accounts,
            Data.TinyBankDbContext dbContext)
        {
            _accounts = accounts;
            _dbContext = dbContext;
        }

        public ApiResult<CheckoutCardOptions> CheckOut(CheckoutCardOptions options)
        {
            try {
                if (options == null) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.BadRequest,
                        ErrorText = $"Null {nameof(options)}"
                    };
                }


                if (string.IsNullOrWhiteSpace(options.CardNumber)) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.BadRequest,
                        ErrorText = $"Null or empty {nameof(options.CardNumber)}"
                    };
                }

                var now = DateTime.Today;

                int ExpirationMonth = int.Parse(options.ExpirationMonth);
                int ExpirationYear = int.Parse(options.ExpirationYear);

                if (ExpirationMonth == 0 || ExpirationMonth > 12) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.BadRequest,
                        ErrorText = $"invalid expiration data"
                    };
                }

                if (options.Amount <= 0) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.BadRequest,
                        ErrorText = $"Invalid {nameof(options.Amount)}"
                    };
                }

                var r = GetByNumber(options.CardNumber);
                if (r.Code != ApiResultCode.Success) {
                    return new ApiResult<CheckoutCardOptions> { Code = r.Code, ErrorText = r.ErrorText };
                }


                var card = r.Data;

                if (card.Expiration.Year != ExpirationYear || card.Expiration.Month != ExpirationMonth) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.Forbidden,
                        ErrorText = $"Transaction denied"
                    };
                }

                if (card.Expiration.Year < now.Year || (card.Expiration.Year == now.Year && card.Expiration.Month < now.Month)) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.Forbidden,
                        ErrorText = $"Card has expired"
                    };
                }

                if (!card.Active) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.Forbidden,
                        ErrorText = $"Card is inactive"
                    };
                }

                if (card.Accounts.Count == 0) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.Forbidden,
                        ErrorText = $"Card has no connected accounts"
                    };
                }



                //assume first active account
                var account = card.Accounts.AsQueryable().Where(a => a.State == AccountState.Active).SingleOrDefault();

                if (account == null) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.Forbidden,
                        ErrorText = $"no active accounts"
                    };
                }


                if (account.Balance < options.Amount) {
                    return new ApiResult<CheckoutCardOptions>() {
                        Code = ApiResultCode.Forbidden,
                        ErrorText = $"Issuficient amount"
                    };
                }

                account.Balance -= options.Amount;
                _dbContext.SaveChanges();

                return new ApiResult<CheckoutCardOptions> { Code = ApiResultCode.Success, Data = options };
            }
            catch (Exception e) {
                return new ApiResult<CheckoutCardOptions>() {
                    Code = ApiResultCode.InternalServerError,
                    ErrorText = $"General Error {e.Message}" //not for production
                };
            }

        }

        public ApiResult<Card> GetByNumber(string cardnumber)
        {
            if (string.IsNullOrWhiteSpace(cardnumber)) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.BadRequest,
                    ErrorText = $"Invalid cardnumber"
                };
            }

            try {
                var card = _dbContext.Set<Card>()
                    .Where(c => c.CardNumber == cardnumber)
                    .Include(c => c.Accounts)
                    .SingleOrDefault();

                if (card == null) {
                    return new ApiResult<Card>() {
                        Code = ApiResultCode.NotFound,
                        ErrorText = $"card doesn't exist"
                    };
                }

                return new ApiResult<Card>() {
                    Data = card,
                    ErrorText = "",
                    Code = ApiResultCode.Success

                };
            }
            catch (Exception) {
                return new ApiResult<Card>() {
                    Code = ApiResultCode.InternalServerError,
                    ErrorText = $"Internal error"
                };
            }

        }

        public ApiResult<Card> Create(string cardnumber, CreateCardOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
