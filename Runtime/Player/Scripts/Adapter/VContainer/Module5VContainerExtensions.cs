#if MODULE5_USE_VCONTAINER
using Module5.Player;
using VContainer;

namespace Module5.DI.VContainerAdapter
{
    /// <summary>
    /// 提供外部 VContainer 註冊 Module5 核心服務與 Adapter 的擴充方法。
    /// </summary>
    public static class Module5VContainerExtensions
    {
        /// <summary>
        /// 註冊 VContainerResolverAdapter，使 DLL 內部類別（依賴 Module5.DI.IObjectResolver）能無縫使用外部 VContainer。
        /// </summary>
        public static void AddModule5ResolverAdapter(this VContainer.IContainerBuilder builder)
        {
            builder.Register<Module5.DI.IObjectResolver>((VContainer.IObjectResolver resolver) =>
            {
                return new VContainerResolverAdapter(resolver);
            }, VContainer.Lifetime.Scoped);
        }

        /// <summary>
        /// 批次將 Module5 Core 核心服務與 Adapter 註冊至 VContainer。
        /// </summary>
        public static void AddModule5CoreServices(this VContainer.IContainerBuilder builder)
        {
            // 1. 註冊 Resolver 轉接器
            builder.AddModule5ResolverAdapter();

            // 2. 註冊 Core 各類管理器與服務
            builder.Register<ProjectPathState>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ProjectManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ProjectAssetManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetDataBaseManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetLoaderManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AnchorManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ARAnchorTracker>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<AssetMetaManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetImportPipeline>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AssetStorageManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<VariableTypeRegistry>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<VariableManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<CommandRegistry>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ConditionRegistry>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<EventManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UnityActionExecutor>(VContainer.Lifetime.Singleton);
            builder.Register<TweenManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<TimerManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ObjectInteractionService>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ConsoleManager>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayerInteractionInputProvider>(VContainer.Lifetime.Singleton).AsImplementedInterfaces();

            // 註冊預設 Storages
            builder.Register<PngStorage>(VContainer.Lifetime.Singleton);
            builder.Register<AudioStorage>(VContainer.Lifetime.Singleton);
            builder.Register<GltfStorage>(VContainer.Lifetime.Singleton);
            builder.Register<SceneStorage>(VContainer.Lifetime.Singleton);
            builder.Register<PrefabStorage>(VContainer.Lifetime.Singleton);
            builder.Register<UnknownAssetStorage>(VContainer.Lifetime.Singleton);
        }
    }
}
#endif
