function voteup(issueid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/voteup/' + issueid + '/' + userhandle,
        success: function () {
            // Get the element
            var elementID = "vote_" + issueid;
            var thisElement = document.getElementById(elementID);
            // Change its picture to voted on, function to unvoteup()
            thisElement.src = '/Content/Images/voteup-clicked.png';
            thisElement.onclick = new Function("unvoteup('" + issueid + "', '" + userhandle + "')");
            // Update the count displayed beneath it
            var voteCount = parseInt(document.getElementById("votecount_" + issueid).innerHTML, 10);
            document.getElementById("votecount_" + issueid).innerHTML = voteCount + 1;
            //alert('Voted up');
        },
        error: function () {
            alert('Could not vote up.');
        }
    });
}

function unvoteup(issueid, userhandle) {
    $.ajax({
        type: "POST",
        url: '/unvoteup/' + issueid + '/' + userhandle,
        success: function () {
            // Get the element
            var elementID = "vote_" + issueid;
            var thisElement = document.getElementById(elementID);
            // Change its picture to not voted on, function to voteup()
            thisElement.src = '/Content/Images/voteup.png';
            thisElement.onclick = new Function("voteup('" + issueid + "', '" + userhandle + "')");
            // Update the count displayed beneath it
            var voteCount = parseInt(document.getElementById("votecount_" + issueid).innerHTML, 10);
            document.getElementById("votecount_" + issueid).innerHTML = voteCount - 1;
            //alert('Unvoted up');
        },
        error: function () {
            alert('Could not unvote up.');
        }
    });
}