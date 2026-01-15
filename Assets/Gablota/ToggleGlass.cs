using System.Collections.Generic;
using UnityEngine;

namespace Gablota
{

public class ToggleGlass : MonoBehaviour
{
    [Header("Distance Bounds")]
    public float cutoffDist = 1f;
    public float minDist = 2f;
    public float maxDist = 10f;

    [Header("Centering Bounds (Dot Product)")]
    [Range(0, 1)] public float centerThreshold = 0.9f;
    [Range(0, 1)] public float edgeThreshold = 0.7f;

    public float maxAlpha = 0.5f;

    private List<MeshRenderer> glassRenderers = new List<MeshRenderer>();
    private Transform playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (!Camera.main)
        {
            Debug.LogError("No main camera found!");
            return;
        }
        playerCamera = Camera.main.transform;

        // Find all children with the "Glass" tag and cache their components
        foreach (var child in GetComponentsInChildren<Transform>())
        {
            if (!child.CompareTag("Glass"))
            {
                continue;
            }

            if (child.TryGetComponent<MeshRenderer>(out var ren)) glassRenderers.Add(ren);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!playerCamera) return;

        float visibility = 1f;

        foreach (var ren in glassRenderers)
        {
            float dist = Vector2.Distance(new Vector2(playerCamera.position.x, playerCamera.position.z), new Vector2(ren.transform.position.x, ren.transform.position.z));
            if (dist <= cutoffDist)
            {
                visibility = 0f;
                break;
            }
            if (dist >= maxDist) continue;

            Vector3 dirToGlass = (ren.transform.position - playerCamera.position);
            dirToGlass.y = 0f;
            float dotProduct = Vector3.Dot(new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized, dirToGlass.normalized);
            if (dotProduct <= edgeThreshold) continue;

            float normalizedDist = Mathf.InverseLerp(minDist, maxDist, dist);
            float normalizedAngle = Mathf.InverseLerp(edgeThreshold, centerThreshold, dotProduct);

            float suggestedVisibility = Mathf.Lerp(1f, normalizedDist, normalizedAngle);
            if (suggestedVisibility < visibility) visibility = suggestedVisibility;
        }
        ApplyAlpha(visibility * maxAlpha);
    }

    private void ApplyAlpha(float alpha)
    {
        if (alpha <= 0f)
        {
            foreach (var ren in glassRenderers) ren.enabled = false;
            return;
        }
        foreach (var ren in glassRenderers)
        {
            var c = ren.material.color;
            ren.material.color = new Color(c.r, c.g, c.b, alpha);
            ren.enabled = true;
        }
    }
}

}