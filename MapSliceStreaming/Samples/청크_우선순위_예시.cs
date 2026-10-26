using UnityEngine;

// 공개 예시: 중심 청크에서 가까운 청크에 우선순위를 부여하는 개념
public static class ChunkPriorityExample
{
    public static int Calculate(Vector2Int center, Vector2Int chunk, bool mustLoadFull)
    {
        int distance = Mathf.Max(Mathf.Abs(chunk.x - center.x), Mathf.Abs(chunk.y - center.y));
        return (mustLoadFull ? 0 : 1000) + distance * 10;
    }
}
