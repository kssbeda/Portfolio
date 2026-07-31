using System;
using System.Collections.Generic;

namespace Portfolio.Runtime.Events
{
    /// <summary>
    /// 이벤트 타입별 구독자를 관리하는 간단한 Typed Event Bus입니다.
    ///
    /// 설계 의도:
    /// - UI와 게임 시스템의 직접 참조를 줄입니다.
    /// - 구독 결과를 IDisposable로 반환해 해제 책임을 명확히 합니다.
    /// - 포트폴리오 예제이므로 스레드 안전성과 우선순위 기능은 포함하지 않았습니다.
    /// </summary>
    public sealed class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        /// <summary>
        /// 지정한 이벤트 타입의 처리기를 등록합니다.
        /// 반환된 IDisposable을 화면 또는 기능의 종료 시점에 반드시 해제해야 합니다.
        /// </summary>
        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            Type eventType = typeof(TEvent);
            if (!_handlers.TryGetValue(eventType, out List<Delegate>? handlers))
            {
                handlers = new List<Delegate>();
                _handlers.Add(eventType, handlers);
            }

            handlers.Add(handler);
            return new Subscription(() => Unsubscribe(handler));
        }

        /// <summary>
        /// 지정한 이벤트를 현재 등록된 모든 구독자에게 전달합니다.
        /// </summary>
        public void Publish<TEvent>(TEvent message)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out List<Delegate>? handlers))
                return;

            // 콜백 실행 중 구독자가 자기 자신을 해제할 수 있으므로 Snapshot을 사용합니다.
            // 고빈도 이벤트에서는 배열 할당 비용이 발생하므로 전용 채널이나 버퍼 재사용을 검토합니다.
            Delegate[] snapshot = handlers.ToArray();
            foreach (Delegate handler in snapshot)
                ((Action<TEvent>)handler).Invoke(message);
        }

        private void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            Type eventType = typeof(TEvent);
            if (!_handlers.TryGetValue(eventType, out List<Delegate>? handlers))
                return;

            handlers.Remove(handler);
            if (handlers.Count == 0)
                _handlers.Remove(eventType);
        }

        /// <summary>
        /// Dispose가 여러 번 호출되더라도 해제 동작이 한 번만 실행되도록 보장합니다.
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            private Action? _dispose;

            public Subscription(Action dispose) => _dispose = dispose;

            public void Dispose()
            {
                Action? dispose = _dispose;
                _dispose = null;
                dispose?.Invoke();
            }
        }
    }
}
