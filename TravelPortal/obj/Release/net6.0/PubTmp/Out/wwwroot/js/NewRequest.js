
function ClickNextStage2() {
    if (CheckValidationStage2()) {
        if ($("#selectDirector").is(":disabled")) {
            $("#selectDirector").attr("data-isCheckdisbaled", "true");
        }
        else {
            $("#selectDirector").attr("data-isCheckdisbaled", "false");
        }
        
        $("#NewTravelRequestStep1").find("input").attr("disabled", "disabled");
        $("#NewTravelRequestStep2").find("input").attr("disabled", "disabled");
        $("#NewTravelRequestStep1").find("select").attr("disabled", "disabled");
        $("#NewTravelRequestStep2").find("select").attr("disabled", "disabled");
        $("#comments").prop('disabled', true);
        $(".tilediv").addClass("disabled-div");
        $("#NewTravelRequestStep1").show();
        $("#NewTravelRequestStep2").show();
        $("#btnSubmit").show();
        $(".ClassForHideOnSubmit").hide();
        $("#previousButtonStage2").show();
     
        if (!IsEditMode()) {
            SaveOrSubmitTravelRequest(false);
        }

    }

}

function PreviousButtonStage1() {
    $("#NewTravelRequestStep1").show();
    $("#NewTravelRequestStep2").hide();
    $("#previousButton").hide();
    $("#nextButton").show();
    $("#nextButtonStage2").hide();

}

    function PreviousButtonStage2(){
        $("#NewTravelRequestStep1").find("input").prop("disabled", false);
    $("#NewTravelRequestStep2").find("input").prop( "disabled", false );
    $("#NewTravelRequestStep2").find("select").prop( "disabled", false );
    $("#comments").prop('disabled', false);
    $(".tilediv").removeClass("disabled-div");
    $("#NewTravelRequestStep2").show();
    $("#NewTravelRequestStep1").hide();
        $("#previousButtonStage2").hide();
        $("#previousButton").show();
        $("#btnSubmit").hide();
        $("#nextButtonStage2").show();
        disableTextboxesInSection();
        if ($("#selectDirector").attr("data-isCheckdisbaled") == "true") {
            $("#selectDirector").prop("disabled", true);
        }
        else if ($("#selectDirector").attr("data-isCheckdisbaled") == "false") {
            $("#selectDirector").prop("disabled", false);
        } 

        if ($("#tileInternational").hasClass("selected-tile")) {
            $('#selectCurrency').prop('disabled', false);
        }
        else {
            $('#selectCurrency').prop('disabled', true);
        }
           // ClickNextStage1();
        }


function ClickNextStage1() {

    if (CheckValidationStage1()) {
        updateTotalTravelDays();
        $("#NewTravelRequestStep2").show();
        $("#NewTravelRequestStep1").hide();
        $("#previousButton").show();
        $("#nextButton").hide();
        $("#nextButtonStage2").show(); 

        disableTextboxesInSection();
        $('#employeeSectionContent').slideDown();
        
        if ($("#tileInternational").hasClass("selected-tile")) {
            $("#rowForPassport").show();
            $("#rowForNominee").show();
            $('#selectCurrency').prop('disabled', false);
        }
        else {
            $("#rowForPassport").hide();
            $("#rowForNominee").hide();
            $('#selectCurrency').prop('disabled', true);
            $('#selectCurrency option[value=32]').attr("selected", "selected");
        }

       
        // Collapse the accordion after 1 second
        setTimeout(function () {
            $('#employeeSectionContent').slideUp();
        }, 1000);

        if (!IsEditMode()) {
            SaveOrSubmitTravelRequest(false);
        }
    }
        
}

