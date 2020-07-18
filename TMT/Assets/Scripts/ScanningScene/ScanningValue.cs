using System.Collections;
using System.Collections.Generic;

public static class ScanningValue
{
    private static float data = 0.0f;

    public static void setData(float value)
    {
        data = value;
    }

    public static float getData()
    {
        return data;
    }
}
