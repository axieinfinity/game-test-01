using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] Camera gameCamera = null;

    public void ZoomOut ()
    {
        gameCamera.fieldOfView += 5f;
    }

    public void ZoomIn ()
    {
        float newFieldOfView = gameCamera.fieldOfView - 5f;
        if (newFieldOfView < 1f)
        {
            newFieldOfView = 1f;
        }
        gameCamera.fieldOfView = newFieldOfView;
    }
}
