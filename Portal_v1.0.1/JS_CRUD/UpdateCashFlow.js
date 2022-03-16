$(document).ready(function () {
    var eventID = $("#CFID").val();
    var TotalAmount = 0;
    var UnitVal = $("#txtUnit").val();
    var AmountVal = $("#txtAmount").val();
    TotalAmount = parseFloat(UnitVal * AmountVal);

    var date = $('#txtDate').val();
    var title = $('#txtTitle').val();
    var unit = $('#txtUnit').val();
    var amount = $('#txtAmount').val();
    var content = $('#txtContent').val();

    $("#btnUpdate").on('click', function () {
        var formData = new FormData();
        formData.append('ID', eventID);
        formData.append('Date', $('#txtDate').val());
        formData.append('Title', $('#txtTitle').val());
        formData.append('CategoryID', $('#txtCategory :selected').val());
        formData.append('CategoryName', $('#txtCategory :selected').text());
        formData.append('Unit', $('#txtUnit').val());
        formData.append('Amount', $('#txtAmount').val());
        formData.append('TotalAmount', TotalAmount);
        formData.append('Content', $('#txtContent').val());
        formData.append('isActive', $('#chckisActive').is(':checked'));
        formData.append('isDelete', $('#chckisDelete').is(':checked'));

        if (date == null || date == "") {
            $("#txtDate").removeClass("is-valid").addClass("is-invalid");
            return;
        }
        else {
            $("#txtDate").removeClass("is-invalid").addClass("is-valid");
        }
        if (title == null || title == "") {
            $("#txtTitle").removeClass("is-valid").addClass("is-invalid");
            return;
        }
        else {
            $("#txtTitle").removeClass("is-invalid").addClass("is-valid");
        }
        if (unit == null || unit == "") {
            $("#txtUnit").removeClass("is-valid").addClass("is-invalid");
            return;
        }
        else {
            $("#txtUnit").removeClass("is-invalid").addClass("is-valid");
        }
        if (unit == 0) {
            unit = 1;
        }
        if (AmountVal == null || AmountVal == "") {
            $("#txtAmount").removeClass("is-valid").addClass("is-invalid");
            return;
        }
        else {
            $("#txtAmount").removeClass("is-invalid").addClass("is-valid");
        }
        if (content == null || content == "") {
            $("#txtContent").removeClass("is-valid").addClass("is-invalid");
            return;
        }
        else {
            $("#txtContent").removeClass("is-invalid").addClass("is-valid");
        }                
        $.ajax({
            type: "POST",
            url: '/Admin/UpdateCashFlowEvent/' + eventID,
            dataType: 'json',
            data: formData,
            contentType: false,
            processData: false,
            success: function (data, status) {
                if (status == "success") {
                    alert("Bilgiler başarıyla güncellendi!");
                    window.location.replace("/Home/CashFlowSchedule");
                }               
            },
            error: (function () {
                if (status != "success") {
                    alert("Bilgiler güncellenirken bir sorun oluştu!");
                }   

            })
        });
    });

    $('#txtUnit').keyup(function () {
        var newUnit;
        var newAmount;
        var newTotalAmount;
        newUnit = parseFloat($('#txtUnit').val());
        newAmount = parseFloat($('#txtAmount').val());
        newTotalAmount = newUnit * newAmount;
        TotalAmount = 0;
        TotalAmount = newTotalAmount;
        $('#txtTotalAmount').val(newTotalAmount);
    });
    $('#txtAmount').keyup(function () {
        var newUnit;
        var newAmount;
        var newTotalAmount;
        newUnit = parseFloat($('#txtUnit').val());
        newAmount = parseFloat($('#txtAmount').val());
        newTotalAmount = newUnit * newAmount;
        TotalAmount = 0;
        TotalAmount = newTotalAmount;
        $('#txtTotalAmount').val(newTotalAmount);
    });
});