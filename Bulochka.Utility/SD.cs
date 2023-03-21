using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulochka.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Клиент";
        public const string Role_Employee = "Работник";
        public const string Role_Admin = "Администратор";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProgress = "Processing";
        public const string StatusFinished = "Finished";
        public const string StatusCanceled = "Canceled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";
    }
}
