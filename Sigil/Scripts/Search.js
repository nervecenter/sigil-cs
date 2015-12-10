
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

/*
bindWithDelay jQuery plugin
Author: Brian Grinstead
MIT license: http://www.opensource.org/licenses/mit-license.php
http://github.com/bgrins/bindWithDelay
http://briangrinstead.com/files/bindWithDelay
Usage:
    See http://api.jquery.com/bind/
    .bindWithDelay( eventType, [ eventData ], handler(eventObject), timeout, throttle )
Examples:
    $("#foo").bindWithDelay("click", function(e) { }, 100);
    $(window).bindWithDelay("resize", { optional: "eventData" }, callback, 1000);
    $(window).bindWithDelay("resize", callback, 1000, true);
*/

(function ($) {
    $.fn.bindWithDelay = function (type, data, fn, timeout, throttle) {
        if ($.isFunction(data)) {
            throttle = timeout;
            timeout = fn;
            fn = data;
            data = undefined;
        }
        // Allow delayed function to be removed with fn in unbind function
        fn.guid = fn.guid || ($.guid && $.guid++);
        // Bind each separately so that each element has its own delay
        return this.each(function () {
            var wait = null;

            function cb() {
                var e = $.extend(true, {}, arguments[0]);
                var ctx = this;
                var throttler = function () {
                    wait = null;
                    fn.apply(ctx, [e]);
                };

                if (!throttle) { clearTimeout(wait); wait = null; }
                if (!wait) { wait = setTimeout(throttler, timeout); }
            }
            cb.guid = fn.guid;
            $(this).bind(type, data, cb);
        });
    };
})(jQuery);

//$('#issues-by-org-search').keyup(function () {
$('#issues-by-org-search').bindWithDelay("keyup", function () {
    var searchQuery = { id: $(this).data('orgid'), term: $(this).val() };
    $.ajax({
        url: 'https://localhost:44301/searchissuesbyorg/',
        type: 'POST',
        data: JSON.stringify(searchQuery),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: false,
        success: function (data) {
            $("#issues").html("");
            $("#issues").append(data);
        },
        error: function () {
            alert('Search didn\'t work.');
        }
    });
}, 300, true);

$('#issues-by-product-search').bindWithDelay("keyup", function () {
    var searchQuery = { id: $(this).data('orgid'), term: $(this).val() };
    $.ajax({
        url: 'https://localhost:44301/searchissuesbyproduct/',
        type: 'POST',
        data: JSON.stringify(searchQuery),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        async: false,
        success: function (data) {
            $("#issues").html("");
            $("#issues").append(data);
        },
        error: function () {
            alert('Search didn\'t work.');
        }
    });
}, 300, true);

var issuePartialInner = function (partialVM) {
    return "";
}

$('#AdminProductSearch').autocomplete({
    source: 'https://localhost:44301/search/AdminProductSearch/'
})

$('#UserSearch').autocomplete({
    source: 'https://localhost:44301/search/AdminUserSearch/',
});

