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
                window.location.href = result;
            }
        },
    });
}

function sendQueueRequest(url) {
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        async: false,
        success: function (result) {
            if (result) {
                const resultObject = JSON.parse(result);

                if (resultObject.UrlIndex != null)
                    window.location.href = resultObject.UrlIndex;
            }
        },
    });
}

function getQueueStatus(url) {
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        async: true,
        success: function (result) {
            if (result) {
                const resultObject = JSON.parse(result);

                if (resultObject.UrlIndex != null)
                    window.location.href = resultObject.UrlIndex;
            }
        },
    });
}

function hearthbeat(url) {
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        async: true,
        success: function (result) {
            if (result) {
                const resultObject = JSON.parse(result);

                if (resultObject.UrlIndex != null && !window.location.href.includes(resultObject.UrlIndex)) {
                    alert(window.location.href);
                    window.location.href = resultObject.UrlIndex;
                }

            }
        }
    });
}

function quitQueueRequest(url) {
    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        async: false,
        success: function (result) {
            if (result) {
                const resultObject = JSON.parse(result);

                if (resultObject.UrlIndex != null)
                    window.location.href = resultObject.UrlIndex;
            }
        }
    });
}