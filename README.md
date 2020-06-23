An ASP.NET Web Single-Page Applications to Inquire Real-Time Flights Prices
==============================================
A Web App implementing a Web Service that will provide the end-user with the functionality to 
inquire about real-time flights prices.

Skyscanner - API limitation
==============================
Rapidapi-Skyscanner api most often does not return inbound flights quotes. you can verify this by running the same query using https://rapidapi.com/skyscanner/api/skyscanner-flight-search
Rapidapi-Skyscanner does not provide a booking link to view the quote deals on Skyscanner website itself.  
This project will redirect you to the real skyscanner website on “View More Deals”, but don't expected to find the same deals return by Rapidapi-Skyscanner.

Azure Live
===========
http://realtimeflightprices.azurewebsites.net/

Prep Work
===============
1. Get YOUR API rapidapi-kyscanner-flight key at https://rapidapi.com/skyscanner/api/skyscanner-flight-search
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

This will give you a directory named `RealTimeFlightPrices` complete with the source code for the application and all of the necessary tools.

**Deploy ASP.Net Website in IIS on Localhost**
1. Using  Visual Studio IDE 2017 File > Open > Project/Solution
2. Select the solution file in the RealTimeFlightPrices folder
3. BUild the project and Launch the application using IIS Express(Firefox).

Once the application has loaded, a new Firefox browser tab will be launched to the home page, default IIS `http://localhost:24635/`. (This may be different in your environment)

Image from Working App
==========================
![Alt text](README/twoway.jpg?raw=true "two-way flight")

![Alt text](README/oneway.jpg?raw=true "one-way flight")

![Alt text](README/noresult.jpg?raw=true "non flight result")

![Alt text](README/unsupported.jpg?raw=true "unsupported Browser")

What is the expected input
===========================
User will manually enter the airport or city code information or select it from the auto-complete drop-down in the 'FROM'/'TO' text box, select the depart and return date then click search.

**auto-complete**: Will show top 5 matching based on the user input starts with the airports IATA code or city name or country. 

Note: For a production version of similar App, the user should be limit to only allow to select values from the drop-down list. This would avoid unnecessary search for invalid airport code. 

What is the expected output
===========================
If flights found... each flight response will contain the `search-source`, `carrier`, `source-destination`, `number of stops`, `flight-price`, `link` to view more deals.

**Link to view deals** this will take the user to either Kayak or Skyscanner to browse for t deals.

Note: There might be cases where flight results might seem duplicated in the home page, but it actual means that the flight service provider might have similar flights departing on different time.
this edge case will be ignore for the present occasion.

If no flights found. The user will see message in the home page indicating 'no flights found.'

If users using unsupported browser, a unsupported browser message will be displayed.

Application Log
===============================
Navigate to `<project root>\Flightscraper.Spa\Logs`


Web Routes
==========
`api/airport/{query}` to query for airport information. ( functionality not yet wired to the home page)
`api/flight/{originPlace}/{destinationPlace}/{outboundDate}` to search one-way flight prices
`api/flight/{originPlace}/{destinationPlace}/{outboundDate}/{inboundDate}` to search two-way flight prices
