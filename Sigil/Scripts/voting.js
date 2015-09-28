﻿function voteup(votebutton, issueid) {
    $.ajax({
        type: "POST",
        url: '/voteup/' + issueid + '/',
        success: function () {
            votebutton.classList.remove('voteup');
            votebutton.classList.add('unvoteup');
            votebutton.src = "../Content/Images/voted.png";
            votebutton.setAttribute('onClick', 'unvoteup(this, ' + issueid + ')');
            var countid = "#count-" + issueID.toString();
            var count = $(countid).html();
            $("#count-" + issueID).html(parseInt(count, 10) + 1);
        },
        error: function (ts) {
            //alert('Could not vote up.');
            alert(ts.responseText);
        }
    });
}

function unvoteup(votebutton, issueid) {
    $.ajax({
        type: "POST",
        url: '/unvoteup/' + issueid + '/',
        success: function () {
            votebutton.classList.remove('unvoteup');
            votebutton.classList.add('voteup');
            votebutton.src = "../Content/Images/notvoted-hover.png";
            votebutton.setAttribute('onClick', 'voteup(this, ' + issueid + ')');
            var countid = "#count-" + issueID.toString();
            var count = $(countid).html();
            $("#count-" + issueID).html(parseInt(count, 10) + 1);
        },
        error: function (ts) {
            alert('Could not unvote up.');
            alert(ts.responseText);
        }
    });
}

function votehover(votebutton) {
    if (votebutton.classList.contains('voteup')) {
        votebutton.src = "../Content/Images/notvoted-hover.png";
    }
}

function voteunhover(votebutton) {
    if (votebutton.classList.contains('voteup')) {
        votebutton.src = "../Content/Images/notvoted.png";
    }
}

function redirectToLogin() {
    window.location = "/login";
}