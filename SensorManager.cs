using UnityEngine;
using UnityEngine.Android;
using System.Collections;
public static class SensorManager
{
    /// <summary>
    ///위치 서비스를 실행시키는 메서드
    /// </summary>
    /// <param name="maxWait">서비스 초기화에 걸리는 최대 대기 시간(초 단위)</param>
    public static IEnumerator StartService(int maxWait)
    {
        if (!isCheck)
        {
            Check();
            yield return new WaitUntil(() => isCheck);
        }
        Input.location.Start();
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
            yield break;

        if (Input.location.status == LocationServiceStatus.Failed)
            yield break;
        Input.compass.enabled = true;
        Input.gyro.enabled = true;
    }
    /// <summary>
    ///허용을 요청하는 메서드
    /// </summary>
    public static void Check()
    {
        if (Application.platform == RuntimePlatform.Android)
            Permission.RequestUserPermission(Permission.FineLocation);
    }
    /// <summary>
    ///위치정보를 허용했는지 확인하는 메서드
    /// </summary>
    /// <returns>
    ///위치 허용 여부
    ///</returns>
    public static bool isCheck
    {
        get
        {
            return Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        }
    }
    /// <summary>
    ///위치정보가 제대로 실행되는지 확인하는 메서드
    /// </summary>
    /// <returns>
    ///위치 정보 서비스가 실행 중인지 나타내는 여부
    ///</returns>
    public static bool isEnabled
    {
        get
        {
            return Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running;
        }
    }
    /// <summary>
    ///GPS 클래스
    /// </summary>
    public static class GPS
    {
        /// <summary>
        ///위도 프로퍼티
        /// </summary>
        /// <returns>
        ///위도
        ///</returns>
        public static float lat
        {
            get
            {
                return Input.location.lastData.latitude;
            }
        }
        /// <summary>
        ///경도 프로퍼티
        /// </summary>
        /// <returns>
        ///경도
        ///</returns>
        public static float lon
        {
            get
            {
                return Input.location.lastData.longitude;
            }
        }
        /// <summary>
        ///고도 프로퍼티
        /// </summary>
        /// <returns>
        ///고도
        ///</returns>
        public static float alt
        {
            get
            {
                return Input.location.lastData.altitude;
            }
        }
        /// <summary>
        ///현재 위치를 2D로 구현한 프로퍼티
        /// </summary>
        /// <returns>
        ///현재 위치(2D)
        ///</returns>
        public static Vector2 position2
        {
            get
            {
                return new Vector2(lon, lat);
            }
        }
        /// <summary>
        ///현재 위치를 3D로 구현한 프로퍼티
        /// </summary>
        /// <returns>
        ///현재 위치(3D)
        ///</returns>
        public static Vector3 position3
        {
            get
            {
                return new Vector3(lon, lat, alt);
            }
        }

        /// <summary>
        /// 목표 위치와 현재 위치의 거리를 계산하는 메서드(2D)
        /// </summary>
        /// <param name="targetLat">목표 위치의 위도</param>
        /// <param name="targetLon">목표 위치의 경도</param>
        /// <returns>현재 위치와 목표 위치의 거리(단위 : 미터)</returns>
        public static float Distance2D(float targetLat, float targetLon)
        {
            var targetLocation = new Vector2(targetLat, targetLon);

            return Vector2.Distance(position2, targetLocation) * 111000; 
        }
        /// <summary>
        /// 목표 위치와 현재 위치의 거리를 계산하는 메서드(2D)
        /// </summary>
        /// <param name="target">목표 위치</param>
        /// <returns>현재 위치와 목표 위치의 거리(단위 : 미터)</returns>
        public static float Distance2D(Vector2 target)
        {
            return Vector2.Distance(position2, target) * 111000;
        }

        /// <summary>
        /// 목표 위치와 현재 위치의 거리를 계산하는 메서드(3D)
        /// </summary>
        /// <param name="targetLat">목표 위치의 위도</param>
        /// <param name="targetLon">목표 위치의 경도</param>
        /// <param name="targetAlt">목표 위치의 고도</param>
        /// <returns>현재 위치와 목표 위치의 거리(단위 : 미터)</returns>
        public static float Distance3D(float targetLat, float targetLon,float targetAlt)
        {
            var targetLocation = new Vector3(targetLat, targetLon,targetAlt);

            return Vector3.Distance(position3, targetLocation) * 111000; 
        }
        /// <summary>
        /// 목표 위치와 현재 위치의 거리를 계산하는 메서드(3D)
        /// </summary>
        /// <param name="target">목표 위치</param>
        /// <returns>현재 위치와 목표 위치의 거리(단위 : 미터)</returns>
        public static float Distance3D(Vector3 target)
        {
            return Vector3.Distance(position3, target) * 111000;
        }
    }
    /// <summary>
    ///나침반 클래스
    /// </summary>
    public static class Compass
    {
        private static bool m = false;
        /// <summary>
        ///자북 진북을 설정하는 프로퍼티 true는 자북 false는 진북(기본값 : 진북)
        /// </summary>
        public static bool magnetic
        {
            set
            {
                m = value;
            }
        }
        /// <summary>
        ///현재 각도를 변수값으로 반환하는 프로퍼티
        /// </summary>
        /// <returns>
        ///현재 각도
        ///</returns>
        public static float rotateValue
        {
            get
            {
                return m ? Input.compass.magneticHeading : Input.compass.trueHeading;
            }
        }
        /// <summary>
        ///현재 각도를 2D의 각도로 변환한 프로퍼티
        /// </summary>
        /// <returns>
        ///new Vector3(현재 각도,0,0)
        ///</returns>
        public static Vector3 rotate2D
        {
            get
            {
                return new Vector3(m ? Input.compass.magneticHeading : Input.compass.trueHeading, 0, 0);
            }
        }
        /// <summary>
        ///현재 각도를 3D의 각도로 변환한 프로퍼티
        /// </summary>
        /// <returns>
        ///new Vector3(0,현재 각도,0)
        ///</returns>
        public static Vector3 rotate3D
        {
            get
            {
                return new Vector3(0, m ? Input.compass.magneticHeading : Input.compass.trueHeading, 0);
            }
        }
    }

    /// <summary>
    ///자이로 센서 클래스
    /// </summary>
    public static class Gyro
    {
        private static Vector3 previousRotationRate;
        private static float movementThreshold = 0.1f;

        /// <summary>
        ///현재 자이로 센서값을 Vector3로 반환한 프로퍼티
        /// </summary>
        /// <returns>
        ///new Vector3(현재 자이로 센서값)
        ///</returns>
        public static Vector3 to_Vector3
        {
            get
            {
                return Input.gyro.attitude.eulerAngles;
            }
        }

        /// <summary>
        ///현재 자이로 센서값을 Quaternion로 반환한 프로퍼티
        /// </summary>
        /// <returns>
        ///new Quaternion(현재 자이로 센서값)
        ///</returns>
        public static Quaternion to_Quaternion
        {
            get
            {
                return Input.gyro.attitude;
            }
        }

        /// <summary>
        ///휴대폰이 움직였는지 안움직였는지 판단하는 프로퍼티
        /// </summary>
        /// <returns>
        ///움짇임(true), 안움직임(false)
        ///</returns>

        public static bool isMove
        {
            get
            {
                Vector3 currentRotationRate = Input.gyro.rotationRate;
                float difference = Vector3.Distance(previousRotationRate, currentRotationRate);
                previousRotationRate = currentRotationRate;
                return difference > movementThreshold;
            }
        }
    }
}
