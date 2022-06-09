


$(document).ready(function () {
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });
    console.log(1);
    let value = params.status;
    let active = document.getElementsByClassName("list-group-item");
    switch (value) {
        case "inprocess":
            active[1].classList.add("active");
            break;
        case "pending":
            active[2].classList.add("active");
            break;
        case "completed":
            active[3].classList.add("active");
            break;
        default:
            active[0].classList.add("active");
    }
    loadDataTable();
});

function loadDataTable() {
    $("#myTable").DataTable({
        ajax: {
            url: "/Admin/Orders/GetAll",
        },
        columns: [
            { data: "id" },
            { data: "name" },
            { data: "phoneNumber" },
            { data: "appUser.email" },
            { data: "orderStatus" },
            { data: "paymentStatus" },
            { data: "totalPrice" },
            {
                data: "id",
                render: function (data) {
                    return `<a href="/admin/Orders/Details?orderId=${data}" class="text-decoration-none">
                                <i class="bi bi-pencil-square"></i> Details
                            </a>`
                },
            },
        ],
    });
}

