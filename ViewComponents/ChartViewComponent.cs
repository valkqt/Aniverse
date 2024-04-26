using Capstone.Models;
using Capstone.Models.ViewModels.SubModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Capstone.ViewComponents
{
    public class ChartViewComponent : ViewComponent
    {
        AniDbContext db = new AniDbContext();

        public async Task<IViewComponentResult> InvokeAsync(List<SingleStat>? Stats, string caption)
        {
            List<string> Status = new List<string>();
            List<int> Amounts = new List<int>();
            List<int> Scores = new List<int>();


            for (int i = 0; i < Stats.Count; i++)
            {
                Scores.Add(Stats[i].Score / 10);
                Amounts.Add(Stats[i].Amount);
                Status.Add(Stats[i].Status ?? "");
            }






            var chartData = @"
            {
                type: 'line',
                responsive: false,
                data:
                {
                    labels: " + $"[{(Scores.All(s => s == 0) ? "'Watching', 'Planning', 'Completed', 'Dropped', 'On-Hold'" : string.Join(", ", Scores))}]," + @"
                    datasets: [{
                        label:" + $"'# of {caption}'," +
                        "data: " + $"[{string.Join(", ", Amounts)}]," + @"
                        backgroundColor: [
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(54, 162, 235, 0.2)'
                            ],
                        borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(54, 162, 235, 1)'
                            ],
                        borderWidth: 1
                    }]
                },
                options:
                {
                    scales:
                    {
                        yAxes: [{
                            ticks:
                            {
                                beginAtZero: true
                            }
                        }]
                    }
                }
            }";

            var chart = JsonConvert.DeserializeObject<ChartJs.Root>(chartData);

            var chartModel = new ChartJsViewModel
            {
                Chart = chart,
                ChartJson = JsonConvert.SerializeObject(chart, new JsonSerializerSettings

                {
                    NullValueHandling = NullValueHandling.Ignore
                }),
                Name = Guid.NewGuid().ToString(),
            };

            return View(chartModel);
        }
    }
}
