// Call the dataTables jQuery plugin
$(document).ready(function() {
    $('#dataTable').DataTable({
        "order": [[5, "desc"]],
        "columnDefs": [
            {
                "targets": "Onaylanmadý",
                "visible": false,
                "searchable": false
            }]
    });
   
 
});

