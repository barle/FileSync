document.onload = _onload();

function _onload() {
    var promise = $.ajax("http://localhost:3771/Statistics/GetFilesPerDays?days=10").promise();
    promise.then(_initGraph);
}

function _initGraph(data) {
    var dates = [];
    data.forEach(function (dayToCount) {
        dates.push(dayToCount.date);
    });
    var maxCount = 0;
    data.forEach(function (dayToCount) {
        if (dayToCount.count > maxCount)
            maxCount = dayToCount.count;
    })

    var vis = d3.select("#daysToFilesCount"),
    WIDTH = 450,
    HEIGHT = 250,
    MARGINS = {
        top: 20,
        right: 20,
        bottom: 20,
        left: 50
    },

    xScale = d3.scale.ordinal().domain(dates).rangePoints([MARGINS.left, WIDTH - MARGINS.right]),
    yScale = d3.scale.linear().range([HEIGHT - MARGINS.top, MARGINS.bottom]).domain([0, maxCount]);

    xAxis = d3.svg.axis()
        .scale(xScale),

    yAxis = d3.svg.axis()
        .scale(yScale);

    vis.append("svg:g")
        .attr("class", "axis")
        .attr("transform", "translate(0," + (HEIGHT - MARGINS.bottom) + ")")
        .call(xAxis);

    yAxis = d3.svg.axis()
        .scale(yScale)
        .orient("left");

    vis.append("svg:g")
        .attr("class", "axis")
        .attr("transform", "translate(" + (MARGINS.left) + ",0)")
        .call(yAxis);

    var lineGen = d3.svg.line()
        .x(function (d) {
            return xScale(d.date);
        })
        .y(function (d) {
            return yScale(d.count);
        });

    vis.append('svg:path')
      .attr('d', lineGen(data))
      .attr('stroke', 'green')
      .attr('stroke-width', 2)
      .attr('fill', 'none');

}




