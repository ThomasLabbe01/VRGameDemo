using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class GrabSetUp2 : MonoBehaviour
{
    public GameObject AllowedPose;
    public GameObject Target;

    public bool Test;
    public bool Freedom;

    private float DistanceForPhantomPosesMin = 0.1f;
    private float DistanceForPhantomPosesMax = 0.5f;

    private float DistanceForGrab = 0.1f;

    private bool follow = false;
    private bool testfollow = false;
    private string current_class = "xx";

    private bool flag = true;
    private EMGRawReader _emgRawReader;
    private EMGSetUp _emg;

    private List<GameObject> AllowedPosePositions = new List<GameObject>();
    private List<GameObject> AllowedPhantomPosePositions = new List<GameObject>();

    private MeshRenderer childCRenderer;

    
    private string GetContext(string obj_grab_classes){
        string context = "";
	    var grab_classes = new List<string>(9) {"Neutral","H1", "H2", "H3", "H4", "T1", "T2", "T3", "T4"};
        string current_class = grab_classes[int.Parse(_emgRawReader.readVal)];

        /*
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                string keyCodeString = keyCode.ToString();
                int classIndex = int.Parse(keyCodeString[keyCodeString.Length - 1].ToString()) - 1;
                if (classIndex >= 0 && classIndex < grab_classes.Count)
                {
                    current_class = grab_classes[classIndex];
                }
            }
        }
        */
        if (obj_grab_classes.Contains(current_class)){
                context = "P ";
            } else {context = "N ";}
        return context;
    }
    

    private string GetContraction()
    {
        string contraction = "";

        Transform parent = transform.parent;

        if (parent != null)
        {
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = parent.GetChild(i);
                string firstTwoLetters = child.name.Substring(0, Mathf.Min(2, child.name.Length));
                contraction += firstTwoLetters + " ";
            }
        }
        return contraction;
    }

    public float CalculateDistance(Transform objectA, Transform objectB)
    {
        // Get the positions of the two objects
        Vector3 positionA = objectA.position;
        Vector3 positionB = objectB.position;

        // Calculate the differences in coordinates
        float deltaX = positionB.x - positionA.x;
        float deltaY = positionB.y - positionA.y;
        float deltaZ = positionB.z - positionA.z;

        // Calculate the squared distance using Pythagorean theorem
        float distanceSquared = (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);

        // Calculate the actual distance by taking the square root of the squared distance
        float distance = Mathf.Sqrt(distanceSquared);

        return distance;
    }

    public void GrabActivationFreedom()
    {
        // Get the DistanceCalculator component attached to a GameObject
        GrabSetUp2 calculator = GetComponent<GrabSetUp2>();

        // Calculate the distance between the transforms of the two game objects
        float distance = calculator.CalculateDistance(Target.transform, transform.parent.transform);

        SkinnedMeshRenderer GhostAppear = transform.GetComponentInChildren<SkinnedMeshRenderer>();
        
        if( distance >= DistanceForPhantomPosesMin && distance <= DistanceForPhantomPosesMax) 
        {
            // Activate phantom pose
            GhostAppear.enabled = true;
        }
        if (distance < DistanceForPhantomPosesMin && !follow)  // c'est ici qu'on veut checker la distance
        {
            // Activate phantom pose
            GhostAppear.enabled = false;
        }
        if (distance >= DistanceForPhantomPosesMax)
        {
            GhostAppear.enabled = false;
        }
        TrackTowards component = GetComponent<TrackTowards>();
        if (component.onlyOneGrabAtATime == false)
        {
            GhostAppear.enabled = false;
        }
    }
    public void GrabActivationTest()
    {
        // Get the DistanceCalculator component attached to a GameObject
        GrabSetUp2 calculator = GetComponent<GrabSetUp2>();

        // Calculate the distance between the transforms of the two game objects
        float distance = calculator.CalculateDistance(Target.transform, transform.parent.transform);


        Transform parent = transform.parent;
        testfollow = false;
        foreach(Transform child in parent)
        {
            TrackTowards component = child.GetComponent<TrackTowards>();
            testfollow = testfollow || component.enabled;
        }
        foreach (Transform child in parent)
        {
            TrackTowards component = child.GetComponent<TrackTowards>();
            child.transform.Find("RightHand").GetComponent<SkinnedMeshRenderer>().enabled = (testfollow && component.enabled) || (!testfollow && !component.enabled);
        }



        if (distance < DistanceForPhantomPosesMax && _emg.EMG)  // c'est ici qu'on veut checker la distance
        {
            string timestamp = _emgRawReader.timestamp;
            string contraction = GetContraction();
            string context = GetContext(contraction);
            string message = context + timestamp + contraction;
            Debug.Log(contraction);
            Debug.Log(context);
            _emgRawReader.Write(message);
        }

    }

    void Start()
    {
        _emgRawReader = FindObjectOfType<EMGRawReader>();
        _emg = FindObjectOfType<EMGSetUp>();


        AllowedPosePositions = new List<GameObject>();
        
        //index tip
        //GameObject IndexTip = AllowedPose.transform.Find("R_Wrist/R_IndexMetacarpal/R_IndexProximal/R_IndexIntermediate/R_IndexDistal/R_IndexTip").gameObject;
        
        //middle tip
        //GameObject MiddleTip = AllowedPose.transform.Find("R_Wrist/R_MiddleMetacarpal/R_MiddleProximal/R_MiddleIntermediate/R_MiddleDistal/R_MiddleTip").gameObject;
        
        //ring tip
        //GameObject RingTip = AllowedPose.transform.Find("R_Wrist/R_RingMetacarpal/R_RingProximal/R_RingIntermediate/R_RingDistal/R_RingTip").gameObject;
        
        //thumb tip
        //GameObject ThumbTip = AllowedPose.transform.Find("R_Wrist/R_ThumbMetacarpal/R_ThumbProximal/R_ThumbDistal/R_ThumbTip").gameObject;
        
        //palm
        GameObject palm = AllowedPose.transform.Find("R_Wrist/R_Palm").gameObject;

        //AllowedPosePositions.Add(IndexTip);
        //AllowedPosePositions.Add(MiddleTip);
        //AllowedPosePositions.Add(RingTip);
        //AllowedPosePositions.Add(ThumbTip);
        AllowedPosePositions.Add(palm);


        AllowedPhantomPosePositions = new List<GameObject>();

        //index tip
        //GameObject IndexTipG = transform.Find("R_Wrist/R_IndexMetacarpal/R_IndexProximal/R_IndexIntermediate/R_IndexDistal/R_IndexTip").gameObject;

        //middle tip
        //GameObject MiddleTipG = transform.Find("R_Wrist/R_MiddleMetacarpal/R_MiddleProximal/R_MiddleIntermediate/R_MiddleDistal/R_MiddleTip").gameObject;

        //ring tip
        //GameObject RingTipG = transform.Find("R_Wrist/R_RingMetacarpal/R_RingProximal/R_RingIntermediate/R_RingDistal/R_RingTip").gameObject;

        //thumb tip
        //GameObject ThumbTipG = transform.Find("R_Wrist/R_ThumbMetacarpal/R_ThumbProximal/R_ThumbDistal/R_ThumbTip").gameObject;

        //palm
        GameObject palmG = transform.Find("R_Wrist/R_Palm").gameObject;

        //AllowedPhantomPosePositions.Add(IndexTipG);
        //AllowedPhantomPosePositions.Add(MiddleTipG);
        //AllowedPhantomPosePositions.Add(RingTipG);
        //AllowedPhantomPosePositions.Add(ThumbTipG);
        AllowedPhantomPosePositions.Add(palmG);
    }

    public void GrabHandeller()
    {
        //float sum = 0;
        //{
        //    for (int i = 0; i < AllowedPosePositions.Count; i++)
        //    {
        //        sum += CalculateDistance(AllowedPosePositions[i].transform, AllowedPhantomPosePositions[i].transform);
        //    }
        //}

        //float meanDistance = sum / AllowedPosePositions.Count;
        float meanDistance = CalculateDistance(AllowedPosePositions[0].transform, AllowedPhantomPosePositions[0].transform);

        if (meanDistance < DistanceForGrab)
        {
            follow = true;
        }
        SkinnedMeshRenderer GhostAppear = GetComponentInChildren<SkinnedMeshRenderer>();
        childCRenderer = AllowedPose.transform.Find("RightHand/Sphere").GetComponent<MeshRenderer>();

        if (follow)
        {
            GetComponent<TrackTowards>().enabled = true;
            GhostAppear.enabled = true;
            childCRenderer.enabled = false;
            follow = false;
        }

        if (!AllowedPose.activeSelf) //
        {
            follow = false;
            childCRenderer.enabled = true;
            GetComponent<TrackTowards>().enabled = false;
            AllowedPose.transform.position = new Vector3(0, 0, 0);
        }
    }


    void Update()
    {
        CustomGravity customGravity = transform.parent.GetComponent<CustomGravity>();
        if (Freedom)
        {
            GrabActivationFreedom();
        }
        if (Test)
        {
            if (flag)
            {
                customGravity.CheckTest = true;
                flag = false;
            }
            GrabActivationTest();

        }
        if (!Freedom && !Test && !customGravity.CheckTest)
        {
            SkinnedMeshRenderer GhostAppear = GetComponentInChildren<SkinnedMeshRenderer>();
            GhostAppear.enabled = false;
        }
        GrabHandeller();

    }
}