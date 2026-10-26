# Android APK 실기기 검증

| 항목 | 기록 |
|---|---|
| 확인일 | 2026-10-26 |
| Unity | 6000.3.24f1 / URP |
| 빌드 | Development / IL2CPP / ARM64 |
| 기기 | Samsung SM-F958N |
| OS | Android 17 (API 37) |
| GPU | Qualcomm Adreno 750 |
| API | Vulkan |
| APK 빌드·설치·실행 | **성공** |
| Terrain 렌더링 | **Android GPU Instancing OFF 후 정상** |

실기기 확인 로그:

```text
[MapSlice Terrain] Android Terrain GPU Instancing OFF (drawInstanced=False)
MapSliceTerrainNeighbors:Register(Vector2Int, GameObject)
```

APK 파일 자체는 현재 공개 문서 업로드에 포함되지 않았으며 GitHub Releases에 직접 첨부해야 합니다. 개발 빌드 디버그 UI를 포함하고 장시간 성능이나 모든 Android 기기 호환성 검증은 아직 끝나지 않았습니다.

**후속 검증:** 로딩/언로드 반복, P95/P99, 발열·메모리, UI Safe Area, 최신 시야·안개 패치.
