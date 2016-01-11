$("#product-select").hide();

$("#new-feedback-button").click(function () {
    var $issuesearchpostform = $("#issue-search-post-form");
    $(this).remove();
    var $text = $("<textarea/>")
                    .attr("id", "issues-by-product-search")
                    .attr("type", "text")
                    .attr("name", "text")
                    .attr("placeholder", "Talk about your feedback in-depth")
                    .attr("style", "height:100px;min-height:100px;max-width:100%;min-width:100%;")
                    .addClass("form-control");
    var $submit = $("<button/>")
                    .attr("type", "submit")
                    .attr("class", "btn btn-primary pull-right")
                    .attr("style", "padding:4px 9px;")
                    .text("Post new feedback");
    $issuesearchpostform.append($("<div/>").addClass("form-group").append($text));
    $issuesearchpostform.append($("<div/>").addClass("form-group").append($submit));
});