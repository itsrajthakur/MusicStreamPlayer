document.addEventListener("DOMContentLoaded", function () {
    const container = document.querySelector('#PlaylistCreated');
    const playlistCreated = container?.dataset.playlistCreated;

    if (playlistCreated === "True") {
        Swal.fire({
            icon: 'success',
            title: 'Playlist created!',
            text: 'Your new playlist has been successfully created.',
            confirmButtonColor: '#198754'
        });
    }
});