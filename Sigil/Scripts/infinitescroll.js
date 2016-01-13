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

    // If we got a full batch of issues, add them and add the listener again
    //add issues
    $(window).scroll(scrollListener);

    // If we didn't get a full batch, put a rock at the bottom and don't add the listener
    //add issues
    //add rock
}