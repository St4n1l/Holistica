$(document).ready(function () {
    var loginForum = document.getElementsId("username").value;
    var passwordForum = document.getElementById("password").value;

    if (loginForum == "Stalincho" && passwordForum == "iDidthisShitManWoohHooo12@")
    {

    }    var conditionMet = true;

    if (conditionMet)
    {
        $.ajax({
            url: '@Url.Action("AssignRole", "User")', // URL to the controller action
            type: 'POST',
            data: { userId: Guid.NewGuid(), roleName: 'Admin' }, // Pass userId and roleName
            success: function (response)
            {
                if (response.success)
                {
                    alert('Role assigned successfully!');
                } else
                {
                    alert('Failed to assign role: ' + response.message);
                }
            },
            error: function ()
            {
                alert('An error occurred while processing your request.');
            }
        });
    }
});