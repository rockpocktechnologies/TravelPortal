
        // JavaScript code here
function addRow() {
    // Clone the row using jQuery
    var newRow = $("#expense-row").clone();
    newRow.removeAttr('id');
    newRow.css("display", "");
    // Append the cloned row to the table
    $(".expense-table").append(newRow);
    $(".expense-row:last").find(".expenseDate").removeAttr("id").removeClass("hasDatepicker");

    // Show the table (if hidden)
    $(".expense-table").show();
    InitializeKeyEvents();
    InitializeCalenders();
    SetDomesticCurrency();
}

    function removeExpenseRow(button) {


       
        if (confirm('Are you sure you want permanently to delete this expense row?')) {
            var row = button.parentElement.parentElement;
            row.remove();

      

            var expenseID = $(row).find('.expenseID').text();


            if (expenseID != null && expenseID != "" && expenseID != '0') {
                // Perform AJAX request to call the controller action
                $.ajax({
                    url: '/AddExpense/DeleteExpenseRow', // Update the URL with your controller and action
                    type: 'POST',
                    data: { id: expenseID },
                    success: function (response) {
                        // Handle the success response if needed
                        alert("Expense Deleted")
                    },
                    error: function (error) {
                        // Handle the error response if needed
                        console.error('Error deleting Expense:', error);
                    }
                });

            }
            else {
                alert("Expense Deleted")
            }
        }


        }

    function openNav() {
        document.getElementById("mySidebar").style.width = "250px";
    document.getElementById("content").style.marginLeft = "250px";
        }

    function closeNav() {
        document.getElementById("mySidebar").style.width = "0";
    document.getElementById("content").style.marginLeft = "0";
        }

    // Initialize the magnificPopup plugin
    //$(document).ready(function () {
    //    $('.popup-link').magnificPopup({
    //        type: 'inline',
    //        removalDelay: 300,
    //        mainClass: 'mfp-fade'
    //    });
    //    });

$(document).ready(function () {
    // Fetch currencies
    $.ajax({
        url: "/NewRequest/GetCurrencies",
        type: "GET",
        dataType: "json",
        success: function (data) {
            var currencySelect = $("#selectCurrency");

            // Clear existing options
            currencySelect.empty();

            // Add a "Please select" option as the first item
            currencySelect.append($("<option></option>")
                .attr("value", "")
                .text("Please select"));

            // Add currency options from the data returned by the AJAX call
            $.each(data, function (index, currency) {
                currencySelect.append($("<option></option>")
                    .attr("data-rate", currency.exchangeRate)
                    .attr("value", currency.currencyId)
                    .text(currency.currencyCode));
            });
            PopulateCurrency();
            //if (window.location.href.indexOf("EditTravelRequest") > -1) {
            //    if ($("#selectCurrency").attr("data-value") !== undefined) {
            //        $("#selectCurrency").val($("#selectCurrency").attr("data-value"));
            //    }
            //}
        },
        error: function (error) {
            console.error(error);
        }
    });


});

$(document).ready(function () {
    // Fetch categories
    $.ajax({
        url: "/AddExpense/GetTravelCategories", // Replace with your actual controller and action
        type: "GET",
        dataType: "json",
        success: function (data) {
            var isDomestic = IsDomesticRequest();
            var categorySelect = $("#selectCategory");

            // Clear existing options
            categorySelect.empty();

            // Add a "Please select" option as the first item
            categorySelect.append($("<option></option>")
                .attr("value", "")
                .text("Please select"));

            // Add category options from the data returned by the AJAX call
            $.each(data, function (index, category) {
                if (isDomestic) {
                    if (category.forDomestic) {
                        categorySelect.append($("<option></option>")
                            .attr("value", category.id)
                            .text(category.title));
                    }
                    
                }
                else {
                    if (category.forInternational) {
                        categorySelect.append($("<option></option>")
                            .attr("value", category.id)
                            .text(category.title));
                    }
                }
               
            });

            PopulateExpenseType();
            calculateBalance();

            var strView = getUrlParameter("view");

            if (strView != "emp") {
                PopulateEligibility(false);
            }
            //AddEntitlementToEachRow();
        },
        error: function (error) {
            console.error(error);
        }
    });
});


function PopulateEligibility(isFromEmp) {
    var strRequestToken = getUrlParameter("requestToken");
    var isAsync = true;
    if (isFromEmp) {
        isAsync = false;
    }

    $.ajax({
        url: "/AddExpense/GetEligibility",
        type: "POST",
        async: isAsync,
        data: {
            requestToken: strRequestToken
        },
        success: function (data) {

            var selectCategories = $('.selectCategory');
         
            // Loop over each dropdown with the class selectCategory
            selectCategories.each(function () {
                var selectCategory = $(this);

                // Loop over the received data
                $.each(data, function (index, item) {
                    // Find the option with the corresponding value (id) in the current dropdown
                    var option = selectCategory.find('option[value="' + item.categoryID + '"]');

                    // If the option is found, set the data-entitlement attribute
                    if (option.length > 0) {
                        option.attr("data-entitlement", item.entitlement);
                    }

                });
            });

            highlightCategoriesWithExceedingExpense(data, isFromEmp);

        },
        error: function (error) {
            // Handle error
        }
    });
}

