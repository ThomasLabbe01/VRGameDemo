using UnityEngine;

public class LiveDisplayObject : MonoBehaviour
{
    // Reference to the EMGRawReader or relevant data source
    public EMGRawReader _emg;

    // Start is called before the first frame update
    void Start()
    {
        // No need to initialize cubeObjects explicitly since we'll use transform.GetChild
    }

    // Update is called once per frame
    void Update()
    {
        // Update cube colors based on received data
        UpdateCubeColors();
    }

    // Update the color of each cube based on the received data
    private void UpdateCubeColors()
    {
        if (_emg != null && _emg.receivedData != null)
        {
            for (int i = 0; i < _emg.receivedData.Count; i++)
            {
                // Construct the cube name dynamically
                string cubeName = "Cube" + i.ToString("D2");

                // Find the cube by name in the hierarchy
                Transform cube = transform.Find("PolygonContainer").Find(cubeName);

                if (cube != null)
                {
                    // Map the received data value to a color between blue and yellow
                    float t = Mathf.InverseLerp(0, 1200, _emg.receivedData[i]);
                    Color color = Color.Lerp(Color.blue, Color.yellow, t);

                    // Set the color of the cube
                    cube.GetComponent<Renderer>().material.color = color;
                }
            }
        }
    }
}