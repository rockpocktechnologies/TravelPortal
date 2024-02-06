
    //$("#saveButton").click(function () {
    //    var bookingData = {
    //        EmployeeName: "John Doe", // Replace with actual values
    //        // Add other booking details here
    //    };

    //    $.ajax({
    //        url: "/Booking/SaveBooking",
    //        type: "POST",
    //        contentType: "application/json",
    //        data: JSON.stringify(bookingData),
    //        success: function (data) {
    //            // Handle success (e.g., show a success message)
    //        },
    //        error: function (error) {
    //            // Handle error (e.g., show an error message)
    //        }
    //    });
    //});

function SubmitBookingDetails() {
    if (confirm("Are you sure you want to submit booking?") == true) {
        SaveOrSubmitBooking(true);
    } else {
        $('#preloader').hide();
        text = "You canceled!";
    }
}

function SaveBookingDetails() {
    SaveOrSubmitBooking(false);
}

function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
    return false;
};


function CheckValidationBeforeSaveOrSubmitBooking() {
    var isValid = true;

    
    $(".employeeSectionHeader").removeClass("error");
    $(".mandatoryFields").removeClass("error");

   //$(".employeeSectionContent").each(function () {

        // Reset the style of all input fields
        
    var isCacnelledmessageshownonce = false;
        $(".mandatoryFields").each(function () {
            if (!$(this).is(':disabled') && $(this).val() == "") {
                var haserror = false;
                if ($(this).hasClass("attachmentFiles")) {
                    if ($(this).is(":Visible")){
                        $(this).next().addClass("error");
                        haserror = true;
                    }
                }
                else {
                    $(this).addClass("error");
                    haserror = true;

                    if ($(this).is("#cancellationCharges") && isCacnelledmessageshownonce == false) {
                        alert("Add cancellation charges");
                        isCacnelledmessageshownonce = true;
                    }
                }

                if (haserror) {
                    $(this).closest(".employeeSectionContent").prev().addClass("error");
                    isValid = false;
                }
                
            }
       });
        

        // Get values from the input fields
        //var departureCity = ($("#travelFrom").val() == "" ? false : true);
        //var destinationCity = ($("#travelTo").val() == "" ? false : true);
        //var departureDate = ($("#travelDepartureDate").val() == "" ? false : true);


        //var returnDate = ($("#travelReturnDate").val() == "" ? false : true);
        //if ($('#travelReturnDate').prop('disabled') == true || $('#travelReturnDate').is(':Visible') == false) {//skip vlidation when disbaled or hidden return date
        //    returnDate = true;
        //}
        //if (
        //    !travelMode ||
        //    !ticketBookedBy ||
        //    !travelDestination ||
        //    !travelType ||
        //    !departureCity ||
        //    !destinationCity ||
        //    !departureDate ||
        //    !returnDate

        //) {
        //    isValid = false;

        //    // Highlight the empty fields in red and set focus on the first one
        //    if (!travelMode) $("#divForTravelMode").find(".tilediv").addClass("error");
        //    if (!ticketBookedBy) $("#divTicketBookedBy").find(".tilediv").addClass("error");
        //    if (!travelDestination) $("#divTravelDestination").find(".tilediv").addClass("error");
        //    if (!travelType) $("#divTravelType").find(".tilediv").addClass("error");
        //    if (!departureCity) $("#travelFrom").addClass("error")
        //    if (!destinationCity) $("#travelTo").addClass("error")
        //    if (!departureDate) $("#travelDepartureDate").addClass("error")
        //    if (!returnDate) $("#travelReturnDate").addClass("error")
        //}

        // You can add additional validation logic here

        if (!isValid) {
            // alert("Please fill in all mandatory fields.");
        }


/*    });*/
    return isValid;

}

$(".mandatoryFields").on("click", function () {
    // Your code to handle the click event here
    // For example, you can add a CSS class or perform some action
    $(this).removeClass("error");
});

function SaveOrSubmitBooking(isSubmitted) {
    if (isSubmitted) {//validate only in case of submit
        if (CheckValidationBeforeSaveOrSubmitBooking()) {
            SaveOrSubmitBookingData(isSubmitted);
        }
    }
    else {
        SaveOrSubmitBookingData(isSubmitted);
    }
}

