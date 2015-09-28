function voteup(votebutton, issueid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/voteup/' + issueid + '/',
        success: function () {
            votebutton.classList.remove('unchecked');
            votebutton.classList.add('checked');
            var count = document.getElementById('count-' + issueID);
            count.innerHTML = parseInt(count.innerHTML, 10) + 1;
        },
        error: function (ts) {
            //alert('Could not vote up.');
            alert(ts.responseText);
        }
    });
}

function unvoteup(votebutton, issueid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/unvoteup/' + issueid + '/',
        success: function () {
            var count = document.getElementById('count-' + issueID);
            count.innerHTML = parseInt(count.innerHTML, 10) - 1;
        },
        error: function (ts) {
            alert('Could not unvote up.');
            alert(ts.responseText);
        }
    });
}


function vote(votebutton, issueID, userhandle) {
    var count = document.getElementById('count-' + issueID);
    if (votebutton.classList.contains('unchecked')) {
        voteup(issueID, userhandle);
        votebutton.classList.remove('unchecked');
        votebutton.classList.add('checked');
        votebutton.src = "../Content/Images/checked-hover.png";
        count.innerHTML = parseInt(count.innerHTML, 10) + 1;
    } else if (votebutton.classList.contains('checked')) {
        unvoteup(issueID, userhandle);
        votebutton.classList.remove('checked');
        votebutton.classList.add('unchecked');
        votebutton.src = "../Content/Images/unchecked-hover.png";
        count.innerHTML = parseInt(count.innerHTML, 10) - 1;
    }
}

function votehover(votebutton) {
    if (votebutton.classList.contains('unchecked')) {
        votebutton.src = "../Content/Images/unchecked-hover.png";
    } else if (votebutton.classList.contains('checked')) {
        votebutton.src = "../Content/Images/checked-hover.png";
    }
}

function voteunhover(votebutton) {
    if (votebutton.classList.contains('unchecked')) {
        votebutton.src = "../Content/Images/unchecked.png";
    } else if (votebutton.classList.contains('checked')) {
        votebutton.src = "../Content/Images/checked.png";
    }
}

function redirectToLogin() {
    window.location = "/login";
}