using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour{
    private readonly float minCameraDistance = 5.0f;
    private readonly float maxCameraDistance = 10.0f;
    private float cameraDistance = 5.0f;
    
    [SerializeField] private Camera mainCamera;
    
    [Range(0.0f, 10.0f)]
    [SerializeField] private float distanceValue = 1.0f;

    public void Initialize(){
        mainCamera = Camera.main;
    }

    private void Update(){
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, FarthestPlayerDistance(), Time.deltaTime * 2);
    }

    /// <summary>
    /// 중앙으로부터 떨어진 거리만큼 확장 (5 ~ 10 사이)
    /// </summary>
    /// <returns></returns>
    float FarthestPlayerDistance(){
        var maxDistance = minCameraDistance;
        foreach (var player in Spawner.playerList){
            var targetDistance = Vector3.Distance(Vector3.zero, player.transform.position) + distanceValue;
            
            maxDistance = Mathf.Max(maxDistance, targetDistance);
        }
        
        return Mathf.Min(maxDistance, maxCameraDistance);
    }

    /// <summary>
    /// 해당 캐릭터 위치로 이동
    /// TODO: 캐릭터 ScriptableObject에서 캐릭터를 구분하는 기능 삽입
    /// </summary>
    private void MoveCameraToPlayer(){
        
    }

}
