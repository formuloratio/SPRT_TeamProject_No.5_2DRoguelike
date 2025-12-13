# Forest without tomorrow
<img width="860" height="540" alt="UITitleImg" src="https://github.com/user-attachments/assets/bfcfefbf-112a-4743-b8e8-e829d2363c67" />


## 목차
1. [프로젝트 장르 및 소개](#프로젝트-장르-및-소개)
2. [주요기능](#주요기능)
3. [개발기간](#개발기간)
4. [역할분담](#역할분담)
5. [기술스택](#기술스택)
6. [사용에셋 목록](#사용에셋-목록)

## 프로젝트 장르 및 소개
* 장르: 탑뷰 2D 슈팅 액션 뱀서라이크
* 소개: 캐릭터를 중심으로 한계 없는 성장을 통해 무한히 몰려오는 적들을 처치하고 생존하는 탑뷰 2D 슈팅 액션 뱀서라이크

## 주요기능
### 게임플레이
- 끊임없이 몰려오는 적들을 해치우며 게임 시작 3분 뒤에 등장하는 보스를 처치해 게임 클리어를 목표로 함.
- 적을 해치우면 나오는 경험치를 모아 레벨업.
- 레벨업하면 나오는 랜덤 장비를 이용해 플레이어 캐릭터를 강화.
- 스테이지 내의 레벨업으로 해금 가능한 플레이어 캐릭터의 궁극기를 이용한 위기 모면 모먼트.
- 플레이어 캐릭터의 체력이 0이 되거나, 보스를 처치하면 GameEnd 팝업을 표시, 팝업의 버튼을 이용한 게임 재시작 가능.

### 핵심기술
- GameManager에서 게임 흐름/로직 결정.
- SceneLoader를 이용해 씬 전환 기능.
- 공통으로 사용될 수 있는 IDmagable, IProjectilable, IBossAttackable, IExpReceiver 인터페이스 선언 및 구현
- RangeWeaponHandler를 이용해 플레이어 캐릭터의 사거리 내의 적들을 탐지하고, 가장 가까운 적을 자동 공격(투사체 발사)하는 로직 구현
- StatHandler로 플레이어 캐릭터의 모든 능력치를 관리.
- ScriptableObject로 정의한 캐릭터, 적, 무기, 장비, 아이템, 페이즈, 스킬
- 각 효과의 사운드와 이미지, UI. 중복 재생을 막기 위한 쿨다운 기능. 
- ObjectPoolManager에서 사용할 프리팹을 미리 생성하고 관리.
- SO로 정의한 데이터를 이용한 적 자동 생성 기능.

## 개발기간
- 총 4일 { 2025.12.01 ~ 2025.12.04 }

## 역할분담
|PM|
|:---:|
|<img src="https://avatars.githubusercontent.com/u/233664198?v=4" width="100">|
|[김지훈](https://github.com/EunHyul769)|

|Player/Weapon|
|:---:|
|<img src="https://avatars.githubusercontent.com/u/230301673?v=4" width="100"/>|
|[엄성진](https://github.com/formuloratio)|

|Enemy|
|:---:|
|<img src="https://avatars.githubusercontent.com/u/101345563?v=4" width="100">|
|[김하늘](https://github.com/Hagill)|

|UI(Scene)/GameFlow|
|:---:|
|<img src="https://avatars.githubusercontent.com/u/233680670?v=4" width="100">|
|[박재아](https://github.com/jaeapark)|

|Map/Item/Sound|
|:---:|
|<img src="https://avatars.githubusercontent.com/u/231193899?v=4" width="100">|
|[김동관](https://github.com/kdk7992-sketch)|

## 기술스택
### Language
[![My Skills](https://skillicons.dev/icons?i=cs&perline=1)](https://skillicons.dev)
### Engine
[![My Skills](https://skillicons.dev/icons?i=unity&perline=1)](https://skillicons.dev)
### Version Control
[![My Skills](https://skillicons.dev/icons?i=git,github&perline=1)](https://skillicons.dev)
### IDE
[![My Skills](https://skillicons.dev/icons?i=visualstudio&perline=1)](https://skillicons.dev)

## 사용에셋 목록
맵 타일셋 : [Free Topdown Fantasy - Forest - Pixelart Tileset] (https://aamatniekss.itch.io/topdown-fantasy-forest)

### 이 외 모든 에셋은 AI로 제작하였습니다.