function updateTotalTravelDays() {
    if ($("#tileMultiCity").hasClass("selected-tile")) {
        var date2 = $(".travelDepartureDate").closest(".rowForSourceAndDestination :visible").last().val();
        var date1 = $(".travelDepartureDate").closest(".rowForSourceAndDestination :visible").first().val();
        $("#travelDays").val(dateDiffInDays(date1, date2));
    }
    else if ($("#tileRoundTrip").hasClass("selected-tile")) {

        var date2 = $("#travelReturnDate").val();
        var date1 = $("#travelDepartureDate").val();
        $("#travelDays").val(dateDiffInDays(date1, date2));
       
    }
    else {
        $("#travelDays").val("1");
    }
    $("#travelDays").attr("disabled", "disabled");
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

function disableTextboxesInSection() {
    // Select all textboxes within the collapsed section
    $('#employeeSectionContent input[type="text"]').each(function () {
        // Check if the textbox has a value
        if ($(this).val() !== '') {
            // Disable the textbox
            $(this).prop('disabled', true);
        }
    });
}

function selectTile(tileId, group) {

    if (tileId == "International" && $("#tileTrain").hasClass("selected-tile")) {
        alert("You can not select International destination with train travel mode.");
        return false;
    }
    else if (tileId == "Train" && $("#tileInternational").hasClass("selected-tile")) {
        alert("You can not select train travel mode for international destination");
        return false;
    }

            // Deselect all tiles in the same group
            const tiles = document.querySelectorAll(`.${group}`);
            tiles.forEach((tile) => {
                tile.classList.remove("selected-tile");
                tile.classList.remove("error");
            });

    // Select the clicked tile
    const selectedTile = document.getElementById(`tile${tileId}`);
    if (selectedTile) {
        selectedTile.classList.add("selected-tile");
            }
        }

    // Attach click event listeners to tiles within each group
    const travelModeTiles = document.querySelectorAll('.travel-mode');
        travelModeTiles.forEach((tile) => {
        tile.addEventListener('click', () => {
            selectTile(tile.id.replace('tile', ''), 'travel-mode');
        });
        });

    const bookedByTiles = document.querySelectorAll('.booked-by');
        bookedByTiles.forEach((tile) => {
        tile.addEventListener('click', () => {
            selectTile(tile.id.replace('tile', ''), 'booked-by');
        });
        });

    const destinationTiles = document.querySelectorAll('.destination');
        destinationTiles.forEach((tile) => {
        tile.addEventListener('click', () => {
            selectTile(tile.id.replace('tile', ''), 'destination');
        });
        });

    const travelTypeTiles = document.querySelectorAll('.travel-type');
        travelTypeTiles.forEach((tile) => {
        tile.addEventListener('click', () => {
            selectTile(tile.id.replace('tile', ''), 'travel-type');
        });
        });

function SubmitTravelRequest() {

    if (window.location.href.indexOf("EditTravelRequest") > -1) {
        if (confirm("Are you sure you want to update travel request?") == true) {
            SaveOrSubmitTravelRequest(true);
        } else {
            $('#preloader').hide();
            text = "You canceled!";
        }
    }
    else {
        if (confirm("Are you sure you want to create new travel request?") == true) {
            SaveOrSubmitTravelRequest(true);
        } else {
            $('#preloader').hide();
            text = "You canceled!";
        }
    }

       
    
}


function SaveOrSubmitTravelRequest(isSubmitted) {

    //var strTravelMode = $("#divForTravelMode").find(".selected-tile").attr("data-TravelModeId");
    //var strTicketBookedBy = $("#divTicketBookedBy").find(".selected-tile").attr("data-TravelBookedById");
    //var strTravelDestination = $("#divTravelDestination").find(".selected-tile").attr("data-TravelDestinationId");
    //var strdivTravelType = $("#divTravelType").find(".selected-tile").attr("data-TravelTypeId");

    //var strTravelMode = $("#divForTravelMode").find(".selected-tile").find("h6").text();
    var strTravelDestination = $("#divTravelDestination").find(".selected-tile").attr("data-TravelDestinationId");
    var strTravelMode = $("#divForTravelMode").find(".selected-tile").attr("data-TravelModeId");
    var strTicketBookedBy = $("#divTicketBookedBy").find(".selected-tile").attr("data-travelbookedbyid");
    /*       var strTicketBookedBy = $("#divTicketBookedBy").find(".selected-tile").find("h6").text(); */
    /* var strTravelDestination = $("#divTravelDestination").find(".selected-tile").find("h6").text();*/
    var strTravelDestination = $("#divTravelDestination").find(".selected-tile").attr("data-TravelDestinationId");
    /*var strdivTravelType = $("#divTravelType").find(".selected-tile").find("h6").text();*/
    var strdivTravelType = $("#divTravelType").find(".selected-tile").attr("data-traveltypeid");
    var strtravelFrom = "";
    if ($(".travleFromFirst option:selected").text() !== undefined &&
        $(".travleFromFirst option:selected").text() != "") {
        strtravelFrom = $(".travleFromFirst option:selected").text();
    }
    var strtravelTo = "";
    if ($(".travelToFirst option:selected").text() !== undefined &&
        $(".travelToFirst option:selected").text() != "") {
        strtravelTo = $(".travelToFirst option:selected").text();
    }

    var strtravelDepartureDate = "";
    if ($("#travelDepartureDate").val() !== undefined && $("#travelDepartureDate").val() !== "") {
         strtravelDepartureDate = $("#travelDepartureDate").val();

    }

    var strttravelReturnDate = "";
    if ($("#travelReturnDate").val() !== undefined && $("#travelReturnDate").val() !== "") {
        strttravelReturnDate = $("#travelReturnDate").val();

    }
    var stremployeeNo = $("#employeeNo").val();
    var stremployeeName = $("#employeeName").val();
    var strgrade = $("#grade").val();
    var strposition = $("#position").val();
    var strdepartment = $("#department").val();
    var strlocation = $("#location").val();
    var strmobileNumber = $("#mobileNumber").val();
    var strage = $("#age").val();
    var strselectGender = $("#selectGender").val();
    var strselectPurpose = $("#selectPurpose").val();
    var strtravelDays = $("#travelDays").val();
    var strmanager1 = $("#manager1").val();
    var strmanager2 = $("#manager2").val();
    var strdirector = "";
    if ($("#managingdirector").is(":visible")) {
        strdirector = $("#managingdirector").attr("data-empNo");
    }
    else {
        strdirector = $("#selectDirector").val();
    }
    var stradvanceAmount = $("#advanceAmount").val();
    var strselectCurrency = $("#selectCurrency").val();
    var strselectProjectCode = $("#selectProjectCode").val();
    var strcomments = $("#comments").val();
    var strRequestedFor = $('input[name="requestedFor"]:checked').val();
    var strEmpType = $('input[name="employeeType"]:checked').val();
    var selectCurrency = $("#selectCurrency").val();
    var strRequestToken = "";
    if (window.location.href.indexOf("EditTravelRequest") > -1) {
        strRequestToken = getUrlParameter("requestToken");
    }
    else if ($("#hdnRequestToken").val() != "") {
        strRequestToken = $("#hdnRequestToken").val();
    }

    var multiCityData = [];
    if ($("#tileMultiCity").hasClass("selected-tile")){

        $('.rowForSourceAndDestination').each(function (index) {
            if ($(this).css('display') != 'none') {

                var travelFrom = $(this).find('.travelFrom option:selected').text();
                var travelTo = $(this).find('.travelTo option:selected').text();

               

                //var travelFrom = $(this).find('.travelFrom').val();
                //var travelTo = $(this).find('.travelTo').text();
                var travelDepartureDate = $(this).find('.travelDepartureDate').val();

                if (travelFrom && travelTo && travelDepartureDate) {
                    multiCityData.push({
                        travelFrom: travelFrom,
                        travelTo: travelTo,
                        travelDepartureDate: travelDepartureDate
                    });
                }

                if (index == 1) {//To maintain everwhere in my request and my approval list, 
                    //beacuse multiple source are not visible
                    strtravelFrom = travelFrom;
                    strtravelTo = travelTo;
                    strtravelDepartureDate = travelDepartureDate;
                    strttravelReturnDate = null;
                }
            }
        });

        
    }

    // Add multi-city data to the existing 'data' object

    var NameonPassport = "";
    var PassportNumber = "";
    var PassportIssueDate = "";
    var PassportExpiryDate = "";

    var NameofNominee = "";
    var RelationwithNominee = "";
    var DOBofNominee = "";
    var selectGenderofNominee = "";

    if ($("#tileInternational").hasClass("selected-tile")) {// check for internation flow
        NameonPassport = $("#NameonPassport").val();
        PassportNumber = $("#PassportNumber").val();
        PassportIssueDate = $("#PassportIssueDate").val();
        PassportExpiryDate = $("#PassportExpiryDate").val();

        NameofNominee = $("#NameofNominee").val();
        RelationwithNominee = $("#RelationwithNominee").val();
        DOBofNominee = $("#DOBofNominee").val();
        selectGenderofNominee = $("#selectGenderofNominee").val();
    }

    var isCreateMode = true;
    if (window.location.href.indexOf("EditTravelRequest") > -1) {
        isCreateMode = false;
    }
    


    var data = {
        strTravelMode: strTravelMode,
        strTicketBookedBy: strTicketBookedBy,
        strTravelDestination: strTravelDestination,
        strdivTravelType: strdivTravelType,
        strtravelFrom: strtravelFrom,
        strtravelTo: strtravelTo,
        strtravelDepartureDate: strtravelDepartureDate,
        strttravelReturnDate: strttravelReturnDate,
        stremployeeNo: stremployeeNo,
        stremployeeName: stremployeeName,
        strgrade: strgrade,
        strposition: strposition,
        strdepartment: strdepartment,
        strlocation: strlocation,
        strmobileNumber: strmobileNumber,
        strage: strage,
        strselectGender: strselectGender,
        strselectPurpose: strselectPurpose,
        strtravelDays: strtravelDays,
        strmanager1: strmanager1,
        strmanager2: strmanager2,
        strdirector: strdirector,
        stradvanceAmount: stradvanceAmount,
        strselectCurrency: strselectCurrency,
        strselectProjectCode: strselectProjectCode,
        strcomments: strcomments,
        strRequestedFor: strRequestedFor,
        strEmpType: strEmpType,
        selectCurrency: selectCurrency,
        strRequestToken: strRequestToken,
        isSubmitted: isSubmitted,
        MultiCityData: multiCityData,
        strNameonPassport: NameonPassport,
        strPassportNumber: PassportNumber,
        strPassportIssueDate: PassportIssueDate,
        strPassportExpiryDate: PassportExpiryDate,
        isCreateMode: isCreateMode,
        strNameofNominee: NameofNominee,
        strRelationwithNominee: RelationwithNominee,
        strDOBofNominee: DOBofNominee,
        strGenderofNominee: selectGenderofNominee

    };
    $('#preloader').show();
    // Make an AJAX request
    $.ajax({
        type: "POST", // Use the appropriate HTTP method
        url: "/NewRequest/SubmitTravelRequest", // Specify the URL of your server-side method
        data: JSON.stringify(data), // Serialize data as JSON if needed
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $('#preloader').hide();
            if (response != "0") {
                if (isSubmitted) {
                    if (window.location.href.indexOf("EditTravelRequest") > -1) {
                        alert("Your travel request updated successfully")
                    }
                    else {
                        alert("Your travel request raised successfully")
                    }
                   
                    window.location.href = '/MyRequests';
                }
                else {
                    
                    $("#hdnRequestToken").val(response[0].newRequestToken);
                    if (response[0].isDirectorApprovalNeeded) {
                        if (strTravelDestination == "2") {
                            $("#managingdirector").val(response[0].mdDirectorPersonnelList[0].employeeName);
                            $("#managingdirector").attr("data-empNo", response[0].mdDirectorPersonnelList[0].employeeID)
                            $(".ManagingDirectorRow").show();
                            $(".NormalDirectorRow").hide();
                        }
                        else {
                            if (!$("#btnSubmit").is(":visible")) {
                                $("#selectDirector").prop("disabled", false);
                                $(".ManagingDirectorRow").hide();
                                $(".NormalDirectorRow").show();
                            }
                           
                        }
                       
                    }
                    else {
                        $("#selectDirector").prop("disabled", true);
                        $(".ManagingDirectorRow").hide();
                        $(".NormalDirectorRow").show();
                    }
                }
                
            }
            else {
                alert("Error occurred");
            }

        },
        error: function (error) {
            $('#preloader').hide();
            alert("error-" + error)
        }
    });

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

function CheckValidationStage1() {
    var isValid = true;

    // Reset the style of all input fields
    $(".mandatoryFields").removeClass("error");

    // Get values from the input fields
    var travelMode = ($("#divForTravelMode .selected-tile").length > 0 ? true : false );
    var ticketBookedBy = ($("#divTicketBookedBy .selected-tile").length > 0 ? true : false );
    var travelDestination = ($("#divTravelDestination .selected-tile").length > 0 ? true : false );
    var travelType = ($("#divTravelType .selected-tile").length > 0 ? true : false );
    //var departureCity = ($(".FillCities ").val() == "" ? false : true);
    //var destinationCity = ($(".FillCities ").val() == "" ? false : true);
    var departureDate = ($("#travelDepartureDate").val() == "" ? false : true);
    

    var returnDate = ($("#travelReturnDate").val() == "" ? false : true);
    if ($('#travelReturnDate').prop('disabled') == true || $('#travelReturnDate').is(':Visible') == false) {//skip vlidation when disbaled or hidden return date
        returnDate = true;
    }

    var isMulticityValid = true;
    if ($("#tileMultiCity").hasClass("selected-tile")) {
        if ($(".rowForSourceAndDestination").length == 2) {
            isMulticityValid = false;
        }

    }

    var isMulticityDepartureDatesValid = true;
    if ($("#tileMultiCity").hasClass("selected-tile")) {
        $('.travelDepartureDate:not(:disabled)').each(function () {
            if ($(this).is(":visible")) {
                var value = $(this).val().trim();

                if (value === '') {
                    isMulticityDepartureDatesValid = false;
                    $(this).addClass("error");
                }
            }
            
        });

    }

    var isvalidsourcedestination = true;
    if (!SOurceandDestinationvalidationcheck()) {
        isvalidsourcedestination = false;
    }

    if (
        !travelMode ||
        !ticketBookedBy ||
        !travelDestination ||
        !travelType ||
        //!departureCity ||
        //!destinationCity ||
        !departureDate ||
        !returnDate ||
        !isMulticityValid ||
        !isvalidsourcedestination ||
        !isMulticityDepartureDatesValid
    ) {
        isValid = false;

        // Highlight the empty fields in red and set focus on the first one
        if (!travelMode) $("#divForTravelMode").find(".tilediv").addClass("error");
        if (!ticketBookedBy) $("#divTicketBookedBy").find(".tilediv").addClass("error");
        if (!travelDestination) $("#divTravelDestination").find(".tilediv").addClass("error");
        if (!travelType) $("#divTravelType").find(".tilediv").addClass("error");
        //if (!departureCity) $("#travelFrom").addClass("error")
        //if (!destinationCity) $("#travelTo").addClass("error")
        if (!departureDate) $("#travelDepartureDate").addClass("error")
        if (!returnDate) $("#travelReturnDate").addClass("error")
        if (!isMulticityValid) {
            alert("Please add more than one source and destination for multicity request");
        }
        
    }

    $(".FillCities").each(function () {
        if ($(this).is(":visible") && $(this).val() == "") {
            
                $(this).addClass("error");
   
            isValid = false;
        }
    });

    // You can add additional validation logic here

    if (!isValid) {
       // alert("Please fill in all mandatory fields.");
    }

    return isValid;

}

function ValidateTravelRequest() {
    
    
}

function CheckValidationStage2() {
    var isValid = true;

    $(".mandatoryFields").removeClass("error");
    var requestedFor = ($('input[name="requestedFor"]:checked').val() === undefined ? false : true);
    var employeeType = ($('input[name="employeeType"]:checked').val() === undefined ? false : true);
    var employeeNo = ($("#employeeNo").val().trim() == "" ? false : true);
    var mobileNumber = ($("#mobileNumber").val() == "" ? false : true);
    var purpose = ($("#selectPurpose").val() == "" ? false : true);
    var projectCode = ($("#selectProjectCode").val() == "" ? false : true);
    var selectGender = ($("#selectGender").val() == "" ? false : true);
    var selectCurrency = ($("#selectCurrency").val() == "" ? false : true);




    if ($('#radioOnBehalfOf').is(':checked') && $('#radioInternal').is(':checked')) {
        if ($("#employeeNo").attr("data-isempexist") !== undefined &&
            $("#employeeNo").attr("data-isempexist") == "false") {
            employeeNo = false;
        }
    }
    else if ($('#radioNonEmployee').is(':checked')) {
        employeeNo = true;
    }

    var NameonPassport = true;
    var PassportNumber = true;
    var PassportIssueDate = true;
    var PassportExpiryDate = true;
    var selectDirector = true;

    var NameofNominee = true;
    var RelationwithNominee = true;
    var DOBofNominee = true;
    var selectGenderofNominee = true;

    if ($("#tileInternational").hasClass("selected-tile")) {// check for internation flow
         NameonPassport = ($("#NameonPassport").val() == "" ? false : true);
         PassportNumber = ($("#PassportNumber").val() == "" ? false : true);
         PassportIssueDate = ($("#PassportIssueDate").val() == "" ? false : true);
        PassportExpiryDate = ($("#PassportExpiryDate").val() == "" ? false : true);
        NameofNominee = ($("#NameofNominee").val() == "" ? false : true);
        RelationwithNominee = ($("#RelationwithNominee").val() == "" ? false : true);
        DOBofNominee = ($("#DOBofNominee").val() == "" ? false : true);
        selectGenderofNominee = ($("#selectGenderofNominee").val() == "" ? false : true);

        // Define a regular expression pattern for passport numbers
        // Modify the pattern to match the format you require
        var passportPattern = /^[A-Z0-9]{8}$/;

        if (!passportPattern.test($("#PassportNumber").val())) {
            // Passport number is valid
            PassportNumber = false;
        } 

    }
    else {
    }

    if ($('#selectDirector').is(':enabled') && $('#selectDirector').is(':visible')) {
        selectDirector = ($("#selectDirector").val() == "" ? false : true);
    }

    if (!mobileNumber || !selectGender) {
        $("#employeeSectionContent").slideDown();
    }

    if ($("#selectPurpose").val() == "8") {
        projectCode = true;
    }

    if ($("#advanceAmount").val().trim() == "") {
        selectCurrency = true;
    }


    // Check if any of the mandatory fields are empty
    if (
        !requestedFor ||
        !employeeType ||
        !employeeNo ||
        !mobileNumber ||
        !purpose ||
        !projectCode ||
        !NameonPassport ||
        !PassportNumber ||
        !PassportIssueDate ||
        !PassportExpiryDate ||
        !selectDirector ||
        !selectGender ||
        !selectCurrency ||
        !NameofNominee ||
        !RelationwithNominee ||
        !DOBofNominee ||
        !selectGenderofNominee
    ) {
        isValid = false;

        if (!requestedFor) $('input[name="requestedFor"]').addClass("error");
        if (!requestedFor) $('input[name="employeeType"]').addClass("error");
        if (!employeeNo) $("#employeeNo").addClass("error");
        if (!mobileNumber) $("#mobileNumber").addClass("error");
        if (!purpose) $("#selectPurpose").addClass("error");
        if (!projectCode) $("#selectProjectCode").addClass("error");
        if (!NameonPassport) $("#NameonPassport").addClass("error");
        if (!PassportNumber) $("#PassportNumber").addClass("error");
        if (!PassportIssueDate) $("#PassportIssueDate").addClass("error");
        if (!PassportExpiryDate) $("#PassportExpiryDate").addClass("error");
        if (!selectDirector) $("#selectDirector").addClass("error");
        if (!selectGender) $("#selectGender").addClass("error");
        if (!selectCurrency) $("#selectCurrency").addClass("error");
        if (!NameofNominee) $("#NameofNominee").addClass("error");
        if (!RelationwithNominee) $("#RelationwithNominee").addClass("error");
        if (!DOBofNominee) $("#DOBofNominee").addClass("error");
        if (!selectGenderofNominee) $("#selectGenderofNominee").addClass("error");

    }

    // You can add additional validation logic here

    if (!isValid) {
        // alert("Please fill in all mandatory fields.");
    }

    return isValid;
}



$(".mandatoryFields").on("click", function () {
    // Your code to handle the click event here
    // For example, you can add a CSS class or perform some action
    $(this).removeClass("error");
});

$(document).ready(function () {
    $('#preloader').show();
    try {
    // Initialize the page with the default UI state
    handleTravelTypeSelection();

    // Add click event listeners for "Travel Type" tiles
    $('#tileOneWay').click(function () {
        selectTile('OneWay', 'travel-type');
        handleTravelTypeSelection();
    });

    $('#tileRoundTrip').click(function () {
        selectTile('RoundTrip', 'travel-type');
        handleTravelTypeSelection();
    });

    $('#tileMultiCity').click(function () {
        selectTile('MultiCity', 'travel-type');
        handleTravelTypeSelection();
    });

    // Add click event listener for "Add More" button
    $('#addMoreButton').click(function () {
        // Show/hide the additional input fields
        var clonedRow = $("#addMoreRowContents").clone();

        // Remove the id attribute to prevent duplicate IDs
        clonedRow.removeAttr('id');

        // Clear the input values in the cloned row
        clonedRow.find('input').val('');

        // Append the cloned row to the last .rowForSourceAndDestination
        $(".rowForSourceAndDestination:last").after(clonedRow);
        $(".rowForSourceAndDestination:last").find(".travelDepartureDate").removeAttr("id").removeClass("hasDatepicker");
        // Show the cloned row
        clonedRow.show();

        InitializeCalenders();
    });

        $("#travelDepartureDate").datepicker({
            //showOn: "button",
            //buttonImage: "./css/images/icons8-calendar-50.png",  // path to your calendar icon image
            //buttonImageOnly: true,
            //buttonText: "Select date",
        minDate: 0, // Disable past dates
        dateFormat: "dd/mm/yy",
        onSelect: function (selectedDate) {
            var fromDate = new Date(formatDateForDisplay(selectedDate));
            fromDate.setDate(fromDate.getDate()); // Minimum Return Date is Departure Date + 1 day
            $("#travelReturnDate").datepicker("option", "minDate", fromDate);
            SetMulticityDates(this,fromDate);
        }
    });

    // Initialize Datepicker for Return Date
    $("#travelReturnDate").datepicker({
        minDate: 0, // Disable past dates
        dateFormat: "dd/mm/yy",
    });

     InitializeCalenders();

    if (IsEditMode()) {
        PopuplateTravelFromAndTravelToCity();
    }

        if (IsEditMode() && getUrlParameter("mode") == "view") {
            var hdnEmpNumber = $("#hdnEmpNumber").val();
            if (hdnEmpNumber !== undefined && hdnEmpNumber != null && hdnEmpNumber != "") {
                GetEmpDataByEmpNo(hdnEmpNumber);

            }
        }
        else {
                fetchEmployeeData();

        }


    

     

    // Toggle accordion on click
    $('#employeeSectionHeader').click(function () {
        $('#employeeSectionContent').slideToggle();
    });

    var select = $("#selectProjectCode");

    // Clear existing options
    select.empty();

    // Add the "Please select" option as the first item
    select.append($("<option></option>")
        .attr("value", "")
        .text("Please select"));

    $.ajax({
        url: "/NewRequest/BindProjects",
        type: "GET",
        dataType: "json",
        success: function (data) {
            // Add new options from the data returned by the AJAX call
            $.each(data, function (index, project) {
                select.append($("<option></option>")
                    .attr("value", project.projectId)
                    .text(project.projectName));
            });

            //In Edit mode bind projects
            if (window.location.href.indexOf("EditTravelRequest") > -1) {
                if ($("#selectProjectCode").attr("data-value") !== undefined) {
                    $("#selectProjectCode").val($("#selectProjectCode").attr("data-value"));
                }
            }
        },
        error: function (error) {
            console.error(error);
        }
    });

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
                    .attr("value", currency.currencyId)
                    .text(currency.currencyCode));
            });

            if (window.location.href.indexOf("EditTravelRequest") > -1) {
                if ($("#selectCurrency").attr("data-value") !== undefined) {
                    $("#selectCurrency").val($("#selectCurrency").attr("data-value"));
                }
            }
        },
        error: function (error) {
            console.error(error);
        }
    });

    // Fetch purposes
    $.ajax({
        url: "/NewRequest/GetPurposes",
        type: "GET",
        dataType: "json",
        success: function (data) {
            var purposeSelect = $("#selectPurpose");

            // Clear existing options
            purposeSelect.empty();

            // Add a "Please select" option as the first item
            purposeSelect.append($("<option></option>")
                .attr("value", "")
                .text("Please select"));

            // Add purpose options from the data returned by the AJAX call
            $.each(data, function (index, purpose) {
                purposeSelect.append($("<option></option>")
                    .attr("value", purpose.purposeId)
                    .text(purpose.purposeName));
            });

            if (window.location.href.indexOf("EditTravelRequest") > -1) {
                if ($("#selectPurpose").attr("data-value") !== undefined) {
                    $("#selectPurpose").val($("#selectPurpose").attr("data-value"));
                }
            }
        },
        error: function (error) {
            console.error(error);
        }
    });

    $.ajax({
        url: "/NewRequest/GetDirectors",
        type: "GET",
        dataType: "json",
        success: function (data) {
            var selectDirector = $("#selectDirector");

            // Clear existing options
            selectDirector.empty();

            // Add a "Please select" option as the first item
            selectDirector.append($("<option></option>")
                .attr("value", "")
                .text("Please select"));

            // Add currency options from the data returned by the AJAX call
            $.each(data, function (index, director) {
                selectDirector.append($("<option></option>")
                    .attr("value", director.employeeID)
                    .text(director.employeeName));
            });

            if (IsEditMode()) {
                
               
                if (getUrlParameter("mode") == "view") {
                    ShowRequestInDisabledMode();
                }
                if ($("#hdnDirector").val() != null && $("#hdnDirector").val() != "") {
                    $('#selectDirector option[value=' + $("#hdnDirector").val() + ']').attr("selected", "selected");
                }
            }
        },
        error: function (error) {
            console.error(error);
        }
    });
    }
    catch (error) {
        $('#preloader').hide();
    }

    $('.travelFrom , .travelTo').on('change', function () {
        // Get selected values

        SOurceandDestinationvalidationcheck();
       
    });

    if (IsEditMode()) {
        if ($('#radioNonEmployee').is(':checked')) {
            EnableSelectionOfInternalEmp();
        }
        if (getUrlParameter("mode") == "view") {
            ShowRequestInDisabledMode();
        }
    }

    if (window.location.href.indexOf("EditTravelRequest") > -1) {
        if ($("#selectGenderofNominee").attr("data-value") !== undefined) {
            $("#selectGenderofNominee").val($("#selectGenderofNominee").attr("data-value"));
        }

        if ($("#hdnIsDirectorApprovalNeeded").val() == "true") {
            $('#selectDirector').prop('disabled', false);
        }
        else {
            $('#selectDirector').prop('disabled', true);
        }
    }


    if (IsOnBehalfRequestInViewModeForNonEmployee()) {
        UpdateEmployeeNameAndTravellerNameConditionally();
    }

    $('#preloader').hide();
});

