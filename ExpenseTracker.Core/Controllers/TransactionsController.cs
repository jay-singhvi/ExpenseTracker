using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using ExpenseTracker.Core.Services.Foundations.Transactions;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : RESTFulController
    {
        private readonly ITransactionService transactionService;

        public TransactionsController(ITransactionService transactionService) =>
            this.transactionService = transactionService;

        [HttpPost]
        public async ValueTask<ActionResult<Transaction>> PostTransactionAsync(Transaction transaction)
        {
            try
            {
                Transaction addTransaction = await transactionService.AddTransactionAsync(transaction);

                return Created(addTransaction);
            }
            catch (TransactionValidationException transactionValidationException)
            {
                return BadRequest(transactionValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
                when(transactionDependencyValidationException.InnerException is InvalidTransactionReferenceException)
            {
                return FailedDependency(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
                when(transactionDependencyValidationException.InnerException is AlreadyExistsTransactionException)
            {
                return Conflict(transactionDependencyValidationException.InnerException);
            }
            catch(TransactionDependencyValidationException transactionDependencyValidationException)
                when(transactionDependencyValidationException.InnerException is LockedTransactionException)
            {
                return Locked(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch(TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }
    }
}
