using System.Collections.Generic;
using UnityEngine;

public class ObstacleFader : MonoBehaviour
{
    [SerializeField] private Camera targetCamera = default;
    [SerializeField] private Transform targetTransform = default;
    [SerializeField] private Vector3 rayOffset = default;
    [SerializeField, Range(0.1f, 2f)] private float targetSkin = 1f;
    [SerializeField] private bool debugRay;

    private const int ObstacleLayerMask = 1 << 11;

    private List<Fadeable> fadeableList = new List<Fadeable>();

    private void LateUpdate()
    {
        FadeObstacles();
    }

    private void FadeObstacles()
    {
        float distance = Vector3.Distance(targetCamera.transform.position, targetTransform.position) - targetSkin;
        Ray ray = targetCamera.ScreenPointToRay(targetTransform.position + new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f) + rayOffset);
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, ObstacleLayerMask);
        HashSet<Fadeable> newFadeableSet = new HashSet<Fadeable>();
        int originalCount = fadeableList.Count;
        foreach (RaycastHit hit in hits)
        {
            Fadeable obstacle = hit.transform.GetComponent<Fadeable>();
            if (obstacle != null)
            {
                newFadeableSet.Add(obstacle);
                if (!obstacle.IsFaded)
                {
                    StartCoroutine(obstacle.FadeOut());
                    fadeableList.Add(obstacle);
                }
            }
        }
        for (int i = originalCount - 1; i >= 0; i--)
        {
            Fadeable obstacle = fadeableList[i];
            if (obstacle.IsFaded && !newFadeableSet.Contains(obstacle))
                StartCoroutine(obstacle.FadeIn());
        }

        if (debugRay)
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
    }
}