function dateDiffInDays(date1, date2) {
    date1 = formatDateForDisplay(date1);
    date2 = formatDateForDisplay(date2);

    const parts1 = date1.split('/');
    const parts2 = date2.split('/');
    const date1Obj = new Date(parts1[2], parts1[0] - 1, parts1[1]);
    const date2Obj = new Date(parts2[2], parts2[0] - 1, parts2[1]);

    const timeDiff = date2Obj - date1Obj;
    const daysDiff = Math.ceil(timeDiff / (1000 * 3600 * 24));
    return daysDiff + 1; //Include start date as well
}

function formatDateForDisplay(date) {
    var dateParts = date.split("/");
    if (dateParts.length === 3) {
        var day = dateParts[0];
        var month = dateParts[1];
        var year = dateParts[2];
        // Reformat the date as "dd/mm/yy" for display
        return month + "/" + day + "/" + year;
    }
    // If the input date format is not as expected, return it as-is
    return date;
}

//function AddEntitlementToEachRow() {
//    $(".expense-table .expense-row").each(function (index) {
//        var row = $(this);
//        if ($(row).is(":visible")) {

//        }
//    });

//}

function SubmitFoodExpenses(isSubmit) {

    if (confirm("Are you sure you want to submit expense?") == true) {
        SaveSubmitFoodExpenses(isSubmit,false);
    } else {
        $('#preloader').hide();
        text = "You canceled!";
    }

}

function saveFoodExpenses() {
    SaveSubmitFoodExpenses(false,false);
}

function SaveSubmitFoodExpenses(isSubmit, isFromAccounts) {
    if (CheckValidationBeforeSaveOrSubmitExpense()) {
        $("#preloader").show();
        var expenses = []; // An array to store data for all rows

        var strRequestToken = getUrlParameter("requestToken");
        var strNumber = getUrlParameter("number");
        var RequestNumber = $("#lblRequestNumber").text();
        var strView = getUrlParameter("view");

        var isfilethere = true;
        // Iterate through each row in the table
        $(".expense-table .expense-row").each(function () {
            var row = $(this);
            if ($(row).is(":visible")) {
                var expense = {};
                if (row.find(".expenseID").text() != "0") {



                    if (row.find(".expenseID").text() != "") {
                        expense.expenseID = row.find(".expenseID").text();
                    }
                    else {
                        expense.expenseID = "";
                    }


                    var fileInput = row.find(".file-input")[0]; // Assuming you have an input for file attachment


                    var IsUpdateAttachment = false;
                    if (fileInput.files.length > 0) {
                        IsUpdateAttachment = true;
                    }
                   

                    expense.requesttoken = strRequestToken;
                    expense.expenseType = row.find(".selectCategory").val();
                    expense.fromDate = row.find(".fromDate").val();
                    expense.toDate = row.find(".toDate").val();
                    expense.currency = row.find(".selectCurrency").val();
                    expense.byCompany = row.find(".byCompany").val();
                    expense.byEmployee = row.find(".byEmployee").val();
                    expense.comment = row.find(".comment").val();
                    expense.isSubmit = isSubmit;
                    expense.strNumber = strNumber
                    expense.RequestNumber = RequestNumber;
                    expense.IsUpdateAttachment = IsUpdateAttachment;
                    expense.strCurrencyText = row.find(".selectCurrency option:selected").text();
                    expense.strExpenseTypeText = row.find(".selectCategory option:selected").text();

                    var isSkipAttchment = false;

                    if (row.find(".selectCategory").val() == "9" ||
                        row.find(".selectCategory").val() == "10" ||
                        strView == "acc") {//skip for without bills
                        isSkipAttchment = true;
                    }


                    if (!isSkipAttchment) {

                        if (fileInput.files.length > 0) {
                            var file = fileInput.files[0];
                            expense.fileData = file;

                            isfilethere = true;
                        }
                        else if (row.find(".anchorPaperclick").is(":Visible")) {
                            isfilethere = true;
                        }
                       
                        else {
                            isfilethere = false;
                            if (isSubmit) {
                                row.find(".fa-upload").addClass("error");
                            }

                        }

                    }
                        expenses.push(expense);
                    

                }
            }
        });

        if (expenses.length > 0) {

            if (!isSubmit) { //if save then skip attchment check flow
                isfilethere = true;
            }

            if (isfilethere) {


                var isSendToManagerForApproval = true;
                if (strView == "emp" && isSubmit) {//don't send to manager if expense within limit
                    CheckExpenseWithinLimit();


                    if ($("#hdnIsExpenseWithinLimit").val() == "false") {
                        isSendToManagerForApproval = true;
                    }
                    else {
                        isSendToManagerForApproval = false;
                    }

                }
                else if (strView == "acc") {
                    isSendToManagerForApproval = false;
                }


                // Send the array of expenses to the server
                var formData = new FormData();
                for (var i = 0; i < expenses.length; i++) {
                    formData.append("expenses[" + i + "].expenseID", expenses[i].expenseID);
                    formData.append("expenses[" + i + "].requesttoken", expenses[i].requesttoken);
                    formData.append("expenses[" + i + "].expenseType", expenses[i].expenseType);
                    formData.append("expenses[" + i + "].fromDate", expenses[i].fromDate);
                    formData.append("expenses[" + i + "].toDate", expenses[i].toDate);
                    formData.append("expenses[" + i + "].currency", expenses[i].currency);
                    formData.append("expenses[" + i + "].byCompany", expenses[i].byCompany);
                    formData.append("expenses[" + i + "].byEmployee", expenses[i].byEmployee);
                    formData.append("expenses[" + i + "].comment", expenses[i].comment);
                    formData.append("expenses[" + i + "].fileData", expenses[i].fileData);
                    formData.append("expenses[" + i + "].isSubmit", expenses[i].isSubmit);
                    formData.append("expenses[" + i + "].strNumber", expenses[i].strNumber);
                    formData.append("expenses[" + i + "].RequestNumber", expenses[i].RequestNumber);
                    formData.append("expenses[" + i + "].IsUpdateAttachment", expenses[i].IsUpdateAttachment);
                    formData.append("expenses[" + i + "].strCurrencyText", expenses[i].strCurrencyText);
                    formData.append("expenses[" + i + "].strExpenseTypeText", expenses[i].strExpenseTypeText);
                    formData.append("expenses[" + i + "].strFinalAmount", $("#lblBalance").text());
                    formData.append("expenses[" + i + "].isSendToManagerForApproval", isSendToManagerForApproval);
                    formData.append("expenses[" + i + "].isFromAccounts", isFromAccounts);

                }


                $.ajax({
                    url: "/AddExpense/AddExpenses",
                    type: "POST",
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        if (!isFromAccounts) {
                            if (response == "1") {
                                if (isSubmit) {
                                    alert("Expenses submitted successfully!");

                                }
                                else {
                                    alert("Expenses saved successfully!");

                                }

                            }
                            else {
                                alert("Error occurred!");
                            }

                            $("#preloader").hide();
                            // location.reload(true);
                            if (strView == "emp") {
                                 window.location.href = '/MyRequests';
                            }
                            else {
                                window.location.href = '/MyApprovals';
                            }
                           
                        }
                        else {
                            $("#preloader").hide();
                        }

                    },
                    error: function (error) {
                        $("#preloader").hide();
                        alert("Error saving food expenses: " + error.responseText);
                    }
                });

            }
            else {
                alert("Please add attachment");
                $("#preloader").hide();
            }

        }
        else {
            if (isSubmit) {
                if (strView == "emp") {
                    UpdateStatusToPendingWithAccounts();
                }
                else if(strView == "acc") {
                    UpdateStatusToClose();
                }
               
            }
            else {
                alert("Please submit this request, no data to save.");
                $("#preloader").hide();
            }
           
        }
    }
}

