# Unity 6.3 LTS | Android 오픈월드 스트리밍 포트폴리오

**MapSliceStreaming · Unity 6000.3.24f1 · URP · Addressables · Android**

1,024×1,024m 샘플 필드를 128m 청크 64개로 나누고, 플레이어와의 거리·메모리 조건에 따라 Full Terrain 또는 원거리 HLOD/Proxy를 비동기 로드하는 Unity 모바일 클라이언트 기술 데모입니다.

## 실기기 시연 결과 (2026-10-26)

| 항목 | 상태 |
|---|---|
| Android APK 빌드·설치·실행 | **성공** |
| Samsung SM-F958N, Android 17, Adreno 750, Vulkan | 실기기 로그 확인 |
| Unity 6000.3.24f1, IL2CPP, ARM64 Development Build | 확인 |
| 근거리 Terrain 표면 렌더링 | **정상 표시 확인** |
| 나무·수면·이동·미니맵·전체 지도 | 실기기 확인 범위의 화면/동작 |
| 장시간 스트리밍·P95/P99·발열 | 추가 검증 예정 |

### APK 받기

**[Android APK — GitHub Releases](https://github.com/kssbeda/Portfolio/releases)**

> 마지막으로 성공한 APK 바이너리는 아직 Release에 업로드해야 합니다. 현재는 **빌드 성공과 기기에서의 정상 실행이 확인된 상태**이며, 다운로드 가능한 Release 파일이 존재한다는 뜻은 아닙니다. 개발용 APK에는 진단 UI가 표시될 수 있습니다.

## 가장 중요한 문제 해결: Android Terrain 렌더링

**현상:** Editor는 정상이지만 Android에서 가까운 Terrain 표면이 그려지지 않고 나무와 원거리 HLOD만 보이는 현상.

**진단:** Terrain 컴포넌트 활성화, `drawHeightmap=True`, URP Terrain/Lit 셰이더 지원, 카메라 컬링, Terrain Hole 등을 확인했습니다. 하늘을 끄면 빈 영역이 배경색으로 보이고 Proxy를 숨기면 먼 수직 Skirt만 사라지는 것을 확인하여 문제를 분리했습니다.

**수정:** `MapSliceTerrainNeighbors.Register()`에서 Full Terrain의 `drawInstanced`를 강제로 활성화하던 경로를 찾아, Android에서만 GPU Instancing을 비활성화했습니다.

**결과:** Logcat의 `[MapSlice Terrain] Android Terrain GPU Instancing OFF (drawInstanced=False)` 및 실기기 정상 Terrain 표시 확인. 다만 엔진과 GPU 드라이버 중 정확한 결함 계층은 단정하지 않습니다.

## 주요 기능

- Full/Proxy/HLOD 청크 스트리밍 및 Addressables 비동기 로딩
- Terrain 연결, Collider 준비 대기, 수면·해안·급경사 이동 제한
- 나무 줄기 장애물을 반영한 Route Grid 및 NavMesh 경로 처리
- 미니맵·전체 지도·탐험 Fog of War·목적지 지정·경로 안내
- 발견한 이정표의 PC 키보드/Android 터치 상호작용과 빠른 이동
- 거리 안개, Skybox, 해안 수면 메시, Android UI Safe Area
- 청크 상태, 로딩, 프레임·메모리·GC 진단

기초 월드 검증 시 Terrain 64개, 나무 1,614개, 풀 인스턴스 32,182개, 돌·바위 874개를 확인했습니다. 후속 Biome 변경 뒤에는 총수 재측정이 필요합니다.

## 기술 문서

- [시스템 구조](Docs/01_시스템_아키텍처.md)
- [핵심 구현](Docs/02_핵심_구현_설명.md)
- [이슈 분석과 해결](Docs/03_개발_이슈와_해결.md)
- [Android 실기기 검증](Docs/04_Android_APK_검증.md)
- [성능 측정 원칙](Docs/05_성능_측정_기록.md)
- [공개 범위](Docs/06_공개범위_안내.md)
- [공개 상태](Docs/07_공개_체크리스트.md)
- [개발 이력](Docs/08_개발_이력.md)
- [공개 코드 유지 확인](Docs/09_공개코드_변경여부.md)
- [APK 업로드 순서](Docs/10_업로드_마무리_안내.md)

**공개 코드:** [청크 우선순위 예시](Samples/청크_우선순위_예시.cs), [해안선 경계 판정 예시](Samples/해안선_판정_예시.cs).

## 공개 정책

전체 Unity 런타임·에디터 Bake 툴체인·Addressables 운영 소스는 비공개입니다. 공개된 두 C# 파일은 **설계 원리를 보여주는 독립적인 개념 예시**이며, 이것만으로 APK를 재빌드할 수는 없습니다. 최신 카메라·안개·탐험 영역 튜닝은 기준 APK에 적용·검증됐다고 주장하지 않습니다.
