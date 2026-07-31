using System;
using System.Collections.Generic;
using UnityEngine;

namespace Portfolio.Runtime.Pooling
{
    /// <summary>
    /// 풀에서 대여되거나 반환될 때 객체가 자체 상태를 초기화하도록 하는 계약입니다.
    /// </summary>
    public interface IPoolable
    {
        void OnRent();
        void OnReturn();
    }

    /// <summary>
    /// Unity Component를 재사용하는 단순 Stack 기반 풀입니다.
    /// 반복되는 Instantiate/Destroy 구간의 CPU 비용과 GC Spike를 줄이기 위한 예제입니다.
    ///
    /// 주의:
    /// - 반환 중복 검사는 생략된 예제이므로 상용 구현에서는 활성 대여 목록을 검증해야 합니다.
    /// - 최대 보유 수와 유휴 객체 정리 정책을 추가해야 메모리 증가를 막을 수 있습니다.
    /// </summary>
    public sealed class ComponentPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _root;
        private readonly Stack<T> _available = new();

        public ComponentPool(T prefab, Transform root, int prewarmCount = 0)
        {
            _prefab = prefab != null ? prefab : throw new ArgumentNullException(nameof(prefab));
            _root = root != null ? root : throw new ArgumentNullException(nameof(root));

            // 첫 사용 프레임의 생성 집중을 줄이기 위해 필요한 수만큼 미리 준비합니다.
            for (int i = 0; i < prewarmCount; i++)
                Return(Create());
        }

        /// <summary>
        /// 사용 가능한 객체를 꺼내 위치와 회전을 설정한 뒤 활성화합니다.
        /// </summary>
        public T Rent(Vector3 position, Quaternion rotation)
        {
            T instance = _available.Count > 0 ? _available.Pop() : Create();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);

            if (instance is IPoolable poolable)
                poolable.OnRent();

            return instance;
        }

        /// <summary>
        /// 객체 상태를 초기화하고 비활성화한 뒤 풀에 반환합니다.
        /// </summary>
        public void Return(T instance)
        {
            if (instance == null)
                return;

            if (instance is IPoolable poolable)
                poolable.OnReturn();

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_root, false);
            _available.Push(instance);
        }

        private T Create()
        {
            T instance = UnityEngine.Object.Instantiate(_prefab, _root);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
