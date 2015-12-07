
$('#searchBox').autocomplete({
    source: 'https://localhost:44301/search/',
    _renderItem: function( ul, item ) {
        return $( "<li>" )
          .attr( "data-value", item.label )
          .append( item.label )
          .appendTo( ul );
    },
    response: function( event, ui ) {},
    select: function (event, ui) {
        //alert(ui.item.label);
        window.location = ui.item.value;
        ui.item.value = ui.item.label;
    }
});


$('#issues-by-org-search').keyup(function () {
    //var searchText = $(this).val();
    //var orgID = $(this).data('orgid');
    var searchQuery = { SearchModel: { id: $(this).data('orgid'), term: $(this).val() } };
    $.ajax({
        url: 'https://localhost:44301/searchissuesbyorg/',
        type: 'POST',
        data: JSON.stringify(searchQuery),
        //data: searchQuery,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: false,
        success: function (searchResults) {
            $(".issue-panel-partial").remove();
            alert(searchResults);
        },
        error: function (ts) {
            alert(ts.responseText);
        }
        /*_renderItem: function (ul, item) {
            return $("<li>")
              .attr("data-value", item.label)
              .append(item.label)
              .appendTo(ul);
        },
        response: function (event, ui) { },
        select: function (event, ui) {
            //alert(ui.item.label);
            window.location = ui.item.value;
            ui.item.value = ui.item.label;
        }*/
    });
});

$('#AdminProductSearch').autocomplete({
    source: 'https://localhost:44301/search/AdminProductSearch/'
})

$('#UserSearch').autocomplete({
    source: 'https://localhost:44301/search/AdminUserSearch/',
});
