

using ExpenseTracker.Core.Models.Transactions;

namespace ExpenseTracker.Core.Services.Foundations.Transactions
{
    public partial class TransactionService
    {
        private delegate ValueTask<Transaction> ReturningPostFunction();
        private delegate IQueryable<Transaction> ReturningPostsFunction();

        private async ValueTask<Transaction> TryCatch(ReturningPostFunction returningPostFunction)
        {
            try
            {
                return await returningPostFunction();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
