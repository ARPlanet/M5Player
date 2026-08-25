#if MODULE5_USE_VCONTAINER
using System;
using Module5.DI;
using VContainer;

namespace Module5.DI.VContainerAdapter
{
    /// <summary>
    /// 將 VContainer.IObjectResolver 轉接為 Module5.DI.IObjectResolver 的轉接器（Adapter）。
    /// 讓 Core DLL 內部類別在不引用 VContainer 的情況下，透過抽象介面直接由外部 VContainer 提供解析與注入服務。
    /// </summary>
    public class VContainerResolverAdapter : Module5.DI.IObjectResolver
    {
        private readonly VContainer.IObjectResolver _vcontainer;

        public VContainer.IObjectResolver UnderlyingResolver => _vcontainer;

        public VContainerResolverAdapter(VContainer.IObjectResolver vcontainer)
        {
            _vcontainer = vcontainer ?? throw new ArgumentNullException(nameof(vcontainer));
        }

        public T Resolve<T>()
        {
            return _vcontainer.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _vcontainer.Resolve(type);
        }

        public bool TryResolve<T>(out T result)
        {
            return _vcontainer.TryResolve<T>(out result);
        }

        public bool TryResolve(Type type, out object result)
        {
            return _vcontainer.TryResolve(type, out result);
        }

        public void Inject(object instance)
        {
            if (instance != null)
            {
                _vcontainer.Inject(instance);
            }
        }

        public Module5.DI.IObjectResolver CreateScope(Action<Module5.DI.IContainerBuilder> configuration = null)
        {
            var subScope = _vcontainer.CreateScope(vBuilder =>
            {
                if (configuration != null)
                {
                    var tempBuilder = new ContainerBuilder();
                    configuration(tempBuilder);
                }
            });
            return new VContainerResolverAdapter(subScope);
        }

        public void Dispose()
        {
            // 生命週期統一由外部 VContainer LifetimeScope 管理
        }
    }
}
#endif
