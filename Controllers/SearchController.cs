using Capstone.Models;
using Capstone.Models.ViewModels;
using Capstone.Models.ViewModels.SubModels;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class SearchController : Controller
    {
        AniDbContext db = new AniDbContext();

        public static string CheckNullString(dynamic json)
        {
            if (json != null)
            {
                var value = json;
                return value;
            }
            else
            {
                return "";
            }


        }
        public static int CheckNullInt(dynamic json)
        {
            if (json != null)
            {
                var value = json;
                return value;
            }
            else
            {
                return 0;
            }
        }


        public static List<Anime> FromGraphQLElement(dynamic json)
        {
            var media = new List<Anime>();

            foreach (var anime in json)
            {
                Anime entry = new Anime()
                {
                    Id = anime.id,
                    Title = anime.title.romaji,
                    Format = anime.format,
                    Status = anime.status,
                    Season = anime.season,
                    SeasonYear = anime.seasonYear,
                    Episodes = CheckNullInt(anime.episodes),
                    CoverImage = anime.coverImage.large,
                    ColorAvg = anime.coverImage.color,
                    AverageScore = anime.averageScore,

                };

                foreach (var studio in anime.studios.nodes)
                {
                    entry.Studios.Add(new Studio()
                    {
                        Name = studio.name,
                    });
                }

                foreach (var genre in anime.genres)
                {
                    entry.Genres.Add(genre.ToString());
                }

                media.Add(entry);
            }

            return media;
        }

        [HttpGet]
        async public Task<IActionResult> Search()
        {
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri("https://graphql.anilist.co")
            };

            var client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());

            var query = @"
                query ($page: Int, $perPage: Int, $season: MediaSeason, $nextSeason: MediaSeason, $seasonYear: Int) {
                  score:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:SCORE_DESC type: ANIME isAdult: false) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }

                popular:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:POPULARITY_DESC type: ANIME isAdult: false) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }

                trending:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:TRENDING_DESC type: ANIME isAdult: false) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres
                        studios(isMain: true) { nodes { name } } }
                    }

                currentSeason:Page (page: $page, perPage: $perPage) {
                    pageInfo {total,currentPage,lastPage,hasNextPage,perPage}
                    media (sort:POPULARITY_DESC type: ANIME isAdult: false season: $season seasonYear: $seasonYear) 
                    {id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres
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
                perPage = 4
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

            var model = new SearchResultsCategories()
            {
                CurrentSeason = FromGraphQLElement(results.currentSeason.media),
                Score = FromGraphQLElement(results.score.media),
                Popular = FromGraphQLElement(results.popular.media),
                Trending = FromGraphQLElement(results.trending.media),
                NextSeason = FromGraphQLElement(results.nextSeason.media)

            };

            return View(model);
        }

        private static string ParseSearchQuery(string season, int year)
        {
            string parameters = "";

            if (season != "pepe")
            {
                parameters += "season: " + season + " ";
            }
            if (year != 0)
            {
                parameters += "seasonYear: " + year.ToString() + " ";
            }

            return parameters;
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchQuery model)
        {
            string parameters = ParseSearchQuery(model.Season ?? "pepe", model.Year ?? 0);
            var searchGenre = $",$genres: String" ?? "";

            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri("https://graphql.anilist.co")
            };

            var client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());

            var query = @"query ($id: Int, $page: Int, $perPage: Int, $search: String " + $"{searchGenre}" + @") {
  Page (page: $page, perPage: $perPage) {
    pageInfo {
      total
      currentPage
      lastPage
      hasNextPage
      perPage
    } media (id: $id, type: ANIME, sort: POPULARITY_DESC, isAdult: false, search: $search " + $"{parameters} {"genre: $genres"}" + @" ) { id title { romaji } format status 
                        season seasonYear episodes  
                        coverImage { large color }  genres averageScore
                        studios(isMain: true) { edges { isMain} nodes { name } }
    }}}";
            Console.WriteLine(parameters);
            object variables;
            if (model.Genre != "plofi")
            {
                variables = new
                {
                    search = model.Name,
                    page = 1,
                    perPage = 25,
                    genres = model.Genre,
                };
            }
            else
            {
                variables = new
                {
                    search = model.Name,
                    page = 1,
                    perPage = 25
                };

            }

            var request = new GraphQLRequest
            {
                Query = query,
                Variables = variables
            };

            var response = await client.SendQueryAsync<dynamic>(request);
            List<Anime> media = new List<Anime>();

            if (response.Errors != null)
            {

                foreach (var error in response.Errors)
                {
                    Console.WriteLine($"GraphQL Error: {error.Message}");
                }
                return NotFound();

            }
            else
            {
                var results = response.Data;

                foreach (var anime in results.Page.media)
                {
                    Anime entry = new Anime()
                    {
                        Id = anime.id,
                        Title = anime.title.romaji,
                        Format = anime.format,
                        Status = anime.status,
                        Season = anime.season,
                        SeasonYear = anime.seasonYear,
                        Episodes = CheckNullInt(anime.episodes),
                        CoverImage = anime.coverImage?.large,
                        ColorAvg = anime.coverImage?.color,
                        AverageScore = anime.averageScore,
                    };
                    try
                    {
                        foreach (var studio in anime.studios.nodes)
                        {
                            entry.Studios.Add(new Studio()
                            {
                                Name = studio.name,
                                IsAnimationStudio = studio.isAnimationStudio,

                            });
                        }
                    }
                    catch
                    {
                    }
                    foreach (var genre in anime.genres)
                    {
                        entry.Genres.Add(genre.ToString());
                    }
                    media.Add(entry);

                }
            }
            SearchResultsCategories resultsModel = new SearchResultsCategories();
            resultsModel.Results = media;

            return View(resultsModel);
        }
        [HttpGet]
        [Route("Search/TopRated")]
        async public Task<IActionResult> TopRated()
        {

            List<Anime> resultsList = new List<Anime>();

            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri("https://graphql.anilist.co")
            };

            var client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());

            var query = "query ($page: Int, $perPage: Int) {Page (page: $page, perPage: $perPage) {" +
                "pageInfo {total,currentPage,lastPage,hasNextPage,perPage}" +
                "media (sort:SCORE_DESC type: ANIME isAdult: false) " +
                "{id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres " +
                "studios(isMain: true) { nodes { name } } }" +
                "}}";

            var variables = new
            {
                page = 1,
                perPage = 50
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
            else
            {

                var results = response.Data.Page;


                foreach (var anime in results.media)
                {
                    Anime media = new Anime();
                    media.Id = anime.id;
                    media.Title = anime.title.romaji;
                    media.CoverImage = anime.coverImage.large;
                    media.ColorAvg = anime.coverImage.color;
                    media.Season = anime.season;
                    media.SeasonYear = anime.seasonYear;
                    media.AverageScore = anime.averageScore;
                    media.Format = anime.format;
                    if (anime.episodes != null)
                    {
                        media.Episodes = anime.episodes;
                    }
                    List<string> genreList = new List<string>();
                    foreach (string genre in anime.genres)
                    {
                        genreList.Add(genre);
                    }
                    media.Genres = genreList;
                    media.Studios = new List<Studio>() {
                        new Studio() {
                            Name = anime.studios.nodes[0].name
                            }
                    };
                    resultsList.Add(media);
                }
            }

            return View(resultsList);
        }
        [HttpGet]
        [Route("Search/Popular")]
        async public Task<IActionResult> MostPopular()
        {

            List<Anime> resultsList = new List<Anime>();

            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri("https://graphql.anilist.co")
            };

            var client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());

            var query = "query ($page: Int, $perPage: Int) {Page (page: $page, perPage: $perPage) {" +
                "pageInfo {total,currentPage,lastPage,hasNextPage,perPage}" +
                "media (sort:POPULARITY_DESC type: ANIME isAdult: false) " +
                "{id title {romaji} season seasonYear format status episodes coverImage {large color} averageScore genres " +
                "studios(isMain: true) { nodes { name } } }" +
                "}}";

            var variables = new
            {
                page = 1,
                perPage = 50
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
            else
            {

                var results = response.Data.Page;


                foreach (var anime in results.media)
                {
                    Anime media = new Anime();
                    media.Id = anime.id;
                    media.Title = anime.title.romaji;
                    media.CoverImage = anime.coverImage.large;
                    media.ColorAvg = anime.coverImage.color;
                    media.Season = anime.season;
                    media.SeasonYear = anime.seasonYear;
                    media.AverageScore = anime.averageScore;
                    media.Format = anime.format;
                    if (anime.episodes != null)
                    {
                        media.Episodes = anime.episodes;
                    }
                    List<string> genreList = new List<string>();
                    foreach (string genre in anime.genres)
                    {
                        genreList.Add(genre);
                    }
                    media.Genres = genreList;
                    media.Studios = new List<Studio>() {
                        new Studio() {
                            Name = anime.studios.nodes[0].name
                            }
                    };
                    resultsList.Add(media);
                }
            }

            return View(resultsList);
        }
    }
}
