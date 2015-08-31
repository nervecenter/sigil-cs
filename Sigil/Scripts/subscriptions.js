function subscribe(orgurl) {
    $.ajax({
        type: "POST",
        url: '/' + orgurl + '/subscribe/',
        success: function () {

        },
        error: function (ts) {
            //alert('Could not vote up.');
            alert(ts.responseText);
        }
    });
}

function unsubscribe(orgurl) {
    $.ajax({
        type: "POST",
        url: '/' + orgurl + '/unsubscribe/',
        success: function () {
        },
        error: function (ts) {
            //alert('Could not unvote up.');
            alert(ts.responseText);
        }
    });
}


function set_sub(subbutton, orgURL) {
    if (subbutton.classList.contains('unsubscribed')) {
        subscribe(orgURL);
        votebutton.classList.remove('unsubscribed');
        votebutton.classList.add('subscribed');
        votebutton.innerHTML = "Unsubscribe";
    } else if (subbutton.classList.contains('subscribed')) {
        unsubscribe(orgURL)
        votebutton.classList.remove('subscribed');
        votebutton.classList.add('unsubscribed');
        votebutton.innerHTML = "Subscribe";
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