using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using UnityEngine.Android;
using Unity.VisualScripting;
using UnityEngine.UI;

/// <summary>
/// Singleton Class
/// Handles initialization and error state management of the AR Core Session, AR Core Extensions, and Earth Manager.
/// Contains public properties for and events raised on error state and geospatial tracking/accuracy changes.
/// Public methods are provided for getting new Geospatial Anchors.
/// </summary>
/// <summary>
/// All possible error states. Used to inform other components' behaviors.
/// </summary>
[System.Flags] public enum ErrorState { Null = 0, NoError = 1, Tracking = 2, Message = 4, Camera = 8, Location = 16 }

public class GeospatialManager : Singleton<GeospatialManager>
{
    [Header("[ AR Components ]")]
    public ARSessionOrigin SessionOrigin;
    public ARSession Session;
    public ARAnchorManager AnchorManager;
    public AREarthManager EarthManager;
    public ARCoreExtensions ARCoreExtensions;

    /// <summary>
    /// True while Earth Manager is tracking and accuracy minimums are met
    /// </summary>
    public bool IsTracking { get => _trackingValid; }

    /// <summary>
    /// True once we've reached target accuracy and for the remainder of the session
    /// </summary>
    public bool IsAccuracyTargetReached { get => _targetAccuracyReached; }

    /// <summary>
    /// The current error message, if there is one
    /// </summary>
    public string CurrentErrorMessage { get => _errorMessage; }

    /// <summary>
    /// Best horizontal accuracy value reached at any point during the current session
    /// </summary>
    public double BestHorizontalAccuracy { get => _bestHorizontalAccuracy; }

    /// <summary>
    /// Best heading accuracy value reached at any point during the current session
    /// </summary>
    public double BestHeadingAccuracy { get => _bestYawAccuracy; }

    /// <summary>
    /// Best altitude accuracy value reached at any point during the current session
    /// </summary>
    public double BestVerticalAccuracy { get => _bestVerticalAccuracy; }

    /// <summary>
    /// Current error state enum
    /// </summary>
    public ErrorState CurrentErrorState { get => _errorState; }

    /// <summary>
    /// Raised once when all components are ready
    /// </summary>
    [HideInInspector] public UnityEvent InitCompleted;

    /// <summary>
    /// Raised on any frame that accuracy has reached better values than any previous 
    /// </summary>
    [HideInInspector] public UnityEvent AccuracyImproved;

    /// <summary>
    /// Raised once when the specified target accuracy values are reached
    /// </summary>
    [HideInInspector] public UnityEvent TargetAccuracyReached;

    /// <summary>
    /// Raised on any frame that there is a change in error state
    /// Includes the error state enum and error message string if applicable
    /// </summary>
    [HideInInspector] public UnityEvent<ErrorState, string> ErrorStateChanged;

    [Header("[ Accuracy Minimums ] - Required to start experience")]
    [SerializeField] private float _minimumHorizontalAccuracy = 10;
    [SerializeField] private float _minimumYawAccuracy = 15;
    [SerializeField] private float _minimumVerticalAccuracy = 1.5f;

    [Header("[ Accuracy Targets ] - Event raised when reached")]
    [SerializeField] private float _targetHorizontalAccuracy = 1;
    [SerializeField] private float _targetYawAccuracy = 2;
    [SerializeField] private float _targetVerticalAccuracy = 0.5f;

    [Header("[ Settings ]")]
    [SerializeField] private float _initTime = 3f;

    private ErrorState _errorState = ErrorState.NoError;
    private string _errorMessage;
    private double _bestHorizontalAccuracy = Mathf.Infinity;
    private double _bestYawAccuracy = Mathf.Infinity;
    private double _bestVerticalAccuracy = Mathf.Infinity;
    private bool _trackingValid,
                 _enablingGeospatial,
                 _initComplete,
                 _targetAccuracyReached,
                 _requestCamPerm,
                 _requestLocPerm,
                 _startedAR;

    [Header("[ Debug ]")]
    [SerializeField] private Text errorMassage;
    [SerializeField] private Text latitudeDebug;
    [SerializeField] private Text longtitudeDebug;
    [SerializeField] private Text altitudeDebug;
    [SerializeField] private Text yawDebug;
    [SerializeField] private Text dotDebug;
    [HideInInspector] public float dotParameter;
    internal readonly object dotProduct;

    private void Start()
    {
        SetErrorState(ErrorState.NoError);

#if UNITY_IOS && !UNITY_EDITOR
            Debug.Log("Start location services.");
            Input.location.Start();
#endif
    }

