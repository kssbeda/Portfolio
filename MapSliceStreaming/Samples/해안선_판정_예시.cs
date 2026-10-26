using UnityEngine;

// 공개 예시: 유효한 지면 구역 검사 개념만 발췌
public static class CoastBoundaryExample
{
    public static bool IsInsidePlayableRegion(Vector2 position, float worldSize, float safeMargin)
    {
        // 사방 외곽 여유 폭을 확보해 낙하 및 유효하지 않은 목적지를 막음
        return position.x >= safeMargin && position.y >= safeMargin &&
               position.x < worldSize - safeMargin && position.y < worldSize - safeMargin;
    }
}