// Function to fetch employee data
function fetchEmployeeData() {
    var empEmailId = $("#hdnLoggedInEmail").val();

    if (window.location.href.indexOf("EditTravelRequest") > -1) {
        var tempempEmailId = sessionStorage.getItem("loggedinNameEmail");

        if (tempempEmailId !== undefined && tempempEmailId != null
            && tempempEmailId != "") {
            empEmailId = tempempEmailId;
        }
    }



    if (empEmailId != "" && empEmailId != null && empEmailId !== undefined) {

        sessionStorage.setItem("loggedinNameEmail", empEmailId);

        $.ajax({
            url: '/NewRequest/GetEmployeeData',
            type: 'GET',
            data: { employeeEmailId: empEmailId }, // Pass the employee ID
            dataType: 'json',
            success: function (response) {

                if (response != null) {
                    var data = response.employeeData;
                    var isEmployeeTravelDesk = response.isEmployeeTravelDesk;
                    if (data) {

                        if (!isEmployeeTravelDesk) {
                            if (data.grade == '' || data.grade === undefined || data.grade == null) {
                                $.fancybox.open({
                                    src: "Grade not found in your profile, Please contact admin.",
                                    type: "html"
                                });
                                $('#nextButton').prop('disabled', true);
                            }
                            else if (data.isGradeFindInEligibility == false) {
                                $.fancybox.open({
                                    src: "Grade not found in grade eligibility table, Please contact admin.",
                                    type: "html"
                                });
                                $('#nextButton').prop('disabled', true);
                            }
                            else if (data.isGradeFindInPolicyDetails == false) {
                                $.fancybox.open({
                                    src: "Grade not found in policy table, Please contact admin",
                                    type: "html"
                                });
                                $('#nextButton').prop('disabled', true);
                            }
                            else if (data.isBlockNewTicket == true) {
                                if (!IsEditMode()) {
                                    $.fancybox.open({
                                        src: "Close previous pending request before creating a new one.",
                                        type: "html"
                                    });
                                    $('#nextButton').prop('disabled', true);
                                }
                            }
                            else {
                                $('#nextButton').prop('disabled', false);
                            }
                        }
                        // Employee No
                        updateField('#employeeNo', data.employeeNo);

                        // Employee Name
                        updateField('#employeeName', data.name);

                        // Grade
                        updateField('#grade', data.grade);

                        // Position
                        updateField('#position', data.position);

                        // Department
                        updateField('#department', data.department);


                        // Location
                        updateField('#location', data.spsLocation);

                        // Mobile Number
                        updateField('#mobileNumber', data.mobileNumber);

                        // Gender
                        if (data.gender !== undefined && data.gender != null && data.gender != "") {
                            if (data.gender.toLowerCase() == "male") {
                                updateField('#selectGender', "1");
                            }
                            else if (data.gender.toLowerCase() == "female") {
                                updateField('#selectGender', "2");
                            }
                            else if (data.gender.toLowerCase() == "other") {
                                updateField('#selectGender', "3");
                            }
                            $("#selectGender").prop("disabled", true);
                        }


                        // Manager 1
                        updateField('#manager1', data.manager);

                        // Manager 2
                        updateField('#manager2', data.manager2);

                        updateField('#age', calculateAge(data.dateOfBirth));

                        // Function to update a field based on data value
                        function updateField(selector, value) {
                            if (value !== undefined && value !== null && value.trim() !== '') {
                                $(selector).attr("orgvalue", value);
                                $(selector).val(value);
                            }
                            else if (selector == "#manager1" || selector == "#manager2") {
                                $(selector).attr("disabled", true);
                            }
                        }

                        if ($('#radioNonEmployee').is(':checked') &&
                            $("#hdnEmpName").val() != null && $("#hdnEmpName").val() != "") {

                            $('#employeeName').val($("#hdnEmpName").val());
                        }
                        else {
                            updateField('#employeeName', data.name);
                        }
                        //var loggedinName = "Welcome, " + data.name;
                        /* $("#spnLoggedinName").text(loggedinName)*/
                        sessionStorage.setItem("loggedinNameSession", data.name);
                        //localStorage.setItem('loggedinName', loggedinName);
                        SetLoginName();

                    } else {
                        $.fancybox.open({
                            src: "Your employee details not found.",
                            type: "html"
                        });

                        $('#nextButton').prop('disabled', true);
                    }
                }
                else {
                    $.fancybox.open({
                        src: "Your employee details not found.",
                        type: "html"
                    });

                    $('#nextButton').prop('disabled', true);
                }

            },
            error: function (error) {
                console.error('Error:', error);
            }
        });
    }
    else {
        alert("Error occurred, Please try login again")
    }
}