function SaveOrSubmitBookingData(isSubmitted) {

        $('#preloader').show();
        var bookingData = [];
        var strRequestToken = getUrlParameter("requestToken").toString();
        // Iterate over each .employeeSectionContent and gather data
        $(".employeeSectionContent").each(function () {
            var bookingDetails = {
                RequestToken: strRequestToken,
                Bookingid: $(this).attr("data-bookingid"),
                FromPlace: $(this).find("#fromPlace").val(),
                ToPlace: $(this).find("#toPlace").val(),
                DepartureDate: $(this).find(".departureDate").val(),
                ArrivalDate: $(this).find("#arrivalDate").val(),
                TravelClass: $(this).find("#travelClass").val(),
                AirlineNumber: $(this).find("#airlineNumber").val(),
                PnrNumber: $(this).find("#pnrNumber").val(),
                InvoiceNumber: $(this).find("#invoiceNumber").val(),
                InvoiceDate: $(this).find(".InvoiceDate").val(),
                Fare: $(this).find("#fare").val(),
                Tax: $(this).find("#tax").val(),
                TotalAmount: $(this).find("#totalAmount").val(),
                Discount: $(this).find("#discount").val(),
                ServiceCharges: $(this).find("#serviceCharges").val(),
                ServiceTax: $(this).find("#serviceTax").val(),
                NetAmount: $(this).find("#netAmount").val(),
                Attachment: ($(this).find(".classforfapaperclip").css('display') !== 'none' ? $(this).find(".classforfapaperclip").attr("data-Attachment"): $(this).find("#attachment").val()),
                TicketStatus: $(this).find("#ticketStatus").val(),
                CancellationCharges: $(this).find("#cancellationCharges").val(),
                Comments: $(this).find("#comments").val(),
                IsSubmit: isSubmitted
            };
            bookingData.push(bookingDetails);
        });


        // UploadAttachments(12);
        // Send the data to the controller using an AJAX POST request
        $.ajax({
            type: "POST",
            url: "/Booking/SaveBooking",
            data: JSON.stringify(bookingData),
            contentType: "application/json",
            success: function (data) {
                // Handle the success response here

                var iscancelled = getUrlParameter("iscancelled").toString();
                if (iscancelled == "true") {
                    $('#preloader').hide();
                    window.location.href = '/MyApprovals';
                }
                else {
                    UploadAttachments(strRequestToken, isSubmitted, data);
                }
            },
            error: function (xhr, status, error) {
                // Handle any errors here
                $('#preloader').hide();
                console.log(error);
            }
        });

    }

function UploadAttachments(strRequestToken, isSubmitted, bookingIds) {

    // Create a FormData object to store the files
    var formData = new FormData();
    var bookingIdsNew = []; 
    // Loop through each file input element with class "attachmentFiles"
    var isFileExist = false;
    $('.attachmentFiles').each(function (index, element) {
        var files = element.files;
        for (var i = 0; i < files.length; i++) {
            formData.append('attachments', files[i]);
            bookingIdsNew.push(bookingIds[index]);
            isFileExist = true;
        }
    });

    if (isFileExist) {
        formData.append('RequestToken', strRequestToken);
        formData.append('strEmployeeName', $("#hdnEmployeeName").val());
        formData.append('strEmployeeEmail', $("#hdnEmployeeEmail").val());
        formData.append('bookingIds', JSON.stringify(bookingIdsNew));

        // Send the data to the controller using an AJAX POST request
        $.ajax({
            type: 'POST',
            url: '/Booking/UploadAttachments', // Update the URL as per your routing
            data: formData,
            processData: false, // Prevent jQuery from processing data
            contentType: false, // Prevent jQuery from setting content type
            success: function (data) {
                // Handle the success response here
                if (data == "1") {
                    if (isSubmitted) {
                        sendemailtouserticketbooked(strRequestToken)
                        alert("Booking submitted successfully")
                    }
                    else {
                        if ($("#savebtn").text() == "Update") {
                            alert("Booking updated successfully")
                        }
                        else {
                            alert("Booking saved successfully")
                        }
                        
                    }
                   

                    window.location.href = '/MyApprovals';
                }
                else {
                    alert("Error occurred");
                    $('#preloader').hide();
                }
            },
            error: function (xhr, status, error) {
                // Handle any errors here
                $('#preloader').hide();
                console.log(error);
            }
        });
    }
    else {
        if (isSubmitted) {
            sendemailtouserticketbooked(strRequestToken)
            alert("Booking submitted successfully")
        }
        else {
            if ($("#savebtn").text() == "Update") {
                alert("Booking updated successfully")
            }
            else {
                alert("Booking saved successfully")
            }
        }
        window.location.href = '/MyApprovals';
    }
    // Add the bookingId to the FormData
   
}

