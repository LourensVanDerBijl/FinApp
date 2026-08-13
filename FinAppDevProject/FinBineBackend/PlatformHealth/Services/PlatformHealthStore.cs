using System.Collections.Concurrent;
using FinBineBackend.PlatformHealth.Models;

namespace FinBineBackend.PlatformHealth.Services
{
    // Keeps the latest health check result for each service in memory,
    // so the dashboard can read it instantly without waiting on a live
    // check. Same idea as LogQueue — a shared in-memory "current state."
    public static class PlatformHealthStore
    {
        private static readonly ConcurrentDictionary<string, ServiceHealthStatus> _statuses = new();

        public static ServiceHealthStatus GetOrCreate(string serviceName)
        {
            return _statuses.GetOrAdd(serviceName, name => new ServiceHealthStatus { Name = name });
        }

        public static List<ServiceHealthStatus> GetAll()
        {
            return _statuses.Values.ToList();
        }
    }
}