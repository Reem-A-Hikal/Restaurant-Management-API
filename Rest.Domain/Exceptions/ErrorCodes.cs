using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Domain.Exceptions
{
    public static class ErrorCodes
    {
        public const string KitchenClosed = "KITCHEN_CLOSED";
        public const string ProductUnavailable = "PRODUCT_UNAVAILABLE";
        public const string NoSavedAddress = "NO_SAVED_ADDRESS";
        public const string AddressNotOwned = "ADDRESS_NOT_OWNED";
        public const string OrderEmpty = "ORDER_EMPTY";
        public const string OrderCancelNotAllowed = "ORDER_CANCEL_NOT_ALLOWED";
    }
}
