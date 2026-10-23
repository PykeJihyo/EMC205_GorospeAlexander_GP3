using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    public static readonly List<NPC> Active = new List<NPC>();

    public float heightOffset = 1f;

    public float MoveSpeed { get; set; }
    public int SlotIndex { get; private set; } = -1;
    public Vector3 SlotPosition { get; private set; }
    public Vector3 ExitPosition { get; private set; }
    public INPCState CurrentState { get; private set; }

    protected Renderer body;
    private WorldLabel speechLabel;
    private float speechTimer;
    private bool hasLeft;

    protected virtual void Awake()
    {
        body = GetComponentInChildren<Renderer>();
        speechLabel = WorldLabel.Create(transform, Vector3.up * 2.4f, Color.white);
    }

    void OnEnable() { Active.Add(this); }
    void OnDisable() { Active.Remove(this); }

    // Called by the spawner right after the factory creates this NPC.
    public void AssignSlot(int slotIndex, Vector3 slotPosition, Vector3 exitPosition)
    {
        SlotIndex = slotIndex;
        SlotPosition = slotPosition;
        ExitPosition = exitPosition;
    }

    // State pattern
    public void ChangeState(INPCState newState)
    {
        if (CurrentState != null) CurrentState.Exit(this);
        CurrentState = newState;
        if (CurrentState != null) CurrentState.Enter(this);
    }

    protected virtual void Update()
    {
        if (CurrentState != null) CurrentState.Execute(this);

        if (speechTimer > 0f)
        {
            speechTimer -= Time.deltaTime;
            if (speechTimer <= 0f) speechLabel.SetText("");
        }
    }

    // What to do when the NPC reaches its slot
    public abstract void OnArrivedAtSlot();

    // Walks toward a point
    public bool MoveTo(Vector3 destination)
    {
        Vector3 target = new Vector3(destination.x, destination.y + heightOffset, destination.z);
        transform.position = Vector3.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);

        Vector3 flatDirection = new Vector3(target.x - transform.position.x, 0f, target.z - transform.position.z);
        if (flatDirection.sqrMagnitude > 0.001f) transform.forward = flatDirection;

        return (transform.position - target).sqrMagnitude < 0.0025f;
    }

    public void LeaveShop()
    {
        ChangeState(new LeaveState());
    }

    // Frees this NPC's slot so someone new can use it
    public void ReleaseSlot()
    {
        if (SlotIndex < 0) return;
        GameManager.Instance.spawner.FreeSlot(SlotIndex);
        SlotIndex = -1;
    }

    // Reached the Exit Door (or the shop closed). Thief overrides this to handle a stolen shape.
    public virtual void OnLeftShop()
    {
        if (hasLeft) return;
        hasLeft = true;

        ReleaseSlot();
        Destroy(gameObject);
    }

    public void Say(string message, float seconds = 2.5f)
    {
        speechLabel.SetText(message);
        speechTimer = seconds;
    }
}