function SOurceandDestinationvalidationcheck() {
    var isvalid = true;
    if ($("#tileMultiCity").hasClass("selected-tile")) {

        $('.rowForSourceAndDestination').each(function (index) {
            if ($(this).css('display') != 'none') {

                var travelFrom = $(this).find('.travelFrom option:selected').text();
                var travelTo = $(this).find('.travelTo option:selected').text();
                if (travelFrom != "" || travelTo != "") {
                        if (travelFrom === travelTo) {
                            alert('Please select different values for source and destination.');
                            isvalid = false;
                            // You can also reset one of the dropdowns or take other actions as needed.
                        }
                    }
            


            }
        });


    }
    else {
        var strtravelFrom = "";
        if ($(".travleFromFirst option:selected").text() !== undefined &&
            $(".travleFromFirst option:selected").text() != "") {
            strtravelFrom = $(".travleFromFirst option:selected").text();
        }
        var strtravelTo = "";
        if ($(".travelToFirst option:selected").text() !== undefined &&
            $(".travelToFirst option:selected").text() != "") {
            strtravelTo = $(".travelToFirst option:selected").text();
        }

        if (strtravelFrom != "" || strtravelTo != "") {
            if (strtravelFrom === strtravelTo) {
                alert('Please select different values for source and destination.');
                isvalid = false;
                // You can also reset one of the dropdowns or take other actions as needed.
            }
        }
    }
    return isvalid;
}

