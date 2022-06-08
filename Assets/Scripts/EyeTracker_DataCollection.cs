using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using System.IO;
using System;

/// <summary>
/// Example usage for eye tracking callback
/// Note: Callback runs on a separate thread to report at ~120hz.
/// Unity is not threadsafe and cannot call any UnityEngine api from within callback thread.
/// </summary>
public class EyeTracker_DataCollection : MonoBehaviour
{
    public string filename;
    private static EyeData eyeData = new EyeData();
    private static bool eye_callback_registered = false;
    

    private static StreamWriter writer;

    private void Start()
    {
         var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // create directory to store files
        System.IO.Directory.CreateDirectory("Data");
        // initialize txt file
        writer = new StreamWriter(string.Format("Data/{0}_{1}.txt", filename, unixTimestamp));

        string header = "sample_timestamp,combined_pupil_diameter,combined_originX,combined_originY,combined_originZ," +
            "combined_directionX,combined_directionY,combined_directionZ,combined_openess," +
            "left_pupil_diameter, left_originX,left_originY,left_originZ,left_directionX,left_directionY,left_directionZ,left_openess," +
            "right_pupil_diameter,right_originX,right_originY,right_originZ,right_directionX,right_directionY,right_directionZ,right_openess";

        // first row indicates when data collection started
        writer.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        writer.WriteLine(header);
    }

    private void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) 
        {
            return; }

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            Debug.Log("Eye data registered");
            SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    private void OnDisable()
    {
        Release();
    }

    void OnApplicationQuit()
    {
        Release();
    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private static void Release()
    {   
        if (eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
            
        }
        try
        {
            // end writing to file
            writer.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            writer.Close();
        }
        catch (Exception ex)
        {
            Debug.Log("File has been closed before");
            Debug.Log(ex.Message);
        }
            
    }

    /// <summary>
    /// Required class for IL2CPP scripting backend support
    /// </summary>
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    /// <summary>
    /// Eye tracking data callback thread.
    /// Reports data at ~120hz
    /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
    /// 
    /// Data description:
    ///     * bool no_user
    ///     * int frame sequence (?)
    ///     * int timesamp - in ms
    ///     * VerboseData
    ///         1. left | 2. right | 3. CombinedEyeData
    ///             - Vector3 gaze_origin
    ///             - Vector3 gaze_direction
    ///             - float pupil diameter (mm)
    ///             - float eye_openess
    ///             - Vector2 pupil position in sensor area
    ///             - bool get validity
    ///         4. TrackingImprovements (?)
    /// </summary>
    /// <param name="eye_data">Reference to latest eye_data</param>
    ///

    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData eye_data)
    {
        
        eyeData = eye_data;

        // do something with data
        writeFile(eye_data);

    }

    private static void writeFile(EyeData eye_data)
    {
        // timestamp
        int sample_timestamp = eyeData.timestamp;

        // combined
        float combined_pupil_diameter = eyeData.verbose_data.combined.eye_data.pupil_diameter_mm;
        Vector3 combined_origin = eyeData.verbose_data.combined.eye_data.gaze_origin_mm;
        Vector3 combined_direction = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;
        float combined_openess = eyeData.verbose_data.combined.eye_data.eye_openness;

        // left eye
        float left_pupil_diameter = eyeData.verbose_data.left.pupil_diameter_mm;
        Vector3 left_origin = eyeData.verbose_data.left.gaze_origin_mm;
        Vector3 left_direction = eyeData.verbose_data.left.gaze_direction_normalized;
        float left_openess = eyeData.verbose_data.left.eye_openness;

        // right eye
        float right_pupil_diameter = eyeData.verbose_data.right.pupil_diameter_mm;
        Vector3 right_origin = eyeData.verbose_data.right.gaze_origin_mm;
        Vector3 right_direction = eyeData.verbose_data.right.gaze_direction_normalized;
        float right_openess = eyeData.verbose_data.right.eye_openness;

        string data_row = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
            sample_timestamp, combined_pupil_diameter, vector3ToString(combined_origin), vector3ToString(combined_direction), combined_openess,
            left_pupil_diameter, vector3ToString(left_origin), vector3ToString(left_direction), left_openess,
            right_pupil_diameter, vector3ToString(right_origin), vector3ToString(right_direction), right_openess);

        writer.WriteLine(data_row);
    }

    private static string vector3ToString(Vector3 v3)
    {
        return string.Format("{0},{1},{2}", v3.x, v3.y, v3.z);
    }
}