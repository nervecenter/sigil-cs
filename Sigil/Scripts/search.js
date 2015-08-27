$(document).on(function () {
    $('searchBox').live("focus", "searchBox" ,autocomplete({
        source: '/Search/SearchDB'
    })
    )});