$(window).on('load', function () {
    $('#preloader').hide();
});

function calculateAge(dateOfBirth) {
    if (dateOfBirth !== undefined && dateOfBirth !== null && dateOfBirth.trim() !== '') {


        // Split the DOB string into month, day, and year components
        var dobParts = dateOfBirth.split('/');

        // Ensure that the DOB has valid parts
        if (dobParts.length === 3) {
            var birthMonth = parseInt(dobParts[0]);
            var birthDay = parseInt(dobParts[1]);
            var birthYear = parseInt(dobParts[2]);

            // Get the current date
            var currentDate = new Date();

            // Calculate the age
            var age = currentDate.getFullYear() - birthYear;

            // Check if the birthday has occurred this year
            if (currentDate.getMonth() < birthMonth || (currentDate.getMonth() === birthMonth && currentDate.getDate() < birthDay)) {
                age--;
            }

            return age.toString();
        } else {
            // Return an error value or handle the invalid dateOfBirth format
            // return 'Invalid DOB format';
            return '';
        }
    }
    else {
        return '';
    }
}



function handleTravelTypeSelection() {
    var selectedTravelType = $('.selected-tile.travel-type').attr('data-TravelTypeId');
    var returnDateInput = $('#returnDateColumn');
    var addMoreButton = $('#addMoreButtonSection');
    var addMoreRow = $('.rowForSourceAndDestinationDummy');  

    if (selectedTravelType == 1) { // One Way
        $('#travelReturnDate').prop('disabled', true);
        returnDateInput.show();
        addMoreButton.hide();
        addMoreRow.hide();
        
    } else if (selectedTravelType == 2) { // Round Trip
        $('#travelReturnDate').prop('disabled', false);
        returnDateInput.show();
        addMoreButton.hide();
        addMoreRow.hide();
        
    } else if (selectedTravelType == 3) { // Multi City
        $('#travelReturnDate').prop('disabled', true);
        returnDateInput.hide();
        addMoreButton.show();
        
    }
}


