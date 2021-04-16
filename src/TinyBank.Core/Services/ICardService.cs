using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TinyBank.Core.Model;

namespace TinyBank.Core.Services
{
    public interface ICardService
    {
        public ApiResult<Card> Create(string cardnumber,
            Options.CreateCardOptions options);

        public ApiResult<Options.CheckoutCardOptions> CheckOut(Options.CheckoutCardOptions options);
    }
}