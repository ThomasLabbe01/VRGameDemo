using System.IO;
using UnityEngine;

public class PositionSaver : MonoBehaviour
{
    public int rate = 60; // Number of frames between position saves
    public GameObject[] objectsToSave; // List of GameObjects whose positions will be saved
    public string SavePath = ""; // The path where the file will be saved
    public TimerController timerController; // Reference to the TimerController script
    public bool saveOnReset = true; // Save positions on timer reset if true, otherwise don't save

    public Transform _camera;
    public Transform _hand;

    public OnlyOneGrabAtATime _grab;

    private int frameCount = 0;
    private StreamWriter fileWriter;
    private bool savePositions = false; // Indicates whether to save positions
    //SMI.SMIEyeTrackingUnity _eye;

    private void Start()
    {
        GameObject _objects = GameObject.Find ("NewObjects v2");
        GameObject _apple = GameObject.Find ("NewObjects v2/Apple");
        _grab = _objects.GetComponent<OnlyOneGrabAtATime>();
        //_camera = GetComponent<Transform>();
        //_hand = GetComponent<Transform>();
        //_eye = SMI.SMIEyeTrackingUnity.Instance;

        PlayerPrefs.SetString("Trialtimes", "");

        if (timerController != null)
        {
            timerController.OnTimerStart += OnTimerStart;
            //timerController.OnTimerStop += OnTimerStop;
            timerController.OnTimerReset += OnTimerReset;
            timerController.OnTimerComplete += OnTimerComplete;
        }

        // // Generate the filename based on the current time
        // string fileName = "positions_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
        // string filePath = Path.Combine(SavePath, fileName);

        // // Create the directory if it doesn't exist
        // Directory.CreateDirectory(SavePath);

        // // Open the file for writing
        // fileWriter = new StreamWriter(filePath);

        // // Write the header row with object names
        // fileWriter.Write("Timestamp"); // Adding the timestamp column header

        // for (int i = 0; i < objectsToSave.Length; i++)
        // {
        //     GameObject obj = objectsToSave[i];
        //     fileWriter.Write("\t" + obj.name); // Use tab as a separator for columns
        // }

        // fileWriter.WriteLine(); // Move to the next line after writing the header row
    }

    private void Update()
    {
        if (timerController != null && timerController.IsTimerRunning && savePositions)
        {
            frameCount++;

            // Save the positions at every "rate" frame
            if (frameCount >= rate)
            {
                SavePositions();
                frameCount = 0;
            }
        }

        // Save positions when 'R' is pressed and saveOnReset is true
        if (saveOnReset && Input.GetKeyDown(KeyCode.R))
        {
            SavePositions();
            SaveTrialTimes();
        }
    }

    private void SavePositions()
    {
        // Write the timestamp in the first column
        string timestamp = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        fileWriter.Write(timestamp);

        // Write the positions for each object in the corresponding column
        for (int i = 0; i < objectsToSave.Length; i++)
        {
            GameObject obj = objectsToSave[i];
            Vector3 position = obj.transform.position;

            fileWriter.Write($"\t{position.x},{position.y},{position.z}"); // Use tab as a separator for columns
        }
        fileWriter.Write($"\t{_camera.position.x},{_camera.position.y},{_camera.position.z}"); // Use tab as a separator for columns
        fileWriter.Write($"\t{_camera.rotation.x},{_camera.rotation.y},{_camera.rotation.z}"); // Use tab as a separator for columns
        fileWriter.Write($"\t{_hand.position.x},{_hand.position.y},{_hand.position.z}"); // Use tab as a separator for columns
        fileWriter.Write($"\t GazeTracking \t{_grab.grab}");
        fileWriter.WriteLine(); // Move to the next line after writing the positions
    }

    private void SaveTrialTimes()
    {
        fileWriter.Write(PlayerPrefs.GetString("Trialtimes"));
        PlayerPrefs.SetString("Trialtimes", "");
    }

    private void OnApplicationQuit()
    {
        // Close the file when the application is quitting or the scene is stopped
        if (fileWriter != null)
        {
            fileWriter.Close();
        }
    }

    // Callback when the timer starts
    private void OnTimerStart()
    {
        PlayerPrefs.SetInt("trialtime", 1);
        // Generate a new filename based on the current time and start recording positions
        string fileName = PlayerPrefs.GetString("Mode") + "_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
        string filePath = Path.Combine(SavePath, fileName);
        // Create the directory if it doesn't exist
        Directory.CreateDirectory(SavePath);

        // Open the file for writing
        fileWriter = new StreamWriter(filePath);

        // Write the header row with object names
        fileWriter.Write("Timestamp"); // Adding the timestamp column header

        for (int i = 0; i < objectsToSave.Length; i++)
        {
            GameObject obj = objectsToSave[i];
            fileWriter.Write("\t" + obj.name); // Use tab as a separator for columns
        }
        fileWriter.Write("\tCamPosition");
        fileWriter.Write("\tCamRotation");
        fileWriter.Write("\tHand");
        //fileWriter.Write("\tLPupilRadius");
        //fileWriter.Write("\tRPupilRadius");
        fileWriter.Write("\tGaze");
        fileWriter.Write("\tGrab");
        fileWriter.WriteLine(); // Move to the next line after writing the header row

        savePositions = true;
    }

    // Callback when the timer is reset
    private void OnTimerReset()
    {
        // Close the file when the timer is reset if saveOnReset is true
        if (saveOnReset && fileWriter != null)
        {
            fileWriter.Close();
        }

        savePositions = false;
    }

    // Callback when the timer completes (hits 0)
    private void OnTimerComplete()
    {
        // Close the file when the timer completes (hits 0)
        if (fileWriter != null)
        {
            SaveTrialTimes();
            fileWriter.Close();
        }

        savePositions = false;
    }
}
