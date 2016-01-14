function scrollListener() {
    var $window = $(window);
    if ($window.scrollTop() + $window.height() > $(document).height() - 500) {
        // If we're 500 pixels above the bottom, grab issues
        $window.off("scroll");
        grabIssues();
    }
}

$(window).scroll(scrollListener);

function grabIssues(/*might need parameters*/) {
    // Query server for issues

    $("#issues").append(/*html we got back*/);

    if (/*full batch*/) {
        // If we got a full batch of issues, add them and add the listener again
        $(window).scroll(scrollListener);
    } else {
        // If we didn't get a full batch, put a rock at the bottom and don't add the listener
        $("#issues").append($("<img/>").attr("src", "rock.png"));
    }
}