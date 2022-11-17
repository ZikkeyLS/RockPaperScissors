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
        }
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
        }
    });
}

function sendQueueRequest(url, level) {
    $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data: { "level" : level },
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
        }
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

                if (resultObject.UrlIndex != null && !window.location.href.includes(resultObject.UrlIndex))
                    window.location.href = resultObject.UrlIndex;
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

function sendInput(url, input) {
    $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data: { "input" : input },
        async: true
    });
}

function verifyButtonState(url, button, level) {
    $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        data: { "level" : level },
        async: true,
        success: function (result) {
            if (result) {
                button.disabled = result;
            }
        }
    });
}

function verifyTextState(url, text) {
    $.ajax({
        type: "POST",
        url: url,
        dataType: "json",
        async: true,
        success: function (result) {
            if (result) {
                text.textContent = result.toString() + " баллов";
            }
        }
    });
}

function roundDataRequest(url, roundText, scoreText, leftTimeText) {
    $.ajax({
        type: "POST",
        url: url,
        async: true,
        success: function (result) {
            if (result != null) {
                const resultObject = JSON.parse(result);

                if (resultObject != null)
                {
                    if (resultObject.RoundNumber != null && resultObject.Level != null)
                        roundText.textContent = "Раунд: " + resultObject.Level.toString() + "-" + resultObject.RoundNumber.toString();

                    if (resultObject.Score != null)
                        scoreText.textContent = "Взнос: " + resultObject.Score.toString();

                    if (resultObject.LeftTime != null)
                        leftTimeText.textContent = "Осталось: " + resultObject.LeftTime.toString() + " секунд";
                }
            }
        }
    });
}

function queueDataRequest(url, levelText, playersText, waitTimeText) {
    $.ajax({
        type: "POST",
        url: url,
        async: true,
        success: function (result) {
            if (result != null) {
                const resultObject = JSON.parse(result);

                if (resultObject != null) {

                    if (resultObject.Level != null)
                        levelText.textContent = "Уровень: " + resultObject.Level.toString();

                    if (resultObject.PlayersCount != null)
                        playersText.textContent = "Игроков в очереди: " + resultObject.PlayersCount.toString();

                    if (resultObject.WaitTime != null)
                        waitTimeText.textContent = "Время ожидания: " + resultObject.WaitTime + " минут";
                }
            }
        }
    });
}

function getMenuPageStatus(url, scoreText, gamesText) {
    $.ajax({
        type: "POST",
        url: url,
        async: true,
        success: function (result) {
            if (result != null) {
                const resultObject = JSON.parse(result);

                if (resultObject != null) {

                    if (resultObject.Score != null)
                        scoreText.textContent = resultObject.Score.toString() + " баллов";

                    if (resultObject.Games != null)
                        gamesText.textContent = resultObject.Games.toString() + " игр";
                }
            }
        }
    });
}
