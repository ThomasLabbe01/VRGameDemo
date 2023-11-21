The current project includes :

An environment in which the user can move objects around from one table to a another. We can access an exploration mode where the user can interact with all objects, and a test mode where the user is specifically asked to move certain objects. There are currently 8 different poses that the user can make, and every object can be grabbed with 2 or 3 poses.
A tool to save data of tests is also present. The status of the grab and the position of the objects is recorded
Training with context by comparing the current pose with the available poses for the object
Vibrotactile feedback. When the user grabs an object, we can send a vibration through a bracelet
EMG sensors : the user can put on the HD-EMG sensors to controll the different poses

Steps to get the game running :
1. Create a folder a use git to clone the project : 
git clone https://github.com/ThomasLabbe01/VRGameDemo.git
2. Open Unity hub, Use add project from disk and select the VRGameDemo folder
3. Open the game, The first loading requires Unity to build libraries, this can be long
4. In Assets/Scenes drag the DemoScene and the SGTScene under Hierarchy
5. Remove the Unititled scene and unload the DemoScene
6. Connect the EMaGer bracelet to your computer
7. In Assets\Scripts\EMaGerCodes open live_64_channel.py and run the script. Make sure the signals are ok
8. In Assets\Scripts\SGT open serverEMaGer.py an run the script. Wait until the following message is displayed in the terminal : "Server started on localhost:12346". If you also receive "Error occured", restart step 8
9. Once the server is started, run the game
10. Once in the game, select the desired configuration for screen guided training. Example : Number of repetitions : 3, Time per repetition: 3, Time between rep : 3. 
11. Press on select Input. Remove the headset and select the desired poses in Assets\Resources\Images. After the selection, make sure it was done correctly by pressing view next inputs
12. Press on Select output. Remove the headset and select the Data folder
13. Press on start training. An interface will appear on the right for you to follow along. 
14. Once the interface dissapears, Remove the headset and wait until data is displayed in the ServerEMaGer.py terminal. Then, put the headset back on
15. Press on Next scene. You will access the game environment, and you should be able to see data displayed in rea time around your arm and on the screen in front of you.
16. In Unity o your computer, press on the game scene. Then, press "Enter" to see the game appear in front of you, and press "f" to access the exploration mode. You can now interact with objects by making the gestures you previously trained