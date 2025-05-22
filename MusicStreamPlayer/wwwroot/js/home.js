const player = document.getElementById("audioPlayer");
const songs = Array.from(document.querySelectorAll(".song-item"));
let currentIndex = 0;

function updateUI(index, isPlaying) {
    songs.forEach((li, i) => {
        const icon = li.querySelector(".play-icon i");
        if (i === index) {
            li.classList.add("active");
            icon.className = isPlaying ? "fas fa-pause-circle" : "fas fa-play-circle";
        } else {
            li.classList.remove("active");
            icon.className = "fas fa-play-circle";
        }
    });
}

async function playSong(index) {
    const song = songs[index];
    if (!song) return;
    currentIndex = index;
    player.src = song.dataset.url;
    player.play();
    updateUI(index, true);

    // Store in DB via API
    const data = {
        title: song.querySelector(".song-title").innerText,
        image: song.querySelector(".song-image").src,
        mediaUrl: song.dataset.url,
        artist: song.querySelector(".song-artist").innerText
    };

    try {
        await fetch("/Home/LogPlay", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        });
    } catch (err) {
        console.error("Error logging play history", err);
    }
}

async function addToPlaylist(song, playlistId) {
    try {
        const response = await fetch(`/Playlist/AddSong?playlistId=${playlistId}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(song)
        });

        const result = await response.json();

        if (result.success) {
            Swal.fire({
                icon: 'success',
                title: 'Success!',
                text: 'Song added to playlist.',
                timer: 2000,
                showConfirmButton: false
            });
        } else {
            Swal.fire({
                icon: 'info',
                title: 'Info',
                text: 'Song already exists in playlist.',
                timer: 2000,
                showConfirmButton: false
            });
        }
    } catch (err) {
        console.error("Error adding song to playlist:", err);
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Failed to add song to playlist!'
        });
    }
}

songs.forEach((li, i) => {
    li.addEventListener("click", () => {
        if (currentIndex === i) {
            if (player.paused) {
                player.play();
            } else {
                player.pause();
            }
        } else {
            playSong(i);
        }
    });
});

// Add event listeners for playlist buttons
// Replace the existing add-to-playlist-btn click handler with this:
document.querySelectorAll('.add-to-playlist-btn').forEach(button => {
    button.addEventListener('click', async () => {
        const songData = {
            title: button.dataset.songTitle,
            image: button.dataset.songImage,
            mediaUrl: button.dataset.songUrl,
            artist: button.dataset.songArtist
        };

        try {
            const response = await fetch('/Playlist/GetUserPlaylists');
            const playlists = await response.json();

            const playlistList = document.getElementById('playlistList');
            playlistList.innerHTML = '';

            if (playlists.length != 0) {
            playlists.forEach(playlist => {
                const item = document.createElement('button');
                item.className = 'list-group-item list-group-item-action bg-dark text-white';
                item.textContent = playlist.name;
                item.onclick = async () => {
                    await addToPlaylist(songData, playlist.id);
                    const modal = bootstrap.Modal.getInstance(document.getElementById('playlistModal'));
                    modal.hide();
                };
                playlistList.appendChild(item);
            });
            }
            else {
                const item = document.createElement('button');
                item.className = 'list-group-item list-group-item-action bg-dark text-white';
                item.textContent = "Create playlist";
                item.onclick = async () => {
                    window.location.href = "/Playlist/Index";
                    const modal = bootstrap.Modal.getInstance(document.getElementById('playlistModal'));
                    modal.hide();
                };
                playlistList.appendChild(item);
            }

            const modal = new bootstrap.Modal(document.getElementById('playlistModal'));
            modal.show();
        } catch (err) {
            console.error("Error loading playlists:", err);
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Failed to load playlists!'
            });
        }
    });
});

player.addEventListener("ended", () => {
    if (currentIndex < songs.length - 1) {
        currentIndex++;
        playSong(currentIndex);
    } else {
        window.location.href = "/Home/Index";
    }
});

player.addEventListener("pause", () => updateUI(currentIndex, false));
player.addEventListener("play", () => updateUI(currentIndex, true));

window.onload = () => playSong(0);