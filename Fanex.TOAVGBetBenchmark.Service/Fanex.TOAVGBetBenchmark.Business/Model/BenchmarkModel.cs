namespace Fanex.TOAVGBetBenchmark.Business.Model
{
    public class BenchmarkModel
    {
        public string AgentName { get; set; }

        public string Currency { get; set; }

        public string Data { get; set; }

        public decimal Benchmark { get; set; }

        public decimal AvgBenchmark { get; set; }

        public decimal Mtd { get; set; }

        public decimal AvgMtd { get; set; }

        public decimal Increased { get; set; }
    }
}