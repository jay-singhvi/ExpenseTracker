using ExpenseTracker.Core.Models.Transactions;
using ExpenseTracker.Core.Models.Transactions.Exceptions;
using ExpenseTracker.Core.Services.Foundations.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Controllers
{
    [Authorize]
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
                when (transactionDependencyValidationException.InnerException is InvalidTransactionReferenceException)
            {
                return FailedDependency(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
                when (transactionDependencyValidationException.InnerException is AlreadyExistsTransactionException)
            {
                return Conflict(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
                when (transactionDependencyValidationException.InnerException is LockedTransactionException)
            {
                return Locked(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Transaction>> GetAllTransactions()
        {
            try
            {
                IQueryable<Transaction> retrieveTransactions =
                    transactionService.RetrieveAllTransactions();

                return Ok(retrieveTransactions);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }

        [HttpGet("{transactionId}")]
        public async ValueTask<ActionResult<Transaction>> GetTransactionByIdAsync(Guid transactionId)
        {
            try
            {
                Transaction transaction = 
                    await this.transactionService.RetrieveTransactionByIdAsync(transactionId);

                return Ok(transaction);
            }
            catch (TransactionValidationException transactionValidationException)
                when (transactionValidationException.InnerException is NotFoundTransactionException)
            {
                return NotFound(transactionValidationException.InnerException);
            }
            catch (TransactionValidationException transactionValidationException)
            {
                return BadRequest(transactionValidationException.InnerException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Transaction>> PutTransactionAsync(Transaction transaction)
        {
            try
            {
                Transaction modifiedTransaction =
                    await this.transactionService.ModifyTransactionAsync(transaction);

                return Ok(modifiedTransaction);
            }
            catch(TransactionValidationException transactionValidationException)
                when(transactionValidationException.InnerException is NotFoundTransactionException)
            {
                return NotFound(transactionValidationException.InnerException);
            }
            catch (TransactionValidationException transactionValidationException)
            {
                return BadRequest(transactionValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
             when(transactionDependencyValidationException.InnerException is LockedTransactionException)
            {
                return Locked(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
            {
                return BadRequest(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }

        [HttpPost]
        public async ValueTask<ActionResult<Transaction>> DeleteTransactionByIdAsync(Guid transactionId)
        {
            try
            {
                Transaction deletedTransaction =
                    await this.transactionService.RemoveTransactionByIdAsync(transactionId);

                return Ok(deletedTransaction);
            }
            catch (TransactionValidationException transactionValidationException)
                when(transactionValidationException.InnerException is NotFoundTransactionException)
            {
                return NotFound(transactionValidationException.InnerException);
            }
            catch (TransactionValidationException transactionValidationException)
            {
                return BadRequest(transactionValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
                when(transactionDependencyValidationException.InnerException is LockedTransactionException)
            {
                return Locked(transactionDependencyValidationException.InnerException);
            }
            catch (TransactionDependencyValidationException transactionDependencyValidationException)
            {
                return BadRequest(transactionDependencyValidationException);
            }
            catch (TransactionDependencyException transactionDependencyException)
            {
                return InternalServerError(transactionDependencyException);
            }
            catch (TransactionServiceException transactionServiceException)
            {
                return InternalServerError(transactionServiceException);
            }
        }
    }
}
