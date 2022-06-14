//$(document).ready(function () {
//    $("#myTable").DataTable({
//        "ajax": {
//            "url": "/Admin/Products/GetAll"
//        },
//        "columns": [
//            { "data": "title" },
//            { "data": "isbn" },
//            { "data": "price" },
//            { "data": "author" },
//            { "data": "category.name" },
//            { "data": "coverType.name" },
//            {
//                "data": "id",
//                "render": Function(data) {
//                return 
//                    `<a href="/admin/products/upsert?id=${data}" class="text-decoration-none">
//                        <i class="bi bi-pencil-square"></i> Edit
//                    </a> |
//                    <a href="/admin/products/delete?id=${data}" class="text-decoration-none text-danger">
//                        <i class="bi bi-trash3"></i> Delete
//                    </a>`;
//            }
//            },
//        ]
//    });
//});

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $("#myTable").DataTable({
        ajax: {
            url: "/Admin/Products/GetAll",
        },
        columns: [
            { data: "title" },
            { data: "isbn" },
            { data: "price" },
            { data: "author" },
            { data: "category.name" },
            { data: "coverType.name" },
            {
                data: "id",
                render: function (data) {
                    return `<div class="d-flex justify-content-between">
                                <a href="/admin/products/Upsert/${data}" class="text-decoration-none">
                                    <i class="bi bi-pencil-square"></i> Edit
                                </a> |
                                <a href="/admin/products/delete/${data}" onclick="del()" class="text-decoration-none text-danger">
                                    <i class="bi bi-trash3"></i> Delete
                                </a>
                            </div>`;
                },
            },
        ],
    });
}

function del() {
    //Swal.fire({
    //    title: "Are you sure?",
    //    text: "You won't be able to revert this!",
    //    icon: "warning",
    //    showCancelButton: true,
    //    confirmButtonColor: "#3085d6",
    //    cancelButtonColor: "#d33",
    //    confirmButtonText: "Yes, delete it!",
    //}).then((result) => {
    //    if (result.isConfirmed) {
    //        loadDataTable();
    //    }
    //});

    var decision = confirm("are you sure?");
    if (decision == false) {
        event.preventDefault();
    }
};
