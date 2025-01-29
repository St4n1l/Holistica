document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.querySelector('input[name="searchInput"]');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const input = searchInput.value;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            const formData = new URLSearchParams();
            formData.append('input', input);
            formData.append('__RequestVerificationToken', token);

            fetch(searchUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: formData.toString()
            })
                .then(response => response.text())
                .then(html => {
                    document.querySelector('.items').innerHTML = html;
                })
                .catch(error => console.error('Error:', error));
        });
    } else {
        console.error('Search input not found!');
    }
});
