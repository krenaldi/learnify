using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace API.ErrorResponse
{
  public class ApiValidationErrorResponse : ApiResponse
  {
    public ApiValidationErrorResponse() : base(400)
    {
    }

    public IEnumerable<string> Errors { get; set; }
  }
}