function sendemailtouserticketbooked(strRequestToken) {
    var formData = new FormData();

    formData.append('requestToken', strRequestToken);
    formData.append('employeeName', $("#hdnEmployeeName").val());
    formData.append('employeeEmail', $("#hdnEmployeeEmail").val());

    $.ajax({
        type: "POST",
        url: "/Booking/SendEmailWithAttachments",
        data: formData,
        contentType: false,
        processData: false,
        success: function (result) {
            // Handle success (email sent) here
            $('#preloader').hide();
            console.log("Email sent successfully!");
        },
        error: function (error) {
            // Handle errors here
            console.error("Error sending email:", error);
        }
    });
}


$('.attachmentFiles').change(function () {
    var fileInput = $(this)[0];
    if (fileInput.files.length > 0) {
        var file = fileInput.files[0];
        var maxSizeInBytes = 5 * 1024 * 1024; // 5 MB
        var allowedFileTypes = ['application/pdf'];

        // Check file size
        if (file.size > maxSizeInBytes) {
            alert('File size exceeds the maximum allowed (5 MB). Please choose a smaller file.');
            $(this).val(''); // Clear the file input
            return;
        }

        // Check file type
        if (allowedFileTypes.indexOf(file.type) === -1) {
            alert('Invalid file type. Please select a PDF file.');
            $(this).val(''); // Clear the file input
            return;
        }

        // Update the attachment name and show delete icon
        $(this).closest('.form-group').find('.attachmentName').text(file.name);
        $(this).closest('.form-group').find('.deleteIcon').show();
        $(this).next('.attachmentName').removeClass("error");
    }
});


function DeleteCurrentAttachment(control) {
    $(control).closest('.form-group').find(".attachmentFiles").val('');
    $(control).closest('.form-group').find('.attachmentName').text("Choose File");
    $(control).hide();
    $(control).closest('.form-group').find(".custom-file").show();
    $(control).closest('.form-group').find('.classforfapaperclip').hide();
    $(control).closest('.form-group').find('.classfordownloadheader').hide();
    $(control).closest('.form-group').find('.classforAttachmentheader').show();
    
}

    // Function to calculate Total Amount
function calculateTotalAmount(control) {

    var parent = $(control).closest(".employeeSectionContent");
        var fare = parseFloat($(parent).find('#fare').val()) || 0;
    var tax = parseFloat($(parent).find('#tax').val()) || 0;
    var totalAmount = fare + tax;
    $(parent).find('#totalAmount').val(totalAmount);
    calculateNetAmount(control);
    }

    // Function to calculate Net Amount
function calculateNetAmount(control) {

    var parent = $(control).closest(".employeeSectionContent");
        var totalAmount = parseFloat($(parent).find('#totalAmount').val()) || 0;
    var discount = parseFloat($(parent).find('#discount').val()) || 0;
    var serviceCharges = parseFloat($(parent).find('#serviceCharges').val()) || 0;
    var serviceTax = parseFloat($(parent).find('#serviceTax').val()) || 0;
    var netAmount = totalAmount - discount + serviceCharges + serviceTax;
    $(parent).find('#netAmount').val(netAmount);
    }

$(document).on('click', '.employeeSectionHeader', function () {
    $('.employeeSectionContent').not($(this).next('.employeeSectionContent')).slideUp();
    $(this).next('.employeeSectionContent').slideToggle();
});

$(document).ready(function () {
    var currentSection = 0;
    $('.employeeSectionHeader').click(function () {
    });

    var newBookingSection = $('.divMainContents').clone();
    $('#addMoreButton').click(function () {
        var sectionNumber = $('#bookingSections .accordion-content').length + 1;
        currentSection = sectionNumber; // Update the current section number
        var newSection = newBookingSection.clone();
        $('#bookingSections').append(newSection);
        newSection.find('.bookingCounter').text(sectionNumber);
        newSection.find('.employeeSectionHeader').attr('data-section', sectionNumber);
        newSection.find('.employeeSectionContent').attr('data-section', sectionNumber);
        $('.employeeSectionContent').slideUp();
        newSection.find('.employeeSectionContent').slideDown(); // Expand the new section

    });
});

function openNav() {
    document.getElementById("mySidebar").style.width = "250px";
    document.getElementById("content").style.marginLeft = "250px";
}

function closeNav() {
    document.getElementById("mySidebar").style.width = "0";
    document.getElementById("content").style.marginLeft = "0";
}

//$(".InvoiceDate").datepicker({
//    minDate: 0, // Disable past dates
//    dateFormat: "dd/mm/yy",
//    onSelect: function (selectedDate) {

