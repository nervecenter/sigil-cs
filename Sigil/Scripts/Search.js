
$('#searchBox').autocomplete({
    source: 'search/',
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
    source: 'search/',
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