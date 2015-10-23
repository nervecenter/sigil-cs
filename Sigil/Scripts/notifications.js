function shownotifications() {
    var icon = document.getElementById("header-user-icon");
    icon.onclick = function () { hidenotifications() }

    var shade = document.createElement("div");
    shade.id = "notifications-shade";
    shade.innerHTML = "<img class='callout' src='/Content/Images/callout.png' /><div class='panel panel-default notifications-panel'><div class='panel-body'><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>Why hello durr, dis is a notification.</span></div></div><div class='media'><a href='#' class='pull-left'><img src='/Images/User/nervecenter_100.png' class='notification-icon' alt='Sample Image'></a><div class='media-body'><span>This is a second notification with a longer text body because in the Ministry of Feedback there really is just too much room and space to use and we need to fill it with useless shit.</span></div></div></div></div>";
    document.getElementById("navbar-list").appendChild(shade);
}

function hidenotifications() {
    var icon = document.getElementById("header-user-icon");
    icon.onclick = function () { shownotifications() }

    document.getElementById("notifications-shade").remove();
}