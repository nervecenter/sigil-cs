function subscribe(subbutton, orgurl) {
    $.ajax({
        type: "POST",
        url:  '/subscribe/' + orgurl + '/',
        success: function () {
            subbutton.classList.remove('subscribe');
            subbutton.classList.add('unsubscribe');
        },
        error: function (ts) {
            //alert('Could not vote up.');
            alert(ts.responseText);
        }
    });
}

function unsubscribe(subbutton, orgurl) {
    $.ajax({
        type: "POST",
        url:  '/unsubscribe/' + orgurl + '/',
        success: function () {
            subbutton.classList.remove('unsubscribe');
            subbutton.classList.add('subscribe');
        },
        error: function (ts) {
            //alert('Could not unvote up.');
            alert(ts.responseText);
        }
    });
}