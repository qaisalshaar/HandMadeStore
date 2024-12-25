$(document).ready(function () {
  /*  loadData()*/

    var url = window.location.search;
    if (url.includes("ppending")) {
        loadDataTable("ppending");
    }
    else if (url.includes("cancel")) {
        loadDataTable("cancel");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    }
    else {
        loadDataTable("all");
    }


});
let dataTable;
const loadDataTable = (status) => {
    dataTable = $('#orderTable').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?Status="+status,
            contentType: 'application/json; charset=utf-8',
            //success: function (result) {
               
            //    console.log(result);
            //},

            error: function (response) {
                alert('Something went wrong..!!');
                alert(response);
            },
        },
       
        
        columns: [
            { "data": "id", "width": "5%", "aaSorting": [[0, "desc"]] },
            { "data": "name", "width": "25%", "bSearchable": false },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `    <div class="d-flex justify-content-around">
                                <a class="btn btn-warning " href="/Admin/Order/Details?orderid=${data}">  
                                <i class="bi bi-pencil-square"></i></a>
                                                           </div>
                            `
                },
                width: "10%", "orderable": false
            }
        ],
        columnDefs: [
            {
                targets: 0,
                className: 'dt-left'
            }
            
           
        ],
        language: {
            searchPanes: {
                i18n: {
                    emptyMessage: " < i > <b>No results returned</b></i>"
                }
                //emptyMessage: "</i></b>No results returned</b></i>"
            },
            infoEmpty: "No results returned ",
            zeroRecords: "No Results On Search",
            emptyTable: "No Data Found",
        },
    });
}


