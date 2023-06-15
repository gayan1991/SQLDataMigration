using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;

namespace Db.Infrastructure.Migrations
{
    public class ResultModel<T>
    {
        public bool IsSuccessful { get; set; } = true;
        public string EntityName { get; set; } = null!;
        public List<T> Results { get; set; } = new();
        public int Count => Results.Count;
        public VerificationQueryModel VerificationQueryModel { get; set; }

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public ResultModel()
        {
            _stopwatch.Start();
        }

        public TimeSpan CompleteTask
        {
            get
            {
                _stopwatch.Stop();
                var millisecs = _stopwatch.ElapsedMilliseconds;
                return TimeSpan.FromMilliseconds(millisecs);
            }
        }
    }

    [Keyless]
    public class VerificationCount
    {
        public int RecordCount { get; set; }
    }
}
