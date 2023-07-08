using UnityEngine;

public class PlayerAuxSensor : MonoBehaviour
{
    public Player Player;
    public Player.StateFlags Flag;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Player.State |= Flag;
    }
}
