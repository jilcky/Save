===============================
Safe Area Helper
===============================

Please note: This is a Third-Party asset, developed by Crystal Pug. Many thanks to him for giving permission to include it in the Kings Asset!
Detailed Information about this asset: https://connect.unity.com/p/updating-your-gui-for-the-iphone-x-and-other-notched-devices
Asset Store Link: https://assetstore.unity.com/packages/tools/gui/safe-area-helper-130488



Usage:

1. Create desired aspect ratios in the Game Window, e.g. 195:90 and 90:195 for iPhone X series.
2. Add the SafeArea.cs component to your GUI panels.
3. Add the SafeAreaDemo.cs once to a component in your scene.
4. Run the game and use the Toggle hotkey (default "A") to toggle between simulated devices. 
    Be sure to match the simulated device with the correct aspect ratio in your Game Window.

Add your own simulated devices:

1. Run an empty scene to the target mobile device with one GUI panel containing the SafeArea.cs component.
2. Rotate the device to each of the four orientations. Copy the pixel coordinates for each orientation from the debug output.
3. Enter the simulated device into SafeArea.cs using the same technique for the iPhone X.

See notes in SafeArea.cs and read our article online for a full breakdown of how the Safe Area works.

If you have a suggestion or request for a simluated device to be added to the list, please let us know in the Unity forums.

If this asset helped you, please rate us on the Asset Store!
