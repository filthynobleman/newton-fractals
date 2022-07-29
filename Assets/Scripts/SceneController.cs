using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private Vector2 mDeltaTouch;
    public Vector2 DeltaTouch
    {
        get
        {
            return mDeltaTouch;
        }
    }

    private float mDeltaZoom;
    public float DeltaZoom
    { 
        get
        {
            return mDeltaZoom;
        }
    }


    [SerializeField]
    private RectTransform[] Panels;

    private Camera mCamera;

    void Start()
    {
        mCamera = FindObjectOfType<Camera>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Cancel") > 0)
            LoadMainMenu();

        if (Input.touchCount == 0)
        {
            mDeltaTouch = Vector2.zero;
            mDeltaZoom = 0.0f;
        }
        else if (Input.touchCount == 1)
        {
            Touch TInfo = Input.GetTouch(0);
            bool Ignore = false;
            for (int i = 0; i < Panels.Length; ++i)
                Ignore = Ignore || RectTransformUtility.RectangleContainsScreenPoint(Panels[i], TInfo.position);

            if (Ignore)
            {
                mDeltaTouch = Vector2.zero;
                mDeltaZoom = 0.0f;
            }
            else
            {
                mDeltaTouch = TInfo.deltaPosition;
                mDeltaZoom = 0.0f;
            }
        }
        else
        {
            Touch TInfo1 = Input.GetTouch(0);
            Touch TInfo2 = Input.GetTouch(1);
            bool Ignore = false;
            for (int i = 0; i < Panels.Length; ++i)
            {
                Ignore = Ignore || RectTransformUtility.RectangleContainsScreenPoint(Panels[i], TInfo1.position);
                Ignore = Ignore || RectTransformUtility.RectangleContainsScreenPoint(Panels[i], TInfo2.position);
            }

            if (Ignore)
            {
                mDeltaTouch = Vector2.zero;
                mDeltaZoom = 0.0f;
            }
            else
            {
                mDeltaTouch = Vector2.zero;
                mDeltaZoom = TInfo1.deltaPosition.magnitude + TInfo2.deltaPosition.magnitude;
                Vector2 PrevDist = TInfo1.position - TInfo2.position;
                Vector2 CurDist = PrevDist + TInfo1.deltaPosition - TInfo2.deltaPosition;
                if (PrevDist.sqrMagnitude > CurDist.sqrMagnitude)
                    mDeltaZoom = -mDeltaZoom;
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
