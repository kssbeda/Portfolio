# 렌더링 및 모바일 발열 최적화

## 주요 병목

- Material 분리로 인한 Draw Call 증가
- 투명 UI·파티클의 Overdraw
- 과도한 Shader Variant와 불필요한 Pass
- 큰 RenderTexture와 후처리
- 화면 밖 객체·이펙트의 불필요한 갱신

## 개선 방향

- Material 공유, Atlas, Mesh 결합 가능성을 검토합니다.
- Frame Debugger로 실제 Batch 단절 원인을 확인합니다.
- 파티클 수와 투명 면적을 줄이고 품질 단계를 제공합니다.
- Shader 기능을 빌드 타깃에 맞게 분리하고 Variant를 정리합니다.
- CPU·GPU Frame Time과 장시간 발열을 함께 측정합니다.

## 검증

단순 FPS 평균만 보지 않고 P95/P99 Frame Time, 발열 후 Throttling, 배터리 사용량, 메모리 변화를 함께 확인합니다.
