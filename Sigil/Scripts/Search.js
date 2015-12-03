
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

$('#issuesByOrgBox').autocomplete({
    source: 'https://localhost:44301/issueSearch/',
    _renderItem: function (ul, item) {
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
    }
});

$('#AdminProductSearch').autocomplete({
    source: 'https://localhost:44301/search/AdminProductSearch/'
})

$('#UserSearch').autocomplete({
    source: 'https://localhost:44301/search/AdminUserSearch/',
});
