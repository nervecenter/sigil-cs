
var searchV = $('#Search').val();
$(function () {
    setTimeout(checkSearchChanged, 0.1);
});

function checkSearchChanged() {
    var curr_Value = $('#Search').val();
    if ((curr_Value) && curr_Value != searchV && curr_Value != '')
    {
        searchV = $('#Search').val();
        $('#submit').click();
    }
    else
    {
        setTimeout(checkSearchChanged, 0.1);
    }
}


function submitSearch(qVal) {
    $.ajax({
        type: "POST",
        url: '/search/' + qVal + '/',
        success: function () {

            //parse the search results
        }
    })
}