/*function shownotifications() {
    var icon = document.getElementById("header-user-icon");
    icon.onclick = function () { hidenotifications() }
    
    var shade = document.createElement("div");
    shade.id = "notifications-shade";
    shade.innerHTML = "<img class='callout' src='/Content/Images/callout.png' /><div class='panel panel-default notifications-panel'><div id='NoteParent' class='panel-body'></div></div>";
    document.getElementById("navbar-list").appendChild(shade);
    
    
    $.ajax({
        url: "/check_notes",
        success: function (data) {
            $.each(data, function (index, Note) {
                var media = document.createElement("div"); //parent div for entire notification
                media.classList.add("media");
                var imganchor = document.createElement("a"); //icon for from user -- need to change return value of from userid to a link to their icon instead
                imganchor.classList.add('pull-left');
                var img = document.createElement("img"); //img container for the user icon
                img.setAttribute("src", '/Images/User/nervecenter_100.png');
                img.classList.add('media-object');
                img.classList.add('notification-icon');
                imganchor.appendChild(img);
                var mediabody = document.createElement("div"); //div container that includes the title and link
                mediabody.classList.add('media-body');
                var notificationtext = document.createElement("a"); //link container that allows clicking the notification to take you to where it is
                notificationtext.setAttribute("href", Note.url);
                //mediabody.setAttribute('href', Note.url);
                //var mediabodySpan = document.createElement("span"); //The span that includes the issue title where the notification took place
                notificationtext.innerHTML = Note.title;
                mediabody.appendChild(notificationtext);
                //nURL.appendChild(mediabodySpan);
                media.appendChild(imganchor);
                media.appendChild(mediabody);
                document.getElementById("NoteParent").appendChild(media); 
            });
            //should probably include a check on data to see if there is stuff there and if not just have a message that shows no notifications
        }
    });
       
}*/

/*function hidenotifications() {
    var icon = document.getElementById("header-user-icon");
    icon.onclick = function () { shownotifications() }

    document.getElementById("notifications-shade").remove();
}*/

function shownotifications() {
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

    $("#navbar-list").append($shade);

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
                    //var mediabodySpan = document.createElement("span"); //The span that includes the issue title where the notification took place
                    var $mediabody = $("<div>") //div container that includes the title and link
                        .addClass("media-body")
                        .append($notificationtext);
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



//"<img class='callout' src='/Content/Images/callout.png' /><div class='panel panel-default notifications-panel'><div id='NoteParent' class='panel-body'><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>Why hello durr, dis is a notification.</span></div></div><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>This is a second notification with a longer text body because in the Ministry of Feedback there really is just too much room and space to use and we need to fill it with useless shit.</span></div></div></div></div>"

function hidenotifications() {
    $("#header-user-icon").off("click").click(function () { shownotifications() });
    $("#notifications-shade").remove();
}

$("#header-user-icon").click(shownotifications);