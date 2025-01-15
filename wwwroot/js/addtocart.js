<><script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script><script>
    $(document).ready(function() {$('.product-button').click(function() {
        var productId = $(this).data('product-id');
        var userId = $('#userId').val();

        $.ajax({
            url: '@Url.Action("HandleButtonClick", "Product")',
            type: 'POST',
            data: {
                productId: productId,
                userId: userId
            },
            success: function(response) {
                console.log('Server response:', response);
            },
            error: function(xhr, status, error) {
                console.error('Error:', error);
            }
        });
    })};
    });
</script></>