function DeleteCurrentRow(current) {
    $(current).closest(".rowForSourceAndDestinationDummy").remove();
}



function InitializeCalenders() {

    $(".travelDepartureDate").datepicker({
        minDate: 0, // Disable past dates
        dateFormat: "dd/mm/yy",
        onSelect: function (selectedDate) {
            var fromDate = new Date(formatDateForDisplay(selectedDate));
           

            SetMulticityDates(this,fromDate);

        }
    });

    AddMinDateToMulticityDate();
    // Initialize the Passport Issue Date date picker
    $("#PassportIssueDate").datepicker({
        maxDate: 0, // Disable future dates
        dateFormat: "dd/mm/yy",
    });

    // Initialize the Passport Expiry Date date picker
    $("#PassportExpiryDate").datepicker({
        minDate: "+6M", // Minimum date is 6 months from today
        dateFormat: "dd/mm/yy",
    });

    $("#DOBofNominee").datepicker({
        maxDate: 0, // Disable future dates
        dateFormat: "dd/mm/yy",
    });

}

function PopuplateTravelFromAndTravelToCity() {

    var isEditMode = false;
    if (IsEditMode()) {
        isEditMode = true;
    }

    // Populate the "travelFrom" select dropdown
    var strTravelDestination = $("#divTravelDestination").find(".selected-tile").attr("data-TravelDestinationId");

    var FillCities = $(".FillCities");
    $.ajax({
        url: "/NewRequest/GetCities", // Adjust the URL to your API endpoint
        type: "POST", // Assuming you're fetching data
        data: { Destination: strTravelDestination },
        dataType: "json",
        success: function (data) {
           
            $.each(FillCities, function (index, cityName) {

                var strSelectedText = "";

            // Add a "Please select" option as the first item
                var control = $(this);

                if (IsEditMode) {
                    strSelectedText = $(control).find(":selected").text();
                }

                control.empty();
                $(control).append($("<option></option>")
                .attr("value", "")
                .text("Please select"));

            // Add currency options from the data returned by the AJAX call
            $.each(data, function (index, cityName) {
                $(control).append($("<option></option>")
                    .attr("value", index)
                    .text(cityName));
            });

                if (IsEditMode) {
                    $(control).find("option:contains(" + strSelectedText + ")").attr('selected', 'selected');
                }
            });

            $(".FillCities").prop("disabled", false);
            if (IsEditMode()) {

                if (getUrlParameter("mode") == "view") {
                    ShowRequestInDisabledMode();
                }
            }
        }
    });

}

