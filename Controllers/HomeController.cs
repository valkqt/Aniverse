using Capstone.Models.ViewModels;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Capstone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        async public Task<IActionResult> Index()
        {
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri("https://graphql.anilist.co")
            };

            var client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());

            var query = @"
                query ($page: Int, $perPage: Int, $perPage2: Int, $season: MediaSeason, $nextSeason: MediaSeason, $seasonYear: Int) {
                  score:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:SCORE_DESC type: ANIME isAdult: false) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }

                popular:Page (page: $page, perPage: $perPage2) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:POPULARITY_DESC type: ANIME isAdult: false) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large  color} bannerImage averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }

                trending:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:TRENDING_DESC type: ANIME isAdult: false) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} bannerImage averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }

                currentSeason:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:POPULARITY_DESC type: ANIME isAdult: false season: $season seasonYear: $seasonYear) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} bannerImage averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }

                nextSeason:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:POPULARITY_DESC type: ANIME isAdult: false season: $nextSeason seasonYear: $seasonYear) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }
                }";


            var variables = new
            {
                season = "SPRING",
                nextSeason = "SUMMER",
                seasonYear = "2024",
                page = 1,
                perPage = 4,
                perPage2 = 10
            };
            var request = new GraphQLRequest
            {
                Query = query,
                Variables = variables
            };

            var response = await client.SendQueryAsync<dynamic>(request);

            if (response.Errors != null)
            {
                foreach (var error in response.Errors)
                {
                    Console.WriteLine($"GraphQL Error: {error.Message}");
                }

                return NotFound();
            }

            dynamic results = response.Data;
            HomeViewModel model = new HomeViewModel()
            {
                CurrentSeason = SearchController.FromGraphQLElement(results.currentSeason.media),
                Score = SearchController.FromGraphQLElement(results.score.media),
                Trending = SearchController.FromGraphQLElement(results.trending.media),
                NextSeason = SearchController.FromGraphQLElement(results.nextSeason.media)

            };


            foreach (var result in results.popular.media)
            {

                Anime anime = new Anime()
                {
                    CoverImage = result.bannerImage
                };
                model.Carousel.Add(anime);
            }

            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