    private void Update()
    {
        // check the camer & location permission, if not permission continue ask
        if (!CheckCameraPermission())
            return;

        if (!CheckLocationPermission())
            return;

        // if user allow camera & location permission
        // start the AR
        if (!_startedAR)
        {
            // activate the ARSession, Session Origin and AR Core
            SessionOrigin.gameObject.SetActive(true);
            Session.gameObject.SetActive(true);
            ARCoreExtensions.gameObject.SetActive(true);
            _startedAR = true;
        }

        // check if ar session works
        UpdateSessionState();

        //keep check session, if the session is not working
        if (ARSession.state != ARSessionState.SessionInitializing &&
            ARSession.state != ARSessionState.SessionTracking)
        {
            return;
        }

        //check if GeospatialMode is suported on the device
        FeatureSupported featureSupport = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);

        switch (featureSupport)
        {
            case FeatureSupported.Unknown:
                SetErrorState(ErrorState.Message, "Geospatial API encountered an unknown error.");
                return;
            case FeatureSupported.Unsupported:
                SetErrorState(ErrorState.Message, "Geospatial API is not supported by this device.");
                enabled = false;
                return;
            case FeatureSupported.Supported:
                //Enable the Geospatial in AR Extension config
                if (ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode == GeospatialMode.Disabled)
                {
                    SetErrorState(ErrorState.Message, "Enabling Geospatial Mode...");
                    ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode =
                        GeospatialMode.Enabled;
                    _enablingGeospatial = true;
                    return;
                }
                break;
        }

        /// Waiting for new configuration taking effect
        ///
        if (_enablingGeospatial)
        {
            _initTime -= Time.deltaTime;

            if (_initTime < 0)
                _enablingGeospatial = false;
            else
                return;
        }

        /// Check earth state
        EarthState earthState = EarthManager.EarthState;

        if (earthState == EarthState.ErrorEarthNotReady)
        {
            SetErrorState(ErrorState.Message, "Error:Initializing Geospatial functionalities.");
            return;
        }
        else if (earthState != EarthState.Enabled)
        {
            SetErrorState(ErrorState.Message, "Error: Unable to start Geospatial AR" + earthState.ToString());
            enabled = false;
            return;
        }


#if UNITY_IOS && !UNITY_EDITOR
            bool isSessionReady = ARSession.state == ARSessionState.SessionTracking &&
            Input.location.status == LocationServiceStatus.Running;
#else
        bool isSessionReady = ARSession.state == ARSessionState.SessionTracking;

#endif

        /// **** Init Complete ****
        if (!_initComplete)
        {
            InitCompleted.Invoke();
            _initComplete = true;
            SetErrorState(ErrorState.Tracking);
        }

        //if the geospacial yaw offset is acceptable
        if (TrackingIsValid())
        {
            // if accuracy is improved
            if (CheckAccuracyImproved())
            {
                /// Raise event if accuracy has improved since last check
                AccuracyImproved.Invoke();
            }

            // if reach the accuracy range
            if (!_targetAccuracyReached && CheckTargetAccuracyReached())
            {
                Debug.Log("** Target Accuracy Reached!! **");
                /// Raise event if target accuracy reached
                TargetAccuracyReached.Invoke();
                _targetAccuracyReached = true;
            }
        }

        DebugText();
    }

    /// <summary>
    /// Ensure we have Camera usage permission
    /// </summary>
    /// <returns></returns>
    private bool CheckCameraPermission()
    {
        // if user hase not authorized the camera permission
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // if now the error state is not camera, change the error state to camera
            if (_errorState != ErrorState.Camera)
                SetErrorState(ErrorState.Camera);

            // if haven't request camera permission, then request
            if (!_requestCamPerm) Permission.RequestUserPermission(Permission.Camera);
            _requestCamPerm = true;
            return false;
        }

        //if not have permission issue, then there is no error
        if (_errorState == ErrorState.Camera)
            SetErrorState(ErrorState.NoError);

        return true;
    }

    /// <summary>
    /// Ensure we have Location usage permission
    /// </summary>
    /// <returns></returns>
    private bool CheckLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            if (_errorState != ErrorState.Location)
                SetErrorState(ErrorState.Location);

            if (!_requestLocPerm) Permission.RequestUserPermission(Permission.FineLocation);
            _requestLocPerm = true;
            return false;
        }

        if (_errorState == ErrorState.Location)
            SetErrorState(ErrorState.NoError);

        return true;
    }

    /// <summary>
    /// Monitor the state of the AR session
    /// </summary>
    private void UpdateSessionState()
    {
        /// Pressing 'back' button quits the app.
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        /// Only allow the screen to sleep when not tracking.
        var sleepTimeout = SleepTimeout.NeverSleep;
        if (ARSession.state != ARSessionState.SessionTracking)
        {
            sleepTimeout = SleepTimeout.SystemSetting;
        }

        Screen.sleepTimeout = sleepTimeout;

        /// ARSession Status
        if (ARSession.state != ARSessionState.CheckingAvailability &&
            ARSession.state != ARSessionState.Ready &&
            ARSession.state != ARSessionState.SessionInitializing &&
            ARSession.state != ARSessionState.SessionTracking)
        {
            Debug.Log("ARSession error state: " + ARSession.state);
            SetErrorState(ErrorState.Message, "AR Error Encountered: " + ARSession.state);
            enabled = false;
        }

#if UNITY_IOS && !UNITY_EDITOR
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                SetErrorState(ErrorState.Message, "Please start the app again and grant precise location permission.");
            }
