A Single-Page Application to Inquire Real-Time Flight Prices
==============================================================
A Web App implementing web services to provide user with ability to inquire for real-time flight prices.

Skyscanner - API limitation
==============================
Rapidapi-Skyscanner most often does not return inbound flights price quotes for a round trip. This can be verified by running the same query using https://rapidapi.com/skyscanner/api/skyscanner-flight-search.

Rapidapi-Skyscanner will not return a link to book the quote deal. 

This project will redirect the user to either Kayak or Skyscanner website by clicking on the “View More Deals” button, but don't expected to find the same deals as this is a real-time search event.

Azure Live
===========
http://realtimeflightprices.azurewebsites.net/

Prep Work
===============
1. Get YOUR API key at https://rapidapi.com/skyscanner/api/skyscanner-flight-search
2. Create the following external file `C:\secrets\rapidApiSettingSecrets.config` file.
For example, in the following markup, the file rapidApiSettingSecrets.config contains all the app secrets:
```
<appSettings>
  <!-- skyscanner -->
  <add key="x-rapidapi-host" value="skyscanner-skyscanner-flight-search-v1.p.rapidapi.com" />
  <add key="x-rapidapi-key" value="YOUR API KEY" />
</appSettings>
```

Getting Started
================
From the terminal, run: `git clone https://github.com/Windesson/RealTimeFlightPrices.git`

This will give you a directory named `RealTimeFlightPrices` with the source code for the application and all of the necessary tools.

**Deploy ASP.Net Website in IIS on Localhost**
1. Using  Visual Studio IDE 2017 File > Open > Project/Solution
2. Select the solution file in the RealTimeFlightPrices folder
3. Build the project and Launch the application using IIS Express(Firefox).

Once the application has loaded, a new Firefox browser tab will be launched to the home page, default IIS `http://localhost:24635/`. (This may be different in your environment)

What is the expected input
===========================
User will manually enter the airport's IATA code or city's name information or select an option from the auto-complete drop-down list in the 'FROM'/'TO' text box.\ 
User will select the depart and return dates.\
User will then click search.

**On Auto-complete**: shows the top 5 matching airports where the user input starts with the airports IATA code or city name. 

**On Search**: Retrieves flight from both Rapidapi-Skyscanner and Kayak website. Since scraping website is not ideal or reliable for computer-to-computer communication, search at least twice will return better results.   

Note: For a production version of similar App, the user should be limited to select airports from the drop-down list. 
This is to avoid unnecessary search for invalid airport or city code. 

What is the expected output
===========================
If flights found... each flight response will contain the `search-source`, `carrier`, `source-destination`, `number of stops`, `flight-price`, `link` to view more deals.

**Link to view deals**: This will take the user to either Kayak or Skyscanner to browse for more relevant flight deals.

Note: 
There might be cases where flight results might seem duplicated in the home page, but it actually means that the flight service provider might have similar flights departing on different time.
If no flights are found, the user will see a message in the home page indicating "no flights found"
If the user browser is not support, a message will be displayed. "Application does not currently support this browser. Please use latest firefox or chrome."

Images
==========================

![Alt text](README/twoway.jpg?raw=true "two-way flight")

![Alt text](README/oneway.jpg?raw=true "one-way flight")

![Alt text](README/noresult.jpg?raw=true "non flight result")

![Alt text](README/unsupported.jpg?raw=true "unsupported Browser")

Application Log
===============================
Navigate to `<project root>\Flightscraper.Spa\Logs`

Web Routes
==========
`api/airport/{query}` to query for airport information.\
`api/flight/{originPlace}/{destinationPlace}/{outboundDate}` to search one-way flight prices.\
`api/flight/{originPlace}/{destinationPlace}/{outboundDate}/{inboundDate}` to search two-way flight prices.
