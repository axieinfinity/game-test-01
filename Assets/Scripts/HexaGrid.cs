using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGrid : MonoBehaviour
{
    [SerializeField] int gridSize = 1;
    [SerializeField] HexaUnit cyanPrefab = null;
    [SerializeField] HexaUnit greenPrefab = null;

    private const int HEXA_GRID_CONSTANT = 6;
    private const float HEXA_RADIUS_CONSTANT = 0.75f;
    Vector2 CENTER_POINT = Vector2.zero;

    private List<HexaUnit> units = new List<HexaUnit>();
    
    private void Start()
    {
        // init defenders
        for (int i = 0; i <= gridSize; i++)
        {
            int radius = i;
            int numberOfElement = radius == 0 ? 1 : radius * HEXA_GRID_CONSTANT;

            for (int j = 0; j < numberOfElement; j++)
            {
                HexaUnit hexaUnit = Instantiate(greenPrefab, transform);
                int idx = j % numberOfElement;
                float rotation = idx * (360f / numberOfElement);
                float positionX = CENTER_POINT.x + radius * HEXA_RADIUS_CONSTANT * Mathf.Cos(Mathf.Deg2Rad * rotation);
                float positionY = CENTER_POINT.y + radius * HEXA_RADIUS_CONSTANT * Mathf.Sin(Mathf.Deg2Rad * rotation);
                hexaUnit.gameObject.SetActive(true);
                hexaUnit.transform.localPosition = new Vector3(positionX, positionY, 0);
            }
        }

        //init attackers
        {
            for (int i = 0; i <= gridSize; i++)
            {
                int radius = gridSize + i + 2;
                int numberOfElement = radius == 0 ? 1 : radius * HEXA_GRID_CONSTANT;

                for (int j = 0; j < numberOfElement; j++)
                {
                    HexaUnit hexaUnit = Instantiate(cyanPrefab, transform);
                    int idx = j % numberOfElement;
                    float rotation = idx * (360f / numberOfElement);
                    float positionX = CENTER_POINT.x + radius * HEXA_RADIUS_CONSTANT * Mathf.Cos(Mathf.Deg2Rad * rotation);
                    float positionY = CENTER_POINT.y + radius * HEXA_RADIUS_CONSTANT * Mathf.Sin(Mathf.Deg2Rad * rotation);
                    hexaUnit.gameObject.SetActive(true);
                    hexaUnit.transform.localPosition = new Vector3(positionX, positionY, 0);
                }
            }
        }
    }
}
 