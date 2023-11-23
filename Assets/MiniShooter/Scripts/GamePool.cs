using MasterServerToolkit.Utils;

namespace MiniShooter
{
    public static class GamePool
    {
        private static GenericPool<ShellEmitter> shellEmitterPool;

        public static void CreateShellEmitterPool(ShellEmitter prefab)
        {
            if (shellEmitterPool == null)
                shellEmitterPool = new GenericPool<ShellEmitter>(prefab);
        }
    }
}