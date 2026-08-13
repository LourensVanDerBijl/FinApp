using FinBineBackend.LoggingLogic.LogDbWrite.Data;
using FinBineBackend.LoggingLogic.LogDbWrite.Models;

namespace FinBineBackend.LoggingLogic.LogDbWrite.Services
{
    public class LogDbWriteService
    {
        private readonly LogDbContext _dbContext;

        public LogDbWriteService(LogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task WriteLogAsync(LogDbRecord log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            await _dbContext.Logs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}