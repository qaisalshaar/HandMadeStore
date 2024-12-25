



$(document).ready(function () {
    loadData()
});

const loadData = () => {
  DataTable=  $('#shopTable').DataTable({
        "ajax": {
            "url": "/Admin/Shop/GetAll"
        },
        success: function (response) {

            debugger
        },
        columns: [
            { "data": "name", width: "auto" },
            { "data": "city", width: "auto" },
            { "data": "streetAddress", width: "auto" },
            { "data": "postalCode", width: "auto" },
            { "data": "phoneNumber", width: "auto" },
           
 
            {
                "data": "id",
                "render": function (data) {
                   
                    return `<div class="d-flex justify-content-around"><a class="btn btn-warning" href="/Admin/Shop/Upsert?id=${data}"><i class="bi bi-pencil-square"></i></a>
                    <a onClick=deleteShop('/Admin/Shop/Delete/${data}') class="btn btn-danger"><i class="bi bi-x-square"></i></a></div>`
               
                }


                , width: "auto", "orderable": false

            },
          /*  { title: "", "defaultContent": "<button onclick='edititem();'>Edit</button>" },*/
                
           

               

            
           
        ],
        "language": {
            "processing": "<img scr='~/images/shopimages/preloader.gif' />",
            "emptyTable": "No Data Found Kais Alshaar"

        },
        columnDefs: [
           
            {
                targets: 0,
                className: 'dt-left'
            },
           /* { className: 'text-center', targets: [1,2,3,4] }*/
           

          
        ]
       
    });
}

const deleteShop = (URL)=> {

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            //Swal.fire({
            //    title: "Deleted!",
            //    text: "Your file has been deleted.",
            //    icon: "success"
            //});

            $.ajax({

                url: URL,
                type: 'delete',
                success: function (data) {

                    if (data.success) {

                        DataTable.ajax.reload();
                        toastr.success(data.message, 'Home Made Store',
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
                    } else {

                        toastr.error(data.message, 'Home Made Store',
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
                    }

                }

            })
        }
    });

}