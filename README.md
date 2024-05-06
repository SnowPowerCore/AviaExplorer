[![made-by- net (1)](https://github.com/SnowPowerCore/.NET-Custom-Console-App-Template/assets/35460261/d97613a9-740d-4f08-a931-81b77cfffb36)](https://dot.net)

# (Discontinued) Avia Explorer
Open application for tracking avia prices.

### Description
Android & iOS test application for checking flights and its prices. Made with Xamarin.Forms.

Features:
- **Android & iOS support;**
- **Material design;**
- **Auto suggestion of IATA code as user types;**
- **Map navigation;**
- **Destination airport selection via map pins or manually;**

Connected libraries:
- **AsyncAwaitBestPractices.MVVM** (for AsyncCommands);
- **FFImageLoading.ImageSourceHandler** (for image better handling on iOS);
- **glidex.forms** (for image better handling on Android)
- **Microsoft.AppCenter.[Name]** (AppCenter connection libs);
- **Microsoft.Extensions.[Name]** (Host & DI libs);
- **Refit** (for beautiful API description);
- **Xamarin.FFImageLoading.Svg.Forms** (for SVG support);
- **Xamarin.Forms** (main lib);
- **Xamarin.Forms.Maps** (maps control for Xamarin Forms);
- **XamEffects** (for ripple effect on button + easy commanding);
- **XF.Material** (for Material Design controls)

### Support
This application is intended for **testing** purposes only. Thanks to every developer, who made those awesome libraries anyone can use.

### Issues & notes
- Unfortunately, API for tickets' prices always returns empty array. [Try](http://map.aviasales.ru/prices.json?origin_iata=LED&period=2014-12-01:season&direct=true&one_way=false&price=50000&no_visa=true&schengen=true&need_visa=true&locale=ru&min_trip_duration_in_days=13&max_trip_duration_in_days=15).
- Data for CollectionViews population was limited to 25 items due to the testing nature. Real life projects would implement [this](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/collectionview/populate-data#load-data-incrementally).
