$(document).ready(function () {
    var eventID = $("#CFID").val();
    $.ajax({
        type: 'POST',
        url: '/Admin/UpdateCFEventGetCategories/' + eventID,
        dataType: 'json',
        data: {},
        success: function (CategoryList) {
            $("#txtCategory").empty();
            $.each(CategoryList,
                function (i, getCategories) {
                    if (getCategories.Value == true) {
                        alert("Evet");
                        $("#txtCategory").append('<option value="' + getCategories.Value + '" selected>' + getCategories.Text + '</option>');
                    } else {
                        alert("Hayır");
                        $("#txtCategory").append('<option value="' + getCategories.Value + '">' + getCategories.Text + '</option>');
                    }
                });
            console.log(CategoryList);
        }
    });
});