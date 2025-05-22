using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamPlayer.Models;
using MusicStreamPlayer.Services;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Web;

namespace MusicStreamPlayer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IPlayHistoryService _playHistoryService;

        public HomeController(ILogger<HomeController> logger, 
                             IHttpClientFactory clientFactory,
                             IPlayHistoryService playHistoryService)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _playHistoryService = playHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            string[] keywords = { "hindi", "love", "romantic", "new", "hits", "favorites", "bollywood" };
            var random = new Random();
            string keyword = keywords[random.Next(keywords.Length)];

            var url = $"https://saavn.dev/api/search/songs?query={keyword}&limit=10";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return View(new List<SongModel>());

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("data").GetProperty("results");

            var songs = new List<SongModel>();
            foreach (var item in results.EnumerateArray())
            {
                Console.WriteLine(item.ToString());
                var title = item.GetProperty("name").GetString();
                var image = item.GetProperty("image").EnumerateArray().Last().GetProperty("url").GetString();
                var mediaUrl = item.GetProperty("downloadUrl").EnumerateArray().Last().GetProperty("url").GetString();
                var artist = item.GetProperty("artists").GetProperty("primary").EnumerateArray().FirstOrDefault().GetProperty("name").GetString();

                songs.Add(new SongModel
                {
                    Title = title,
                    Image = image,
                    MediaUrl = mediaUrl,
                    Artist = artist
                });
            }
            ViewData["Title"] = "🎶 Treanding Songs";
            return View(songs);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return View("Index");

            var client = _clientFactory.CreateClient();
            var encodedQuery = HttpUtility.UrlEncode(query);
            var url = $"https://saavn.dev/api/search/songs?query={encodedQuery}&limit=10";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return View(new List<SongModel>());

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("data").GetProperty("results");

            var songs = new List<SongModel>();
            foreach (var item in results.EnumerateArray())
            {
                Console.WriteLine(item.ToString());
                var title = item.GetProperty("name").GetString();
                var image = item.GetProperty("image").EnumerateArray().Last().GetProperty("url").GetString();
                var mediaUrl = item.GetProperty("downloadUrl").EnumerateArray().Last().GetProperty("url").GetString();
                var artist = item.GetProperty("artists").GetProperty("primary").EnumerateArray().FirstOrDefault().GetProperty("name").GetString();

                songs.Add(new SongModel
                {
                    Title = title,
                    Image = image,
                    MediaUrl = mediaUrl,
                    Artist = artist
                });
            }
            ViewData["Title"] = "🎵 Music Stream";
            return View("Index", songs);
        }

        [HttpGet]
        public async Task<JsonResult> GetSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var client = _clientFactory.CreateClient();
            var encodedTerm = HttpUtility.UrlEncode(term);
            var url = $"https://saavn.dev/api/search/songs?query={encodedTerm}&limit=5";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return Json(new List<string>());

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var suggestions = new List<object>();
            var results = doc.RootElement.GetProperty("data").GetProperty("results");

            foreach (var item in results.EnumerateArray())
            {
                suggestions.Add(new
                {
                    title = item.GetProperty("name").GetString()
                });
            }

            return Json(suggestions);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> LogPlay([FromBody] PlayHistory playHistory)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _playHistoryService.LogPlayAsync(userId, playHistory);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging play history");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPlayHistory()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var history = await _playHistoryService.GetUserPlayHistoryAsync(userId);

                var songs = new List<SongModel>();
                foreach (var item in history)
                {
                    Console.WriteLine(item.ToString());
                    var title = item.Title;
                    var image = item.Image;
                    var mediaUrl = item.MediaUrl;
                    var artist = item.Artist;

                    songs.Add(new SongModel
                    {
                        Title = title,
                        Image = image,
                        MediaUrl = mediaUrl,
                        Artist = artist
                    });
                }
                ViewData["Title"] = "🎶 Play Music History";
                return View("Index", songs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Get play history");
                return StatusCode(500);
            }
        }
    }
}
