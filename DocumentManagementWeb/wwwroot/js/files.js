
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/home/getall'

        },

        "columns": [
            { data: 'userName', "width": "20%" },
            { data: 'fileName', "width": "45%" },
            { data: 'createdAt', "width": "25%" },
//            {
//                data: 'fileName',
//                "render": function (data) {
//                    return `
//<a  href="" asp-controller="Home" asp-action="DownloadFile" asp-route-fileName="${data}">
//Helo
//</a>
//`
//                }, 
//                "width": "10%"
//            }
        ]
    });
}