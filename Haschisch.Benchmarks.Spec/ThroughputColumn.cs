using System.Globalization;
using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Haschisch.Benchmarks
{
    public class ThroughputColumn : IColumn
    {
        public bool IsDefault(Summary summary, Benchmark benchmark) => false;

        public string GetValue(Summary summary, Benchmark benchmark)
        {
            var bytes = benchmark.Parameters.Items
                .Where(x => x.Name == nameof(HashByteArray.Bytes))
                .Select(x => x.Value)
                .OfType<int>()
                .SingleOrDefault();

            if (bytes != 0)
            {
                const double MiBFactor = 1024 * 1024;
                var nanoSeconds = summary[benchmark].ResultStatistics.Mean;
                var throughputInBps = 1_000_000_000 * (bytes / nanoSeconds);
                return string.Format(CultureInfo.InvariantCulture, "{0:N0} MiB/s", throughputInBps / MiBFactor);
            }
            else
            {
                return "NA (no 'Bytes') parameter";
            }
        }

        // ignore style
        public string GetValue(Summary summary, Benchmark benchmark, ISummaryStyle style) =>
            this.GetValue(summary, benchmark);

        public bool IsAvailable(Summary summary) =>
            summary.Benchmarks.Any(b => b.Parameters.Items.Any(x => x.Name == "Bytes" && x.Value is int));

        public bool AlwaysShow => false;

        public ColumnCategory Category => ColumnCategory.Custom;

        public string Id => "Throughput";

        public string ColumnName => this.Id;

        public int PriorityInCategory => 10;

        public bool IsNumeric => true;

        public UnitType UnitType => UnitType.Dimensionless;

        public string Legend => null;

        public override string ToString() => "Throughput";
    }
}