function isValidExpense(expense) {
    // You can add validation logic here if needed
    return !!expense.expenseType && !!expense.fromDate && !!expense.toDate && !!expense.currency;
}

$(document).ready(function () {
    // Trigger file input click when the paperclip icon is clicked
    $(".attachment-icon").on("click", function () {

        $(this).siblings(".file-input").click();
    });

    // When a file is selected, you can access it
    //$(".file-input").change(function () {
    //    var selectedFile = $(this).prop('files')[0];
      
    //});
});

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

$(document).ready(function () {
    if ($(".expense-row").length > 1) {
        $(".expense-table").show();
    }

        
});


function PopulateExpenseType() {
    // Get the first select control
    var firstSelect = $('#selectCategory');

    // Iterate through each selectCategory control (excluding the first one)
    $('.selectCategory:not(#selectCategory)').each(function () {
        var select = $(this);

        // Clone the options for this specific dropdown
        var options = firstSelect.find('option').clone();

        select.empty(); // Remove existing options
        select.append(options); // Append the cloned options

        if (select.attr("data-value") !== undefined) {
            select.val(select.attr("data-value"));
        }
    });
}



function PopulateCurrency() {
    // Get the first select control
    var firstSelect = $('#selectCurrency');

    // Iterate through each selectCurrency control (excluding the first one)
    $('.selectCurrency:not(#selectCurrency)').each(function () {
        var select = $(this);

        // Clone the options for this specific dropdown
        var options = firstSelect.find('option').clone();

        select.empty(); // Remove existing options
        select.append(options); // Append the cloned options
        console.log(options); 
        if (select.attr("data-value") !== undefined) {
            select.val(select.attr("data-value"));
        }
    });

    SetDomesticCurrency();
    calculateBalance();

}

