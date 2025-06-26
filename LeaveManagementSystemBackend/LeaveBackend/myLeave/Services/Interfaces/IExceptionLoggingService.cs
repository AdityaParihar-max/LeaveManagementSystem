public interface IExceptionLogger
{
    Task LogAsync(Exception ex);
}
