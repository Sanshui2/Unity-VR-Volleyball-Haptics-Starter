//using UnityEngine;
//using Bhaptics.SDK2;

//public class CollisionPushback : MonoBehaviour
//{
//    public enum HandType { Left, Right }

//    [Header("Hand Settings")]
//    public HandType handType;

//    public float forceMagnitude = 10f; // 弹开的力度



//    private void OnCollisionEnter(Collision collision)
//    {
//        //Debug.Log(message: $"[触发检测] 碰到了物体：{collision.gameObject.name}");

//        // 检查被碰到的物体 (other) 的 Tag 是否是 "haha" (即排球的 Tag)

//        if (collision.gameObject.CompareTag("haha")) // 确认 haha 是排球的 Tag
//        {
//            float impactForce = collision.relativeVelocity.magnitude;

//           // Debug.Log($"[力度检测]{handType}手 击球！ 碰撞力度：{impactForce}");

//            float hapticIntensity = Remap(impactForce, minForceVelocity, maxForceVelocity, minHapticIntensity, maxHapticIntensity);

//            PlayHaptics(hapticIntensity);

//            //Debug.Log($"[匹配成功] 物体 {collision.gameObject.name} 的Tag是 'haha' (排球)。");


//            // --- 排球弹开逻辑 ---
//            Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();

//            if (hitRb != null && !hitRb.isKinematic)
//            {
                
//                Vector3 pushDirection = (collision.gameObject.transform.position - transform.position).normalized;

//                // 动态调整反弹力的大小
//                float dynamicForceMagnitude = Mathf.Clamp(impactForce, minForceVelocity, maxForceVelocity) * forceMagnitude;

//               // Debug.Log($"[添加力] 向方向 {pushDirection} 添加力 {dynamicForceMagnitude}");

//                hitRb.AddForce(pushDirection * dynamicForceMagnitude, ForceMode.Impulse);

//            }
//            else
//            {
//                Debug.Log($"[忽略]物体{collision.gameObject.name}的tag不是haha,不处理");

//            }

//        }
//    }


//    private void PlayHaptics(float intensity)
//    {
//        //触觉反馈开始
//        string eventIdToPlay;

//        switch (handType)
//        {
//            case HandType.Left:
//                eventIdToPlay = leftHapticsEventId;
//                break;
//            case HandType.Right:
//                eventIdToPlay = rightHapticsEventId;
//                break;
//            default:
//                //出现意外，报错
//                eventIdToPlay = "";
//                Debug.LogError("HandType 设置不正确");
//                break;
//        }


//        //确保选择的eventID不是空的
//        if (!string.IsNullOrEmpty(eventIdToPlay))
//        {
//            //Debug.Log($"[Bhaptics]尝试为{handType}手 播放触觉反馈 EventID：{eventIdToPlay}  动态强度：{intensity}");



//            // 严格按照文档使用 BhapticsLibrary.Play 方法
//            int requestId = BhapticsLibrary.Play(
//                eventIdToPlay,
//                hapticDelayMillis,
//                intensity,
//                hapticDuration,
//                hapticAngleX,
//                hapticOffsetY
//            );

//            if (requestId != -1)

//            {

//               // Debug.Log($"[Bhaptics] 已成功发送触觉反馈指令，请求ID: {requestId}。");

//            }

//            else

//            {

//                Debug.LogWarning($"[Bhaptics] 发送触觉反馈指令失败。请确保 bhaptics Player 正在运行,设备已配对，并且 Unity 中的 API Key 和 Event ID 配置正确。");

//            }

//            // --- 触觉反馈逻辑结束 ---
//        }
//    }

//    public static float Remap(float value, float form1, float to1, float from2, float to2)
//    {
//        value = Mathf.Clamp(value, form1, to1);
//        return (value - form1) / (to1 - form1) * (to2 - from2) + from2;

//    }
//}
