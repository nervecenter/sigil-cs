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
        votebutton.src = "../Content/Images/unsubscribe_hover.png";
    } else if (subbutton.classList.contains('subscribed')) {
        unsubscribe(orgURL)
        votebutton.classList.remove('subscribed');
        votebutton.classList.add('unsubscribed');
        votebutton.src = "../Content/Images/subscribe_hover.png";
    }
}

function subhover(votebutton) {
    if (votebutton.classList.contains('unsubscribed')) {
        votebutton.src = "../Content/Images/subscribe_hover.png";
    } else if (votebutton.classList.contains('subscribed')) {
        votebutton.src = "../Content/Images/unsubscribe_hover.png";
    }
}

function subunhover(votebutton) {
    if (votebutton.classList.contains('unsubscribed')) {
        votebutton.src = "../Content/Images/subscribe.png";
    } else if (votebutton.classList.contains('subscribed')) {
        votebutton.src = "../Content/Images/unsubscribe.png";
    }
}