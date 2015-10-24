function shownotifications() {
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
                var note = document.createElement("div"); //parent div for entire notification
                note.className = "media";
                var Nicon = document.createElement("a"); //icon for from user -- need to change return value of from userid to a link to their icon instead
                Nicon.className = 'pull-left';
                var NImg = document.createElement("img"); //img container for the user icon
                NImg.setAttribute("src", '/Images/User/nervecenter_100.png');
                NImg.setAttribute("class", 'notification-icon');
                Nicon.appendChild(NImg);
                var nTitle = document.createElement("div"); //div container that includes the title and link
                nTitle.className = 'media-body';
                var nURL = document.createElement("a"); //link container that allows clicking the notification to take you to where it is
                nURL.setAttribute("href", Note.url);
                //nTitle.setAttribute('href', Note.url);
                var nTitleSpan = document.createElement("span"); //The span that includes the issue title where the notification took place
                nTitleSpan.innerHTML = Note.title;
                nURL.appendChild(nTitleSpan);
                nTitle.appendChild(nURL);
                Nicon.appendChild(nTitle);
                document.getElementById("NoteParent").appendChild(Nicon); 
            });
            //should probably include a check on data to see if there is stuff there and if not just have a message that shows no notifications
        }
    });
       
}



//"<img class='callout' src='/Content/Images/callout.png' /><div class='panel panel-default notifications-panel'><div id='NoteParent' class='panel-body'><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>Why hello durr, dis is a notification.</span></div></div><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>This is a second notification with a longer text body because in the Ministry of Feedback there really is just too much room and space to use and we need to fill it with useless shit.</span></div></div></div></div>"

function hidenotifications() {
    var icon = document.getElementById("header-user-icon");
    icon.onclick = function () { shownotifications() }

    document.getElementById("notifications-shade").remove();
}