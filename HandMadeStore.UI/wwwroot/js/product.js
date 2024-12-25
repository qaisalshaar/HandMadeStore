
/*onClick = deleteProduct('/Admin/Product/Delete/${data}')*/
/*href = "/Admin/Product/delete/${data}"*/

$(document).ready(function () {
    loadData()
});

const loadData = () => {
    DataTable = $('#productTable').DataTable({

        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        success: function (response) {

            debugger
        },
        columns: [
            { "data": "name", width: "auto", order: [[0, 'desc']] },
            { "data": "price", width: "auto" },
            { "data": "category.name", width: "auto" },
            { "data": "brand.name", width: "auto" },
            { "data": "createdDate", width: "auto" },
            { "data": "imageUrl",
            
                "render": function (data) {

                    return `<img src="${data}"  class= "border border-primary border border-3 rounded-lg" width="30%" height="20%" />`

                }
           
            
                , width: "15%", "orderable": false },
 
            {
                "data": "id",
                "render": function (data) {
                   
                    return `<div class="d-flex justify-content-around">
                    <a class="btn btn-warning" href="/Admin/Product/Upsert?id=${data}"><i class="bi bi-pencil-square"></i></a>
                    <a onClick = deleteProduct('/Admin/Product/Delete/${data}')  class="btn btn-danger"><i class="bi bi-x-square"></i></a>
                    
                    <a class="btn btn-primary" href="/Admin/Product/Print?id=${data}"><i class="bi bi-printer"></i></a>


                    </div>`
               
                }


                , width: "auto", "orderable": false

            },
          /*  { title: "", "defaultContent": "<button onclick='edititem();'>Edit</button>" },*/
                
           

               

            
           
        ],
        "language": {
            "processing": "<img scr='~/images/productimages/preloader.gif' />",
            "emptyTable": "No Data Found Kais Alshaar"

        },
        columnDefs: [
            {
                targets: 4,
                render: DataTable.render.date()
            },
            {
                targets: 0,
                className: 'dt-left',
                
              
                

            },
           
           /* { className: 'text-center', targets: [1,2,3,4] }*/
           

          
        ],

       /* add Buttouns*/
        dom: 'Bfrtip',
        responsive: true,
        lengthChange: false,
        autoWidth: false,
        buttons: ["copy",
            {
                extend: 'excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4,5]
                }
            },
            {
                extend: 'pdf',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4,5]
                }
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4,5]
                }
            }]

       
    });

    DataTable.buttons().container().prpendTo('#productTable_wrapper');
}



function deleteProduct(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
        //    alert(url);
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {

                    if (data.success) {
                       


                        setTimeout(function () {

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

  }, 2000);

                        DataTable.ajax.reload();


                        


                     //   toastr.success(data.message);

                   //     toaster.success(data.message);
                    } else {

                        toastr.error(data.message);
                    }



                   
                }
            })
        }
    })
}