
public interface INPCState
{
    void Enter(NPC npc);   // runs once when the state starts
    void Execute(NPC npc); // runs every frame
    void Exit(NPC npc);    // runs once when the state ends
}
