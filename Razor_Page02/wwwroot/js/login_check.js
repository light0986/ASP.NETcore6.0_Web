var Acc = localStorage.getItem("KeyCode");

function load_storge() {
    var ls01 = localStorage.getItem("KeyCode");
    document.getElementById("container").innerHTML = ls01;
}