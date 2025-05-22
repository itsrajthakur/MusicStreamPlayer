document.addEventListener("DOMContentLoaded", function () {
    const Message = document.body.dataset.playlistCreated;

    if (Message === "Login") {
        Swal.fire({
            icon: 'success',
            title: "Logged in successfully!",
            showConfirmButton: false,
            timer: 1000
        });
    }
    else if (Message === "Logout") {
        Swal.fire({
            icon: 'success',
            title: "You have been logged out.",
            showConfirmButton: false,
            timer: 1000
        });
    }
    else if (Message === "Register") {
        Swal.fire({
            icon: 'success',
            title: "Registration successful!",
            showConfirmButton: false,
            timer: 1000
        });
    }
    else if (Message) {
        Swal.fire({
            icon: 'error',
            title: Message,
            showConfirmButton: true
        });
    }
});