﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

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
        { }

        public FailedTransactionStorageException(string message, Exception innerException)
            : base(message: "Failed transaction storage error occurred, contact support.", innerException)
        { }

    }
}
