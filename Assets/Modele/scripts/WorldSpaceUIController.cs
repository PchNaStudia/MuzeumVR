using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WorldSpaceUIController : MonoBehaviour
{
    [Header("UI Data")]
    [SerializeField] public string title = "Lorem ipsum";
    [SerializeField] public string body = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
    [SerializeField] public Sprite icon;

    [Header("Visibility Settings")]
    [SerializeField] private float maxDistance = 2.0f;
    [SerializeField] private float viewAngleLimit = 0.8f;

    [Header("UI References(do not change)")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image profileImage;
    [SerializeField] private Canvas canvas;

    [Header("Orientation Fix")]
    [Tooltip("Adjust this if the UI is upside down or backwards (e.g., Y = 180)")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    private XRGrabInteractable parentGrabScript;
    private Transform mainCamera;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
        if (titleText != null) titleText.text = title;
        if (bodyText != null) bodyText.text = body;
        if (profileImage != null && icon != null) profileImage.sprite = icon;
        parentGrabScript = GetComponentInParent<XRGrabInteractable>();
    }

    void Update()
    {
        transform.localRotation = Quaternion.Euler(rotationOffset);
        canvas.enabled = CheckVisibility();
    }

    private bool CheckVisibility()
    {
        if (parentGrabScript && parentGrabScript.isSelected) return true;

        var dist = Vector2.Distance(new Vector2(mainCamera.position.x, mainCamera.position.z), new Vector2(transform.position.x, transform.position.z));
        if (dist > maxDistance) return false;

        var dirToUI = (transform.position - mainCamera.position).normalized;
        dirToUI.y = 0f;
        var dot = Vector3.Dot(mainCamera.forward, dirToUI);

        return dot > viewAngleLimit;
    }
}