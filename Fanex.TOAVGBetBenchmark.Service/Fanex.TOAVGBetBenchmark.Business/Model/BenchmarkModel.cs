namespace Fanex.TOAVGBetBenchmark.Business.Models
{
    public class BenchmarkModel
    {
        public string AgentName { get; set; }

        public string Currency { get; set; }

        public string Data { get; set; }

        public decimal Benchmark { get; set; }

        public decimal AVGBenchmark { get; set; }

        public decimal MTD { get; set; }

        public decimal AVGMTD { get; set; }

        public decimal Increased { get; set; }
    }
}