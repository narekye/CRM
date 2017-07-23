$(document).ready(function () {
    $("#showid").fadeOut();
    var movementStrength = 50;
    var height = movementStrength / $(window).height();
    var width = movementStrength / $(window).width();
    $("#top-image").mousemove(function (e) {
        var pageX = e.pageX - ($(window).width() / 2);
        var pageY = e.pageY - ($(window).height() / 2);
        var newvalueX = width * pageX * -1 - 30;
        var newvalueY = height * pageY * -1 - 50;
        $('#top-image').css("background-position", newvalueX + "px     " + newvalueY + "px");
    });
    for (var i = 0; i <100; i++) {
        $("#block1").fadeToggle(3000);
        $("#block2").fadeToggle(3000);
        $("#block3").fadeToggle(3000);
        $("#block4").fadeToggle(3000);
        $("#bet").fadeToggle(3000);
    }
});