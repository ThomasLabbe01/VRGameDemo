using UnityEngine;

public class PolygonGenerator : MonoBehaviour
{
    public GameObject cubePrefab;
    private int sides = 16; // Number of sides for the polygon
    public float radius = 5f; // Radius of the polygon
    private int numStages = 4; // Number of stages to generate
    private int[] channelMap = {10, 22, 12, 24, 13, 26, 7, 28, 1, 30, 59, 32, 53, 34, 48, 36,
                                62, 16, 14, 21, 11, 27, 5, 33, 63, 39, 57, 45, 51, 44, 50, 40,
                                8, 18, 15, 19, 9, 25, 3, 31, 61, 37, 55, 43, 49, 46, 52, 38,
                                6, 20, 4, 17, 2, 23, 0, 29, 60, 35, 58, 41, 56, 47, 54, 42};
    private int cubeCounter = 0; // Counter for cube numbering
    void Start()
    {
        GeneratePolygons();
    }

    void GeneratePolygons()
    {
        Vector3 yOffset = Vector3.up * CalculateYOffset(); // Calculate the Y-axis offset based on the cube size

        for (int i = 0; i < numStages; i++)
        {
            GeneratePolygon(i * yOffset); // Generate each stage with a Y-axis offset
        }
    }

    void GeneratePolygon(Vector3 offset)
    {
        GameObject liveDisplayGenerator = GameObject.Find("LiveDisplayGenerator"); // Find the LiveDisplayGenerator GameObject

        if (liveDisplayGenerator == null)
        {
            Debug.LogError("LiveDisplayGenerator GameObject not found.");
            return;
        }

        GameObject polygonContainer = new GameObject("PolygonContainer"); // Create an empty GameObject to hold all cubes
        polygonContainer.transform.parent = liveDisplayGenerator.transform; // Set PolygonContainer as a child of LiveDisplayGenerator

        float angleIncrement = 360f / sides;

        for (int i = 0; i < sides; i++)
        {
            float angle = i * angleIncrement;
            float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float z = radius * Mathf.Sin(Mathf.Deg2Rad * angle);

            Vector3 position = new Vector3(x, 0f, z) + offset; // Apply the offset

            GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, polygonContainer.transform); // Set the cube as a child of the container

            // Calculate scale based on the distance from the center
            float scaleFactor = 2f * Mathf.Tan(Mathf.PI / sides) * radius;

            // Apply the scale factor
            cube.transform.localScale = new Vector3(scaleFactor, scaleFactor, 0.001f); // Set scale.z to 0.001

            // Make the cube look at the center position
            cube.transform.LookAt(polygonContainer.transform.position + offset);

            // Set the cube name based on the channel map
            int cubeNumber = channelMap[cubeCounter++];
            cube.name = "Cube" + cubeNumber.ToString("D2");
        }

        // Position the container at the center with the offset
        polygonContainer.transform.position = liveDisplayGenerator.transform.position;
    }

    float CalculateYOffset()
    {
        float angleIncrement = 360f / sides;
        float scaleFactor = 2f * Mathf.Tan(Mathf.PI / sides) * radius;
        return scaleFactor;
    }
}