function calculateBalance() {
    var totalAdvaneByCompany = 0;
    var totalByCompany = 0;
    var totalByEmployee = 0;

    // Iterate through each row in the table
    $(".expense-table .expense-row").each(function () {
        var row = $(this);

        if ($(row).is(":Visible")) {
            var byCompanyValue = parseFloat(row.find(".byCompany").val()) || 0;
            var byEmployeeValue = parseFloat(row.find(".byEmployee").val()) || 0;
            var selectCategoryValue = parseFloat(row.find(".selectCategory").val()) || 0;
            var currencyRate = 1;
            if (row.find(".selectCurrency option:selected").text() != "INR" &&
                row.find(".selectCurrency option:selected").text() != "Please select" &&
                row.find(".selectCurrency option:selected").text() != "Select") {
                currencyRate = row.find(".selectCurrency option:selected").attr("data-rate");
            }

            // var currencyRate = parseFloat(row.find(".selectCategory option:selected").data("rate")) || 1;

            if (selectCategoryValue === 8) {
                // This is an advance amount by company
                totalAdvaneByCompany += byCompanyValue * currencyRate;
            }

            totalByEmployee += byEmployeeValue * currencyRate;
            totalByCompany += byCompanyValue * currencyRate;
        }
        });

    var lblCalculateTotal = $("#lblCalculateTotal");
    var total = totalByCompany;

    if (!isNaN(total)) {
        lblCalculateTotal.text(total.toFixed(2));
    }
   

    var lblBalance = $("#lblBalance");
    var balance = totalByEmployee - totalAdvaneByCompany;
   

    if (!isNaN(balance)) {
        lblBalance.text(balance.toFixed(2));
    }
    // Set the label colors based on the balance
    var headerofbalance = $("#headerofbalance");
    if (balance > 0) {
        headerofbalance.text("Balance Payable by Organization");
        lblBalance.css("color", "green");
    } else if (balance < 0) {
        headerofbalance.text("Balance Payable to Organization");
        lblBalance.css("color", "red");
    } else {
        headerofbalance.text("Balance Payable to Organization");
        lblBalance.css("color", "black");
    }
   
}


//function calculateBalance() {
//    var totalAdvaneByCompany = 0;
//    var totalByCompany = 0;
//    var totalByEmployee = 0;

//    // Iterate through each row in the table
//    $(".expense-table .expense-row").each(function () {
//        var row = $(this);
//        var byCompanyValue = parseFloat(row.find(".byCompany").val()) || 0;
//        var byEmployeeValue = parseFloat(row.find(".byEmployee").val()) || 0;
//        var selectCategoryValue = parseFloat(row.find(".selectCategory").val()) || 0;

//        if (selectCategoryValue === 8) {
//            // This is an advance amount by company
//            totalAdvaneByCompany += byCompanyValue;
//        }

//        totalByEmployee += byEmployeeValue;
//        totalByCompany += byCompanyValue;
//    });

//    var lblCalculateTotal = $("#lblCalculateTotal");
//    var total = totalByCompany;
//    lblCalculateTotal.text(total.toFixed(2));

//    var lblBalance = $("#lblBalance");
//    var balance = totalByEmployee - totalAdvaneByCompany;
//    lblBalance.text(balance.toFixed(2));

//    // Set the label colors based on the balance
//    var headerofbalance = $("#headerofbalance");
//    if (balance > 0) {
//        headerofbalance.text("Balance Payable by Organization");
//        lblBalance.css("color", "green");
//    } else if (balance < 0) {
//        headerofbalance.text("Balance Payable to Organization");
//        lblBalance.css("color", "red");
//    } else {
//        headerofbalance.text("Balance Payable to Organization");
//        lblBalance.css("color", "black");
//    }
//}

$(document).ready(function () {
    // Bind the calculateBalance function to the keyup event of the input elements and select elements

    // Trigger the calculation on page load
   // calculateBalance();

//    $(".byCompany, .byEmployee, .selectCategory").trigger("keyup").trigger("change");
});





$(document).ready(function () {
    $(".expense-row").each(function () {
        var expenseID = parseInt($(this).find(".expenseID").text());

        if (expenseID == 0) {
            // Disable all form controls within the current row
            $(this).find(".form-control").prop("disabled", true);
            $(this).find(".file-input").prop("disabled", true);

            // Optionally, you can change the appearance to indicate that the row is disabled
            $(this).addClass("disabled-row");
        }
    });
});


$(".selectCurrency").change(function () {
    calculateBalance();
});

 
$(".byEmployee").change(function () {
    calculateBalance();

});

$(".byCompany").change(function () {
    calculateBalance();
});

