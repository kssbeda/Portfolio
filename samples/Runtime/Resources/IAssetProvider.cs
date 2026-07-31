using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Portfolio.Runtime.Resources
{
    /// <summary>
    /// 로드된 에셋과 해당 에셋의 소유권 반환 기능을 함께 제공합니다.
    /// Dispose를 호출하면 내부 참조 카운트가 감소합니다.
    /// </summary>
    public interface IAssetLease<out T> : IDisposable where T : UnityEngine.Object
    {
        T Asset { get; }
    }

    /// <summary>
    /// Addressables와 같은 실제 로더를 추상화하는 인터페이스입니다.
    /// Domain과 UI가 구체적인 리소스 시스템에 직접 의존하지 않도록 합니다.
    /// </summary>
    public interface IAssetProvider
    {
        /// <summary>
        /// 키에 해당하는 에셋을 비동기로 로드하고 Lease를 반환합니다.
        /// 호출자는 사용이 끝난 뒤 반드시 Lease를 Dispose해야 합니다.
        /// </summary>
        Task<IAssetLease<T>> LoadAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
            where T : UnityEngine.Object;
    }
}
