ko.bindingHandlers.ko_autocomplete = {
    init: function (element, params) {
        $(element).autocomplete(params());
    },
    update: function (element, params) {
        $(element).autocomplete("option", "source", params().source);
    }
};


(function () {

        // Creates an observable version of the flights result model.
        // Initialize with a JSON object fetched from the server.
        function Flight(data) {
            const self = this;
            data = data || {};

            // Data from model
            self.ID = data.Id;
            self.QuoteSource = data.QuoteSource;
            self.TripTotalPrice = data.TripTotalPrice;
            self.BookingLink = data.BookingLink;
            self.FlightItineraries = data.FlightItineraries;
         }

        function Airport(data) {
            const self = this;
            data = data || {};

            self.id = data.AirportId;
            self.label = data.Label;
            self.iata = data.IATA;
        }

        const ViewModel = function () {
            var self = this;

            // View-model observables
            self.loadingMessage = ko.observable();
            self.flightDepartDate = ko.observable();
            self.flightReturnDate = ko.observable();

            self.searchResults = ko.observableArray();
            self.error = ko.observableArray();
            self.flightOriginOption = ko.observable();
            self.flightDestinationOption = ko.observable();


            // Adds a JSON array of flights to the view model.
            function addFlights(data) {
                const mapped = ko.utils.arrayMap(data, function (item) {
                    return new Flight(item);
                });
                self.searchResults(mapped);
            }

            function addAirports(data, response) {
                const mapped = ko.utils.arrayMap(data, function (item) {
                    return new Airport(item);
                });

                response(mapped);
            }

            // Callback for error responses from the server.
            function onError(response) {
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
                const originPlace = self.flightOriginOption(); //require field
                const destinationPlace = self.flightDestinationOption(); //require field
                const depart = self.flightDepartDate(); //require field

                try {
                    if (!originPlace || $("#flight-origin")[0].value !== originPlace.label) {
                        alert("Please enter a valid 'From' airport.");
                        return;
                    }
                    if (!destinationPlace || $("#flight-destination")[0].value !== destinationPlace.label) {
                        alert("Please enter a valid 'To' airport.");
                        return;
                    }
                    if (!depart) {
                        alert("Please enter 'Depart'");
                        return;
                    }

                    self.error(ko.utils.arrayMap()); // Clear the error
                    self.searchResults(ko.utils.arrayMap()); //Reset results
                    self.loadingMessage("searching...");

                    $("#search").prop("disabled", true);
                    self.getFlights(originPlace.iata, destinationPlace.iata, depart, self.flightReturnDate())
                        .then(_ => {
                            if (self.searchResults().length === 0) {
                                self.loadingMessage("no flights found.");
                            } else {
                                self.loadingMessage("");
                            }
                            $("#search").prop("disabled", false);
                        });

                } catch (error) {
                    console.log(error);
                    self.loadingMessage("Oops, something went wrong.");
                    $("#search").prop("disabled", false);
                } 
                 
            };

            // Fetches a list of flights for round-trip
            self.getFlights = function (fromCode, toCode, departDate, returnDate) {
                if (returnDate) {
                    return app.service.RoundTrip(fromCode, toCode, departDate, returnDate).then(addFlights, onError);
                } else {
                    return app.service.OneWayTrip(fromCode, toCode, departDate).then(addFlights, onError);
                }
            };

            self.selectFromAirport = function (event, ui) {
                self.flightOriginOption(ui.item);
            };

            self.selectToAirport = function (event, ui) {
                self.flightDestinationOption(ui.item);
            };

            self.getAirports = function(request, response) {
                const text = request.term;
                app.service.Airport(text).then(data => addAirports(data, response));
            }

            //display binding elements
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
