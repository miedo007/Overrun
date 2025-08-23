# mtl-bonfire
A wrapper for Beacon including Analytics, Terms of Services, App Tracking Transparency, and Facebook SDK support

## Installation
- Install the latest [Unity Facebook SDK](https://developers.facebook.com/docs/unity/)
- Setup Facebook SDK by going to `Facebook > Edit Settings` and fill in App Name, App Id and Client Token
- Setup [Package Manager](https://confluence.rovio.com/pages/viewpage.action?spaceKey=MTL&title=Unity+Packages) in your Unity project
- Import `mtl-bonfire` from Package Manager
- Drag drop the `BeaconWrapper` Prefab into your initial scene or initial prefab (setup might vary from a project to another)
- Fill in `client id` and `client secret` available on the [Beacon dashboard](https://beacon.rovio.com/dashboard/) of your game page in `Settings > Authentication Keys`

## App Transparency Tracking
Call `IBonfire.Initialize(Action onComplete)` on the starting of your app to show the App Transparency Tracking popup.

## Terms of Services
Call `ITermsOfService.Show(Action<bool> onComplete)` to show the Terms of Services native dialog.
Make sure the terms button leads to the right URL. Ask beaconsupport to set that up for you on their slack channel.

## Custom Events
Call any of these two methods:

`IAnalyticsManager.SendEvent(string eventName, params (string, object)[] eventParams)`

`IAnalyticsManager.SendEvent(string eventName, Dictionary<string, object> eventParams)`

To check if the custom events are correctly received, go to the [Beacon dashboard](https://beacon.rovio.com/dashboard/) in the Game section, select your game, and look for Players/Device by using the Player id logged into the Unity Console.
