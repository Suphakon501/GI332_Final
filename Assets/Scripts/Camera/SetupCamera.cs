using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine; // ถ้าใช้ Cinemachine version ใหม่ (Unity 6+) ให้ใช้ Unity.Cinemachine
// using Cinemachine; // ถ้าใช้ Cinemachine version เก่า ให้ใช้ namespace นี้แทน

public class SetupCamera : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // ถ้าเป็นตัวละครของเราเอง
        if (IsOwner)
        {
            // ค้นหา Virtual Camera ในฉาก
            var vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();

            if (vcam != null)
            {
                // สั่งให้กล้อง Focus และ Follow มาที่เรือลำนี้
                vcam.Follow = transform;
                vcam.LookAt = transform;
            }
        }
    }
}