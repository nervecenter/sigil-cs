function voteup(issueid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/voteup/' + issueid + '/',
        success: function () {
            /*// Get the element
            var elementID = "vote_" + issueid;
            var thisElement = document.getElementById(elementID);
            // Change its picture to voted on, function to unvoteup()
            thisElement.src = '/Content/Images/voteup-clicked.png';
            thisElement.onclick = new Function("unvoteup('" + issueid + "', '" + userhandle + "')");
            // Update the count displayed beneath it
            var voteCount = parseInt(document.getElementById("votecount_" + issueid).innerHTML, 10);
            document.getElementById("votecount_" + issueid).innerHTML = voteCount + 1;
            //alert('Voted up');*/
            return true;
        },
        error: function (ts) {
            //alert('Could not vote up.');
            alert(ts.responseText);
            return false;
        }
    });
}

function unvoteup(issueid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/unvoteup/' + issueid + '/',
        success: function () {
            /*// Get the element
            var elementID = "vote_" + issueid;
            var thisElement = document.getElementById(elementID);
            // Change its picture to not voted on, function to voteup()
            thisElement.src = '/Content/Images/voteup.png';
            thisElement.onclick = new Function("voteup('" + issueid + "', '" + userhandle + "')");
            // Update the count displayed beneath it
            var voteCount = parseInt(document.getElementById("votecount_" + issueid).innerHTML, 10);
            document.getElementById("votecount_" + issueid).innerHTML = voteCount - 1;
            //alert('Unvoted up');*/
            return true;
        },
        error: function (ts) {
            alert('Could not unvote up.');
            alert(ts.responseText);
            return false;
        }
    });
}


function vote(votebutton, issueID, userhandle) {
    var count = document.getElementById('count-' + issueID);
    if (votebutton.classList.contains('unchecked') && voteup(issueID, userhandle)) {
        //voteup(issueID, userhandle);
        votebutton.classList.remove('unchecked');
        votebutton.classList.add('checked');
        votebutton.src = "../Content/Images/check_mark_hover_small.png";
        count.innerHTML = parseInt(count.innerHTML, 10) + 1;
    } else if (votebutton.classList.contains('checked') && unvoteup(issueID, userhandle)) {
        //unvoteup(issueID, userhandle);
        votebutton.classList.remove('checked');
        votebutton.classList.add('unchecked');
        votebutton.src = "../Content/Images/vote_circle_hover_small.png";
        count.innerHTML = parseInt(count.innerHTML, 10) - 1;
    }
}

function votehover(votebutton) {
    if (votebutton.classList.contains('unchecked')) {
        votebutton.src = "../Content/Images/vote_circle_hover_small.png";
    } else if (votebutton.classList.contains('checked')) {
        votebutton.src = "../Content/Images/check_mark_hover_small.png";
    }
}

function voteunhover(votebutton) {
    if (votebutton.classList.contains('unchecked')) {
        votebutton.src = "../Content/Images/vote_circle_small.png";
    } else if (votebutton.classList.contains('checked')) {
        votebutton.src = "../Content/Images/check_mark_small.png";
    }
}