using Microsoft.Data.SqlClient;
using System;
using Xeptions;

namespace ExpenseTracker.Core.Models.Transactions.Exceptions
{
    public class FailedTransactionStorageException : Xeption
    {
        private SqlException sqlException;

        public FailedTransactionStorageException(Exception innerException)
            : base(message: "Failed transaction storage error occurred, contact support.", innerException)
        {

        }

        public FailedTransactionStorageException(SqlException sqlException)
        {
            this.sqlException = sqlException;
        }
    }
}
