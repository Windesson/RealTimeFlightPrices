// Autocomplete to show airports on text box 'FROM' 'TO'
ko.bindingHandlers.ko_autocomplete = {
    init: function (element, params) {
        const settings = params(); //{ source: fetchAirportFromTheApi, selected: airportTextFieldToBind }"
        const airports = settings.source;
        const selectedAirport = settings.selected;

        const updateElementValueWithAirportCode = function (event, ui) {

            // Stop the default behavior
            event.preventDefault();

            // Update our SelectedOption observable
            if (ui.item !== null && typeof ui.item !== "undefined") {
                // Update the value of the html element with the label
                // of the activated option in the list (ui.item)
                $(element).val(ui.item.label);

                // ui.item - id|label|...
                selectedAirport(ui.item);
            } else {
                //user enter airport/city not in the dropdown autocomplete
                selectedAirport({ airport: $(element)[0].value });
            } 
        };

        // Event handler to update selected airport
        $(element).autocomplete({
            source: airports,
            select: function (event, ui) {
                updateElementValueWithAirportCode(event, ui);
            },
            focus: function (event, ui) {
                updateElementValueWithAirportCode(event, ui);
            },
            change: function (event, ui) {
                updateElementValueWithAirportCode(event, ui);
            }
    });
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
            self.airport = data.IATA;
        }

        const ViewModel = function () {
            var self = this;

            // View-model observables
            self.loadingMessage = ko.observable();
            self.flightDepartDate = ko.observable();
            self.flightReturnDate = ko.observable();

            self.searchResults = ko.observableArray();
            self.errors = ko.observableArray();
            self.flightOriginOption = ko.observable();
            self.flightDestinationOption = ko.observable();


            // Adds a JSON array of flights to the view model.
            function addFlights(data) {
                const mapped = ko.utils.arrayMap(data, function (item) {
                    return new Flight(item);
                });
                self.searchResults(mapped);
            }

            // mapp a JSON array of airports to the view model.
            function mapAirportsToResponse(data, response) {
                const mapped = data.map(airport => new Airport(airport));

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
                            self.errors(errors);
                            console.log(errors);
                        }).catch( 
                            self.errors(defaultMessage)
                        );
                        return;
                    }
                    catch(error) {
                        console.log(error);
                    }
                }
                self.errors(defaultMessage);
            }

            // Event Listener search for flights 
            self.search = function () {
                const originPlace = self.flightOriginOption(); //require field
                const destinationPlace = self.flightDestinationOption(); //require field
                const depart = self.flightDepartDate(); //require field

                try {
                    if (!originPlace) {
                        alert("Please enter 'From' airport.");
                        return;
                    }
                    if (!destinationPlace) {
                        alert("Please enter 'To' airport.");
                        return;
                    }
                    if (!depart) {
                        alert("Please enter 'Depart'");
                        return;
                    }

                    self.errors(ko.utils.arrayMap()); // Clear the errors
                    self.searchResults(ko.utils.arrayMap()); //Reset results
                    self.loadingMessage("searching...");

                    self.getFlights(originPlace.airport, destinationPlace.airport, depart, self.flightReturnDate())
                        .then(_ => {
                            if (self.searchResults().length === 0) {
                                self.loadingMessage("no flights found.");
                            } else {
                                self.loadingMessage("");
                            }
                        });

                } catch (error) {
                    console.log(error);
                    self.loadingMessage("Oops, something went wrong.");
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

            // Fetch a list of airports from the server
            self.getAirports = function (request, response) {
                const text = request.term;
                app.service.Airport(text).then(data => mapAirportsToResponse(data, response));
            };

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
