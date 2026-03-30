using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector3 movement;
    private Rigidbody rb;
    private bool isGrounded;
    public LayerMask ground;
    public float jumpHeight = 10f;

    [Header("Raycasting")]
    public Camera playerCamera;
    public LayerMask garbage;
    private Vector3 boxSize = new Vector3(0.5f, 0.3f, 0.5f);
    //private float sphereRadius = 10f;

    [Header("Poker")]
    //public GameObject poker;
    public GameObject pokerExtension;
    private float pokerRange = 5f;
    private bool isPokerExtended;
    private float pokerCounter = 0;
    private float pokerCooldown = 0.5f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject guideImage;
    private bool isGuideEnabled = false;
    private int score;

    [Header("Animation")]
    public Animator poleAnimator;
    public Animator guideAnimator;

    [Header("Inventory")]
    public InventoryController inventory;

    [Header("Bins")]
    public LayerMask binLayer;
    private GameObject heldBin;
    private bool isHoldingBin;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        isPokerExtended = false;
        isHoldingBin = false;

        //guideImage.SetActive(false);

        // scoreText.text = $"Current Garbage: {score}";
    }

    private void Update()
    {
        isGrounded = IsPlayerGrounded();

        // On left click, check if player is close enough to garbage/bins and not holding a bin
        if (Input.GetMouseButtonDown(0) && !isHoldingBin)
        {
            CheckGarbage("poker");
        }

        // Pole extending and retracting on right click
        if (Input.GetMouseButtonDown(1))
        {
            // If the poker gets extended, check if it hits a grapple point
            // Only allow poker state change if the cooldown is over
            if (!isPokerExtended && pokerCounter >= pokerCooldown)
            {
                poleAnimator.Play("PokerExtend");
                isPokerExtended = !isPokerExtended;
                pokerCounter = 0f;

                // Extend poker range
                pokerRange = 10f;
            }
            else if (isPokerExtended && pokerCounter >= pokerCooldown)
            {
                poleAnimator.Play("PokerRetract");
                isPokerExtended = !isPokerExtended;
                pokerCounter = 0f;

                // Reset poker range
                pokerRange = 5f;
            }
        }

        // On pressing E while looking at a bin, pick it up so the player can move it
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHoldingBin) CheckGarbage("moveBin");
            else if (isHoldingBin) // Put down the bin the player is holding
            {
                // Reset the held bin
                heldBin.transform.SetParent(GameObject.Find("GarbageBins").transform);
                heldBin.GetComponent<BoxCollider>().enabled = true;

                // Place the bin nicely on the ground
                heldBin.GetComponent<BinController>().MagnetToGround(ground);

                // Reset variables related to holding a bin
                heldBin = null;
                isHoldingBin = false;
            }
        }

        // Bin moving handling
        if (isHoldingBin)
        {
            // Keep bin rotated to look at the player while in front of them
            heldBin.transform.LookAt(gameObject.transform);
            heldBin.transform.Rotate(0, 90, 0);
        }

        // Jumping code
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && rb.velocity.y <= 1f)
        {
            Jump();
            isGrounded = false;
        }

        // Toggle showing the recycling guide
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleGuide();
        }

        pokerCounter += Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Create two temp float variables to hold inputs
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        movement = (transform.forward * v * moveSpeed) + (transform.right * h * moveSpeed);

        // Normalize so the movement doesn't get added incorrectly
        movement = Vector3.ClampMagnitude(movement, moveSpeed);

        // Take current position, add the calc above
        rb.MovePosition(transform.position + movement * Time.deltaTime);
    }

    private bool IsPlayerGrounded()
    {
        // Start above the players feet 
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;

        float rayDistance = 1.15f;

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayDistance, ground))
        {
            float slope = Vector3.Angle(hit.normal, Vector3.up);
            Debug.Log("Ground hit: " + hit.collider.name + " | Slope: " + slope + " | Distance: " + hit.distance);

            return slope < 80f;
        }

        Debug.Log("NO GROUND HIT");
        return false;
    }

    private void Jump()
    {
        // Get current velocity
        Vector3 velocity = rb.velocity;

        // If already going up remove upward force first
        if (velocity.y > 0)
        {
            velocity.y = 0f;
        }

        rb.velocity = velocity;

        // Make the player jump
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void CheckGarbage(string interaction)
    {
        RaycastHit hit;

        // Raycast forward to find garbage
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pokerRange, garbage) && interaction.Equals("poker"))
        {
            if (inventory == null)
            {
                Debug.LogError("InventoryController not assigned on PlayerController.");
                return;
            }

            // Try add to inventory instead of destroying
            bool added = inventory.TryAdd(hit.collider.gameObject);

            // If you still want score for pickup, do it here (optional)
            // if (added) score++;

            return;
        }

        // If we didn't hit garbage, we can check bins with the same left click
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pokerRange, binLayer))
        {
            var bin = hit.collider.GetComponentInParent<BinController>();
            if (interaction.Equals("poker"))
            {
                // Determine which thing to do when looking at the bin
                if (bin != null)
                {
                    bin.TryDeposit(inventory);
                }
                else
                {
                    Debug.Log("[Bin] Hit a bin object but no BinController found.");
                }
            }
            else if (interaction.Equals("moveBin"))
            {
                // pick up bin if not already holding one
                if (!isHoldingBin)
                {
                    // Set gameobject to picked up bin
                    heldBin = bin.gameObject;

                    // Disable collision
                    heldBin.GetComponent<BoxCollider>().enabled = false;

                    // Move bin in front of player
                    heldBin.transform.SetParent(gameObject.transform);
                    //heldBin.transform.position += new Vector3(0, 1, 0);

                    // Toggle holding the bin
                    isHoldingBin = !isHoldingBin;
                }
            }
        }
    }

    private void ToggleGuide()
    {
        if (!isGuideEnabled)
        {
            guideAnimator.Play("GuideEnable");
            isGuideEnabled = true;
        }
        else if (isGuideEnabled)
        {
            guideAnimator.Play("GuideDisable");
            isGuideEnabled = false;
        }
    }
}

