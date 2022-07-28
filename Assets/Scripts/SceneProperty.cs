using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class SceneProperty : MonoBehaviour
{
    private Slider mSlider;
    private TextMeshProUGUI mText;
    private string mName;
    private MeshRenderer mTarget;

    public string PropertyName;


    // Start is called before the first frame update
    void Start()
    {
        mSlider = GetComponent<Slider>();
        mName = gameObject.name.Replace("Slider", "");
        mText = GameObject.Find(mName + "Text").GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(MeshRenderer Target)
    {
        mTarget = Target;
    }

    public void OnValueChange()
    {
        if (mSlider.wholeNumbers)
        {
            mText.text = ((int)mSlider.value).ToString();
            mTarget.material.SetInteger(PropertyName, (int)mSlider.value);
        }
        else
        {
            mText.text = mSlider.value.ToString();
            int MinLen = 4;
            if (mSlider.value < 0)
                MinLen++;
            mText.text = mText.text.Substring(0, Mathf.Min(MinLen, mText.text.Length));
            mTarget.material.SetFloat(PropertyName, mSlider.value);
        }
    }
}
