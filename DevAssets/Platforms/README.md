## 📱 Switching from Windows, Linux, or macOS to Android or iOS

When switching platforms in Unity, you might need to adjust certain assets and prefabs due to unexpected behavior. Follow the steps below to ensure a smooth transition.  
**Note:** These steps must also be repeated if you switch back to **standalone** platforms (Windows, Linux, or macOS).  

### ⚠️ Known Issues on Mobile
- **Fishing Minigame:** Currently broken on mobile platforms. Needs rewritten period to function. 
- **Lightmaps:** Every lightmap needs to be **rebaked** with a low resolution of **512** for mobile compatibility. - July 22, 2025 update: This may be fixed and this step may no longer be required.

### July 22, 2025 update:
The Windows and Mobile Platform switch changes are extremely out of date, untested, and unmaintained. Please proceed with caution. There has been reports that some sprites are missing. See the Report.jpg screenshot to see what. Also see Fix.png to see where to put ```Assets/Game/UI/Homescreen/Images/Homescreen_PlayButton.png```. Since the project was switched from the Legacy Input System to the New Input System, you will need to manually go into ```SettingsCanvas.prefab``` and delete the deprecated Touch Input Module. There should be 1 additonal module in that prefab that says convert to the new input system, that one must stay. All you do is press the convert button that appears.

### August 14, 2025 update:
Mobile assets have been updated to support Unity's new Input System, allowing the game to be built without having to use Both input systems and the old input system for specific parts of the game, which will allow us to build the game for mobile longer in the case Unity ever decides to remove the old input handler. I believe the SettingsCanvas problem and sprites problem mentionned in the July 22 update should be fixed as well.

### 🔄 Asset and Prefab Adjustments

1. **`InputBarContainer.asset`**  
   - **Node Prefab:** Replace `InputBarContainerPC.prefab` with `InputBarContainer.prefab`.  

2. **`WorldAreaControls.asset`**  
   - **Issue:** Check if anything appears as `Missing`.  
   - **Fix:** If missing, replace with `Controls.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  

3. **`VertLayout.asset`**  
   - **Issue:** Check if anything appears as `Missing`.  
   - **Fix:** If missing, replace with `VertLayout.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  

4. **`SettingsCanvas.asset`**  
   - **Issue:** Check if anything appears as `Missing`.  
   - **Fix:** If missing, replace with `SettingsCanvas.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  

5. **`LocomotionJoystickInput.asset`**  
   - **Issue:** Check if anything appears as `Missing` at the **Joystick Prefab** field.  
   - **Fix:** If missing, replace with `JoyStickPanel.prefab`.  
   - ❓ *Note: Not sure why Unity does this — double-check this asset.*  
