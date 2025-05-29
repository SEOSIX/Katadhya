using System.Collections;
using UnityEngine;

public static class StartingScene
{
    public class MovementConfig
    {
        public float baseDuration = 1.0f;
        public float screenMargin = 0.1f; 
    }

    private static MovementConfig config = new MovementConfig();
    private static CoroutineHost host;
    
    static StartingScene()
    {
        InitializeHost();
    }


    #region Interface Principale
    public static void MoveFromLeft(GameObject[] objects, Vector3[] targets)
    {
        if (objects.Length != targets.Length) return;
    
        for (int i = 0; i < objects.Length; i++)
        {
            ExecuteMovement(new GameObject[] { objects[i] }, targets[i], true, config.baseDuration);
        }
    }

    public static void MoveFromRight(GameObject[] objects, Vector3[] targets)
    {
        if (objects.Length != targets.Length) return;
    
        for (int i = 0; i < objects.Length; i++)
        {
            ExecuteMovement(new GameObject[] { objects[i] }, targets[i], false, config.baseDuration);
        }
    }

    public static void CustomMove(GameObject[] objects, Vector3 target, bool fromLeft, float duration)
    {
        ExecuteMovement(objects, target, fromLeft, duration);
    }
    #endregion

    #region Moteur Interne
    private static void ExecuteMovement(GameObject[] objects, Vector3 target, bool fromLeft, float duration)
    {
        host.StartCoroutine(MovementProcess(
            objects, 
            target, 
            fromLeft, 
            duration > 0 ? duration : config.baseDuration
        ));
    }

    private static IEnumerator MovementProcess(GameObject[] objects, Vector3 target, bool fromLeft, float duration)
    {
        var startPositions = new System.Collections.Generic.List<Vector3>();
        Vector3 screenEdge = CalculateEdgePosition(fromLeft, target);

        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.transform.position = screenEdge;
                startPositions.Add(screenEdge);
            }
        }
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed/duration);
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].transform.position = Vector3.Lerp(
                        startPositions[i], 
                        target, 
                        t
                    );
                }
            }
            yield return null;
        }
        foreach (var obj in objects)
        {
            if (obj != null)
                obj.transform.position = target;
        }
    }

    private static Vector3 CalculateEdgePosition(bool leftSide, Vector3 reference)
    {
        Camera cam = Camera.main;
        if (!cam) return reference;

        Vector3 viewportPos = new Vector3(
            leftSide ? -config.screenMargin : 1 + config.screenMargin,
            0.5f,
            cam.WorldToViewportPoint(reference).z
        );

        Vector3 worldPos = cam.ViewportToWorldPoint(viewportPos);
        return new Vector3(worldPos.x, reference.y, reference.z);
    }
    #endregion

    #region Gestion des Coroutines
    private static void InitializeHost()
    {
        if (host != null) return;

        GameObject hostObject = new GameObject("MovementSystem_Host");
        hostObject.hideFlags = HideFlags.HideInHierarchy;
        host = hostObject.AddComponent<CoroutineHost>();
        Object.DontDestroyOnLoad(hostObject);
    }
    private class CoroutineHost : MonoBehaviour { }
    #endregion
}