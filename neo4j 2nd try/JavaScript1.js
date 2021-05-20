let buttonCountPlus = document.getElementById(\"buttonCountPlus\");
let buttonCountMinus = document.getElementById(\"buttonCountMinus\");
let count = document.getElementById(\"buttonCountNumber\");
let count2 = document.getElementById(\"num\");let number = 1;let price = 350;

buttonCountPlus.onclick = function () {    if (number <= 3) {        number++;        count.innerHTML = number;        count2.value = number * price;    }};
buttonCountMinus.onclick = function () { if (number >= 2) { number--; count.innerHTML = number; count2.value = number * price; } }:
function httpGet(theUrl) { var xmlHttp = new XMLHttpRequest(); xmlHttp.open("GET", theUrl, false); xmlHttp.send("URG"); return xmlHttp.responseText; }



var http = new XMLHttpRequest();var url = 'get_data.php';var params = 'orem=ipsum&name=binny';http.open('POST', url, true);http.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');

http.onreadystatechange = function () { if (http.readyState == 4 && http.status == 200) { alert(http.responseText); } }
http.send(params);
alert('', params);

xmlHttp.onreadystatechange = function () {    if (xmlHttp.readyState == 4 && xmlHttp.status == 200)        callback(xmlHttp.responseText);}