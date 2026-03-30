using UnityEngine;
using Unity.XR.PXR;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 中央管理器，负责连接和分配两个Motion Tracker。
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    public enum HandType { None,Left,Right}

    [Header("Tracker代理对象引用")]
    [Tooltip("代表左手Tracker的代理对象")]
    public TrackerProxy leftTrackerProxy;
    [Tooltip("代表右手Tracker的代理对象")]
    public TrackerProxy rightTrackerProxy;
    [Header("校准设置")]
    [Tooltip("将PXR_Origin下的主摄像机拖到这里")]
    public Transform headTransform;
    [Tooltip("校准时，双手需要保持的最小水平距离（米）")]
    public float calibrationMinSeparation = 0.2f;






    void Start()
    {
        if (leftTrackerProxy == null || rightTrackerProxy == null)
        {
            Debug.LogError("请在Inspector中设置左右手的Tracker Proxy!");
            return;
        }

        // 注册回调函数
        PXR_MotionTracking.RequestMotionTrackerCompleteAction += OnRequestMotionTrackerComplete;
        // 请求连接两个Tracker
        PXR_MotionTracking.CheckMotionTrackerNumber(MotionTrackerNum.TWO);
        Debug.Log("正在请求连接两个Motion Tracker...");
    }

    private void OnRequestMotionTrackerComplete(RequestMotionTrackerCompleteEventData eventData)
    {
        if (eventData.trackerCount < 2)
        {
            Debug.LogError("未能连接到两个Tracker！请检查Tracker状态。");
            return;
        }

        Debug.Log("<color=cyan>成功连接到两个Tracker！正在初始化代理...</color>");

        // 将获取到的Tracker ID分配给左右手代理
        //启动携程校准
        StartCoroutine(CalibrateHandsCoroutine(eventData.trackerIds[0], eventData.trackerIds[1]));
    }


    private IEnumerator CalibrateHandsCoroutine(long id1,long id2)
    {
        while (true)
        {
            MotionTrackerLocation location1 = new MotionTrackerLocation();
            MotionTrackerLocation location2 = new MotionTrackerLocation();
            bool isValid1 = false;
            bool isValid2 = false;
            PXR_MotionTracking.GetMotionTrackerLocation(id1, ref location1, ref isValid1);
            PXR_MotionTracking.GetMotionTrackerLocation(id2, ref location2, ref isValid2);

            // 必须两个Tracker都有效才进行下一步
            if (isValid1 && isValid2)
            {
                // 将两个Tracker的位置从XR Origin的局部坐标，转换到世界坐标
                Vector3 worldPos1 = transform.TransformPoint(location1.pose.Position.ToVector3());
                Vector3 worldPos2 = transform.TransformPoint(location2.pose.Position.ToVector3());

                // 再将世界坐标，转换到以头显为中心和朝向的局部坐标
                Vector3 localToHead1 = headTransform.InverseTransformPoint(worldPos1);
                Vector3 localToHead2 = headTransform.InverseTransformPoint(worldPos2);

                // 检查双手在水平方向上是否已明显分开
                if (Mathf.Abs(localToHead1.x - localToHead2.x) > calibrationMinSeparation)
                {
                    // X值更小（更靠左）的是左手
                    if (localToHead1.x < localToHead2.x)
                    {
                        leftTrackerProxy.Initialize(id1, transform);
                        rightTrackerProxy.Initialize(id2, transform);
                        Debug.Log("<color=green>左右手校准成功！</color>");
                    }
                    else
                    {
                        leftTrackerProxy.Initialize(id2, transform);
                        rightTrackerProxy.Initialize(id1, transform);
                        Debug.Log("<color=green>左右手校准成功！</color>");
                    }

                    yield break; // 校准成功，退出协程
                }
            }

            // 如果校准未成功，则等待下一帧再试
            yield return null;
        }
    }


   

    void OnDestroy()
    {
        PXR_MotionTracking.RequestMotionTrackerCompleteAction -= OnRequestMotionTrackerComplete;
    }
}