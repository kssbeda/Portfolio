using System;
using System.Collections.Generic;

namespace Portfolio.Runtime.Combat
{
    /// <summary>
    /// 하나의 능력치에 적용되는 고정값과 비율 보정을 표현합니다.
    /// 계산 데이터만 보유하는 불변 구조체로 구성해 Unity 오브젝트와 분리했습니다.
    /// </summary>
    public readonly record struct StatModifier(
        string Stat,
        float Flat,
        float Percent);

    /// <summary>
    /// 계산이 끝난 능력치를 읽기 전용으로 제공하는 스냅샷입니다.
    /// 외부 코드가 계산 결과 Dictionary를 직접 변경하지 못하도록 감쌉니다.
    /// </summary>
    public sealed class AbilitySnapshot
    {
        private readonly IReadOnlyDictionary<string, float> _values;

        public AbilitySnapshot(IReadOnlyDictionary<string, float> values)
            => _values = values ?? throw new ArgumentNullException(nameof(values));

        /// <summary>
        /// 능력치가 존재하면 값을 반환하고, 존재하지 않으면 0을 반환합니다.
        /// UI와 전투 코드가 매번 키 존재 여부를 검사하지 않도록 정책을 통일합니다.
        /// </summary>
        public float Get(string stat)
            => _values.TryGetValue(stat, out float value) ? value : 0f;
    }

    /// <summary>
    /// 기본 능력치와 보정 목록을 이용해 최종 능력치를 계산합니다.
    ///
    /// 설계 의도:
    /// - MonoBehaviour와 분리해 EditMode 단위 테스트가 가능하도록 합니다.
    /// - 고정값 합산 후 비율을 적용하는 계산 순서를 한곳에서 관리합니다.
    /// - UI는 계산 규칙을 소유하지 않고 결과 Snapshot만 읽습니다.
    /// </summary>
    public static class AbilityCalculator
    {
        /// <summary>
        /// 최종 능력치 스냅샷을 생성합니다.
        /// 계산식은 (기본값 + 고정 보정 합계) × (1 + 비율 보정 합계)입니다.
        /// </summary>
        public static AbilitySnapshot Calculate(
            IReadOnlyDictionary<string, float> baseStats,
            IReadOnlyList<StatModifier> modifiers)
        {
            ArgumentNullException.ThrowIfNull(baseStats);
            ArgumentNullException.ThrowIfNull(modifiers);

            // 고정값과 비율을 별도 Dictionary에 누적해 계산 순서를 명확히 합니다.
            // 매우 빈번하게 호출되는 경로라면 이 임시 Dictionary를 재사용하는 방식도 검토합니다.
            var flatModifiers = new Dictionary<string, float>();
            var percentModifiers = new Dictionary<string, float>();

            foreach (StatModifier modifier in modifiers)
            {
                flatModifiers[modifier.Stat] =
                    flatModifiers.GetValueOrDefault(modifier.Stat) + modifier.Flat;

                percentModifiers[modifier.Stat] =
                    percentModifiers.GetValueOrDefault(modifier.Stat) + modifier.Percent;
            }

            var result = new Dictionary<string, float>(baseStats.Count);
            foreach ((string stat, float baseValue) in baseStats)
            {
                float finalValue =
                    (baseValue + flatModifiers.GetValueOrDefault(stat)) *
                    (1f + percentModifiers.GetValueOrDefault(stat));

                result[stat] = finalValue;
            }

            return new AbilitySnapshot(result);
        }
    }
}
