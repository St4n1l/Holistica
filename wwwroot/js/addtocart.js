//document.querySelector("form").addEventListener("submit", async function (e) {
//    e.preventDefault(); // Prevent the default form submission

//    const formData = new FormData(this);

//    try {
//        const response = await fetch("/Cart/AddToCart", {
//            method: "POST",
//            body: formData
//        });

//        if (response.ok) {
//            window.location.href = "/Cart/ViewCart"; // Redirect to the cart page
//        } else {
//            console.error("Failed to add item to cart");
//        }
//    } catch (error) {
//        console.error("Error:", error);
//    }
//});