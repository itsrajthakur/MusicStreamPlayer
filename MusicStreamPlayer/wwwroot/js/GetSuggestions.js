const input = document.getElementById("searchInput");
const suggestionBox = document.getElementById("suggestions");

if (input) {
    input.addEventListener("input", async () => {
        const query = input.value.trim();
        suggestionBox.innerHTML = "";

        if (query.length < 2) return;

        const response = await fetch(`/Home/GetSuggestions?term=${encodeURIComponent(query)}`);
        const results = await response.json();

        results.forEach(item => {
            const li = document.createElement("li");
            li.textContent = item.title;
            li.className = "list-group-item list-group-item-action";
            li.style.cursor = "pointer";
            li.addEventListener("click", () => {
                input.value = item.title;
                suggestionBox.innerHTML = "";
                document.getElementById("searchForm").submit();
            });
            suggestionBox.appendChild(li);
        });
    });

    // Optional: hide suggestions on outside click
    document.addEventListener("click", (e) => {
        if (!suggestionBox.contains(e.target) && e.target !== input) {
            suggestionBox.innerHTML = "";
        }
    });
}