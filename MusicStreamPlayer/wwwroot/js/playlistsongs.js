const player = document.getElementById("audioPlayer");
const songs = Array.from(document.querySelectorAll(".song-item"));
let currentIndex = 0;

function updateUI(index, isPlaying) {
    songs.forEach((li, i) => {
        if (i === index) {
            li.classList.add("active");
        } else {
            li.classList.remove("active");
        }
    });
}

function playSong(index) {
    const song = songs[index];
    if (!song) return;
    currentIndex = index;
    player.src = song.dataset.url;
    player.play();
    updateUI(index, true);
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

document.querySelectorAll(".delete-playlist-form").forEach(form => {
    form.addEventListener("submit", function (e) {
        e.preventDefault(); // Prevent immediate submission

        Swal.fire({
            title: 'Are you sure?',
            text: "This will delete the playlist permanently.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!'
        }).then(result => {
            if (result.isConfirmed) {
                Swal.fire({
                    icon: 'success',
                    title: 'Removed!',
                    text: 'Playlist deleted permanently.',
                    timer: 1000,
                    showConfirmButton: false,
                    willClose: () => {
                        form.submit(); // Submit after alert closes
                    }
                });
            }
        });
    });
});

document.querySelectorAll(".btn-remove").forEach(button => {
    button.addEventListener("click", async (e) => {
        e.stopPropagation();
        const result = await Swal.fire({
            title: 'Are you sure?',
            text: "Remove this song from playlist?",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, remove it!'
        });

        if (!result.isConfirmed) return;

        const playlistId = button.dataset.playlistId;
        const mediaUrl = button.dataset.mediaUrl;

        try {
            const response = await fetch(`/Playlist/RemoveSong?playlistId=${playlistId}&mediaUrl=${mediaUrl}`, {
                method: "POST"
            });

            if (response.ok) {
                button.closest(".song-item").remove();
                Swal.fire({
                    icon: 'success',
                    title: 'Removed!',
                    text: 'Song has been removed from playlist.',
                    timer: 1000,
                    showConfirmButton: false,
                    willClose: () => {
                         window.location.reload();
                    }
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Failed',
                    text: 'Could not remove song from playlist.'
                });
            }
        } catch (err) {
            console.error("Error removing song:", err);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'An error occurred while removing the song.'
            });
        }
    });
});

player.addEventListener("ended", () => {
    if (currentIndex < songs.length - 1) {
        playSong(currentIndex + 1);
    }
});

if (songs.length > 0) {
    playSong(0);
}