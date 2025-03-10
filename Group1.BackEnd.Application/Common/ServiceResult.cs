using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group1.BackEnd.Application.Common
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ServiceResult(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ServiceResult<T> SuccessResult(T data, string message = "Success")
        {
            return new ServiceResult<T>(true, message, data);
        }

        public static ServiceResult<T> FailureResult(string message)
        {
            return new ServiceResult<T>(false, message, default);
        }
    }
}

