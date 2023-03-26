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

        public const string StatusPending = "Ожидает подтверждения";
        public const string StatusApproved = "Подтвержден";
        public const string StatusInProgress = "В процессе";
        public const string StatusFinished = "Закончен";
        public const string StatusCanceled = "Отменен";
        public const string StatusRefunded = "Возрат";

        public const string PaymentStatusPending = "Ожидает оплаты";
        public const string PaymentStatusApproved = "Оплачен";
        public const string PaymentStatusRejected = "Оплата отменена";
    }
}
