$(document).ready(function () {
    var dayevents = [];
    var events = [];
    var toplam = 0;
    var checkedDate = "";
    var CfEventID = -1;
    var navlinkbtn = "";
    var deletedMessage = "[SİLİNDİ]";
    var activeMessage = "[AKTİF]";
    var passiveMessage = "[PASİF]";
    var newTitle = "";
    var chckactive = "";
    var chckdelete = "";
    var parsedTotal = 0;
    var parsedTotalAmount = 0;
    var newtotal = 0;

    $("#toplam").empty().append(toplam + " ₺");
    $.ajax({
        type: "GET",
        url: "/Home/CashFlowEvent",
        success: function (data) {
            $.each(data, function (i, v) {
                if (v.isDelete == true && v.isActive == true) {
                    newTitle = deletedMessage + " " + v.Title;
                } else if (v.isActive == false && v.isDelete == false) {
                    newTitle = passiveMessage + " " + v.Title;
                } else if (v.isActive == false && v.isDelete == true) {
                    newTitle = deletedMessage + passiveMessage + " " + v.Title;
                } else {
                    newTitle = v.Title;
                }
                events.push({
                    title: newTitle,
                    start: moment(v.Date),
                    color: v.EventColor,
                    allDay: true,
                    content: v.Content,
                    amount: v.Amount,
                    ID: v.ID,
                    unit: v.Unit,
                    totalamount: v.TotalAmount,
                    categoryName: v.CategoryName,
                    isactive: v.isActive,
                    isdelete: v.isDelete,
                    orjTitle: v.Title
                }); 
            })
            
            GenerateCalender(events);
        },
        error: function (error) {
            alert('failed');
        }
    })
    function GenerateCalender(events) {
        $('#calender').fullCalendar('destroy');
        $('#calender').fullCalendar({
            customButtons: {
                myCustomButton: {
                    text: 'Nakit Akışı Ekle',
                    click: function (e) {                        
                        $(".fc-myCustomButton-button").attr("data-toggle", "modal");
                        $(".fc-myCustomButton-button").attr("data-target", "#newCFModal");
                            $('#newCFModal').on('show.bs.modal',
                                function (e) {
                                    $('#newCFModalBody').load("/Admin/NewCFEvent/");                                    
                                }); 
                        }
                }
            },
            contentHeight: 450,
            weekNumbers: true,
            showNonCurrentDates: false,
            header: {
                left: 'prevYear,prev,next,nextYear today, myCustomButton',
                center: 'title',
                right: 'timelineCustom,month,agendaWeek'
            },
            defaultView: 'month',
            views: {
                timelineCustom: {
                    type: 'resourceTimeline',
                    buttonText: 'Year',
                    dateIncrement: { years: 1 },
                    slotDuration: { months: 1 },
                    visibleRange: function (currentDate) {
                        return {
                            start: currentDate.clone().startOf('year'),
                            end: currentDate.clone().endOf("year")
                        };
                    }
                }
            },
            navLinks: true,
            navLinkDayClick: function (date, jsEvent) {
                $(document).ready(function () {
                    $('#detailcashflow #cashflowdetailbody').empty();
                    $("#toplam").empty();
                    toplam = 0;
                    while (dayevents.length > 0 || events.length > 0) {
                        dayevents.pop();
                        events.pop();
                        $('#detailcashflow #cashflowdetailbody').empty();
                        $("#toplam").empty().append(toplam + " ₺");

                    }
                    checkedDate = date.toISOString();
                    $.ajax({
                        type: "GET",
                        url: "/Home/CashFlowDayEvents",
                        dataType: 'json',
                        data: { checkedDate },
                        success: function (data) {
                            $.each(data, function (i, v) {
                                if (v.TotalAmount % 1 == 0) {
                                    parsedTotal = v.TotalAmount;
                                } else {
                                    parsedTotal = parseFloat((v.TotalAmount).toFixed(2));
                                }
                                dayevents.push({
                                    title: v.Title,
                                    start: v.Date,
                                    content: v.Content,
                                    amount: v.Amount,
                                    ID: v.ID,
                                    unit: v.Unit,
                                    totalamount: parsedTotal,
                                    categoryName: v.CategoryName,
                                    isactive: v.isActive,
                                    isdelete: v.isDelete
                                });
                                parsedTotal = 0;
                            });

                            $("#toplam").empty().append(toplam + " ₺");                            
                            $.each(dayevents, function (i, item) { 
                                CfEventID = 0;
                                navlinkbtn = "<button type='button'  style='padding:0 10px 0 10px;' value=" + dayevents[i].ID + " class='btn btn-warning' id='eventUpdate' data-toggle='modal' data-target='#myModal'>Düzenle</button>";

                                if (dayevents[i].isactive == true && dayevents[i].isdelete == false) {
                                    chckactive = "<span class='fa fa-circle text-success fa-lg' title='Aktif'></span>";
                                    chckdelete = "<span class='fa fa-circle text-danger fa-lg' title='Silinmemiş'></span>";
                                } else if (dayevents[i].isactive == false && dayevents[i].isdelete == false) {
                                    chckactive = "<span class='fa fa-circle text-danger fa-lg' title='Aktif Değil'></span>";
                                    chckdelete = "<span class='fa fa-circle text-danger fa-lg' title='Silinmemiş'></span>";
                                } else if (dayevents[i].isactive == false && dayevents[i].isdelete == true) {
                                    chckactive = "<span class='fa fa-circle text-danger fa-lg' title='Aktif Değil'></span>";
                                    chckdelete = "<span class='fa fa-circle text-success fa-lg' title='Silinmiş'></span>";
                                } else if (dayevents[i].isactive == true && dayevents[i].isdelete == true) {
                                    chckactive = "<span class='fa fa-circle text-success fa-lg' title='Aktif'></span>";
                                    chckdelete = "<span class='fa fa-circle text-success fa-lg' title='Silinmiş'></span>";
                                }
                                $('<tr>').html(
                                    "<td>" + dayevents[i].start + "</td><td>" + dayevents[i].title + "</td><td>" + dayevents[i].categoryName + "</td><td>" + dayevents[i].unit + "</td><td>" + dayevents[i].amount + " ₺ </td><td>" + parseFloat(dayevents[i].totalamount) + " ₺ </td><td>" + dayevents[i].content + "</td><td>" + chckactive + " - " + chckdelete + "</td><td>"+ navlinkbtn +"</td>").appendTo('#cashflowdetailbody');
                                if (dayevents[i].isactive == true && dayevents[i].isdelete == false) {
                                    toplam += parseFloat(dayevents[i].totalamount);  
                                }
                                if (toplam % 1 == 0) {
                                    newtotal = toplam;
                                } else {
                                    newtotal = parseFloat((toplam).toFixed(2));
                                }
                                $("#toplam").empty().append(newtotal + " ₺");
                                navlinkbtn = " ";
                                $("button").click(function () {
                                    var clickedValue = $(this).val();
                                    CfEventID = clickedValue;
                                });
                            });
                        },
                        error: function (error) {
                            alert('Hata oluştu. Tekrar deneyiniz!');
                        }
                    });

                });
            },
            eventLimit: true,
            locale: "tr",
            events: events,
            eventClick: function (calEvent, jsEvent, view) {
                while (events.length > 0 || dayevents.length > 0) {
                    events.pop();
                    dayevents.pop();
                    $('#detailcashflow #cashflowdetailbody').empty();
                    $("#toplam").empty();
                }
                CfEventID = 0;
                if (calEvent.totalamount % 1 == 0) {
                    parsedTotal = calEvent.totalamount;
                } else {
                    parsedTotal = parseFloat((calEvent.totalamount).toFixed(2));
                }
                var $description = $('<tr/>');
                $description.append($('<td>').html(calEvent.start.format("DD/MM/YYYY") + '</td>'));
                $description.append($('<td>').html(calEvent.orjTitle + '</td>'));
                $description.append($('<td>').html(calEvent.categoryName + '</td>'));
                $description.append($('<td>').html(calEvent.unit + '</td>'));
                $description.append($('<td>').html(calEvent.amount + ' ₺ </td>'));
                $description.append($('<td>').html(parsedTotal + ' ₺ </td>'));
                $description.append($('<td>').html(calEvent.content + '</td>'));
                if (calEvent.isactive == true && calEvent.isdelete == false) {
                    $description.append($('<td>').html('<span class="fa fa-circle text-success fa-lg" title="Aktif"></span> - <span class="fa fa-circle text-danger fa-lg" title="Silinmemiş"></span></td>'));
                } else if (calEvent.isactive == false && calEvent.isdelete == false) {
                    $description.append($('<td>').html('<span class="fa fa-circle text-danger fa-lg" title="Aktif Değil"></span> - <span class="fa fa-circle text-danger fa-lg" title="Silinmemiş"></span></td>'));
                } else if (calEvent.isactive == false && calEvent.isdelete == true) {
                    $description.append($('<td>').html('<span class="fa fa-circle text-danger fa-lg" title="Aktif Değil"></span> - <span class="fa fa-circle text-success fa-lg" title="Silinmiş"></span></td>'));
                } else if (calEvent.isactive == true && calEvent.isdelete == true) {
                    $description.append($('<td>').html('<span class="fa fa-circle text-success fa-lg" title="Aktif"></span> - <span class="fa fa-circle text-success fa-lg" title="Silinmiş"></span></td>'));
                }
                $description.append($('<td>').html("<button  style='padding:0 10px 0 10px;' value=" + calEvent.ID + " class='btn btn-warning' id='eventUpdate' data-toggle='modal' data-target='#myModal'>Düzenle</button></td>"));

                CfEventID = calEvent.ID;
                if (calEvent.isactive == true && calEvent.isdelete == false) {
                    $("#toplam").empty().append(parsedTotal + " ₺");
                } else {
                    $("#toplam").empty().append(0 + " ₺");
                }                
                $('#detailcashflow #cashflowdetailbody').empty().html($description);   
                parsedTotal = 0;
            }
        })
    }
    $(document).ready(function () {
        $('#myModal').on('show.bs.modal',
            function (e) {
                $('#CashFlowUpdateModalBody').load("/Home/ShowCashFlowUpdateModal/" + CfEventID);
            });
    });
});