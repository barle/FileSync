﻿document.onload = _onload();

function _onload() {
    var canvas = document.getElementById("faceCanvas");
    var ctx = canvas.getContext("2d");

    //head;
    ctx.shadowColor ="rgba(0,0,0,0)";
    ctx.strokeStyle ="rgba(0,0,0,1)";
    ctx.lineWidth = 1;
    ctx.lineCap = "butt";
    ctx.lineJoin = "miter";
    ctx.beginPath();
    ctx.moveTo(98,16);
    ctx.bezierCurveTo(144,16,182,53,182,99);
    ctx.bezierCurveTo(182,145,144,182,98,182);
    ctx.bezierCurveTo(51,182,13,145,13,99);
    ctx.bezierCurveTo(13,53,51,16,98,16);
    ctx.closePath();
    ctx.stroke();
    ctx.shadowOffsetX = 15;
    ctx.shadowOffsetY = 15;
    ctx.shadowBlur = 0;
    ctx.shadowColor = "rgba(0,0,0,0)";
    ctx.fillStyle = "rgba(204,255,2,1)";
    ctx.fill();

    //leftEye;
    ctx.shadowColor ="rgba(0,0,0,0)";
    ctx.strokeStyle ="rgba(0,0,0,1)";
    ctx.lineWidth = 1;
    ctx.lineCap = "butt";
    ctx.lineJoin = "miter";
    ctx.beginPath();
    ctx.moveTo(67,51);
    ctx.bezierCurveTo(77,51,85,57,85,64);
    ctx.bezierCurveTo(85,71,77,77,67,77);
    ctx.bezierCurveTo(56,77,48,71,48,64);
    ctx.bezierCurveTo(48,57,56,51,67,51);
    ctx.closePath();
    ctx.stroke();
    ctx.shadowOffsetX = 15;
    ctx.shadowOffsetY = 15;
    ctx.shadowBlur = 0;
    ctx.shadowColor = "rgba(0,0,0,0)";
    ctx.fillStyle = "rgba(255,255,255,1)";
    ctx.fill();

    //rightEye;
    ctx.shadowColor ="rgba(0,0,0,0)";
    ctx.strokeStyle ="rgba(0,0,0,1)";
    ctx.lineWidth = 1;
    ctx.lineCap = "butt";
    ctx.lineJoin = "miter";
    ctx.beginPath();
    ctx.moveTo(133,53);
    ctx.bezierCurveTo(143,53,151,59,151,66);
    ctx.bezierCurveTo(151,73,143,79,133,79);
    ctx.bezierCurveTo(122,79,114,73,114,66);
    ctx.bezierCurveTo(114,59,122,53,133,53);
    ctx.closePath();
    ctx.stroke();
    ctx.shadowOffsetX = 15;
    ctx.shadowOffsetY = 15;
    ctx.shadowBlur = 0;
    ctx.shadowColor = "rgba(0,0,0,0)";
    ctx.fillStyle = "rgba(255,255,255,1)";
    ctx.fill();

    //Nose
    ctx.shadowColor ="rgba(0,0,0,0)";
    ctx.strokeStyle ="rgba(0,0,0,1)";
    ctx.lineWidth = 1;
    ctx.lineCap = "butt";
    ctx.lineJoin = "miter";
    ctx.beginPath();
    ctx.moveTo(100,85);
    ctx.lineTo(86,103);
    ctx.stroke();

    ctx.shadowColor ="rgba(0,0,0,0)";
    ctx.strokeStyle ="rgba(0,0,0,1)";
    ctx.lineWidth = 1;
    ctx.lineCap = "butt";
    ctx.lineJoin = "miter";
    ctx.beginPath();
    ctx.moveTo(86,101);
    ctx.lineTo(97,104);
    ctx.stroke();

    //Mouth;
    ctx.shadowColor ="rgba(0,0,0,0)";
    ctx.strokeStyle ="rgba(0,0,0,1)";
    ctx.lineWidth = 1;
    ctx.lineCap = "butt";
    ctx.lineJoin = "miter";
    ctx.beginPath();
    ctx.moveTo(50,119);
    ctx.lineTo(50,119);
    ctx.lineTo(50,119);
    ctx.bezierCurveTo(98,119,137,121,137,124);
    ctx.bezierCurveTo(137,126,107,129,65,129);
    ctx.lineTo(50,119);
    ctx.closePath();
    ctx.stroke();
    ctx.shadowOffsetX = 15;
    ctx.shadowOffsetY = 15;
    ctx.shadowBlur = 0;
    ctx.shadowColor = "rgba(0,0,0,0)";
    ctx.fillStyle = "rgba(255,255,255,1)";
    ctx.fill();
}