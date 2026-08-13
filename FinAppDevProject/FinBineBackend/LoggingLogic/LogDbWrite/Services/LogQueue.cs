using System.Collections.Concurrent;
using FinBineBackend.LoggingLogic.LogDbWrite.Models;

namespace FinBineBackend.LoggingLogic.LogDbWrite.Services
{
    public static class LogQueue
    {
        // Holds every log from the current SAST day, for the live
        // dashboard view. Resets automatically at midnight SAST.
        private static readonly ConcurrentQueue<LogDbRecord> _liveBuffer = new();

        // Holds logs waiting to be permanently saved to the database.
        // Drained every 10 minutes by LogDbBackgroundWorker.
        private static readonly ConcurrentQueue<LogDbRecord> _archiveQueue = new();

        private static readonly object _resetLock = new();
        private static DateOnly _currentIngestionDate = GetSastDate(DateTime.UtcNow);

        public static void Enqueue(LogDbRecord log)
        {
            if (log == null) return;

            log.IngestionDate = GetSastDate(log.Timestamp);

            EnsureCurrentDay();

            _liveBuffer.Enqueue(log);
            _archiveQueue.Enqueue(log);
        }

        /// <summary>
        /// Returns logs from the current SAST day. If "after" is given,
        /// only logs strictly newer than that timestamp come back —
        /// this is what lets the frontend fetch only new entries.
        /// </summary>
        public static IEnumerable<LogDbRecord> GetLiveLogs(DateTime? after = null)
        {
            EnsureCurrentDay();

            var logs = _liveBuffer.ToArray().AsEnumerable();

            if (after.HasValue)
            {
                logs = logs.Where(log => log.Timestamp > after.Value);
            }

            return logs;
        }

        public static IEnumerable<LogDbRecord> DrainArchive()
        {
            var drained = new List<LogDbRecord>();
            while (_archiveQueue.TryDequeue(out var log))
            {
                drained.Add(log);
            }
            return drained;
        }

        // If the SAST calendar day has changed since we last checked,
        // empty the live buffer so the dashboard starts today fresh.
        private static void EnsureCurrentDay()
        {
            var today = GetSastDate(DateTime.UtcNow);
            if (today == _currentIngestionDate) return;

            lock (_resetLock)
            {
                if (today == _currentIngestionDate) return;

                _liveBuffer.Clear();
                _currentIngestionDate = today;
            }
        }

        private static DateOnly GetSastDate(DateTime utcTime)
        {
            // South Africa Standard Time is a fixed UTC+2, no daylight saving.
            return DateOnly.FromDateTime(utcTime.AddHours(2));
        }
    }
}