$(document).ready(function () {
    // Initialize a flag to track whether issubmitted is true in any row
   

    InitializeKeyEvents();
    InitializeCalenders();
    var travelRequestNumber = getUrlParameter("travelRequestNumber");
    $("#lblRequestNumber").text(travelRequestNumber);

    var strView = getUrlParameter("view");

    var isAnyRowSubmitted = false;

    if (strView == "emp") {
        $(".btnForRequestor").show();
        $(".btnForManagerAccountant").hide();
    }
    else {
        $(".btnForRequestor").hide();
        $(".btnForManagerAccountant").show();
        if (strView == "acc") {
            
            $(".btnForonlyAccountanttoshow").show();
        }
    }


    // Loop through each row
    $(".expense-table .expense-row").each(function () {
        var row = $(this);

        if (strView == "emp") {
            var expenseID = $(row).find(".expenseID").text();

            if (expenseID != 0) {
                $(row).find(".byEmployee").prop("disabled", false);
            }
        }
        else if (strView == "acc") {
            var expenseID = $(row).find(".expenseID").text();
            if (expenseID != 0) {
                $(row).find(".byEmployee").prop("disabled", false);
                $(row).find(".byCompany").prop("disabled", false);
            }
        }

        var issubmittedText = row.find(".issubmitted").text().trim();

        // Check if issubmittedText is "true"
        if (issubmittedText === "True") {
            isAnyRowSubmitted = true;
            return false; // Exit the loop early
        }
    });

    // If isAnyRowSubmitted is true, disable the entire page
    if (strView == "emp") {
        if (isAnyRowSubmitted) {
            $("#content").find("input, select, button").prop("disabled", true);
        }
    }
    else if (strView == "mgr" || strView == "acc") {
        

        var strApprovalToken = getUrlParameter("approvalToken");
        $("#btnApprove").attr("href", "/RequestApproval/ApproveRequest?token=" + strApprovalToken + "&isredirect=true");
        $("#btnReject").attr("href", "/RequestApproval/RejectRequest?token=" + strApprovalToken + "&isredirect=true");
        

        if (strView == "acc") {
           // $(".EnableForAccounts").prop("disabled", false);
            
            $("#btnApprove").attr("onclick", "SubmitBYAccount()");
        }
        else {
            $("#content").find("input, select, .attachment-icon, .btndelete, .delete-icon").prop("disabled", true);

        }
    }
    $("#PrintPdf").prop("disabled", false);

});

function InitializeKeyEvents() {
    $(".byCompany, .byEmployee, .selectCurrency").on("keyup change", calculateBalance);
}

$(document).ready(function () {
    // Define the button click event
    $("#viewEligibilityButton").click(function () {
        $("#preloader").show();
        var strRequestToken = getUrlParameter("requestToken");
        // Make an AJAX request to the server to fetch eligibility data
        $.ajax({
            url: "/AddExpense/GetEligibility", // Update the URL to match your controller action
            type: "POST", // Use GET or POST based on your controller action
            data: {
                requestToken: strRequestToken // Replace with the actual request token
            },
            success: function (data) {
                // Create HTML content for the popup
                var table = buildPolicyDetailsTable(data);;
                // Add more content as needed
                $.fancybox.open({
                    src: table,
                    type: "html"
                });
                //// Open the Magnific Popup with the content
                //$.magnificPopup.open({
                //    items: {
                //        src: '<div class="white-popup">' + popupContent + '</div>',
                //        type: 'inline'
                //    }
                //});
                $("#preloader").hide();
            },
            error: function (error) {
                $("#preloader").hide();
            }
        });
    });
});

function buildPolicyDetailsTable(data) {

    var strRequestCurrency = "$";
    if (IsDomesticRequest()) {
        strRequestCurrency = "INR";
    }

    var strNumber = getUrlParameter("number");
    var isOldRequest = false;

    if (strNumber <= '25211') {//last old request was 25211
        isOldRequest = true;
    }
    var table = $("<table>");
    var thead = $("<thead>");
    var tbody = $("<tbody>");

    // Create table header
    var headerRow = $("<tr>");
    headerRow.append($("<th>Category</th>"));
    headerRow.append($("<th>Entitlement (" + strRequestCurrency +" per day)</th>"));
    if (isOldRequest) {
        headerRow.append($("<th>City</th>"));

    }
    thead.append(headerRow);

    // Create table rows with policy details
    $.each(data, function (index, policy) {
        var row = $("<tr>");
/*        row.append($("<td>" + policy.travelType + "</td>"));*/
        row.append($("<td>" + policy.categoryTitle + "</td>"));
/*        row.append($("<td>" + policy.classTitle + "</td>"));*/
        row.append($("<td>" + policy.entitlement + "</td>"));
        if (isOldRequest) {
            row.append($("<td>" + policy.cityName + "</td>"));
        }
        tbody.append(row);
    });

    table.append(thead);
    table.append(tbody);

    return table;
}