#endif
        else if (SessionOrigin == null || Session == null || ARCoreExtensions == null)
        {
            Debug.Log("Missing AR Components.");
            SetErrorState(ErrorState.Message, "Error: Something Went Wrong");
            return;
        }
    }

    /// <summary>
    /// Set error state and raise event if needed
    /// </summary>
    /// <param name="errorState"></param>
    /// <param name="message"></param>
    public void SetErrorState(ErrorState errorState, string message = null)
    {
        if (_errorState != errorState)
        {
            _errorState = errorState;
            ErrorStateChanged.Invoke(_errorState, message);
        }

        errorMassage.text = message;
    }

    /// <summary>
    /// Returns whether or not both conditions are true:
    /// <list type="bullet"><item>
    /// Earth Manager is tracking correctly</item><item>
    /// Current accuracy meets the specified minimums</item></list>
    /// Sets error state appropriately.
    /// </summary>
    /// <returns></returns>
    private bool TrackingIsValid()
    {
        bool valid = false;

        if (!valid && EarthManager.EarthTrackingState == TrackingState.Tracking)
        {
            /// Have we met the minimums?
            valid = EarthManager.CameraGeospatialPose.OrientationYawAccuracy <= _minimumYawAccuracy &&
                    EarthManager.CameraGeospatialPose.VerticalAccuracy <= _minimumVerticalAccuracy &&
                    EarthManager.CameraGeospatialPose.HorizontalAccuracy <= _minimumHorizontalAccuracy;
        }

        if (valid != _trackingValid)
        {
            _trackingValid = valid;
            SetErrorState(_trackingValid ? ErrorState.NoError : ErrorState.Tracking);
        }

        return valid;
    }

    /// <summary>
    /// Compare current tracking accuracy against best values.
    /// Return whether or not accuracy has improved since the last check.
    /// </summary>
    /// <returns></returns>
    private bool CheckAccuracyImproved()
    {
        bool horizontal = EarthManager.CameraGeospatialPose.HorizontalAccuracy < _bestHorizontalAccuracy;
        bool heading = EarthManager.CameraGeospatialPose.OrientationYawAccuracy < _bestYawAccuracy;
        bool vertical = EarthManager.CameraGeospatialPose.VerticalAccuracy < _bestVerticalAccuracy;

        bool improved = false;

        if (horizontal)
        {
            improved = true;
            _bestHorizontalAccuracy = EarthManager.CameraGeospatialPose.HorizontalAccuracy;
        }
        if (heading)
        {
            improved = true;
            _bestYawAccuracy = EarthManager.CameraGeospatialPose.OrientationYawAccuracy;
        }
        if (vertical)
        {
            improved = true;
            _bestVerticalAccuracy = EarthManager.CameraGeospatialPose.VerticalAccuracy;
        }

        return improved;
    }

    /// <summary>
    /// Return whether or not we've reached our specified target tracking accuracy values
    /// </summary>
    /// <returns></returns>
    private bool CheckTargetAccuracyReached()
    {
        return EarthManager.CameraGeospatialPose.HorizontalAccuracy <= _targetHorizontalAccuracy &&
               EarthManager.CameraGeospatialPose.OrientationYawAccuracy <= _targetYawAccuracy &&
               EarthManager.CameraGeospatialPose.VerticalAccuracy <= _targetVerticalAccuracy;
    }

    private void DebugText()
    {
        GeospatialPose pose = EarthManager.CameraGeospatialPose;
        latitudeDebug.text = "Latitude: "+ pose.Latitude.ToString();
        longtitudeDebug.text = "Longtitude: " + pose.Longitude.ToString();
        altitudeDebug.text = "Altitude: " + pose.Altitude.ToString();
        yawDebug.text = "Yaw:" + pose.EunRotation.ToString();
    }

    public void DebugDot(float dot)
    {
        dotParameter = dot;
        dotDebug.text = "Dit Product:" + dot.ToString();
    }
}
