using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;

public class SGTManager : MonoBehaviour
{
    [SerializeField] private Slider _numRepsSlider;
    [SerializeField] private TextMeshProUGUI _numRepsSliderText;
    [SerializeField] private Slider _timePerRepSlider;
    [SerializeField] private TextMeshProUGUI _timePerRepSliderText;
    [SerializeField] private Slider _timeBetRepSlider;
    [SerializeField] private TextMeshProUGUI _timeBetRepSliderText;
    [SerializeField] private TextMeshProUGUI _outputFolderText;
    [SerializeField] private RawImage _inputCarrouselImage;
    [SerializeField] private TextMeshProUGUI _inputCarrouselText;

    [SerializeField] private GameObject _trainingPanel;
    [SerializeField] private Slider _timeSlider;
    [SerializeField] private TextMeshProUGUI _timeSliderText;
    [SerializeField] private RawImage _sgtCarrouselImage;
    [SerializeField] private TextMeshProUGUI _sgtCarrouselText;
    [SerializeField] private TcpClientManager _tcpClient;
    private int currentInputShown = 0;
    private string startstr = @"\Resources\";
    private string titlestartstr = @"Images\";

    private string names = "";

    private bool endTraining = false;

    //private bool isFirstFunctionComplete = false;

    private ExtensionFilter[] extensions = new[] {
        new ExtensionFilter("Images", "png", "jpg", "jpeg", "svg")
    };

    // Start is called before the first frame update
    void Start()
    {
        InitializeSlider(_numRepsSlider, _numRepsSliderText);
        InitializeSlider(_timePerRepSlider, _timePerRepSliderText);
        InitializeSlider(_timeBetRepSlider, _timeBetRepSliderText);
        InitializeSlider(_timeSlider, _timeSliderText);
        UpdateOutput();
        InitializeInputCarrousel();
    }

    void InitializeSlider (Slider _slider, TextMeshProUGUI _sliderText) {
        _slider.onValueChanged.AddListener((v) => {
            _sliderText.text = v.ToString("0");
        });
        _slider.value = PlayerPrefs.GetInt(_slider.ToString());
    }

    void SaveSlider (Slider _slider) {
        PlayerPrefs.SetInt(_slider.ToString(), (int)_slider.value);
    }

    void UpdateOutput () {
        _outputFolderText.text = PlayerPrefs.GetString("OutputFolder");
    }

    public void SaveAsDefaultClicked() {
        SaveSlider (_numRepsSlider);
        SaveSlider (_timePerRepSlider);
        SaveSlider (_timeBetRepSlider);
    }

    public void SelectOutputClicked() {
        var path = StandaloneFileBrowser.OpenFolderPanel("Select Output Folder", "", false);
        if (path.Length > 0) {
            PlayerPrefs.SetString("OutputFolder", path[0]);
            UpdateOutput();
        }
    }

    public void SelectInputClicked() {
        //open file explorer and select multiple pictures
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Input Images", "", extensions, true);
        if (paths.Length > 0) {
            //add each path to a playerprefs to save between sessions.
            for (int i = 0; i< paths.Length; i++) {
                PlayerPrefs.SetString("InputImage" + i, paths[i]);
                string path = PlayerPrefs.GetString("InputImage" + i);
                int titlestart = path.IndexOf(titlestartstr,0) + titlestartstr.Length;
                names += path.Substring(titlestart) +",";
            }
            PlayerPrefs.SetString("InputsNames", names);
            PlayerPrefs.SetInt("InputCount",paths.Length);
            //set the carrousel to the first input
            SetInputCarrousel(0);
        } else {PlayerPrefs.SetInt("InputCount",0);}
    }

    public void ViewNextInputClicked() {
        if (PlayerPrefs.GetInt("InputCount") > 0) {
            if (currentInputShown == PlayerPrefs.GetInt("InputCount") - 1) {
                SetInputCarrousel(0);
            } else {
                SetInputCarrousel(currentInputShown+1);
            }
        }
    }

    private void InitializeInputCarrousel() {
        if(PlayerPrefs.GetInt("InputCount") > 0) {
            SetInputCarrousel(0);
        }
    }
    private void SetInputCarrousel(int _index) {
        Texture2D pic = LoadPicture(_index);
        _inputCarrouselImage.texture = pic;
        //change the text associated to the picture
        string path = PlayerPrefs.GetString("InputImage" + _index);
        int titlestart = path.IndexOf(titlestartstr,0) + titlestartstr.Length;
        _inputCarrouselText.text = _index.ToString() + "\n" + path.Substring(titlestart);
        //update current input var
        currentInputShown = _index;
    }

