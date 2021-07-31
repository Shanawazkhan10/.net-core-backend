var toastCount = 0;

function Logout_Application() {
    window.localStorage.removeItem(window.btoa("User_Details"));    // User Details 
    window.localStorage.removeItem("09aSw0tYah2754");   // Auth Details
    window.location = './index.html';
}

function Show_Success_Toastr(message) {
    showNotification('success', message, 'toast-top-right');
}

function Show_Error_Toastr(message) {
    showNotification('error', message, 'toast-top-right');
}

function Show_Info_Toastr(message) {
    showNotification('info', message, 'toast-top-right');
}

const Get_QueryParams = (params) => {
    let href = window.location.href;
    //this expression is to get the query strings
    let reg = new RegExp('[?&]' + params + '=([^&#]*)', 'i');
    let queryString = reg.exec(href);
    return queryString ? queryString[1] : null;
};

const Get_QueryParams_Decrypt = (mainparams, params) => {
    try {

        var HREF_Decrypt = window.atob(Get_QueryParams(mainparams));
        HREF_Decrypt = "?" + mainparams + "=" + HREF_Decrypt;
        let href = HREF_Decrypt;

        //this expression is to get the query strings
        let reg = new RegExp('[?&]' + params + '=([^&#]*)', 'i');
        let queryString = reg.exec(href);
        return queryString ? queryString[1] : null;

    } catch (e) {

        return null;
    }

};

function Is_Valid_Email($email) {
    if ($email.length > 0) {
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        return emailReg.test($email);
    } else {
        return false;
    }
}

function Is_Valid_MobileNo($mobile) {
    if ($mobile.length == 10) {
        var mobileReg = /^\d*(?:\.\d{1,2})?$/;
        return mobileReg.test($mobile);
    } else {
        return false;
    }
}

//function to format date in YYYY-MM-DD
function Format_YYYYMMDD(Date) {
    var Input_Date = Date.split("/");
    var YYYY = Input_Date[2];
    var MM = Input_Date[0];
    var DD = Input_Date[1];

    var Output_Formated_Date = YYYY + "-" + MM + "-" + DD;
    return Output_Formated_Date;
}

function dateFormatter(fdDate) {
    function format(x) {
        return (x < 10) ? '0' + x : x;
    }
    var modifiedDate = fdDate.split('-'),
        d = new Date(modifiedDate),
        convertedDate = [d.getFullYear(), format(d.getMonth() + 1), format(d.getDate())].join('-');
    return convertedDate;
}

//Common Function To convert Jquery date() to MM/DD/YYYY Format 
function getFormattedDate(date) {
    var year = date.getFullYear();

    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return year + '-' + month + '-' + day;
}

function showNotification(colorName, text, placementFrom) {
    var shortCutFunction = colorName;
    var msg = text;
    var title = '';
    var toastIndex = toastCount++;
    var addClear = false;

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": placementFrom,
        "preventDuplicates": true,
        "showDuration": 300,
        "hideDuration": 100,
        "timeOut": 5000,
        "extendedTimeOut": 1000,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    toastr.options.showDuration = 300;
    toastr.options.hideDuration = 100;
    toastr.options.timeOut = 5000;
    toastr.options.extendedTimeOut = 1000;


    if (addClear) {
        msg = getMessageWithClearButton(msg);
        toastr.options.tapToDismiss = false;
    }
    if (!msg) {
        msg = getMessage();
    }

    $('#toastr-log').text('Command: toastr["' +
        shortCutFunction +
        '"]("' +
        msg +
        (title ? '", "' + title : '') +
        '")\n\ntoastr.options = ' +
        JSON.stringify(toastr.options, null, 2)
    );

    var $toast = toastr[shortCutFunction](msg, title); // Wire up an event handler to a button in the toast, if it exists
    $toastlast = $toast;

    if (typeof $toast === 'undefined') {
        return;
    }

    if ($toast.find('#okBtn').length) {
        $toast.delegate('#okBtn', 'click', function () {
            alert('you clicked me. i was toast #' + toastIndex + '. goodbye!');
            $toast.remove();
        });
    }
    if ($toast.find('#surpriseBtn').length) {
        $toast.delegate('#surpriseBtn', 'click', function () {
            alert('Surprise! you clicked me. i was toast #' + toastIndex + '. You could perform an action here.');
        });
    }
    if ($toast.find('.clear').length) {
        $toast.delegate('.clear', 'click', function () {
            toastr.clear($toast,
                {
                    force: true
                });
        });
    }
}

//function showNotification(colorName, text, placementFrom, placementAlign, animateEnter, animateExit) {
//    if (colorName === null || colorName === '') { colorName = 'bg-black'; }
//    if (text === null || text === '') { text = 'Standard alert message'; }
//    if (animateEnter === null || animateEnter === '') { animateEnter = 'animated fadeInDown'; }
//    if (animateExit === null || animateExit === '') { animateExit = 'animated fadeOutUp'; }
//    var allowDismiss = true;

//    $.notify({
//        message: text
//    },
//        {
//            type: colorName,
//            allow_dismiss: allowDismiss,
//            newest_on_top: true,
//            timer: 1000,
//            placement: {
//                from: placementFrom,
//                align: placementAlign
//            },
//            animate: {
//                enter: animateEnter,
//                exit: animateExit
//            },
//            template: '<div data-notify="container" class="bootstrap-notify-container alert alert-dismissible {0} ' + (allowDismiss ? "p-r-35" : "") + '" role="alert">' +
//                '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
//                '<span data-notify="icon"></span> ' +
//                '<span data-notify="title">{1}</span> ' +
//                '<span data-notify="message">{2}</span>' +
//                '<div class="progress" data-notify="progressbar">' +
//                '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
//                '</div>' +
//                '<a href="{3}" target="{4}" data-notify="url"></a>' +
//                '</div>'
//        });
//}

function showBasicMessage(MessageText) {
    swal(MessageText);
}

