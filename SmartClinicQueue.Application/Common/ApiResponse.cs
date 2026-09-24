using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Common
{
    public class ApiResponse<T>
    {
       
            public bool IsSuccess { get; set; }

            public string Message { get; set; } = string.Empty;

            public T? Data { get; set; }

            public List<string> Errors { get; set; } = new();

            public static ApiResponse<T> Success(T data, string message = "")
            {
                return new ApiResponse<T>
                {
                    IsSuccess = true,
                    Message = message,
                    Data = data
                };
            }

            public static ApiResponse<T> Failure(List<string> errors, string message = "")
            {
                var errorList = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList()
                    ?? new List<string>();

                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    Message = string.IsNullOrWhiteSpace(message)
                        ? string.Join(" | ", errorList)
                        : message,
                    Errors = errorList
                };
            }

            public static ApiResponse<T> Failure(string error, string message = "")
            {
                var errors = string.IsNullOrWhiteSpace(error)
                    ? new List<string>()
                    : new List<string> { error };

                return Failure(errors, message);
            }
        }
    }

