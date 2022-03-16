        $(document).ready(function () {

            var AddedCFList = [];
            var total = 0;
            var counter = 0;
            var parsedTotal = 0;    
            var isactive = true;
            var isdelete = false;            
            var txtactive = "";
            var txtdelete = "";
            var ParsedItemTotalAmount = 0;

                if ($('input[name="chckisDelete"]').is(':checked') == false) {
                    isdelete = false;
                    txtdelete = "Hayır";
                }
                if ($('input[name="chckisActive"]').is(':checked')) {
                    isactive = true;
                    txtactive = "Evet";
                }
                $("#chckisActive").change(function () {
                    if (this.checked == false) {
                        isactive = false;
                        txtactive = "Hayır";
                    }
                    else {
                        isactive = true;
                        txtactive = "Evet";
                    }
                });
                $("#chckisDelete").change(function () {
                    if (this.checked) {
                        isdelete = true;
                        txtdelete = "Evet";
                    }
                    else {
                        isdelete = false;
                        txtdelete = "Hayır";
                    }
                });            
            
            $("#btnNewCFbasket").on('click', function () {
                var date = $("#Date").val();
                var title = $("#ItemTitle").val();
                var categoryname = $('#CategoryName option:selected').text();
                var categoryId = $('#CategoryName option:selected').val();
                var unit = $("#Unit").val();
                var amount = parseFloat($("#Amount").val());
                var content = $("#ItemContent").val();
                var totalamount = parseFloat(unit * amount);
                var formatDate = "";
                var newDate = "";
                formatDate = date.split('-');
                newDate = formatDate[2] + '/' + formatDate[1] + '/' + formatDate[0].slice(-2);
                
                if (date == null || date == "") {
                $("#Date").removeClass("is-vaild").addClass("is-invalid");                
                return;
            }
            else {
                $("#Date").removeClass("is-invalid").addClass("is-valid");
            }
            if (title == null || title == "") {
                $("#ItemTitle").removeClass("is-vaild").addClass("is-invalid");
                return;
            }
            else {
                $("#ItemTitle").removeClass("is-invalid").addClass("is-valid");
            }
                if (categoryname == null || categoryname == 0 || categoryname == "Seçiniz") {
                $("#CategoryName").removeClass("is-vaild").addClass("is-invalid");
                return;
            }
            else {
                $("#CategoryName").removeClass("is-invalid").addClass("is-valid");
            }
            if (unit == null || unit == "") {
                $("#Unit").removeClass("is-vaild").addClass("is-invalid");
                return;
            }
            else {
                $("#Unit").removeClass("is-invalid").addClass("is-valid");
            }
            if (amount == null || amount == "") {
                $("#Amount").removeClass("is-vaild").addClass("is-invalid");
                return;
            }
            else {
                $("#Amount").removeClass("is-invalid").addClass("is-valid");
                }
            if (content == null || content == "") {
                $("#ItemContent").removeClass("is-vaild").addClass("is-invalid");
                return;
            }
            else {
                $("#ItemContent").removeClass("is-invalid").addClass("is-valid");
                }                
                if (totalamount % 1 == 0) {
                    ParsedItemTotalAmount = totalamount;
                } else {
                    ParsedItemTotalAmount = parseFloat(totalamount.toFixed(2));
                }
                var newCFList = {
                    Counter: counter,
                    Date: date,
                    Title: title,
                    CategoryName: categoryname,
                    Unit: unit,
                    Amount: amount,
                    TotalAmount: ParsedItemTotalAmount,
                    Content: content,
                    CategoryID: categoryId,
                    isActive: isactive,
                    isDelete: isdelete,
                    ActiveText: txtactive,
                    DeleteText: txtdelete,
                    DateText: newDate
                };
                total += parseFloat(ParsedItemTotalAmount);
                AddedCFList.push(newCFList);
                counter++;
                $("#cfbasket").empty();
                AddedCFList.forEach(function (item) { 
                    var BasketItem = "<tr><td>"
                        + item.DateText +
                        "</td><td>" +
                        item.Title +
                        "</td><td>" +
                        item.CategoryName +
                        "</td><td>" +
                        item.Unit +
                        "</td><td>" +
                        item.Amount + " ₺" +
                        "</td><td>" +
                        item.TotalAmount + " ₺" +
                        "</td><td>" +
                        item.Content +
                        "</td><td>" +
                        item.ActiveText +
                        "</td><td>" +
                        item.DeleteText +
                        "</td><td><td><button style='padding:0 10px 0 10px;' value=" +
                        item.Counter +
                        " class='btn btn-danger'>Sil</button></td>";
                    $("#cfbasket").append(BasketItem);
                })                
                if (total % 1 == 0) {
                    parsedTotal = total;
                } else {
                    parsedTotal = parseFloat(total.toFixed(2));
                }
                $("#TotalAmount").empty().append(parsedTotal + " ₺");
                $("#Date").val("");
                $("#ItemTitle").val("");
                $("#CategoryName").val('0');
                $("#Unit").val("");
                $("#Amount").val("");
                $("#ItemContent").val("");
                $("#chckisActive").prop("checked","checked");
                $("#chckisDelete").prop("checked",false);
                txtactive = "Evet";
                txtdelete = "Hayır";
                isactive = true;
                isdelete = false;
                ParsedItemTotalAmount = 0;                    
            });            
            $("#SaveCF").on('click', function () {
                if (AddedCFList.length == 0) {
                    $("#cfbasket").empty();
                    $("#cfbasket").append("<tr><td colspan='9' class='alert alert-danger text-center'>Girilmiş nakit akışı bulunamadı!</td></tr>")
                    return;
                }
                $("#SaveCF").addClass("disabled");
                $.post("/Admin/NewCashFlowEvent",
                    {
                        AddedCFList: AddedCFList,
                        contentType: "application/json; charset=utf-8"
                    },
                    function (data, status) {
                        if (status == "success") {
                            alert("Nakit akışı kaydedildi!");
                            window.location.replace("/Home/CashFlowSchedule");

                        }
                    });
            });
            $('#cfbasket').on('click', 'tr >td>button', function () {
                var tile = parseInt($(this).val());
                for (var i = 0; i < AddedCFList.length; i++) {
                    if (AddedCFList[i].Counter == tile) {
                        total -= parseFloat(AddedCFList[i].TotalAmount);
                        AddedCFList.splice(i, 1);

                        if (total % 1 == 0) {
                            parsedTotal = parseInt(total);
                        }
                        else {
                            parsedTotal = parseFloat(total.toFixed(2));
                        }
                    }
                }
                $("#cfbasket").empty();
                $("#TotalAmount").empty().append(parsedTotal + "₺");
                AddedCFList.forEach(function (item) {
                    var BasketItems = "<tr><td>"
                        + item.Date +
                        "</td><td>" +
                        item.Title +
                        "</td><td>" +
                        item.Category +
                        "</td><td>" +
                        item.Unit +
                        "</td><td>" +
                        item.Amount +
                        "</td><td>" +
                        item.TotalAmount +
                        "</td><td>" +
                        item.Content +
                        "</td><td><td><button style='padding:0 10px 0 10px;' value=" +
                        item.Counter +
                        " class='btn btn-danger'>Sil</button></td>";
                    $("#cfbasket").append(BasketItems);
                })
            });

        });