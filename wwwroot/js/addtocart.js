
$(document).ready(function () {
    $(".addToCart").click(function (e) {
        e.preventDefault();
        var productId = $(this).data("product-id");

        $.ajax({
            url: "/Cart/AddToCart",
            type: "GET",
            data: { productId: productId },
            success: function (response) {
                alert("Product added to cart!");
            },
            error: function () {
                alert("An error occurred while adding the product to the cart.");
            }
        });
    });
});