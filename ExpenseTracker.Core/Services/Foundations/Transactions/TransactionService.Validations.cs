using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using System;
using System.Reflection.Metadata;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public partial class TransactionService
    {
        public void ValidateTransactionOnAdd(Transaction transaction)
        {
            ValidateTransactionIsNotNull(transaction);

            Validate(
            (Rule: IsInvalid(transaction.Id), Parameter: nameof(transaction.Id)),
            (Rule: IsInvalid(transaction.Category), Parameter: nameof(transaction.Category)),
            (Rule: IsInvalid(transaction.Description), Parameter: nameof(transaction.Description)),
            (Rule: IsInvalid(transaction.PaymentMode), Parameter: nameof(transaction.PaymentMode)),
            (Rule: IsInvalid(transaction.CreatedDate), Parameter: nameof(transaction.CreatedDate)),
            (Rule: IsInvalid(transaction.UpdatedDate), Parameter: nameof(transaction.UpdatedDate)),

            (Rule: IsNotSame(firstDate: transaction.UpdatedDate, secondDate: transaction.CreatedDate,
                secondDateName: nameof(transaction.CreatedDate)),
                Parameter: nameof(transaction.UpdatedDate)),

            (Rule: IsNotRecent(transaction.CreatedDate), Parameter: nameof(transaction.CreatedDate))
                );
        }

        private static void ValidateTransactionIsNotNull(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new NullTransactionException();
            }
        }

        private static void ValidateTransactionId(Guid transactionId) =>
            Validate((Rule: IsInvalid(transactionId), Parameter: nameof(Transaction.Id)));

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidTransactionException = new InvalidTransactionException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidTransactionException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidTransactionException.ThrowIfContainsErrors();
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == default,
            Message = "Id is required."
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required."
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required."
        };

        private static dynamic IsNotSame(DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent."
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

    }
}
