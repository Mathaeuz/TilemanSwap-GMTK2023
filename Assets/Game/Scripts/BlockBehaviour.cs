public class BlockBehaviour : RoleBehaviour
{
    private void OnEnable()
    {
        if (Object == null)
        {
            return;
        }
        Object.ColliderManagement?.SwapPhysicsMaterial(Object.ActiveRole.Material);
    }

    private void OnDisable()
    {
        if (Object == null)
        {
            return;
        }
        Object.ColliderManagement?.RestorePhysicsMaterial();
    }
}