//    }
//});

$(".clanederIconClick").on("click", function () {
    $(this).closest(".clanederIconClickRow").find('input').focus()
});

$(document).ready(function () {
   /* $(".departureDate, .ReturnDate").addClass("AddDatepicker");*/

    // Initialize datepickers for elements with the "AddDatepicker" class
    $(".AddDatepicker").datepicker({
        minDate: 0, // Disable past dates
        dateFormat: "dd/mm/yy",
        onSelect: function (selectedDate) {
            // Your onSelect code here
        }
    });
});


$(document).ready(function () {
    $('#preloader').show(); 
    var stringTravelRequestToken = getUrlParameter("requestToken").toString();

    // Call the WebMethod to fetch edit details
    fetchEditDetails(stringTravelRequestToken);
});

function fetchEditDetails(stringTravelRequestToken) {
    $.ajax({
        type: "POST", // Use POST for WebMethod
        url: "/Booking/GetEditDetails", // Adjust the URL based on your controller and method names
        data: { stringTravelRequestToken: stringTravelRequestToken },
        dataType: "json",
        success: function (data) {
            $('#preloader').hide();
            // Assuming the WebMethod returns data in JSON format
            if (data.length > 0) {
                BindEditDetailsToUI(data);
                $("#savebtn").text("Update");
            }
          
        },
        error: function (error) {
            $('#preloader').hide();
            console.error("Error fetching edit details: ", error);
        }
    });
}

function BindEditDetailsToUI(data) {
    // Loop through each booking section and update the UI controls
    $(".employeeSectionContent").each(function (index, element) {
        var sectionData = data[index];
        $(element).attr("data-bookingid", sectionData.Booking_id);
        // Update UI controls with data from the WebMethod response
        $(element).find("#fromPlace").val(sectionData.FromPlace);
        $(element).find("#toPlace").val(sectionData.ToPlace);
        $(element).find("#departureDate-" + index).val(sectionData.DepartureDate);
        $(element).find("#departureDate").val(sectionData.DepartureDate);
        /*$(element).find("#arrivalDate-" + index).val(sectionData.ArrivalDate);*/
        $(element).find("#airlineNumber").val(sectionData.AirlineNumber); 
        $(element).find("#pnrNumber").val(sectionData.PnrNumber); 
        $(element).find("#invoiceNumber").val(sectionData.InvoiceNumber); 
        $(element).find("#invoiceDate-" + index).val(sectionData.InvoiceDate); 
        $(element).find("#invoiceDate").val(sectionData.InvoiceDate); 
        $(element).find("#fare").val(sectionData.Fare); 
        $(element).find("#tax").val(sectionData.Tax); 
        $(element).find("#totalAmount").val(sectionData.TotalAmount); 
        $(element).find("#discount").val(sectionData.Discount); 
        $(element).find("#serviceCharges").val(sectionData.ServiceCharges); 
        $(element).find("#serviceTax").val(sectionData.ServiceTax); 
        $(element).find("#netAmount").val(sectionData.NetAmount); 
        $(element).find("#discount").val(sectionData.Discount); 
        if (sectionData.TicketStatus != "") {
            $(element).find('#ticketStatus option[value=' + sectionData.TicketStatus + ']').attr("selected", "selected");

        }
        $(element).find("#cancellationCharges").val(sectionData.CancellationCharges);
        $(element).find("#comments").val(sectionData.Comments);
        if (sectionData.TravelClass != "") {
            $(element).find('#travelClass option[value=' + sectionData.TravelClass + ']').attr("selected", "selected");

        }
        if (sectionData.Attachment != "" && sectionData.Attachment != null) {
            $(element).find(".classforfapaperclip").attr("href", "Booking/DownloadAttachment?BookingId=" + sectionData.Booking_id).show();
            $(element).find(".classforfapaperclip").attr("data-Attachment", sectionData.Attachment);
            $(element).find(".custom-file").hide();
            $(element).find(".deleteIcon").show();
            $(element).find(".classfordownloadheader").show();
            $(element).find(".classforAttachmentheader").hide();
        }
       

        if (sectionData.IsCancelled) {
            $(element).find("input, select, button").prop("disabled", true);
            $(element).find(".deleteIcon").remove();
            $(element).find("#cancellationCharges").prop("disabled", false);
        }
    });
}

function getTravelRequestId() {
    // Implement the logic to get the travel request ID, e.g., from a hidden field
    return $("#hdnTravelRequestId").val();
}
