using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Portfolio.Runtime.Resources
{
    /// <summary>
    /// 동일 에셋을 여러 기능이 공유할 수 있도록 참조 카운트를 관리하는 예제입니다.
    /// 실제 프로젝트에서는 LoadUnderlyingAsync와 ReleaseUnderlying을 Addressables API로 구현합니다.
    ///
    /// 포트폴리오 예제의 범위:
    /// - 완료된 에셋 캐시와 참조 카운트 정책을 설명합니다.
    /// - 동일 키 동시 로드 단일화와 스레드 안전성은 실제 구현에서 별도로 추가해야 합니다.
    /// </summary>
    public abstract class ReferenceCountedAssetProvider : IAssetProvider
    {
        private sealed class Entry
        {
            public required UnityEngine.Object Asset;
            public int ReferenceCount;
        }

        // 타입과 키를 함께 사용해 같은 문자열 키를 가진 서로 다른 에셋 타입의 충돌을 방지합니다.
        private readonly Dictionary<(Type Type, string Key), Entry> _cache = new();

        /// <summary>
        /// 캐시에 에셋이 있으면 참조 수를 증가시키고, 없으면 실제 로더를 호출합니다.
        /// </summary>
        public async Task<IAssetLease<T>> LoadAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
            where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("에셋 키가 필요합니다.", nameof(key));

            var cacheKey = (typeof(T), key);
            if (!_cache.TryGetValue(cacheKey, out Entry? entry))
            {
                T asset = await LoadUnderlyingAsync<T>(key, cancellationToken);
                entry = new Entry { Asset = asset, ReferenceCount = 0 };
                _cache.Add(cacheKey, entry);
            }

            entry.ReferenceCount++;

            // 에셋만 반환하지 않고 Lease를 반환해 해제 책임을 호출자에게 명시합니다.
            return new Lease<T>((T)entry.Asset, () => Release(cacheKey));
        }

        /// <summary>
        /// Addressables.LoadAssetAsync와 같은 실제 로드 기능을 구현합니다.
        /// </summary>
        protected abstract Task<T> LoadUnderlyingAsync<T>(
            string key,
            CancellationToken cancellationToken)
            where T : UnityEngine.Object;

        /// <summary>
        /// Addressables.Release와 같은 실제 해제 기능을 구현합니다.
        /// </summary>
        protected abstract void ReleaseUnderlying(UnityEngine.Object asset);

        private void Release((Type Type, string Key) cacheKey)
        {
            if (!_cache.TryGetValue(cacheKey, out Entry? entry))
                return;

            entry.ReferenceCount--;
            if (entry.ReferenceCount > 0)
                return;

            // 마지막 소유자가 반환한 뒤에만 캐시와 내부 리소스를 해제합니다.
            _cache.Remove(cacheKey);
            ReleaseUnderlying(entry.Asset);
        }

        /// <summary>
        /// Dispose를 여러 번 호출해도 Release가 한 번만 실행되도록 보장합니다.
        /// </summary>
        private sealed class Lease<T> : IAssetLease<T> where T : UnityEngine.Object
        {
            private Action? _release;

            public T Asset { get; }

            public Lease(T asset, Action release)
            {
                Asset = asset;
                _release = release;
            }

            public void Dispose()
            {
                Action? release = _release;
                _release = null;
                release?.Invoke();
            }
        }
    }
}
