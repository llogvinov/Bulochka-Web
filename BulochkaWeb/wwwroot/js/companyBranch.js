var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/CompanyBranch/GetAll"
        },
        "columns": [
            { "data": "city", "width": "15%" },
            { "data": "streetAddress", "width": "30%" },            
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/CompanyBranch/Upsert?id=${data}"
                        class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i>Изменить</a>
                        <a onClick=Delete("/Admin/CompanyBranch/Delete/${data}")
                        class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i>Удалить</a>
                        </div>
                        `
                },
                "width": "15%"
            },
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Вы уверены?',
        text: 'Удаленные данные нельзя будет вернуть!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Да, удалить!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}