$(".Travel-Destination").on("click", function () {
    PopuplateTravelFromAndTravelToCity();
    $(".FillCities").prop("disabled", false);
});

$('input[type=radio][name=requestedFor]').change(function () {
   
    CheckOnBehalfAndNonemployeeflow();
    UpdateEmployeeNameAndTravellerNameConditionally();
    //CheckEmployeeTypeFlow();
});

function CheckOnBehalfAndNonemployeeflow() {
    if ($('#radioOnBehalfOf').is(':checked')) {
        $("#mobileNumber").val('');
        ClearEmployeeSection();
        CheckEmployeeTypeFlow();
    }
    else {
        FillEmployeeDetails();
        DisbaleSelectionOfInternalEmp();
    }
}



function ClearEmployeeSection() {
    var isinternational = false;
    if ($("#tileInternational").hasClass("selected-tile")) {
        isinternational = true;
    }

    //$("#employeeSectionContent").find("input[type=text]").prop("disabled", false).val('')
    if (isinternational) {
        $("#employeeSectionContent").find("input[type=text]").each(function (i) {
            if ($(this).attr("id") != "managingdirector") {
                $(this).prop("disabled", false).val('')
            }
        });
    }
    else {
        $("#employeeSectionContent").find("input[type=text]").prop("disabled", false).val('')
    }
   

    $("input[name='employeeType']").each(function (i) {
        $(this).attr('disabled', false);
    });
    $("#selectGender").prop("disabled", false);
    $("#mobileNumber").val('')
}

function DisbaleSelectionOfInternalEmp() {
    $("input[name='employeeType']").each(function (i) {
        $(this).attr('disabled', true);
    });
}

function EnableSelectionOfInternalEmp() {
    $("input[name='employeeType']").each(function (i) {
        $(this).attr('disabled', false);
    });
}

$('input[type=radio][name=employeeType]').change(function () {
    $("#mobileNumber").val('')
    CheckEmployeeTypeFlow();
});

function CheckEmployeeTypeFlow() {
    if ($('#radioNonEmployee').is(':checked')) {
        FillEmployeeDetails();
        //DisbaleSelectionOfInternalEmp();
        ChangesToExternalEmployee();

    }
    else {
        ClearEmployeeSection();
    }
    UpdateEmployeeNameAndTravellerNameConditionally();
}

function FillEmployeeDetails() {
    $(".internalempField").each(function (i) {
        var orgValue = $(this).attr("orgvalue");
        if (orgValue !== undefined && orgValue != null && orgValue != "") {
            $(this).val(orgValue);
            $(this).prop("disabled", true)
        }
        else if ($(this).attr("id") == "manager2") {
            if ($('#radioNonEmployee').is(':checked') &&
                $('#radioOnBehalfOf').is(':checked')) {
                $(this).prop("disabled", true)
          }
        }
    });
}



function ChangesToExternalEmployee() {
   // $("#employeeSectionContent").find("input[type=text]").prop("disabled", false).val('')
   /* $(".internalempField").prop("disabled", true).val('');*/

    $("#selectGender").prop("disabled", false);
    $("#employeeName").prop("disabled", false).val('');
}

function UpdateEmployeeNameAndTravellerNameConditionally() {
    if ($('#radioNonEmployee').is(':checked')) {
        if ($('#radioOnBehalfOf').is(':checked')) {
            $("#hdrEmpName").text("Traveller Name");
        }
        else {
            $("#hdrEmpName").text("Employee Name");
        }
       
    }
    else {
        $("#hdrEmpName").text("Employee Name");
    }
}

$(".clanederIconClick").on("click", function () {
    $(this).closest(".clanederIconClickRow").find('input').focus()
});

