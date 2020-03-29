# Avia Explorer
Open application for tracking avia prices.

[TOC]

### Screenshots
![](https://db3pap005files.storage.live.com/y4pV4wv303ppimA8Cf7TTsdsrCvCO-9OYdRqETzXm5eO1iF7pYFDXI9EPoq9-clKwqwtLYDSG81XPlE8qRfjSHt3K0Iz0LQKGbKSlBX0YtdA430I3hnHrgibBSPTMVoPcq1-uUQNwJvXU6kzZK_GUyrs4c4s1djWkAlHnR0_iQ1PPfejy1ZAX6pEsfvwvm82gdN/Android1.png?psid=1&width=204&height=446)
> Android origin airport selection.

![](https://db3pap005files.storage.live.com/y4ppy-PbiJN90GvMvL5U-X7I91w1lnqVT-tBYJl6mqFTnIE5EvUAjZPAJsLytx6QGzgfepy1RIaKxqmPs-HVDP1qgU4MPIFfu4bseFelQWrO4X2tfvN2ri5wXUK6jKlAocac4ooYfg_T81J2dL40YeVfncFVkswkKl870HArNPW9LyeG5aIqqxtzQJQXu6jobiM/Android2.png?psid=1&width=204&height=446)
> Android destination airport selection (via map pins, or within the list).

![](https://db3pap005files.storage.live.com/y4pnz0n745gY6wPpBKju_SSv7Ta0TQTeGabebtQIpgwMn4klnyZp3_Nd8TPZ3N2h5oZOIHAhAWOixblnEF-Cp5u8bAkDtxOLSCk5hO5ZnRryhQfCT20CnSI7F4p17aqUEqbHrlwy1SjEHOkCTm3g7XRhQy18_WkRVXygUL7AfzAZ08v235mytr2wIQdPsqWwxvS/Android3.png?psid=1&width=204&height=446)
> Android flights screen (unfortunately, API returns empty array).

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
This application is intended for **testing** purposes only. Thanks to every developer, who made those awesome libraries which anyone can use.

### Issues & notes
- Unfortunately, API for tickets' prices always returns empty array.
- Data for CollectionViews population was limited to 25 items due to the testing nature. Real life projects would implement [this](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/collectionview/populate-data#load-data-incrementally).
