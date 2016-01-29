<<<<<<< HEAD
﻿function shownotifications() {
=======
﻿
function shownotifications() {
>>>>>>> Dom_Refactor
    $("#header-user-icon").off("click").click(hidenotifications);

    //<div id='NoteParent' class='panel-body'></div>
    var $noteparent = $("<div>")
        .attr("id", "NoteParent")
        .addClass("panel-body");

    // <img class='callout' src='/Content/Images/callout.png' />
    var $callout = $("<img>")
        .addClass("callout")
        .attr("src", "/Content/Images/callout.png");

    //<div class='panel panel-default notifications-panel'></div>
    var $panel = $("<div>")
        .addClass("panel")
        .addClass("panel-default")
        .addClass("notifications-panel")
        .append($noteparent);

    var $shade = $("<div>")
        .attr("id", "notifications-shade")
        .append($callout)
        .append($panel);

    $("#navbar-header").append($shade);

    $.get("/check_notes",
        function (data) {
            //alert("Data length: " + data.length);
            if (data.response == "none") {
                var $nonotes = $("<h5>").attr("style", "text-align:center;").html("No notifications. You're all caught up. :)")
                $("#NoteParent").append($nonotes);
            } else {
                $.each(data, function (index, Note) {
                    var $img = $("<img>") //img container for the user icon
                        .addClass("media-object")
                        .addClass("notification-icon")
                        .attr("src", Note.icon);
                    var $imganchor = $("<a>")   //icon for from user -- need to change return value of from userid to a link to their icon instead
                        .addClass("pull-left")
                        .append($img);
                    var $notificationtext = $("<a>")    //link container that allows clicking the notification to take you to where it is
                        .attr("href", Note.url)
                        .html(Note.title);
                    var $deleteimg = $("<span>").addClass("glyphicon glyphicon-remove-sign");
                
                    var $deleteLink = $("<a>").click(Note.id,DeleteNotification).append($deleteimg);

                    //var mediabodySpan = document.createElement("span"); //The span that includes the issue title where the notification took place
                    var $mediabody = $("<div>") //div container that includes the title and link
                        .addClass("media-body")
                        .append($notificationtext).append($deleteLink);
                    var $media = $("<div>")     //parent div for entire notification
                        .addClass("media")
                        .append($imganchor)
                        .append($mediabody);
                    $("#NoteParent").append($media);
                });
            }
            //should probably include a check on data to see if there is stuff there and if not just have a message that shows no notifications
        }
    );

}

function DeleteNotification(noteid) {
    $.post("/delete_notification/" + noteid,
        function () { });
}

//"<img class='callout' src='/Content/Images/callout.png' /><div class='panel panel-default notifications-panel'><div id='NoteParent' class='panel-body'><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>Why hello durr, dis is a notification.</span></div></div><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>This is a second notification with a longer text body because in the Ministry of Feedback there really is just too much room and space to use and we need to fill it with useless shit.</span></div></div></div></div>"

function hidenotifications() {
    $("#header-user-icon").off("click").click(function () { shownotifications() });
    $("#notifications-shade").remove();
}

$("#header-user-icon").click(shownotifications);