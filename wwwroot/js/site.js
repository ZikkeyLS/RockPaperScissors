

@section Scripts {
    <script type="text/javascript">
        window.onunload = disconnect;
    </script>
}
function disconnect() {
    $.ajax({
        type: "POST",
        url: "@Url.Action("Disconnect")",
        dataType: "action",
        success: function (result) {
            console.log(result);
        },
        error: function (request, status, error) {
            console.log(status);
        },
    });
}