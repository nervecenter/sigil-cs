

$(function () {
    $('#searchBox').autocomplete({
        source: '@Url.Action("Search","Home")'


    });
});


$(document).ready(
$('#searchBox').autocomplete({
    source: '@Url.Action("Search","Home")'
}));
