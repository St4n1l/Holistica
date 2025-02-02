document.addEventListener("DOMContentLoaded", function () {
    let priceInput = document.getElementById("Price");
    let form = document.querySelector("form");

    // Convert commas to dots while typing
    priceInput.addEventListener("input", function () {
        this.value = this.value.replace(",", ".");
    });

    // Ensure the correct format before submitting
    form.addEventListener("submit", function () {
        priceInput.value = priceInput.value.replace(",", ".");
    });
});
