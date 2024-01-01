using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;

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
            (Rule: IsInvalid(transaction.UpdatedDate), Parameter: nameof(transaction.UpdatedDate))
                );
        }

        private static void ValidateTransactionIsNotNull(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new NullTransactionException();
            }
        }

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
    }
}
