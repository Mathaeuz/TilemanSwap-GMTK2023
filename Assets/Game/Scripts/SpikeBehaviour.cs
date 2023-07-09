public class SpikeBehaviour : RoleBehaviour
{
    private void OnEnable()
    {
        if (Object == null)
        {
            return;
        }
        Object.ColliderManagement?.SwapPhysicsMaterial(Object.ActiveRole.Material);
        Object.ColliderManagement?.SwapTags(nameof(SpikeRole));
    }

    private void OnDisable()
    {
        if (Object == null)
        {
            return;
        }
        Object.ColliderManagement?.RestorePhysicsMaterial();
        Object.ColliderManagement?.RestoreTags();
    }
}