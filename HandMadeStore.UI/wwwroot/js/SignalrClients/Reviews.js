
$(() => {
    var connection = new signalR.HubConnectionBuilder().withUrl("/Reviews").build();
    connection.start();
    connection.on("loadReviews", (id) => {
        if (window.location.search == `?proid=${id}`)
            location.reload();
    })
})