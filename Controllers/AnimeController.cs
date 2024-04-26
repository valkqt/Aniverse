using Capstone.Models;
using Capstone.Models.ViewModels;
using Capstone.Models.ViewModels.SubModels;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.Controllers
{
    public class AnimeController : Controller
    {
        AniDbContext db = new AniDbContext();
        [HttpGet]
        async public Task<IActionResult> Anime(int Id)
        {
            Anime model = new Anime();
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri("https://graphql.anilist.co")
            };

            var client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());

            var query = @"query ($id: Int) {
                Media (id: $id, type: ANIME, isAdult: false) { id title {romaji english} format description(asHtml: true) status 
                        startDate { year month day} endDate { year month day } season seasonYear episodes duration source trailer { site thumbnail id } 
                        coverImage { extraLarge large medium color } bannerImage genres averageScore meanScore
                        relations { edges { node { id title { romaji english } format episodes season seasonYear averageScore status type genres studios(isMain: true) { nodes {name}} coverImage { large color }} relationType } }
                        characters(sort:[ROLE,RELEVANCE,ID]) { edges { role node {name {full} image {large} } voiceActors(language: JAPANESE, sort:[RELEVANCE,ID]) {id name{full} languageV2 image{large}}}}
                        staff(sort:[RELEVANCE,ID]) { edges { role node { name { full } image { large } } } }
                        studios { edges { isMain} nodes { name isAnimationStudio } }
                        nextAiringEpisode { airingAt episode }
                        externalLinks { site url }
                        streamingEpisodes { site url title thumbnail }
                        rankings { rank type format year season allTime context }
                        recommendations(perPage: 6, sort:[RATING_DESC,ID]) {edges {node{id rating mediaRecommendation{id title{romaji} coverImage {large}}}}}
                        stats { scoreDistribution { score amount } statusDistribution { status amount } }
}}";
            var variables = new
            {
                id = Id,
            };
            var request = new GraphQLRequest
            {
                Query = query,
                Variables = variables
            };
            try
            {
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
                    var results = response.Data.Media;

                    model.Id = results.id;
                    model.Title = results.title.romaji;
                    if (results.episodes != null)
                    {
                        model.Episodes = results.episodes;
                    }
                    model.Status = results.status;
                    model.AverageScore = results.averageScore;
                    model.BannerImage = results.bannerImage;
                    model.Duration = results.duration;
                    model.Description = results.description;
                    model.ColorAvg = results.coverImage.color;
                    model.CoverImage = results.coverImage.large;
                    model.Format = results.format;
                    model.MeanScore = results.meanScore;
                    model.Source = results.source;
                    model.Season = results.season;
                    model.SeasonYear = results.seasonYear;
                    model.StartDate = new FuzzyDate()
                    {
                        Year = results.startDate.year,
                        Month = results.startDate.month,
                        Day = results.startDate.day

                    };
                    model.EndDate = new FuzzyDate()
                    {
                        Year = results.endDate.year,
                        Month = results.endDate.month,
                        Day = results.endDate.day

                    };
                    for (int i = 0; i < results.studios.nodes.Count; i++)
                    {
                        var studio = new Studio((string)results.studios.nodes[i].name, (bool)results.studios.edges[i].isMain, (bool)results.studios.nodes[i].isAnimationStudio);
                        model.Studios.Add(studio);
                    }
                    foreach (string genre in results.genres)
                    {
                        model.Genres.Add(genre);
                    }
                    foreach (var relation in results.relations.edges)
                    {
                        if ((string)relation.node.type != "MANGA")
                        {
                            Anime rel = new Anime()
                            {
                                Id = relation.node.id,
                                Title = relation.node.title.romaji,
                                Format = relation.node.format,
                                Status = relation.node.status,
                                RelationType = relation.relationType,
                                CoverImage = relation.node.coverImage.large,
                                AverageScore = relation.node.averageScore
                            };
                            rel.ColorAvg = SearchController.CheckNullString(relation.node.colorAvg);
                            rel.Season = SearchController.CheckNullString(relation.node.season);
                            rel.SeasonYear = SearchController.CheckNullInt(relation.node.seasonYear);



                            foreach (var studio in relation.node.studios.nodes)
                            {
                                rel.Studios.Add(new Studio()
                                {
                                    Name = studio.name,
                                });
                            }

                            foreach (string genre in relation.node.genres)
                            {
                                rel.Genres.Add(genre);
                            }

                            model.Relations.Add(rel);


                        }
                    }
                    foreach (var edge in results.characters.edges)
                    {
                        var voiceActor = new Staff();
                        try
                        {

                            voiceActor.Name = edge.voiceActors[0].name?.full;
                            voiceActor.Image = edge.voiceActors[0].image?.large;
                            voiceActor.Language = edge.voiceActors[0].languageV2;

                        }
                        catch { }
                        var character = new Character()
                        {
                            Name = edge.node.name.full,
                            Role = edge.role,
                            Image = edge.node.image.large,
                            VoiceActor = voiceActor
                        };
                        model.Characters.Add(character);
                    }
                    foreach (var edge in results.staff.edges)
                    {
                        var staff = new Staff()
                        {
                            Name = edge.node.name.full,
                            Role = edge.role,
                            Image = edge.node.image.large,
                        };
                        model.Staff.Add(staff);
                    }
                    foreach (var ranking in results.rankings)
                    {
                        Ranking rank = new Ranking()
                        {
                            Rank = ranking.rank,
                            Type = ranking.type,
                            Format = ranking.format,
                            Year = ranking.year,
                            Season = ranking.season,
                            AllTime = ranking.allTime,
                            Context = ranking.context,
                        };
                        model.Rankings?.Add(rank);
                    }
                    model.Stats = new Stats();
                    foreach (var scoreStat in results.stats.scoreDistribution)
                    {
                        model.Stats?.Score?.Add(new SingleStat()
                        {
                            Score = scoreStat.score,
                            Amount = scoreStat.amount,
                        });
                    }
                    foreach (var statusStat in results.stats.statusDistribution)
                    {
                        model.Stats?.Status?.Add(new SingleStat()
                        {
                            Status = statusStat.status,
                            Amount = statusStat.amount,
                        });
                    }
                    foreach (var ep in results.streamingEpisodes)
                    {
                        model.StreamingEpisodes.Add(new StreamingEpisode()
                        {
                            Title = ep.title,
                            Url = ep.url,
                            Site = ep?.site,
                            Thumbnail = ep.thumbnail,
                        });
                    }
                    foreach (var rec in results.recommendations.edges)
                    {
                        if (rec.node.mediaRecommendation != null)
                        {
                            Anime anime = new Anime();
                            anime.CoverImage = rec.node.mediaRecommendation.coverImage.large;
                            anime.Id = rec.node.mediaRecommendation.id;
                            anime.Title = rec.node.mediaRecommendation.title.romaji;

                            Recommendation recommendation = new Recommendation();
                            recommendation.Rating = rec.node.rating;
                            recommendation.Id = rec.node.id;
                            recommendation.RecommendedMedia = anime;

                            model.Recommendations?.Add(recommendation);
                        }

                    }

                    if (results.trailer != null)
                    {
                        model.Trailer = new Trailer()
                        {
                            Site = results.trailer.site,
                            Thumbnail = results.trailer.thumbnail,
                            Id = results.trailer.id,

                        };
                    }
                    AnimeViewModel viewModel = new AnimeViewModel();

                    if (User.Identity.IsAuthenticated)
                    {

                        var entry = (
                            from item in db.MediaListItems
                            join fav in db.Favourites
                            on model.Id
                            equals fav.FavouriteId into favitem_table
                            from favitem in favitem_table.DefaultIfEmpty()
                            where favitem.FavouriteId == model.Id
                            select new MediaListItem
                            {
                                AnimeId = model.Id,
                                Format = model.Format,
                                Episodes = model.Episodes,
                                Image = model.CoverImage,
                                Title = model.Title,

                                Rating = item.Rating,
                                EpisodesWatched = item.EpisodesWatched,
                                Completion = item.Completion,
                                Notes = item.Notes,
                                IsFavourite = favitem.FavouriteId == null ? false : true
                            }).FirstOrDefault();
                        if (entry != null)
                        {
                            viewModel.ListEntry = entry;
                        }
                    }

                    viewModel.Anime = model;

                    return View(viewModel);
                }
            }
            catch
            {
                return RedirectToAction("Search", "Search");
            }



        }

    }


}
