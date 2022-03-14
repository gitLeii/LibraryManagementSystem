$(document).ready(function () {
    $('#booksDatatable').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,

        "ajax": {
            "url": "/Admin/GetBooks",
            "type": "GET",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "BookId", "name": "BookId", "autoWidth": true },
            { "data": "Title", "name": "Title", "autoWidth": true },
            { "data": "Publication", "name": "Publication", "autoWidth": true },
            { "data": "Author", "name": "Author", "autoWidth": true },
            { "data": "Branch", "name": "Branch", "autoWidth": true },
            { "data": "Quantity", "name": "Quantity", "autoWidth": true },
            {
                "render": function (data, row) { return "<a href='#' class='btn btn-danger' onclick=DeleteCustomer('" + row.BookId + "'); >Delete</a>"; }
            },
        ],
    });
});
