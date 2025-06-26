using myLeave.Data;
public class ExceptionLogger : IExceptionLogger
{
    private readonly AppDbContext _context;

    public ExceptionLogger(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(Exception ex)
    {
        var log = new ExceptionLog
        {
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            Source = ex.Source,
            Timestamp = DateTime.UtcNow
        };

        _context.ExceptionLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
