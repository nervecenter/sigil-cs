function subscribe(orgid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/subscribe/' + orgid + '/',
        success: function () {

        },
        error: function (ts) {
            //alert('Could not vote up.');
            alert(ts.responseText);
        }
    });
}

function unsubscribe(orgid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/unsubscribe/' + orgid + '/',
        success: function () {
        },
        error: function (ts) {
            alert('Could not unvote up.');
            alert(ts.responseText);
        }
    });
}


function set_sub(votebutton, orgID, userhandle) {
    var count = document.getElementById('count-' + issueID);
    if (votebutton.classList.contains('unchecked')) {
        subscribe(orgID, userhandle);
        votebutton.classList.remove('unchecked');
        votebutton.classList.add('checked');
        votebutton.src = "../Content/Images/check_mark_hover_small.png";
        count.innerHTML = parseInt(count.innerHTML, 10) + 1;
    } else if (votebutton.classList.contains('checked')) {
        unsubscribe(orgID, userhandle)
        votebutton.classList.remove('checked');
        votebutton.classList.add('unchecked');
        votebutton.src = "../Content/Images/vote_circle_hover_small.png";
        count.innerHTML = parseInt(count.innerHTML, 10) - 1;
    }
}

function subhover(votebutton) {
    if (votebutton.classList.contains('unchecked')) {
        votebutton.src = "../Content/Images/vote_circle_hover_small.png";
    } else if (votebutton.classList.contains('checked')) {
        votebutton.src = "../Content/Images/check_mark_hover_small.png";
    }
}

function subunhover(votebutton) {
    if (votebutton.classList.contains('unchecked')) {
        votebutton.src = "../Content/Images/vote_circle_small.png";
    } else if (votebutton.classList.contains('checked')) {
        votebutton.src = "../Content/Images/check_mark_small.png";
    }
}