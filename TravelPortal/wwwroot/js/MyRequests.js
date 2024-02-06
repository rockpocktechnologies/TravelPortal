$(document).ready(function () {
    $('#travelTable').DataTable({
        responsive: {
            breakpoints: [
                { name: 'desktop', width: Infinity },
                { name: 'tablet', width: 1024 },
                { name: 'fablet', width: 768 },
                { name: 'phone', width: 480 }
            ]
        },
        columnDefs: [
           // { targets: [0], visible: false } // Hides the first column (Request Id)
        //  { targets: [3, 15], width: "1px" }
        ],
        order: [[0, 'desc']],
        "drawCallback": function (settings) {
            adjustColumnWidth();
            ShowHideAddExpense();
        }
    })

});

function adjustColumnWidth() {
    
    //$(".applydynamicwidthToAndFrom").width(1);
}

function ShowHideAddExpense() {
    // Select all table rows in the table with id "travelTable"
    $('#travelTable tbody tr').each(function () {
        var dateOfJourney = $(this).find('.date-of-journey-column').text(); // Replace with the actual class or attribute that contains the date
        var currentDate = new Date();
        var status = $(this).find('.classForStatus').text();
        // Compare the dateOfJourney with the current date
        dateOfJourney = new Date(formatDateForDisplayMyRequest(dateOfJourney));
        if (dateOfJourney <= currentDate && status == "Booked") {
            // Show the "Add Expense" button for past or today's date
            $(this).find('.add-expense-column').show();
           // $(this).find('.cancelcurrentrequest').hide();
            $(this).find('.editcurrentrequest').show();
        }
        else if (status == "Expenses Submitted" ||
            status == "Expense Approved"
            || status == "Closed" || status == "Rejected") {
            // Show the "Add Expense" button for past or today's date
            $(this).find('.add-expense-column').hide();
            $(this).find('.editcurrentrequest').hide();
            $(this).find('.cancelcurrentrequest').hide();
        }
        else if (status == "Expense Pending for Approval by manager" ||
            status == "Expense Pending for Approval by Accounts") {
            // Show the "Add Expense" button for past or today's date
            $(this).find('.add-expense-column').hide();
            $(this).find('.editcurrentrequest').hide();
            $(this).find('.cancelcurrentrequest').hide();
        }
        else if (status == "Expense Rejected by Manager" ||
            status == "Expense Rejected by Accounts") {
            // Show the "Add Expense" button for past or today's date
            $(this).find('.add-expense-column').show();
            $(this).find('.cancelcurrentrequest').hide();
            $(this).find('.editcurrentrequest').hide();
        }
        else if (status == "Cancelled") {
            // Show the "Add Expense" button for past or today's date
            $(this).find('.actionicons').css('visibility', 'hidden')
        }
        else if (status == "Booking Cancelled") {
            // Show the "Add Expense" button for past or today's date
            $(this).find('.classForStatus').text("Cancelled")
            $(this).find('.actionicons').css('visibility', 'hidden')
        }

        
    });
}

function CancelMyRequest(token) {
    if (confirm('Are you sure you want to cancel this travel request?')) {
        cancelTravelRequest(token)
    } else {
        
    }
}

function cancelTravelRequest(token) {
    
    $('#preloader').show();
    $.ajax({
        url: '/MyRequests/CancelRequest', // Replace with the actual URL of your controller action
        type: 'POST',
        data: { token: token },
        success: function (data) {
            $('#preloader').hide();
            if (data.success) {
                alert('Travel request canceled successfully.');
                // You can update the UI or perform other actions here.
            } else {
                alert('Failed to cancel the travel request.');
            }
            window.location.href = '/MyRequests';
        },
        error: function () {
            $('#preloader').hide();
            alert('An error occurred during the request.');
        }
    });
}



function formatDateForDisplayMyRequest(date) {
    var dateParts = date.split("-");
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

