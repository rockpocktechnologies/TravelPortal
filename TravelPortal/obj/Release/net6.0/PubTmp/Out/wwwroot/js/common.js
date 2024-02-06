
$(document).ready(function () {
    SetLoginName();
});

function SetLoginName() {
    var name = sessionStorage.getItem("loggedinNameSession");

    if (name !== undefined && name != null && name != "") {
        $("#spnLoggedinName").text(name);
    }
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


