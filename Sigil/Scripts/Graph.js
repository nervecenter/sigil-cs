﻿// To use google charts you must load 
// <script type="text/javascript" src="https://www.google.com/jsapi"></script>
// as well as this file on the page you want a chart to appear


google.load('visualization', '1', { 'packages': ['corechart'] });

//default data load function

$(document).ready(function () {

        var orgURL = get_org_url();
        $.ajax({
            url: "/default_graph/" + orgURL,
            success: function (org_data) {
                //alert("Graphs are cool");
                var chart_data = new google.visualization.DataTable();
                chart_data.addColumn({ type: 'date', id: "Date" });
                chart_data.addColumn({ type: 'number', id: "Count" });

                $.each(org_data, function (index, data) {
                    chart_data.addRow([new Date(data.viewDate), data.viewCount]);
                });

                var default_options = {
                    title: 'Org Data',
                    hAxis: { title: 'Past Week', },
                    vAxis: { title: 'Number of Users' }
                };
                
                var default_chart = new google.visualization.LineChart(document.getElementById('org_chart_div'));
                default_chart.draw(chart_data, default_options);
            }
        });
});

$(function () {
    //would be cool to have these auto popluate to the last week duration
    $("#datepickerStart").datepicker();
    $("#datepickerStop").datepicker();
});

function Custom_Chart() {
    var dataOption = document.getElementById('selected_data').value;
    var start_date = jsDateToCSharp($("#datepickerStart").datepicker("getDate"));
    var stop_date = jsDateToCSharp($("#datepickerStop").datepicker("getDate"));

    var orgURL = get_org_url();
    var URL = "/custom_graph/" + orgURL + "/" + dataOption+ "/" + start_date + "/" + stop_date;
    $.ajax({
        url: URL,
        success: function (org_data) {
            var chart_data = new google.visualization.DataTable();
            chart_data.addColumn({ type: 'date', id: "Date" });
            chart_data.addColumn({ type: 'number', id: "Count" });

            $.each(org_data, function (index, data) {
                chart_data.addRow([new Date(data.viewDate), data.viewCount]);
            });

            var default_options = {
                title: dataOption + "Data",
                hAxis: { title: 'Past Week', },
                vAxis: { title: 'Number of Users' }
            };

           // var chartP = document.getElementById('chart_panel');
            //chartP.removeChild(document.getElementById('org_chart_div'));

            var default_chart = new google.visualization.LineChart(document.getElementById('org_chart_div'));//(document.createElement('div').setAttribute("id",'org_chart_div'));
            
            default_chart.draw(chart_data, default_options);
        }
    });

}


function jsDateToCSharp(date)
{
    var time = date.getTime();

    return time;
}

function get_org_url() {
    var url = window.location.href;

    var orgURL = url.split("/")[3];
   
    //need to add a callback to db to verify this is actually an org
    return orgURL;
};