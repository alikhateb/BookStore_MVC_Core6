
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $("#myTable").DataTable({
        ajax: {
            url: "/Admin/Companies/GetAll",
        },
        columns: [
            { data: "name" },
            { data: "phoneNumber" },
            { data: "address" },
            { data: "city" },
            { data: "state" },
            { data: "postalCode" },
            {
                data: "id",
                render: function (data) {
                    return `<a href="/admin/companies/Upsert/${data}" class="text-decoration-none">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a> |
                            <a href="/admin/companies/delete/${data}" onclick="del()" class="text-decoration-none text-danger">
                                <i class="bi bi-trash3"></i> Delete
                            </a>`;
                },
            },
        ],
    });
}

function del() {
    var decision = confirm("are you sure?");
    if (decision == false) {
        event.preventDefault();
    }
};
