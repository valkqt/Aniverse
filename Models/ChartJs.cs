namespace Capstone.Models
{
    public class ChartJs
    {
        public class Data
        {
            public List<string> labels { get; set; }
            public List<Dataset> datasets { get; set; }
        }

        public class Dataset
        {
            public string label { get; set; }
            public List<int> data { get; set; }
            public List<string> backgroundColor { get; set; }
            public List<string> borderColor { get; set; }
            public int borderWidth { get; set; }
        }

        public class Options
        {
            public Scales scales { get; set; }
        }

        public class Root
        {
            public string type { get; set; }
            public bool responsive { get; set; }
            public Data data { get; set; }
            public Options options { get; set; }
        }

        public class Scales
        {
            public List<YAxis> yAxes { get; set; }
        }

        public class Ticks
        {
            public bool beginAtZero { get; set; }
        }

        public class YAxis
        {
            public Ticks ticks { get; set; }
        }


    }
}
