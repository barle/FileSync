﻿document.onload = _onload();

function _onload() {
    var promise = $.ajax("http://localhost:3771/Statistics/GetFoldersSizes").promise();
    promise.then(_initGraph);
}

function _initGraph(data) {
    var w = 450,
    h = 250,
    r = 125,
    color = d3.scale.category20c();

    var vis = d3.select("#filesSizePie")
        .append("svg:svg")
        .data([data])
            .attr("width", w)
            .attr("height", h)
        .append("svg:g")
            .attr("transform", "translate(" + r + "," + r + ")")

    var arc = d3.svg.arc()
        .outerRadius(r);

    var pie = d3.layout.pie()
        .value(function (d) { return d.size; });

    var arcs = vis.selectAll("g.slice")
        .data(pie)
        .enter()
            .append("svg:g")
                .attr("class", "slice");

    arcs.append("svg:path")
            .attr("fill", function (d, i) { return color(i); })
            .attr("d", arc);

    arcs.append("svg:text")
        .attr("transform", function (d) {
                d.innerRadius = 0;
                d.outerRadius = r;
                return "translate(" + arc.centroid(d) + ")";
        })
        .text(function (d, i) { return data[i].folderName; });

    arcs.append("svg:title")
        .text(function (d, i) { return data[i].folderName; });
}