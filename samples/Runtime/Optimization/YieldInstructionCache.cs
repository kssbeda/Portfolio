using System.Collections.Generic;
using UnityEngine;

namespace Portfolio.Runtime.Optimization
{
    /// <summary>
    /// 반복적으로 사용하는 대기 객체를 재사용해 불필요한 할당을 줄입니다.
    ///
    /// 사용 제약:
    /// - 고정된 시간 값에만 사용하는 것을 권장합니다.
    /// - 서로 다른 시간 값이 무제한으로 들어오면 Dictionary가 계속 증가할 수 있습니다.
    /// - TimeScale 영향을 받지 않는 대기에는 WaitForSecondsRealtime 정책을 별도로 설계해야 합니다.
    /// </summary>
    public static class YieldInstructionCache
    {
        private static readonly Dictionary<float, WaitForSeconds> Waits = new();

        public static readonly WaitForEndOfFrame EndOfFrame = new();
        public static readonly WaitForFixedUpdate FixedUpdate = new();

        /// <summary>
        /// 동일한 초 값에 대해 기존 WaitForSeconds 인스턴스를 반환합니다.
        /// </summary>
        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            // 음수 대기는 의미가 없으므로 0초로 보정해 캐시 키를 일관되게 유지합니다.
            if (seconds < 0f)
                seconds = 0f;

            if (!Waits.TryGetValue(seconds, out WaitForSeconds? wait))
            {
                wait = new WaitForSeconds(seconds);
                Waits.Add(seconds, wait);
            }

            return wait;
        }

        /// <summary>
        /// 도메인 리로드를 사용하지 않는 개발 환경이나 테스트 종료 시 캐시를 비웁니다.
        /// </summary>
        public static void Clear() => Waits.Clear();
    }
}
