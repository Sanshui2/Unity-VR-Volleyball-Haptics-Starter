
using UnityEngine;
using Unity.XR.PXR;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Bhaptics.SDK2;

public class MotionTrackerManager: MonoBehaviour
{

 

    [Tooltip("请将场景中的 PXR_Origin 对象拖到这里")] 
    public Transform xrOriginTransform; 

    // 在Inspector中，手动勾选此项来触发获取位置的代码
    [Header("请在连接成功后，手动勾选此项")]
    public bool attemptToGetLocation = false;

    
    [Header("请将场景中的物体拖到这里")]
    public Transform visualCube;

    private bool isConnected = false;
    private long trackerId = 0;

    void Start()
    {
        Debug.Log("--- Tracker开始 ---");

        if (visualCube == null)
        {
            Debug.LogError("请在Inspector中指定Visual Cube!");
            return;
        }

        // 默认隐藏方块
        visualCube.gameObject.SetActive(false);

        // 注册回调函数
        PXR_MotionTracking.RequestMotionTrackerCompleteAction += OnRequestMotionTrackerComplete;

        // 请求一个Tracker
        Debug.Log("正在请求连接一个Tracker...");
        PXR_MotionTracking.CheckMotionTrackerNumber(MotionTrackerNum.ONE);
    }

    private void OnRequestMotionTrackerComplete(RequestMotionTrackerCompleteEventData eventData)
    {
        // 步骤A成功：连接成功
        if (eventData.trackerCount > 0)
        {
            isConnected = true;
            trackerId = eventData.trackerIds[0];
            Debug.Log($"--- 连接成功！Tracker ID: {trackerId} ---");
            Debug.Log("现在可以在Inspector中勾选 'Attempt To Get Location' 来尝试获取位置了。");
        }
        else
        {
            Debug.LogError("--- 连接失败，没有找到Tracker ---");
        }
    }

    void Update()
    {
        // 如果还没连接成功，就什么都不做
        if (!isConnected)
        {
            return;
        }

        // 步骤A成功后，会持续打印此信息，证明App稳定运行
        // Debug.Log("Update循环正常运行，等待指令...");

        // 安全门：只有当用户手动勾选后，才执行下面的危险代码
        if (!attemptToGetLocation)
        {
            return;
        }

        // --- 步骤B：尝试获取位置 ---
        Debug.LogWarning("--- 开始尝试获取位置 (调用GetMotionTrackerLocation) ---");

        MotionTrackerLocation location = new MotionTrackerLocation();
        bool isValidPose = false;

        int result = PXR_MotionTracking.GetMotionTrackerLocation(trackerId, ref location, ref isValidPose);

        if (result == 0 && isValidPose)
        {
            Debug.Log($"获取位置成功! Pos: {location.pose.Position.ToVector3().ToString("F3")}");
            if (!visualCube.gameObject.activeSelf)
            {
                visualCube.gameObject.SetActive(true);
            }

            // 1. 获取Tracker相对于XR Origin的局部位置和旋转
            Vector3 trackerLocalPosition = location.pose.Position.ToVector3();
            Quaternion trackerLocalRotation = location.pose.Orientation.ToQuat();

            // 2. 使用xrOriginTransform，将局部坐标转换为世界坐标
            Vector3 trackerWorldPosition = xrOriginTransform.TransformPoint(trackerLocalPosition);
            Quaternion trackerWorldRotation = xrOriginTransform.rotation * trackerLocalRotation;

            // 3. 将计算出的世界坐标和旋转，赋值给物体
            visualCube.position = trackerWorldPosition; // 使用 .position 设置世界位置
            visualCube.rotation = trackerWorldRotation; // 使用 .rotation 设置世界旋转


        }
        else
        {
            if (Time.frameCount % 60 == 0) // 避免刷屏
            {
                Debug.LogError($"获取位置失败! Result: {result}, IsValidPose: {isValidPose}");
            }
        }
    }

    void OnDestroy()
    {
        // 清理
        PXR_MotionTracking.RequestMotionTrackerCompleteAction -= OnRequestMotionTrackerComplete;
    }
}