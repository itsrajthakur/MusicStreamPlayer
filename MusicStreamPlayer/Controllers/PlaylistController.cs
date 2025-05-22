using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamPlayer.Models;
using MusicStreamPlayer.Services;
using System.Security.Claims;

namespace MusicStreamPlayer.Controllers
{
    [Authorize]
    public class PlaylistController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private readonly ILogger<PlaylistController> _logger;

        public PlaylistController(IPlaylistService playlistService, ILogger<PlaylistController> logger)
        {
            _playlistService = playlistService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlists = await _playlistService.GetUserPlaylistsAsync(userId);
            return View(playlists);
        }

        public async Task<IActionResult> GetUserPlaylists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlists = await _playlistService.GetUserPlaylistsAsync(userId);
            return Json(playlists);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _playlistService.CreatePlaylistAsync(userId, name);
            TempData["PlaylistCreated"] = true;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null) return NotFound();
            return View(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddSong(string playlistId, [FromBody] PlaylistSong song)
        {
            bool added = await _playlistService.AddSongToPlaylistAsync(playlistId, song);
            return Json(new { success = added });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSong(string playlistId, string mediaUrl)
        {
            await _playlistService.RemoveSongFromPlaylistAsync(playlistId, mediaUrl);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _playlistService.DeletePlaylistAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}