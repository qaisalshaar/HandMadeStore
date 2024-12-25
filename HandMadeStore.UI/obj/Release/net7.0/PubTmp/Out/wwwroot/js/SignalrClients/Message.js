
$(() => {


    
    var connection = new signalR.HubConnectionBuilder().withUrl("/Message").build();
    connection.start();
    connection.on("recieveMessage", (message) => {
      //  toastr.success(`${message.sender} send (${message.body})`, 'Kais Alshaar Handmade Store', { timeOut: 10000 })

        toastr.success(`${message.sender} send (${message.body})`, 'Kais Alshaar Home Made Store',
            {
                "positionClass": "toast-top-center",

                "closeButton": true,
                "progressBar": true,
                "showDuration": "300",
                "hideDuration": "3000",
                "timeOut": "5000",
                "extendedTimeOut": "3000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
        );




    })
})