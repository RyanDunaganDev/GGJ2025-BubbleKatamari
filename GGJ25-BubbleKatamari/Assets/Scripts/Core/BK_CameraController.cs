using Unity.Cinemachine;
using UnityEngine;

public class BK_CameraController : MonoBehaviour
{
    CinemachineOrbitalFollow orbitalFollow;

    private float currentRadius = 1f;

    private void Awake()
    {
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    public void IncreaseCameraRadius(float sphereRadius)
    {
        currentRadius += sphereRadius;
        orbitalFollow.Radius = currentRadius * 3f;
    }
}
