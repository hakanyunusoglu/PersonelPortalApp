// Call the dataTables jQuery plugin
$(document).ready(function() {
    $('#dataTable').DataTable({
        "order": [[5, "desc"]],
        "columnDefs": [
            {
                "targets": "Onaylanmadı",
                "visible": false,
                "searchable": false
            }]
    });
   
 
});

