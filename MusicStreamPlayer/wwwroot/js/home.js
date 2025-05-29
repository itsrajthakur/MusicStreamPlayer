const player = document.getElementById("audioPlayer");
const songs = Array.from(document.querySelectorAll(".song-item"));
let currentIndex = 0;

function updateNowPlayingInfo(songElement) {
    const image = document.querySelector(".playlist-icon");
    const title = document.getElementById("title");
    const artist = document.getElementById("artist");

    if (!songElement) return;

    image.style.background = `url('${songElement.querySelector("img").src}') center center / cover no-repeat`;
    title.innerHTML = songElement.querySelector(".song-title")?.innerText || "";
    artist.innerHTML = songElement.querySelector(".song-artist")?.innerText || "";
}

// update icons and active class based on the current song
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

// play the selected song and log it in the database
async function playSong(index) {
    try {
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

// Add event listeners for add to song playlist
async function addToPlaylist(song, playlistId) {
    try {
        const response = await fetch(`/Playlist/AddSong?playlistId=${playlistId}`, {
            method: "POST",
            headers: {"Content-Type": "application/json"},
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
                title: 'Already exists',
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


// Add event listeners for song items
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
            updateNowPlayingInfo(songs[i]);
        }
    });
});

// Add event listeners for add to playlist
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

document.querySelectorAll('.favorite-btn').forEach(button => {
    button.addEventListener('click', async () => {
        // prevent playing the song when clicking the heart
        event.stopPropagation();

        // Read song data
        const songData = {
            title: button.dataset.songTitle,
            image: button.dataset.songImage,
            mediaUrl: button.dataset.songUrl,
            artist: button.dataset.songArtist
        };

        // Determine action: favorite or unfavorite
        const isFav = button.dataset.songIsfavorite === 'true';
        const url = isFav ? '/Home/RemoveToFavorite' : '/Home/AddToFavorite';

        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(songData)
            });
            const result = await response.json();

            if (result.success) {
                // Toggle the icon class
                const icon = button.querySelector('i');
                if (isFav) {
                    icon.classList.replace('fas', 'far');
                } else {
                    icon.classList.replace('far', 'fas');
                }

                // Update the dataset for next click
                button.dataset.songIsfavorite = (!isFav).toString();

                // Show SweetAlert
                Swal.fire({
                    icon: 'success',
                    title: isFav ? 'Removed!' : 'Favorited!',
                    text: isFav ? 'Song removed from favorites.' : 'Song added to favorites.',
                    timer: 1500,
                    showConfirmButton: false
                });
            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'Oops...',
                    text: 'Operation failed.',
                    timer: 1500,
                    showConfirmButton: false
                });
            }
        } catch (err) {
            console.error('Error toggling favorite:', err);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Could not update favorite status.'
            });
        }
    });
});

// Add event listeners for auto change and ended events
player.addEventListener("ended", () => {
    if (currentIndex < songs.length - 1) {
        currentIndex++;
        playSong(currentIndex);
        updateNowPlayingInfo(songs[currentIndex]);
    } else {
        window.location.href = "/Home/Index";
    }
});

// Add event listeners for play/pause
player.addEventListener("pause", () => updateUI(currentIndex, false));
player.addEventListener("play", () => updateUI(currentIndex, true));

window.onload = () => {
    playSong(0);
    updateNowPlayingInfo(songs[0]);
}