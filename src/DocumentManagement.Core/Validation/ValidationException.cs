using System;
using Microsoft.AspNetCore.Http;

namespace DocumentManagement.Core.Validation
{
    public class ValidationException : Exception
    {
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = @"application/json";

        public ValidationException()
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }

        public ValidationException(string message) : base(message)
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}