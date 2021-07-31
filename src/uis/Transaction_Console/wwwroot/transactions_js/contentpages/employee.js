function Show_Employee_Page() {

	alert("Show Employee Page Clicked");

$.get("./contentpages/employee.html", function(data, status){
    document.getElementById("ContentDiv").innerHTML = data;
    //alert("Data: " + data );

  });




}