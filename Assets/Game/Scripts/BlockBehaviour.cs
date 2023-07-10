public class BlockBehaviour : RoleBehaviour
{
    private void OnEnable()
    {
        if (Object == null || Object.Physics == null)
        {
            return;
        }
        Object.Physics?.SwapPhysicsMaterial(Object.ActiveRole.Material);
        Object.Physics.SwapTags("Untagged");
    }
}