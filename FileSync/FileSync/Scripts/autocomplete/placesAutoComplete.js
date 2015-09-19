document.onload = _onload();

function _onload() {
    $("#place").autocomplete({
        source: _loadPlaces,
        minLength: 2
    });
}

function _loadPlaces(request, response) {
    var promise = $.ajax('http://localhost:3771/Places/' + encodeURIComponent(request.term)).promise();
    promise.then(response);
}