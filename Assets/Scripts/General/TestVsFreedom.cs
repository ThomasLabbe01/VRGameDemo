using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.OpenXR.Input;
//using static UnityEngine.Rendering.DebugUI;
//using Unity.VisualScripting;

public class TestVsFreedom : MonoBehaviour
{
    public List<GameObject> poses;
    public bool Freedom = false;
    public TMPro.TextMeshProUGUI instructions;
    public TMPro.TextMeshProUGUI timer;
    public MeshRenderer visualIndicator;
    public bool Test = false;
    public bool TestVisual = false;
    public bool TestVibro = false;
    public bool SGT = false;
    public List<Transform> tables;

    private bool collision = false;
    private GameObject currentTestObject = null;
    private HashSet<GameObject> completedTasks = new HashSet<GameObject>();
    private Transform currentObjectTable = null;
    private Transform targetTable = null;
    private GameObject RightHand;
    private GameObject RightHandPoses;
    [SerializeField] private GameObject _sgtPanel;

    private void Start()
    {
        Random.InitState(1);
        DeactivateComponents();
        instructions.enabled = true;
        timer.enabled = false;
        visualIndicator.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Test = false;
            timer.enabled = false;
            instructions.enabled = true;
            visualIndicator.enabled = false;

            RightHand = transform.parent.Find("Complete XR Origin Hands Set Up/XR Origin (XR Rig)/Camera Offset/Right Hand/Right Hand Interaction Visual/RightHand").gameObject;
            RightHand.SetActive(false);

            RightHandPoses = transform.parent.Find("Poses2 v2").gameObject;
            RightHandPoses.SetActive(true);
            DeactivateComponents();
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (!Test)
            {
                Debug.Log("Test mode enabled");
                Test = true;
                timer.enabled = true;
                visualIndicator.enabled = false;
                PlayerPrefs.SetInt("FeedbackMode", 0);
                PlayerPrefs.SetString("Mode", "OL");
                // Start the test mode
                

                StartTestMode();
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!Test)
            {
                Debug.Log("Visual feedback test mode enabled");
                Test = true;
                timer.enabled = true;
                instructions.enabled = true;
                visualIndicator.enabled = true;
                //Set le mode de feedback pour appliquer le bon
                PlayerPrefs.SetInt("FeedbackMode", 1);
                //Set le mode pour aller l'identifier dans les stats
                PlayerPrefs.SetString("Mode", "TV");
                // Start the test mode
                
                StartTestMode();
            }
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!Test)
            {
                Debug.Log("Vibrotactile feedback test mode enabled");
                Test = true;
                timer.enabled = true;
                instructions.enabled = true;
                visualIndicator.enabled = true;
                //Set le mode de feedback pour appliquer le bon
                PlayerPrefs.SetInt("FeedbackMode", 2);
                //Set le mode pour aller l'identifier dans les stats
                PlayerPrefs.SetString("Mode", "TVT");
                // Start the test mode
                

                StartTestMode();
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!Test)
            {
                Debug.Log("Exploration mode enabled");
                timer.enabled = true;
                instructions.enabled = true;
                visualIndicator.enabled = true;
                // Start the freedom mode
                StartFreedomMode();
                
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!SGT)
            {
                Debug.Log("SGT mode enabled");
                RightHand = transform.parent.Find("Complete XR Origin Hands Set Up/XR Origin (XR Rig)/Camera Offset/Right Hand/Right Hand Interaction Visual/RightHand").gameObject;
                RightHand.SetActive(true);

                RightHandPoses = transform.parent.Find("Poses2 v2").gameObject;
                RightHandPoses.SetActive(false);
                StartSGTMode();
                timer.enabled = false;
                instructions.enabled = false;
                visualIndicator.enabled = false;
            }
        }          

        if (collision)
        {
            // Successfully moved object to the other table, change the random object for the test
            collision = false;
            ChangeRandomTestObject();
        }
    }

    private void StartTestMode()
    {
        _sgtPanel.SetActive(false);
        GameObject randomPose = GetRandomPose();
        SetTestObject(randomPose);
    }

    private void StartSGTMode()
    {
        SGT = true;
        instructions.text = "";

        _sgtPanel.SetActive(true);
    }

    private void StartFreedomMode()
    {
        _sgtPanel.SetActive(false);
        Test = false;
        instructions.text = "Exploration mode. Try to grab objects with different contractions";
        SetAllPosesFreedom();
    }

    private void SetTestObject(GameObject pose)
    {
        currentTestObject = pose;
        foreach (GameObject p in poses)
        {
            GrabSetUp2 grabSetUp2 = p.GetComponent<GrabSetUp2>();
            if (grabSetUp2 != null)
            {
                grabSetUp2.Test = (p == pose);
                grabSetUp2.Freedom = false;
            }
        }
    }
    private void SetAllPosesFreedom()
    {
        foreach (GameObject pose in poses)
        {
            GrabSetUp2 grabSetUp2 = pose.GetComponent<GrabSetUp2>();
            if (grabSetUp2 != null)
            {
                grabSetUp2.Test = false;
                grabSetUp2.Freedom = true;
            }
        }
    }

    private void ChangeRandomTestObject()
    {
        // Mark the current test object as completed
        completedTasks.Add(currentTestObject);

        /*
        // Check if all objects have been tested
        if (completedTasks.Count == poses.Count)
        {
            // All tasks completed, end test mode
            EndTestMode();
            return;
        }
        */

        // Find a new random test object that has not been tested yet
        GameObject newRandomPose = GetRandomPose();
        while (completedTasks.Contains(newRandomPose))
        {
            newRandomPose = GetRandomPose();
        }

        // Set the new test object
        SetTestObject(newRandomPose);
    }

    private GameObject GetRandomPose()
    {
        if (poses.Count == 0)
            return null;

        int randomIndex = Random.Range(0, poses.Count);
        return poses[randomIndex];
    }

    public void SetCollisionStatus(bool colliding)
    {
        collision = colliding;
    }

    /*
    private void EndTestMode()
    {
        Test = false;
        instructions.text = "Test mode completed! Press escape to return";
        completedTasks.Clear();
    }
    */

    private void DeactivateComponents()
    {
        foreach (GameObject pose in poses)
        {
            GrabSetUp2 grabSetUp2 = pose.GetComponent<GrabSetUp2>();
            CustomGravity customGravity = pose.transform.parent.GetComponent<CustomGravity>();
            if (grabSetUp2 != null)
            {
                customGravity.CheckTest = false;
                grabSetUp2.Test = false;
                grabSetUp2.Freedom = false;

                instructions.text = "F: Freedom, O:Training, V:V.Test, T:VT.Test, S:SGT, C:Calibration";
            }
        }

        Test = false;
        TestVisual = false;
        TestVibro = false;
        SGT = false;
        _sgtPanel.SetActive(false);
    }
}