function CheckValidationBeforeSaveOrSubmitExpense() {
    var isValid = true;

    $(".mandatoryFields").removeClass("error");

    //$(".employeeSectionContent").each(function () {

    // Reset the style of all input fields


    $(".mandatoryFields").each(function () {
        if ($(this).closest(".expense-row").is(":Visible")) {
            if (!$(this).is(':disabled') && $(this).val() == "") {
                if ($(this).hasClass("attachmentFiles")) {
                    $(this).next().addClass("error");
                }
                else {
                    $(this).addClass("error");
                }

                isValid = false;
            }
        }
    });


   
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

function InitializeCalenders() {
    var strRequestToken = getUrlParameter("requestToken");

    $.ajax({
        url: "/AddExpense/GetDateRange",  // Replace "YourController" with the actual name of your controller
        type: "POST",
        data: {
            requestToken: strRequestToken // Replace with the actual request token
        } ,
        success: function (data) {
            var fromDate = new Date(data.departureDate);
            var toDate = new Date(data.maxReturnDate);
          

            var defaultMinDate = new Date(2000, 0, 1); // January 1, 1900

            // Check if toDate is earlier than the default minimum date
            if (toDate < defaultMinDate) {
                toDate = new Date();
            }

            /*toDate.setDate(toDate.getDate() + 45);*/

            $(".expenseDate").datepicker({
                minDate: fromDate,
                maxDate: toDate,
                dateFormat: "dd/mm/yy"
            });
        },
        error: function (error) {
            console.error("Error fetching date range: ", error);
        }
    });
}



    //$(".expenseDate").datepicker({
    //    minDate: 0, // Disable past dates
    //    dateFormat: "dd/mm/yy",
    //    onSelect: function (selectedDate) {
    //        var fromDate = new Date(selectedDate);
    //        fromDate.setDate(fromDate.getDate()); // Minimum Return Date is Departure Date + 1 day
    //        //    $("#travelReturnDate").datepicker("option", "minDate", fromDate);
    //    }
    //});






function DeleteAttachment(control) {

    if (confirm('Are you sure you want permanently to delete this attachment?')) {
        $(control).closest(".attachment-label").find(".fa-paperclip").hide();
        $(control).closest(".attachment-label").find(".delete-icon").hide();
        $(control).closest(".attachment-label").find(".fa-upload").show();

        var row = $(control).closest('.expense-row');
        var fileInput = row.find('.file-input');

        // Remove the file input value (clear the attachment)
        fileInput.val('');


        var expenseID = row.find('.expenseID').text();


        if (expenseID != null && expenseID != "" && expenseID != '0') {
            // Perform AJAX request to call the controller action
            $.ajax({
                url: '/AddExpense/DeleteAttachment', // Update the URL with your controller and action
                type: 'POST',
                data: { id: expenseID },
                success: function (response) {
                    // Handle the success response if needed
                    alert("Aattachment Deleted")
                },
                error: function (error) {
                    // Handle the error response if needed
                    console.error('Error deleting attachment:', error);
                }
            });

        }
        else {
            alert("Aattachment Deleted")
        }
    }
    

}


function chnageoffileupload(control) {
    var fileInput = control;
    var file = fileInput.files[0];
   
    if (file) {

       
        var mainrow = $(control).closest(".attachment-label");

        // Display download link
        $(mainrow).find(".fa-upload").hide();
        $(mainrow).find(".anchorPaperclick").show();
        $(mainrow).find(".fa-paperclip").show();
        
        $(mainrow).find(".delete-icon").show();
        $(mainrow).find(".btndelete").show();
        (mainrow).find(".anchorPaperclick").attr("href", URL.createObjectURL(file));
        (mainrow).find(".anchorPaperclick").attr("download", file.name);
    } else {
        // Hide download link if no file is selected
        /* $("#downloadSection").hide();*/
    }
}

function ShowexceededCategories() {
    var uniqueSelectedTexts = [];

    // Loop over elements with class ErrorWithAllowedtoSave
    $('.ErrorWithAllowedtoSave').each(function () {
        // Find the selectCategory within each element
        var selectCategory = $(this).closest(".expense-row").find('.selectCategory');

        // Get the selected text from the selectCategory
        var selectedText = selectCategory.find('option:selected').text();

        // Check if the selectedText is not in the array, then push it
        if (uniqueSelectedTexts.indexOf(selectedText) === -1) {
            uniqueSelectedTexts.push(selectedText);
        }
    });

    // Update the content of the span with unique selected text values
    if (uniqueSelectedTexts.length > 0) {
        var selectedTextsSpan = $('#selectedTexts');
        if (uniqueSelectedTexts.length > 0) {
            selectedTextsSpan.text('Limit exceeded for categories: ' + uniqueSelectedTexts.join(', '));
        } else {
            selectedTextsSpan.text('');
        }
    }
}

function SetDomesticCurrency() {
    if ($('#lblRequestNumber:contains(DTR-)').length > 0) {
        $(".selectCurrency").val("32").attr("disabled", true);
    }
}

function IsDomesticRequest() {
    if ($('#lblRequestNumber:contains(DTR-)').length > 0) {
        return true;
    }
    else {
        return false;
    }

}


function SubmitBYAccount() {
    SaveSubmitFoodExpenses(true,true);
}

//function highlightCategoriesWithExceedingExpense(data) {
//    // Create an object to store the total expense for each category
//    var categoryTotalExpense = {};
//    var categoryTotalExpenseAllowed = {};
//    var traveldays = 1;

//    // Loop over each row
//    $(".expense-row").each(function () {
//        var closestRow = $(this);
//        var selectCategory = closestRow.find(".selectCategory");
//        var amoutByEmp = closestRow.find(".byEmployee").val();
//        var entitlementAmout = parseFloat(selectCategory.find('option:selected').attr("data-entitlement")) || 0;
//        var selectedCurrencyexchnageRate = closestRow.find(".selectCurrency option:selected").attr("data-rate") || 1;

//        // Ensure the data object has the necessary properties
//        var categoryData = data.find(item => item.categoryID == selectCategory.val());

//        if (categoryData === undefined) {
//            return; // Skip this row if data is incomplete
//        }

//        var exchangeRateOfPolicy = parseFloat(categoryData.exchangeRate) || 1;

//        if (exchangeRateOfPolicy == 32) {
//            exchangeRateOfPolicy = 1;
//        }

//        if (selectedCurrencyexchnageRate == "") {
//            selectedCurrencyexchnageRate = 1;
//        }

//        if (amoutByEmp != "" && amoutByEmp != null && amoutByEmp !== undefined &&
//            entitlementAmout != "" && entitlementAmout != null &&
//            entitlementAmout != undefined) {

//            if (!isNaN(amoutByEmp) && !isNaN(entitlementAmout)) {
//                // Calculate the allowed expense based on traveldays
//                var allowedToSpendBasedOnDays = traveldays * entitlementAmout;

//                // Calculate the spent amount after currency conversion
//                var spentByEmployeeAfterConversion = amoutByEmp * Math.abs(selectedCurrencyexchnageRate);

//                // Calculate the total expense for the current category
//                categoryTotalExpense[selectCategory.val()] = (categoryTotalExpense[selectCategory.val()] || 0) + spentByEmployeeAfterConversion;

//                // Store the allowed expense for this category
//                categoryTotalExpenseAllowed[selectCategory.val()] = (categoryTotalExpenseAllowed[selectCategory.val()] || 0) + allowedToSpendBasedOnDays;
//            }
//        }
//    });

//    // Loop over the stored category expenses and highlight textboxes if combined expense exceeds allowed days
//    $.each(categoryTotalExpense, function (categoryID, totalExpense) {
//        var allowedToSpendBasedOnDays = categoryTotalExpenseAllowed[categoryID];

//        if (totalExpense > allowedToSpendBasedOnDays) {
//            $(".expense-row .selectCategory option:selected[value='" + categoryID + "']").closest(".expense-row").find(".byEmployee").addClass("ErrorWithAllowedtoSave");
//        }
//    });
//}


function highlightCategoriesWithExceedingExpense(data, isFromEmp) {
    // Create an object to store the total expense for each category
    var categoryTotalExpense = {};
    var categoryTotalExpenseAllowed = {};
    var DateDiff = 1; // Define DateDiff in the outer scope
    var traveldays = 1;

    if (data.length > 0 && data !== undefined) {
       traveldays = data[0].travelDays;
    }

    // Loop over each row
    $(".expense-row").each(function () {
        var closestRow = $(this);
        var selectCategory = closestRow.find(".selectCategory");
        var amoutByEmp = closestRow.find(".byEmployee").val();
        var entitlementAmout = parseFloat(selectCategory.find('option:selected').attr("data-entitlement")) || 0;
        var selectedCurrencyexchnageRate = closestRow.find(".selectCurrency option:selected").attr("data-rate") || 1;

        // Ensure the data object has the necessary properties
        var categoryData = data.find(item => item.categoryID == selectCategory.val());

        if (categoryData === undefined) {
            return; // Skip this row if data is incomplete
        }

        var exchangeRateOfPolicy = parseFloat(categoryData.exchangeRate) || 1;
        var currency = parseFloat(categoryData.currency) || 1;

     
        var spentByEmployeeAfterConversionOFSelectedCurrencyOfRowINR = 0.0000;

        if (currency == 32) {
            exchangeRateOfPolicy = 1;
        }

        if (selectedCurrencyexchnageRate == "") {
            selectedCurrencyexchnageRate = 1;
        }

       
        if (amoutByEmp != "" && amoutByEmp != null && amoutByEmp !== undefined &&
            entitlementAmout != "" && entitlementAmout != null &&
            entitlementAmout != undefined) {

            if (!isNaN(amoutByEmp) && !isNaN(entitlementAmout)) {
                var totalEntitlement = traveldays * entitlementAmout;

                if (totalEntitlement !== undefined && totalEntitlement != null &&
                    totalEntitlement != 0 && !isNaN(totalEntitlement)) {

                    if (exchangeRateOfPolicy !== undefined && exchangeRateOfPolicy != null &&
                        exchangeRateOfPolicy != "" && !isNaN(exchangeRateOfPolicy)) {

                        if (parseFloat(exchangeRateOfPolicy) > 0) {
                            AllowedToSpendBasedOnDaysINR = totalEntitlement * Math.abs(exchangeRateOfPolicy);
                        } else if (parseFloat(exchangeRateOfPolicy) < 0) {
                            AllowedToSpendBasedOnDaysINR = totalEntitlement / Math.abs(exchangeRateOfPolicy);
                        } else {
                            AllowedToSpendBasedOnDaysINR = totalEntitlement;
                        }
                    } else {
                        AllowedToSpendBasedOnDaysINR = totalEntitlement;
                    }

                    if (selectedCurrencyexchnageRate !== undefined && selectedCurrencyexchnageRate != null &&
                        selectedCurrencyexchnageRate != "" && !isNaN(selectedCurrencyexchnageRate)) {
                        if (parseFloat(selectedCurrencyexchnageRate) > 0) {
                            spentByEmployeeAfterConversionOFSelectedCurrencyOfRowINR = amoutByEmp * Math.abs(selectedCurrencyexchnageRate);
                        } else if (parseFloat(selectedCurrencyexchnageRate) < 0) {
                            spentByEmployeeAfterConversionOFSelectedCurrencyOfRowINR = amoutByEmp / Math.abs(selectedCurrencyexchnageRate);
                        } else {
                            spentByEmployeeAfterConversionOFSelectedCurrencyOfRowINR = amoutByEmp;
                        }
                    }
                }


                //var allowedExpNew = parseFloat(AllowedToSpendBasedOnDaysINR) || 0;
                //categoryTotalExpenseAllowed[selectCategory.val()] = (categoryTotalExpense[selectCategory.val()] || 0) + allowedExpNew;

                var allowedExpNew = parseFloat(AllowedToSpendBasedOnDaysINR) || 0;
                categoryTotalExpenseAllowed[selectCategory.val()] = allowedExpNew;



                // Calculate the total expense for the current category
                var totalExpense = parseFloat(spentByEmployeeAfterConversionOFSelectedCurrencyOfRowINR) || 0;
                categoryTotalExpense[selectCategory.val()] = (categoryTotalExpense[selectCategory.val()] || 0) + totalExpense;
            }
            else {

            }
        }
        else if (entitlementAmout == "" || entitlementAmout == null ||
            entitlementAmout === undefined && entitlementAmout == 0) {

        }
    });

    // Loop over the stored category expenses and highlight textboxes if combined expense exceeds allowed days
    $.each(categoryTotalExpense, function (categoryID, totalExpense) {
        var option = $(".selectCategory option[value='" + categoryID + "']");
        var entitlementAmout = parseFloat(option.attr("data-entitlement")) || 0;
        // var allowedToSpendBasedOnDays = entitlementAmout * DateDiff;
        var allowedToSpendBasedOnDays = categoryTotalExpenseAllowed[categoryID];

        if (totalExpense > allowedToSpendBasedOnDays) {
            if (!isFromEmp) {
                $(".expense-row .selectCategory option:selected[value='" + categoryID + "']").closest(".expense-row").find(".byEmployee").addClass("ErrorWithAllowedtoSave");
                ShowexceededCategories();
            }
            else {
                $("#hdnIsExpenseWithinLimit").val("false");

            }           
        }
        
    });
}


function CheckExpenseWithinLimit() {
    $("#hdnIsExpenseWithinLimit").val("true");
    PopulateEligibility(true);
    
}


function UpdateStatusToPendingWithAccounts() {
    $("#preloader").show();
    var strView = getUrlParameter("view");
    var strRequestToken = getUrlParameter("requestToken");
    // Make an AJAX request to the server to update status
    $.ajax({
        url: "/AddExpense/UpdateStatusToPendingWithAccounts", // Update the URL to match your controller action
        type: "POST", // Use GET or POST based on your controller action
        data: {
            requestToken: strRequestToken // Replace with the actual request token
        },
        success: function (data) {
            alert("Expenses submitted successfully!");
            $("#preloader").hide();
           
            if (strView == "emp") {
                window.location.href = '/MyRequests';
            }
            else {
                window.location.href = '/MyApprovals';
            }
        },
        error: function (error) {
            // Handle error, if needed
            $("#preloader").hide();
        }
    });
}

function UpdateStatusToClose() {
    var strView = getUrlParameter("view");
    $("#preloader").show();
    var strRequestToken = getUrlParameter("requestToken");
    // Make an AJAX request to the server to update status
    $.ajax({
        url: "/AddExpense/UpdateStatusToCloseByAccount", // Update the URL to match your controller action
        type: "POST", // Use GET or POST based on your controller action
        data: {
            requestToken: strRequestToken // Replace with the actual request token
        },
        success: function (data) {
         
            // Handle success, if needed
            $("#preloader").hide();
            if (strView == "emp") {
                window.location.href = '/MyRequests';
            }
            else {
                window.location.href = '/MyApprovals';
            }
        },
        error: function (error) {
            // Handle error, if needed
            $("#preloader").hide();
        }
    });
}

$(document).ready(function () {
    var strRequestToken = getUrlParameter("requestToken");

    $.ajax({
        url: '/AddExpense/GetEmployeeInformationforexpenseScreen',
        type: 'GET',
        dataType: 'json',
        data: { requestToken: strRequestToken },
        success: function (data) {
            if (data.empName !== undefined) {
                $("#lblEmployeeName").text(data.empName);

            }

            if (data.empNumber !== undefined) {
                $("#lblEmployeeNumber").text(data.empNumber);

            }

            if (data.empGrade !== undefined) {
                $("#lblEmployeeGrade").text(data.empGrade);

            }

            if (data.empMobile !== undefined) {
                $("#lblEmployeeMobile").text(data.empMobile);

            }
           
         
            // Process the returned data and update your HTML
           // console.log(data);
        },
        error: function (error) {
            console.error(error);
        }
    });
});