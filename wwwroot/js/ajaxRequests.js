function disconnect(url) {
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        data: "unlock=true",
        dataType: "action",
        success: function (result) {
            console.log(result);
        },
        error: function (request, status, error) {
            console.log(status);
        },
    });
}

function proveAuth(url) {
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        async: false,
        success: function (result) {
            if (result) {
                $('body').html(result);
            }
        },
    });
}