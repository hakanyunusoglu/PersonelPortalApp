$(document).ready(function () {
     $.ajax({
        type: 'POST',
        url: '/Admin/newCFEventGetCategories',
        dataType: 'json',
        data: {},
        success: function (CategoryList) {
            $("#CategoryName").empty();
            $("#CategoryName").append('<option value="0" selected disabled>Seçiniz</option>');
            $.each(CategoryList,
                function (i, getCategories) {
                    $("#CategoryName").append('<option value="' + getCategories.Value + '">' + getCategories.Text + '</option>');

                });
        }
    });
});