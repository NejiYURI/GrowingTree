using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class TreeGeneratorUIControl : MonoBehaviour
{
    public BranchGenerator meshGenerator;
    [Header("Segment Setting")]
    public Slider SegmentSlider;
    public TextMeshProUGUI SegmentNumTxt;

    [Header("Branch Number Setting")]
    public Slider BranchSlider;
    public TextMeshProUGUI BranchNumTxt;

    [Header("Speed Setting")]
    public Slider SpeedSlider;
    public TextMeshProUGUI SpeedNumTxt;

    [Header("Radius Setting")]
    public Slider RadiusSlider;
    public TextMeshProUGUI RadiusNumTxt;

    [Header("Boundary Setting")]
    public TMP_InputField min_X;
    public TMP_InputField Max_X;

    public TMP_InputField min_Y;
    public TMP_InputField Max_Y;

    public TMP_InputField min_Z;
    public TMP_InputField Max_Z;

    private void Start()
    {
        SegmentSliderChange();
        BranchSliderChange();
        SpeedSliderChange();
        RadiusSliderChange();
    }
    public void StartGenerate()
    {
        Debug.Log("Generate Start");
        if (meshGenerator == null) return;
        meshGenerator.TreeParameter.SegmentNum = (int)SegmentSlider.value;
        meshGenerator.TreeParameter.BranchWeight = (int)BranchSlider.value;
        meshGenerator.TreeParameter.GrowSpeedMul = SpeedSlider.value * 100f;
        meshGenerator.TreeParameter.Radius = RadiusSlider.value;
        if (min_X != null && Max_X != null)
        {
            if (!string.IsNullOrEmpty(min_X.text) && !string.IsNullOrEmpty(Max_X.text))
            {
                meshGenerator.TreeParameter.BoundX = new minMaxData(float.Parse(min_X.text), float.Parse(Max_X.text));
            }
        }
        if (min_Y != null && Max_Y != null)
        {
            if (!string.IsNullOrEmpty(min_Y.text) && !string.IsNullOrEmpty(Max_Y.text))
            {
                meshGenerator.TreeParameter.BoundY = new minMaxData(float.Parse(min_Y.text), float.Parse(Max_Y.text));
            }
        }

        if (min_Z != null && Max_Z != null)
        {
            if (!string.IsNullOrEmpty(min_Z.text) && !string.IsNullOrEmpty(Max_Z.text))
            {
                meshGenerator.TreeParameter.BoundZ = new minMaxData(float.Parse(min_Z.text), float.Parse(Max_Z.text));
            }
        }
        meshGenerator.StartGenerate();
    }

    public void SegmentSliderChange()
    {
        if (SegmentNumTxt == null) return;
        SegmentNumTxt.text = ((int)SegmentSlider.value).ToString("0");
    }

    public void BranchSliderChange()
    {
        if (BranchNumTxt == null) return;
        BranchNumTxt.text = ((int)BranchSlider.value).ToString("0");
    }

    public void SpeedSliderChange()
    {
        if (SpeedNumTxt == null) return;
        SpeedNumTxt.text = (SpeedSlider.maxValue - SpeedSlider.value).ToString("0.0");
    }
    public void RadiusSliderChange()
    {
        if (RadiusNumTxt == null) return;
        RadiusNumTxt.text = RadiusSlider.value.ToString("0.0");
    }
}
