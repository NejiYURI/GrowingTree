using UnityEngine;

[System.Serializable]
public class minMaxData
{
    public minMaxData(float i_min, float i_Max)
    {
        if (i_min <= i_Max)
        {
            min = i_min;
            Max = i_Max;
        }
    }
    [SerializeField]
    public float min;
    [SerializeField]
    public float Max;

    public float GetRandResult()
    {
        return Random.Range(min, Max);
    }
}
