using Unity.Cinemachine;
using UnityEngine;

public class BK_CameraController : MonoBehaviour
{
    CinemachineOrbitalFollow orbitalFollow;

    [SerializeField] private float radiusScaleFactor = 5f;

    private void Awake()
    {
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    public void SetCameraRadius(float sphereRadius)
    {
        orbitalFollow.Radius = sphereRadius * radiusScaleFactor;
    }
}