    public void StartTrainingClicked () {
        _trainingPanel.SetActive(true);
        //SendMessageToServer(NumReps + TimePerReps + NumInputs + TimeBetReps + Output)
        string message = "I" + _numRepsSlider.value
                        + _timePerRepSlider.value
                        + _timeBetRepSlider.value
                        + PlayerPrefs.GetString("InputsNames") + ' '
                        + PlayerPrefs.GetString("OutputFolder");
        _tcpClient.SendMessageToServer(message);
        StartCoroutine(Training());
    }
    public void StopTrainingClicked () {
        _trainingPanel.SetActive(false);
        endTraining = true;
    }

    public void NextSceneClicked () {
        SceneManager.LoadScene("DemoScene");
    }

    private IEnumerator Training() {
        //do as many times as number of reps
        for (int j = 0; j < _numRepsSlider.value; j++) {
            //do as many times as number of inputs
            for (int i = 0; i < PlayerPrefs.GetInt("InputCount"); i++) {
                //show picture in grey
                Texture2D pic = LoadPicture(i);
                Texture2D greypic = GetGreyPicture(pic);
                _sgtCarrouselImage.texture = greypic;
                //set text
                string path = PlayerPrefs.GetString("InputImage" + i);
                int titlestart = path.IndexOf(titlestartstr,0) + titlestartstr.Length;
                _sgtCarrouselText.text = "Rep " + (j + 1) + " of " + _numRepsSlider.value + "\nClass: " + path.Substring(titlestart);
                //start timer of time between rep
                yield return StartCoroutine(RunFirstFunction());
                //start timer of time per rep
                //show picture in color
                _sgtCarrouselImage.texture = pic;
                yield return StartCoroutine(RunNextFunction());
                if (endTraining)
                {
                    endTraining = false;
                    goto end;
                }
            }
            _tcpClient.SendMessageToServer("R");
        }
        end:
        StartCoroutine(Waiter());
    }

    System.Collections.IEnumerator Waiter()
    {
        yield return new WaitForSeconds(2);
        //endTraining:
        _tcpClient.SendMessageToServer("F");
        _trainingPanel.SetActive(false);
    }
    System.Collections.IEnumerator RunFirstFunction()
    {
        // Perform any actions or call additional functions
        //set time slider max value
        _timeSlider.maxValue = _timeBetRepSlider.value;
        float firstFunctionDuration = _timeBetRepSlider.value; // Duration of the first function in seconds
        float timer = 0.0f;

        while (timer < firstFunctionDuration)
        {
            int currentStep = Mathf.RoundToInt(timer);
            _timeSlider.value = currentStep;
            timer += Time.deltaTime;
            yield return null;
        }

        //isFirstFunctionComplete = true;
        // Perform any actions or call additional functions after the next function completes
    }

    System.Collections.IEnumerator RunNextFunction()
    {
        // Perform any actions or call additional functions
        float nextFunctionDuration = _timePerRepSlider.value; // Duration of the next function in seconds
        //set time slider max value
        _timeSlider.maxValue = _timePerRepSlider.value;
        float timer = 0.0f;
        _tcpClient.SendMessageToServer("S");

        while (timer < nextFunctionDuration)
        {
            int currentStep = Mathf.RoundToInt(timer);
            _timeSlider.value = currentStep;
            timer += Time.deltaTime;
            yield return null;
        }
        // Perform any actions or call additional functions after the next function completes
        _tcpClient.SendMessageToServer("E");
    }


    private Texture2D LoadPicture(int _index) {
        //get picture relative path
        string path = PlayerPrefs.GetString("InputImage" + _index);
        int start = path.IndexOf(startstr, 0) + startstr.Length;
        int end = path.IndexOf(".",start);
        string relpath = path.Substring(start, end - start);
        //load the picture onto the screen
        Texture2D pic = Resources.Load<Texture2D>(relpath);
        return pic;
    }

    private Texture2D GetGreyPicture(Texture2D _picture) {
        // Create a new texture to store the modified image
        Texture2D greyPic = new Texture2D(_picture.width, _picture.height);

        // Loop through each pixel of the input image
        for (int y = 0; y < _picture.height; y++)
        {
            for (int x = 0; x < _picture.width; x++)
            {
                // Get the color of the current pixel
                Color pixelColor = _picture.GetPixel(x, y);

                // Calculate the new color values
                float newR = (pixelColor.r + pixelColor.g + pixelColor.b) / 3f;
                float newG = newR;
                float newB = newR;

                // Create a new color with the modified values
                Color newPixelColor = new Color(newR, newG, newB);

                // Set the new color for the corresponding pixel in the modified image
                greyPic.SetPixel(x, y, newPixelColor);
            }
        }
        // Apply the modifications to the modified image
        greyPic.Apply();
        return greyPic;
    }
}
