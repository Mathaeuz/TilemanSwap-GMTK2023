public class BaloonBehaviour : RoleBehaviour
{
    PhysicsMaterialSwapper swapper;
    private void Awake()
    {
        swapper = new PhysicsMaterialSwapper(this);
    }

    private void OnEnable()
    {
        if (Object == null)
        {
            return;
        }

        swapper.Swap((Object.ActiveRole as BaloonRole).Material);
    }

    private void OnDisable()
    {
        swapper.Restore();
    }
}
