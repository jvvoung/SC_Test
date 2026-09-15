# SKCT Practice

Windows와 macOS에서 함께 사용할 수 있는 독립 실행형 SKCT 문제풀이 연습 보조 앱입니다. 실제 시험 프로그램, 화면 캡처, OCR, 키보드 후킹과는 연결되지 않습니다.

## 주요 기능

- 20문제 × 5개 답안 선택, 현재 문제 강조, 직접 이동, 다음/스킵, 답안만 초기화
- 정확한 종료 시각을 기준으로 보정되는 15분 타이머
- 운영체제 IME를 그대로 사용하는 여러 줄 메모장
- 마우스·트랙패드 포인터로 그리는 리사이즈 대응 그림판
- 마우스와 키보드가 하나의 `decimal` 계산 로직을 공유하는 계산기
- 실행 시 주 화면 작업영역의 오른쪽에 자동 배치

## 프로젝트 구조

```text
SKCTPractice.sln / SKCTPractice.csproj  솔루션과 .NET 8 Avalonia 프로젝트
App.axaml                              전역 색상·버튼·카드 스타일
MainWindow.axaml                       좌우 전체 레이아웃
Views/QuestionPanel.*                  문제/답안/이동/답안 초기화
Views/TimerPanel.*                     카운트다운 타이머
Views/MemoDrawingPanel.*               메모·그림판 전환과 전체 지우기
Views/CalculatorPanel.*                계산기 UI, 포커스, 키보드 이벤트
Controls/DrawingCanvas.cs              포인터 Stroke 저장 및 렌더링
Models/CalculatorEngine.cs             공용 decimal 계산 로직
Models/QuestionItem.cs                 문제별 답안 상태
Scripts/                               운영체제별 publish 스크립트
```

## 실행과 빌드

```powershell
dotnet restore
dotnet run
dotnet build -c Release
```

Visual Studio에서는 `SKCTPractice.sln`을 열고 `SKCTPractice`를 시작 프로젝트로 선택한 뒤 실행합니다.

## Windows EXE 만들기

PowerShell에서 다음을 실행합니다.

```powershell
.\Scripts\publish-windows.ps1
```

결과는 `artifacts/win-x64/SKCTPractice.exe`입니다. 프레임워크 종속 publish가 필요하면 일반 `dotnet publish -c Release -r win-x64 --self-contained false` 명령도 사용할 수 있습니다.

## macOS 앱 만들기

대상 Mac에서 .NET 8 SDK를 설치한 뒤 실행합니다.

```bash
chmod +x ./Scripts/publish-macos.sh
./Scripts/publish-macos.sh osx-arm64  # Apple Silicon
./Scripts/publish-macos.sh osx-x64    # Intel Mac
```

결과는 `artifacts/<runtime>/SKCTPractice.app`입니다. 배포용으로 타인에게 전달할 때는 Apple Developer 인증서 서명과 notarization을 추가해야 합니다.

## 자주 수정하는 위치

- 전체 배치: `MainWindow.axaml`
- 각 영역 UI: 해당 `Views/*.axaml`
- 기본 타이머 시간: `Views/TimerPanel.axaml.cs`의 `DefaultDuration`
- 답안 초기화: `Views/QuestionPanel.axaml.cs`의 `ResetAnswersAsync`
- 계산기 키보드: `Views/CalculatorPanel.axaml.cs`의 `OnCalculatorKeyDown`, `OnCalculatorTextInput`
- 강조색과 공통 스타일: `App.axaml`
