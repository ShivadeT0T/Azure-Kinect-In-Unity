
# Azure Kinect On Unity

This Unity project utilizes Azure Kinect's 3D body tracking to record user's movements and displaying them on a 3D model in Unity. The saved recordings can be played back or used in a minigame within the app.

## Features
Here's a list of different features inside the app:

- Motion capture (recording an animation)
- Recording playback
- Avatar selection
- Animation file management
- Free camera inside the playback scene (also in recording scene)
- A simple game utilizing recorded animation

## Screenshots
Here's an overview of the app itself

### Main menu

![main_menu](https://github.com/user-attachments/assets/663c2acd-f19a-4371-940c-69cfaee15d48 "main menu")
![model_selection](https://github.com/user-attachments/assets/b2972fc5-70d5-4f70-91ba-f4b3da0469f5 "model selection")
![load_animation](https://github.com/user-attachments/assets/c7a44b96-0e64-4640-af75-ce33d1a3e66d "load animation")
![load_game](https://github.com/user-attachments/assets/5437ffd2-354a-449b-b43c-044dcb01d90f "load game")

### Recording scene

![recording_scene](https://github.com/user-attachments/assets/c124a6b6-415c-4e42-83d0-725a73dacd53 "recording scene")
![countdown](https://github.com/user-attachments/assets/39e95aae-2cdd-4fa2-8765-afbccd2940c2 "countdown")
![active_recording](https://github.com/user-attachments/assets/352ec751-fac4-40ab-81bc-503814b0470f "active recording")
![save_recording](https://github.com/user-attachments/assets/1425f453-d258-4766-9abc-2675be00a9ad "save recording")


### Playback scene

![playback_scene](https://github.com/user-attachments/assets/021e211a-5533-4ff0-86da-83f0e4705160 "playback scene")
![free_cam_info](https://github.com/user-attachments/assets/e5deb64e-b1bb-40de-864a-84f4bdb5e91a "free cam info")
![different_angle](https://github.com/user-attachments/assets/75ff9250-0d65-4b33-a831-0932455fc999 "different angle")

### Game scene

![game_scene](https://github.com/user-attachments/assets/e69f62b7-deb8-43da-8399-e47d449ff1a1 "game scene")
![calibration](https://github.com/user-attachments/assets/492f6135-fd94-49d0-a377-eb8178f9be4c "calibration")
![game_countdown](https://github.com/user-attachments/assets/7ea4e4c3-9657-4647-82d1-209679b97238 "game countdown")
![final_score](https://github.com/user-attachments/assets/b57cfbff-0534-472c-bfb8-f0c1757d22e9 "final score")

## Run Locally

Once the repo is cloned you should follow the steps according to the "sample-unity-bodytracking" inside the [Azure-Kinect-Samples](https://github.com/microsoft/Azure-Kinect-Samples/tree/master/body-tracking-samples/sample_unity_bodytracking "Repo with samples for Azure Kinect") repo, since this project is essentially an extension of the sample.

## Known Issues and Personal Notes

Right now there are 3 issues that I know of:  
1. Overall code structure is not pretty and lacks comments.
2. The calibration algorithm in the game scene is faulty, so currently, it uses the first frame of the animation file for calibration.
   - I can't think of a good solution for this right now, but it might be related to calculating distances between frames (though that could be computationally expensive).
3. The scoring/accuracy logic in the game scene doesn't work properly if both of the player's hands are on the same side of the Y-axis.
   - A simple if statement could fix this issue, but I didn't have enough time to implement it.

At the beginning I tried designing a good code structure for my project, but over time I realized that I wasn't making progress efficiently and had time constraints. The lack of proper comments was due to me forgetting to add them.

Overall I'm pretty satisfied with this project and have learned lots of valuable lessons. However, since I no longer have access to Azure Kinect DK and want to focus on other projects, this one will likely not receive future updates.
