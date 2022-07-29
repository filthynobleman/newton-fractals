using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private float RefreshRate = 1.0f;
    private float mTimer;
    private int mFrameCount;

    private TextMeshProUGUI mText;

    // Start is called before the first frame update
    void Start()
    {
        mText = GetComponent<TextMeshProUGUI>();
        mText.text = "FPS: -";
        mTimer = 0.0f;
        mFrameCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        mFrameCount++;
        mTimer += Time.unscaledDeltaTime;
        if (mTimer >= RefreshRate)
        {
            mTimer = 0.0f;
            mText.text = "FPS: " + mFrameCount.ToString();
            mFrameCount = 0;
        }
    }
}
