using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGrid : MonoBehaviour
{
    [SerializeField] int battleSize = 1; //the attakers and defenders round, default by 1
    [SerializeField] Character cyanPrefab = null; //the attacker prefab
    [SerializeField] Character greenPerfab = null; // the defender one

    const int HEXA_GRID_CONSTANT = 6;
    const float HEXA_RADIUS_CONSTANT = 0.75f;
    Vector2 CENTER_POINT = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        // init defenders
        for (int i = 0; i <= battleSize; i++)
        {
            int radius = i;
            int numberOfElement = radius == 0 ? 1 : radius * HEXA_GRID_CONSTANT;

            for (int j = 0; j < numberOfElement; j++)
            {
                Character greenChar = Instantiate(greenPerfab, transform);
                int idx = j % numberOfElement;
                float rotation = idx * (360f / numberOfElement);
                float positionX = CENTER_POINT.x + radius * HEXA_RADIUS_CONSTANT * Mathf.Cos(Mathf.Deg2Rad * rotation);
                float positionY = CENTER_POINT.y + radius * HEXA_RADIUS_CONSTANT * Mathf.Sin(Mathf.Deg2Rad * rotation);
                greenChar.gameObject.SetActive(true);
                greenChar.transform.localPosition = new Vector3(positionX, positionY, 0);
            }  
        }

        //init attackers
        {
            for (int i = 0; i <= battleSize; i++)
            {
                int radius = battleSize + i + 2;
                int numberOfElement = radius == 0 ? 1 : radius * HEXA_GRID_CONSTANT;

                for (int j = 0; j < numberOfElement; j++)
                {
                    Character cyanChar = Instantiate(cyanPrefab, transform);
                    int idx = j % numberOfElement;
                    float rotation = idx * (360f / numberOfElement);
                    float positionX = CENTER_POINT.x + radius * HEXA_RADIUS_CONSTANT * Mathf.Cos(Mathf.Deg2Rad * rotation);
                    float positionY = CENTER_POINT.y + radius * HEXA_RADIUS_CONSTANT * Mathf.Sin(Mathf.Deg2Rad * rotation);
                    cyanChar.gameObject.SetActive(true);
                    cyanChar.transform.localPosition = new Vector3(positionX, positionY, 0);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
 