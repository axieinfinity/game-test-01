using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int width, height;

    void Start()
    {
        var j = 0;
        var k = 0;
        var offset = 0f;
        while (j <= height)
        {
            for (int i = -width + k; i <= width; i++)
            {
                offset = k * -0.5f;
                if (j > 0)
                {
                    var obj1 = Instantiate(prefab, transform);
                    var obj2 = Instantiate(prefab, transform);
                    obj1.transform.localPosition = new Vector3(i + offset, -j, 0);
                    obj2.transform.localPosition = new Vector3(i + offset, j, 0);
                    obj1.name += i + "." + j;
                    obj2.name += i + "." + j;
                }
                else
                {
                    var obj = Instantiate(prefab, transform);
                    obj.transform.localPosition = new Vector3(i, j, 0);
                    obj.name += i + "." + j;
                }
            }
            k++;
            j++;
        }
    }

}