function IsEditMode() {
    if (window.location.href.indexOf("EditTravelRequest") > -1 ||
        window.location.href.indexOf("ViewTravelRequest") > -1) {
        return true;
    }
    else {
        return false;
    }
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

$("#employeeNo").on("keyup", function (e) {
    if ($('#radioOnBehalfOf').is(':checked') && $('#radioInternal').is(':checked')) {
        if ($('#employeeNo').val().length == 6) {
            GetEmpDataByEmpNo($('#employeeNo').val());
        }
        else {

        }
       }
})


function ShowRequestInDisabledMode() {
    $("#NewTravelRequestStep1").find("input, select").prop("disabled", true);
    $("#NewTravelRequestStep2").find("input, select").prop("disabled", true);
    $("#NewTravelRequestStep1").find("input").attr("disabled", "disabled");
    $("#NewTravelRequestStep2").find("input").attr("disabled", "disabled");
    $("#NewTravelRequestStep1").find("select").attr("disabled", "disabled");
    $("#NewTravelRequestStep2").find("select").attr("disabled", "disabled");
    $("#comments").prop('disabled', true);
    $(".tilediv").addClass("disabled-div");
    $("#NewTravelRequestStep1").show();
    $("#NewTravelRequestStep2").show();
    $("#btnSubmit").hide();
    $(".ClassForHideOnSubmit").hide();
    $("#previousButtonStage2").hide();
    $(".deletecurrentrowMulticity").hide();
    $(".addMoreButtonSection").hide();

    if ($("#tileInternational").hasClass("selected-tile")) {
        $("#rowForNominee").show();
        $("#rowForPassport").show();
    }
 }

function GetEmpDataByEmpNo(employeeNumber) {

    var strRequestToken = "";
    if (window.location.href.indexOf("EditTravelRequest") > -1) {
        strRequestToken = getUrlParameter("requestToken");
    }
    else if ($("#hdnRequestToken").val() != "") {
        strRequestToken = $("#hdnRequestToken").val();
    }

    $.ajax({
        url: '/NewRequest/GetEmpDataByEmpNo',
        type: 'GET',
        data: {
            EmployeeNumber: employeeNumber,
            StrRequestToken: strRequestToken
        }, // Pass the employee ID
        dataType: 'json',
        success: function (response) {
            if (response != null) {
                var data = response.employeeData;
                var isEmployeeTravelDesk = response.isEmployeeTravelDesk;
                if (data) {
                    $("#employeeNo").attr("data-isempexist", true);
                    if (!isEmployeeTravelDesk) {
                        if (data.grade == '' || data.grade === undefined || data.grade == null) {
                            $.fancybox.open({
                                src: "Grade not found in employee profile, Please contact admin.",
                                type: "html"
                            });
                            $('#nextButton').prop('disabled', true);
                        }
                        else if (data.isGradeFindInEligibility == false) {
                            $.fancybox.open({
                                src: "Grade not found in grade eligibility table, Please contact admin.",
                                type: "html"
                            });
                            //$('#nextButton').prop('disabled', true);
                        }
                        else if (data.isGradeFindInPolicyDetails == false) {
                            $.fancybox.open({
                                src: "Grade not found in policy table, Please contact admin",
                                type: "html"
                            });
                            // $('#nextButton').prop('disabled', true);
                        }
                        else {
                            // $('#nextButton').prop('disabled', false);
                        }
                    }
                    // Employee No
                    updateField('#employeeNo', data.employeeNo);

                    // Employee Name
                    if ($('#radioNonEmployee').is(':checked') &&
                        $("#hdnEmpName").val() != null && $("#hdnEmpName").val() != "") {
                        
                        $('#employeeName').val($("#hdnEmpName").val()); 
                    }
                    else {
                        updateField('#employeeName', data.name);
                    }
                    

                    // Grade
                    updateField('#grade', data.grade);

                    if (!IsOnBehalfRequestInViewModeForNonEmployee()) {
                        // Position
                        updateField('#position', data.position);

                        // Department
                        updateField('#department', data.department);


                        // Location
                        updateField('#location', data.spsLocation);

                        // Mobile Number
                        updateField('#mobileNumber', data.mobileNumber);



                        updateField('#age', calculateAge(data.dateOfBirth));
                    }
                    else {
                        $("#hdrEmpName").text("Traveller Name");
                    }

                    // Gender
                    if (data.gender !== undefined && data.gender != null && data.gender != "") {
                        if (data.gender.toLowerCase() == "male") {
                            updateField('#selectGender', "1");
                        }
                        else if (data.gender.toLowerCase() == "female") {
                            updateField('#selectGender', "2");
                        }
                        else if (data.gender.toLowerCase() == "other") {
                            updateField('#selectGender', "3");
                        }
                        $("#selectGender").prop("disabled", true);
                    }

                    


                    // Manager 1
                    updateField('#manager1', data.manager);

                    // Manager 2
                    updateField('#manager2', data.manager2);

                   

                    // Function to update a field based on data value
                    //function updateField(selector, value) {
                    //    if (value !== undefined && value !== null && value.trim() !== '') {
                    //        $(selector).attr("orgvalue", value);
                    //        $(selector).val(value);
                    //    }
                    //}

                    //updateField('#employeeName', data.name);
                    //var loggedinName = "Welcome, " + data.name;
                    /* $("#spnLoggedinName").text(loggedinName)*/
                    //sessionStorage.setItem("loggedinNameSession", data.name);
                    //localStorage.setItem('loggedinName', loggedinName);
                    //SetLoginName();
                    function updateField(selector, value) {
                        if (value !== undefined && value !== null && value.trim() !== '') {
                            //$(selector).attr("orgvalue", value);
                            $(selector).val(value);
                        }
                        else if (selector == "#manager1" || selector == "#manager2") {
                            $(selector).attr("disabled", true);
                        }
                    }

                    var strTravelDestination = $("#divTravelDestination").find(".selected-tile").attr("data-TravelDestinationId");


                    if (data.isDirectorApprovalNeeded) {
                        if (strTravelDestination == "2") {
                            //$("#managingdirector").val(response[0].mdDirectorPersonnelList[0].employeeName);
                            //$("#managingdirector").attr("data-empNo", response[0].mdDirectorPersonnelList[0].employeeID)
                            $(".ManagingDirectorRow").show();
                            $(".NormalDirectorRow").hide();
                        }
                        else {
                            $("#selectDirector").prop("disabled", false);
                            $(".ManagingDirectorRow").hide();
                            $(".NormalDirectorRow").show();
                        }

                    }
                    else {
                        $("#selectDirector").prop("disabled", true);
                        $(".ManagingDirectorRow").hide();
                        $(".NormalDirectorRow").show();
                    }
                    disableTextboxesInSection();
                  
                    if (IsEditMode() && getUrlParameter("mode") == "view") {
                        $("#employeeNo").prop("disabled", true);
                    }
                    else {
                        $("#employeeNo").prop("disabled", false);
                    }

                } else {
                    console.log('No data retrieved.');
                }
            }
            else {
                $.fancybox.open({
                    src: "Employee not found.",
                    type: "html"
                });
                $("#employeeNo").attr("data-isempexist", false);
            }
            
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}

function IsOnBehalfRequestInViewModeForNonEmployee() {
    if (IsEditMode() &&
        $('#radioOnBehalfOf').is(':checked') &&
        $('#radioNonEmployee').is(':checked')) {
        return true;
    }
    else {
        return false;
    }
}

$("#tileAir").on("click", function () {
    // Your code to handle the click event here
    // For example, you can add a CSS class or perform some action
    $(this).removeClass("error");
});

function EnableDisableTravelDestination() {

}

$(document).ready(function () {
    // Attach keypress event to all text input fields of type 'text'
    $('.BlockSpecialCharacters').on('keypress', function (event) {
        // Get the character code of the pressed key
        var charCode = event.which;

        // Get the id of the current textbox
        var textboxId = $(this).attr('id');

        // Allow numbers (0-9), alphabets (a-z and A-Z), and decimal point (.) for all textboxes except "PassportNumber"
        if (
            ((charCode >= 48 && charCode <= 57) && textboxId == 'PassportNumber') || // Numbers 0-9 only if not PassportNumber
            (charCode >= 65 && charCode <= 90) || // Uppercase alphabets A-Z
            (charCode >= 97 && charCode <= 122) || // Lowercase alphabets a-z
            charCode === 46 || charCode === 32 // Decimal point (.) and space
        ) {
            return true;
        } else {
            // Prevent the input of any other characters
            event.preventDefault();
        }
    });

});

function SetMulticityDates(control,fromDate) {
    if ($("#tileMultiCity").hasClass("selected-tile")) {
        var currentIndex = $(".travelDepartureDate").index(control);

        // Set the date for the next datepicker
        var nextIndex = currentIndex + 1;
        var $nextDatepicker = $(".travelDepartureDate").eq(nextIndex);

        if ($nextDatepicker.length > 0) {
            // If there is a next datepicker, set its date to the selected date or a future date
            var currentDate = new Date(fromDate);
            currentDate.setDate(currentDate.getDate()); // You can adjust this logic as needed

            $nextDatepicker.datepicker("option", "minDate", currentDate);
        }
    }

}

function AddMinDateToMulticityDate() {
    
    var $newDatepicker = $(".travelDepartureDate").last();
    var $previousDatepicker = $(".travelDepartureDate").eq(-2);
    var previousDate = $previousDatepicker.datepicker("getDate");

    if (previousDate !== null) {
        var newMinDate = new Date(previousDate);
        newMinDate.setDate(newMinDate.getDate());

        $newDatepicker.datepicker("option", "minDate", newMinDate);
    }
}