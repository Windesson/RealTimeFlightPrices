(function () {

        // Creates an observable version of the flights result model.
        // Initialize with a JSON object fetched from the server.
        function Flight(data) {
            var self = this;
            data = data || {};

            // Data from model
            self.ID = data.Id;
            self.QuoteSource = data.QuoteSource;
            self.TripTotalPrice = data.TripTotalPrice;
            self.BookingLink = data.BookingLink;
            self.FlightItineraries = data.FlightItineraries;
        }

        var ViewModel = function () {
            var self = this;

            // View-model observables
            self.loadingMessage = ko.observable();
            self.flightDepartDate = ko.observable("2021-01-03");
            self.flightReturnDate = ko.observable("2021-01-20");

            self.searchResults = ko.observableArray();
            self.error = ko.observableArray();
            self.flightOriginOption = ko.observable("MIA");
            self.flightDestinationOption = ko.observable("NYC");

            // Adds a JSON array of flights to the view model.
            function addFlights(data) {
                var mapped = ko.utils.arrayMap(data, function (item) {
                    return new Flight(item);
                });
                self.searchResults(mapped);
                if (mapped.length === 0) {
                    self.loadingMessage("no flights found.");
                } else {
                    self.loadingMessage("");
                }
            }

            // Callback for error responses from the server.
            function onError(response) {
                self.loadingMessage("no flights found.");
                const defaultMessage = [`Error: ${response.status} ${response.statusText}`];
                if (response.status === 400)
                {
                    try {
                        response.json().then(data => {
                            var errors = [];
                            var modelState = data.ModelState;
                            for (const property in modelState) {
                                if (modelState.hasOwnProperty(property)) {
                                    modelState[property].forEach(item => errors.push(item));
                                }
                            }
                            self.error(errors);
                            console.log(errors);
                        }).catch( 
                            self.error(defaultMessage)
                         );
                        return;
                    }
                    catch(error) {
                        //ignore
                    }
                }
                self.error(defaultMessage);
            }

            // Event Listener search for flights 
            self.search = function () {
                var from = self.flightOriginOption(); //require field
                var to = self.flightDestinationOption(); //require field
                var depart = self.flightDepartDate(); //require field
                if (from && to && depart) {
                    self.getFlights(from, to, depart, self.flightReturnDate());
                }
                else {
                    if (!from) alert("Please enter 'From'");
                    else if (!to) alert("Please enter 'To'");
                    else if (!depart) alert("Please enter 'Depart'");
                }
            };

            // Fetches a list of flights for round-trip
            self.getFlights = function (fromCode, toCode, departDate, returnDate) {
                self.error(ko.utils.arrayMap()); // Clear the error
                self.loadingMessage("searching...");
                self.searchResults(ko.utils.arrayMap()) ;
                if (returnDate) {
                    app.service.RoundTrip(fromCode, toCode, departDate, returnDate).then(addFlights, onError);
                } else {
                    app.service.OneWayTrip(fromCode, toCode, departDate).then(addFlights, onError);
                }

            };

            // Initialize the app by getting the trip from Miami to Sao Paulo.
            self.getFlights(self.flightOriginOption(), self.flightDestinationOption(), self.flightDepartDate(), self.flightReturnDate());

            //load complete.
            $("#knockoutBound").show();

        };

    try {

        ko.applyBindings(new ViewModel()); // This makes Knockout get to work

    } catch (err) {

        console.log(err);
        $("#main").append("<p>Application has not loaded correctly.</p>");
        $("#knockoutBound").hide();
    }

})();
