$(document).ready(function () {
    $('#booksDatatable').dataTable({
        
        "ajax": {
            "url": "/Book/GetBooks",
            "type": "GET",
            "datatype": "json"

        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { title: "BookId", data: "bookId", autoWidth: true },
            { title: "Title", data: "title", autoWidth: true },
            { title: "Author", data: "author", autoWidth: true },
            { title: "Branch", data: "branch", autoWidth: true },
            {
                "render": function (data, row) { return "<a href='#' class='btn btn-danger' onclick=DeleteBook('" + row.BookId + "'); >Delete</a>"; }
            },
        ],
    });
});
