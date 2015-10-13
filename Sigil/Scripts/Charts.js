<script type="text/javascript" src="https://www.google.com/jsapi"></script>
<script type="text/javascript">
     
     google.load("visualization", "1", {packages:["corechart"]});    
google.setOnLoadCallback(drawChart); 
     
       
function drawChart()
{
    var humd_data = new google.visualization.arrayToDataTable({{hdata|safe}});

    var hchart = new google.visualization.LineChart(document.getElementById('hchart_div'));
    hchart.draw(humd_data);
}
	  
</script>