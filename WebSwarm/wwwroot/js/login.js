function ProcessLogin() {
    var emailAddress = ($("#Email").val()).trim();
    if (emailAddress == "") {
        alert("Email address is required.");
        $("#Email").focus();
        return false;
    }

    var password = ($("#Password").val()).trim();
    if (password == "") {
        alert("Password is required.");
        $("#Password").focus();
        return false;
    }

    var loginData = {
        "Email" : btoa(emailAddress),
        "Password" : btoa(password),
    };

    $.ajax({
        url: $("#ProcessLogin").val(),
        type: "POST",
        dataType: "JSON",
        data: { model: loginData },
        success: function (data) {
            console.log(data);
            if (data != null) {
                if (data.isSuccess == true) {
                    location.replace($("#Index").val());
                } else {
                    alert(data.Message);
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Failed to login");
            console.log(jqXHR);
        }
    });
}