window.app = window.todoApp || {};
window.app.service = (function () {
    var flightApi = '/api/flight';
    // ------------------------------------------
    //  FETCH FUNCTIONS
    // ------------------------------------------

    var serviceUrls = {
        OneWayTrip: function (originCode, destinationCode, departDate) {
            return `${flightApi}/${originCode}/${destinationCode}/${departDate}`;
        },
        RoundTrip: function (originCode, destinationCode, departDate, returnDate) {
            return `${flightApi}/${originCode}/${destinationCode}/${departDate}/${returnDate}`;
        }
    };

    function fetchData(url) {
        return fetch(url)
            .then(checkStatus)
            .then(res => res.json());
    }

    // ------------------------------------------
    //  HELPER FUNCTIONS
    // ------------------------------------------
    function checkStatus(response) {
        if (response.ok) {
            return Promise.resolve(response);
        } else {
            return Promise.reject(response);
        }
    }

    return {
        OneWayTrip: function (fromCode, toCode, departDate) {
            return fetchData(serviceUrls.OneWayTrip(fromCode, toCode, departDate));
        },
        RoundTrip: function (fromCode, toCode, departDate, returnDate) {
            return fetchData(serviceUrls.RoundTrip(fromCode, toCode, departDate, returnDate));
        }
    };

})();