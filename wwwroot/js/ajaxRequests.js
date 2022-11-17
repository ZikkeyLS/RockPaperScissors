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

function getGameStatusStatus(url, roundInfoText, winnerText, nickname01Text, choice01Image, nickname02Text, choice02Image, nickname03Text, choice03Image, statusText, continueButton, exitButton) {
    $.ajax({
        type: "POST",
        url: url,
        async: true,
        success: function (result) {
            if (result != null) {
                const resultObject = JSON.parse(result);

                if (resultObject != null) {

                    if (resultObject.PlayerWinner != null && resultObject.Players != null)
                    {
                        // Round info

                        roundInfoText.textContent = "Раунд: " + resultObject.Level.toString() + "-" + resultObject.Iteration.toString();

                        winnerText.textContent = "Победители:";

                        for (i = 0; i < resultObject.Players.length; i++) {
                            if (resultObject.Players[i] == null)
                                continue;

                            if (i == 0) {
                                choice01Image.src = "/images/Animations/HandShaking_1.gif";
                            }
                            else if (i == 1) {
                                choice02Image.src = "/images/Animations/HandShaking_1.gif";
                            }
                            else if (i == 2) {
                                choice03Image.src = "/images/Animations/HandShaking_1.gif";
                            }
                        }

                        setTimeout(function ()
                        {
                            for (i = 0; i < resultObject.Players.length; i++) {
                                if (resultObject.Players[i] == null)
                                    continue;

                                if (resultObject.Players[i].Winner) {
                                    winnerText.textContent = winnerText.textContent + " " + resultObject.Players[i].Name;
                                }

                                if (i == 0) {
                                    if (resultObject.Players[i].Name != null) {
                                        nickname01Text.textContent = resultObject.Players[i].Name;
                                    }
                                    else {
                                        nickname01Text.textContent = "Отключился";
                                    }

                                    choice01Image.src = getImageByInput(resultObject.Players[i].Choice);
                                }
                                else if (i == 1) {
                                    if (resultObject.Players[i].Name != null) {
                                        nickname02Text.textContent = resultObject.Players[i].Name;
                                    }
                                    else {
                                        nickname02Text.textContent = "Отключился";
                                    }

                                    choice02Image.src = getImageByInput(resultObject.Players[i].Choice);
                                }
                                else if (i == 2) {
                                    if (resultObject.Players[i].Name != null) {
                                        nickname03Text.textContent = resultObject.Players[i].Name;
                                    }
                                    else {
                                        nickname03Text.textContent = "Отключился";
                                    }

                                    choice03Image.src = getImageByInput(resultObject.Players[i].Choice);
                                }


                                // Main Status

                                if (resultObject.WinnersCount == 0) {
                                    statusText.textContent = "Ничья. Ждём очереди!";
                                    exitButton.remove();
                                    continueButton.remove();
                                }
                                else {
                                    if (resultObject.PlayerWinner) {
                                        statusText.textContent = "Поздравляем, вы выиграли!";
                                        exitButton.remove();
                                    }
                                    else {
                                        statusText.textContent = "К сожалению, вы проиграли!";
                                        continueButton.remove();
                                    }
                                }
                            }
                        }, 1000);
                    }
                }
            }
        }
    });
}

function getImageByInput(input) {
    if (input == 0) {
        return "/images/RawCirclesNamed/Hands_710_01.png";
    }
    else if (input == 1) {
        return "/images/RawCirclesNamed/Hands_710_02.png";
    }
    else if (input == 2) {
        return "/images/RawCirclesNamed/Hands_710_03.png";
    }
    else if (input == 3) {
        return "/images/RawCirclesNamed/Hands_710_04.png";
    }
    else if (input == 4) {
        return "/images/RawCirclesNamed/Hands_710_05.png";
    }
    else if (input == 5) {
        return "/images/RawCirclesNamed/Hands_710_06.png";
    }
    else if (input == 6) {
        return "/images/RawCirclesNamed/Hands_710_07.png";
    }

    return "";
}

function getGameStatusWin(url, scoreText, levelText, moneyText)
{
    $.ajax({
        type: "POST",
        url: url,
        async: true,
        success: function (result) {
            if (result != null) {
                const resultObject = JSON.parse(result);

                if (resultObject != null) {

                    if (resultObject.Score != null)
                        scoreText.textContent = resultObject.Score.toString() + " баллов на счёте";

                    if (resultObject.Level != null)
                        levelText.textContent = "Уровень: " + resultObject.Level.toString();

                    if (resultObject.ScoreAdvance != null)
                        moneyText.textContent = "Награда: " + resultObject.ScoreAdvance.toString() + " баллов";
                }
            }
        }
    });
}