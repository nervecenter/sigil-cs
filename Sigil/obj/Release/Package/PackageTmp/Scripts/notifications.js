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
    $("#header-user-icon").click(function () { hidenotifications() });

    var $shade = $("<div>", { id: "notifications-shade" });
    $shade.append("<img class='callout' src='/Content/Images/callout.png' /><div class='panel panel-default notifications-panel'><div id='NoteParent' class='panel-body'></div></div>");
    $("#navbar-list").append($shade);

    $.ajax({
        url: "/check_notes",
        success: function (data) {
            //alert("Data length: " + data.length);
            $.each(data, function (index, Note) {
                var $media = $("<div>", { class: "media" }); //parent div for entire notification
                var $imganchor = $("<a>", { class: "pull-left" }); //icon for from user -- need to change return value of from userid to a link to their icon instead
                var $img = $("<img>", { class: "media-object notification-icon" }); //img container for the user icon
                $img.attr("src", Note.icon);
                $imganchor.append($img);
                var $mediabody = $("<div>", { class: "media-body" }); //div container that includes the title and link
                var $notificationtext = $("<a>"); //link container that allows clicking the notification to take you to where it is
                $notificationtext.attr("href", Note.url);
                //var mediabodySpan = document.createElement("span"); //The span that includes the issue title where the notification took place
                $notificationtext.html = Note.title;
                $mediabody.append($notificationtext);
                //nURL.appendChild(mediabodySpan);
                $media.append($imganchor);
                $media.append($mediabody);
                $("#NoteParent").append($media);
            });
            //should probably include a check on data to see if there is stuff there and if not just have a message that shows no notifications
        },
        error: function (e) {
            alert(e.Message);
        }
    });

}



//"<img class='callout' src='/Content/Images/callout.png' /><div class='panel panel-default notifications-panel'><div id='NoteParent' class='panel-body'><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>Why hello durr, dis is a notification.</span></div></div><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>This is a second notification with a longer text body because in the Ministry of Feedback there really is just too much room and space to use and we need to fill it with useless shit.</span></div></div></div></div>"

function hidenotifications() {
    $("#header-user-icon").click(function () { shownotifications() });
    $("#notifications-shade").remove();
}