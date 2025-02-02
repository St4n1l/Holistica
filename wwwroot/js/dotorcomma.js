document.addEventListener("DOMContentLoaded", function () {
    let priceInput = document.getElementById("Price");
    let form = document.querySelector("form");

    priceInput.addEventListener("input", function () {
        this.value = this.value.replace(",", ".");
    });

    form.addEventListener("submit", function () {
        priceInput.value = priceInput.value.replace(",", ".");
    });
});
