# pure-onyx-buttplugin

This is a really simple patch and my first experience with unity patching. Please report any issues and incompatibilities. I'm unable to really test most hardware since I only have a [hacked cheap vibration device](https://www.amazon.com/Masturbator-Training-Vibrating-Stimulator-Masturbation/dp/B0BMV9G145/ref=sr_1_1?keywords=vibrating+sex+toy+male&sr=8-1).

To compile, put the dll's from the game's managed folder in to the libs folder, as well as the dll's from the Unity buttplug library. I'm gonna not include those since idk the license implications. When moving the resulting plugin into Unity to run, you'll need to find the dll's in: 

` %userprofile%\.nuget\packages `

assuming you installed the from nuget. You'll need to find and add the dll's manually and put them in your data/managed folder, I hope to find a better way

A good reference to use is the [Bepinex docs](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/2_plugin_start.html)
