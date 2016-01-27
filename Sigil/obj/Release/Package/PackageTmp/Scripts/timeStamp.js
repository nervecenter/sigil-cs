function TimeSince(datePosted) {
    var timePosted = new Date(Date.parse(datePosted));

    var timeDiff = Math.abs(new Date() - timePosted);

    if(timeDiff >= Math.abs(new Date() - new Date(new Date().getFullYear-1)))
    {

    }
    
}