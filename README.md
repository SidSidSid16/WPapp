
<img style="float: left; margin-right: 20px;" src="https://i.imgur.com/mhVSI2Z.png">

## Xamarin.Forms Project to Turn Your WordPress Website Into a Native App 

[![Donate](https://img.shields.io/badge/Donate-Card%20/%20PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=X3D2CQF7798G8&source=url)

### This app is aimed at people with little or no programming knowledge to turn their WordPress website into an Android and iOS app

This is my first Xamarin.Forms project and my second C# project. Code quality may not be up to standards and performance may not be as good. Please bear with me while I improve, you can help me by improving the code.

# Features
* **Great compatibility and consistency between both platforms, iOS and Android**

* **System-wide dark/light mode support**
	* Go into the Android/iOS settings page and change to light or dark mode and WPapp will dynamically adapt
![Dark Mode](https://i.imgur.com/G1w1uHlm.png)
![Light Mode](https://i.imgur.com/oanL1Jzm.png)

* **In app post viewer**
	* This app will natively display the WordPress posts instead of just opening an in-app embedded browser

		![Example Post](https://i.imgur.com/mR4DCcfm.png)
	* Features a platform custom share button	
	
	![Share post on Android](https://i.imgur.com/UlspfDGm.png)
	![Share post on iOS](https://i.imgur.com/uCqjGk9m.png)

* **Author details page**	
	* Click on the Author credits frame above the post title in the post page to reveal this screen:
	![Author Page](https://i.imgur.com/7goTYgYm.png)
	
* **Side Menu**	
	* Pressing the Menu button at the top left side of the screen will reveal a sidebar which lists the categories of the WordPress website. When a category from the list is pressed, a new page will open showing all the posts in the selected category	
	
		![Sidebar](https://i.imgur.com/tDxsz7Km.png)

# Getting Started
1. Clone this repository
2. Open the `Constants.cs` file (WPapp/Models/Constants.cs)
3. Set the variables to match your preferences and WordPress website
4. To change the app name from "WPapp" to a name of your choice and changing the app icon, see below.
5. Once you are happy, you can build the app!

### Change app name and icon on Android
1. Open WPapp.sln in Visual Studio
2. Open the `MainActivity.cs` file (WPapp.Android/MainActivity.cs)
3. Change the `Label` property from "WPapp" to a name of your choice

### Change app name and icon on iOS
1. Open WPapp.sln in Visual Studio
2. Open the `Info.plist` file (WPapp.iOS/Info.plist)
3. Change the `Bundle display name` to the app name of your choice