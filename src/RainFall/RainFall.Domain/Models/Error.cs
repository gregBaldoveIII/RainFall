using System.Net;

namespace RainFall.Domain.Models;

public class Error
{
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
}