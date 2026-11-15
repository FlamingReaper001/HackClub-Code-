using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SlidingDoor : MonoBehaviour
{
    [Header("Door Panels")]
    [Tooltip("Panel that slides to the left.")]
    public Transform leftPanel;
    [Tooltip("Panel that slides to the right.")]
    public Transform rightPanel;

    [Header("Movement")]
    [Tooltip("Local direction the right panel slides. The left panel slides the opposite way. Use -1 to reverse.")]
    public Vector3 slideAxis = Vector3.right;
    [Tooltip("How far each panel slides when fully open.")]
    public float openDistance = 0.9f;
    [Tooltip("Slide speed in units per second.")]
    public float speed = 2f;

    [Header("Detection")]
    [Tooltip("Only objects with this tag will open the doors.")]
    public string triggerTag = "Player";

    private Vector3 leftClosed, rightClosed; // Starting positions
    private int occupants = 0;               // How many tagged objects are in the trigger

    void Start()
    {
        // Make sure the collider is a trigger, so the player can walk into it
        GetComponent<BoxCollider>().isTrigger = true;

        // Remember where the panels start (closed).
        // localPosition keeps this working even if the door is rotated.
        leftClosed = leftPanel.localPosition;
        rightClosed = rightPanel.localPosition;
    }

    void Update()
    {
        // Work out the open positions every frame,
        // so Slide Axis and Open Distance can be tweaked in Play mode
        Vector3 offset = slideAxis.normalized * openDistance;
        Vector3 leftOpen = leftClosed - offset;
        Vector3 rightOpen = rightClosed + offset;

        // Open if anyone is inside the trigger
        bool shouldOpen = occupants > 0;

        // Pick a target for each panel
        Vector3 leftTarget = shouldOpen ? leftOpen : leftClosed;
        Vector3 rightTarget = shouldOpen ? rightOpen : rightClosed;

        // Slide both panels smoothly toward their targets
        float step = speed * Time.deltaTime;
        leftPanel.localPosition = Vector3.MoveTowards(leftPanel.localPosition, leftTarget, step);
        rightPanel.localPosition = Vector3.MoveTowards(rightPanel.localPosition, rightTarget, step);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;
        occupants++;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        // Clamp at zero so the count can never go negative
        occupants = Mathf.Max(0, occupants - 1);
    }
}