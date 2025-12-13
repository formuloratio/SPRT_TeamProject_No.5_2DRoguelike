# 게임명: Forest without tomorrow
<img width="1592" height="890" alt="스크린샷 2025-12-13 164427" src="https://github.com/user-attachments/assets/ff2a012b-0e2a-4902-8ecc-fb63ae828e15" />

## 목차
1. [프로젝트 장르 및 소개](#프로젝트-장르-및-소개)
2. [주요기능](#주요기능)
3. [역할분담](#역할분담)
4. [구현내용](#구현내용)
5. [기술스택](#기술스택)
6. [사용에셋 목록](#사용에셋-목록)

## 프로젝트 장르 및 소개
* 장르: 탑뷰 2D 슈팅 액션 뱀서라이크
* 소개: 캐릭터를 중심으로 한계 없는 성장을 통해 무한히 몰려오는 적들을 처치하고 생존하는 탑뷰 2D 슈팅 액션 뱀서라이크
* 개발 기간: 총 4일 { 2025.12.01 ~ 2025.12.04 }

## 주요기능
### 게임플레이
- 끊임없이 몰려오는 적들을 해치우며 게임 시작 3분 뒤에 등장하는 보스를 처치해 게임 클리어를 목표로 함.
- 적을 해치우면 나오는 경험치를 모아 레벨업.
- 레벨업하면 나오는 랜덤 장비를 이용해 플레이어 캐릭터를 강화.
- 스테이지 내의 레벨업으로 해금 가능한 플레이어 캐릭터의 궁극기를 이용한 위기 모면 모먼트.
- 플레이어 캐릭터의 체력이 0이 되거나, 보스를 처치하면 GameEnd 팝업을 표시, 팝업의 버튼을 이용한 게임 재시작 가능.

<img width="1593" height="891" alt="스크린샷 2025-12-13 164445" src="https://github.com/user-attachments/assets/7eefe035-cfa8-4f90-b652-89a2ae86f1bb" />
<img width="1595" height="891" alt="스크린샷 2025-12-13 165628" src="https://github.com/user-attachments/assets/1eefde8c-a844-4dd7-9d29-78772906d87c" />
<img width="1589" height="891" alt="스크린샷 2025-12-13 165647" src="https://github.com/user-attachments/assets/1525c214-84e8-4a84-a4c4-858970e32ea2" />
<img width="1590" height="891" alt="스크린샷 2025-12-13 165745" src="https://github.com/user-attachments/assets/1168a1d6-162f-46b7-a101-784c2457480b" />
<img width="1588" height="890" alt="스크린샷 2025-12-13 170009" src="https://github.com/user-attachments/assets/2e2dbeb8-a60e-412e-ab32-196309975bd2" />
<img width="1587" height="887" alt="스크린샷 2025-12-13 170027" src="https://github.com/user-attachments/assets/86d94af1-cb55-4f37-8549-d32174536025" />

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

## 역할분담
[김지훈](https://github.com/EunHyul769): PM
[엄성진](https://github.com/formuloratio): Player/Weapon
[김하늘](https://github.com/Hagill): Enemy
[박재아](https://github.com/jaeapark): UI(Scene)/GameFlow
[김동관](https://github.com/kdk7992-sketch): Map/Item/Sound

## 구현내용
### [엄성진]


## 기술스택
* Language: C#
* Engine: Unity
* Version Control: Git, GitHub
* IDE: Visual Studio 2022

## 사용에셋 목록
* 맵 타일셋: [Free Topdown Fantasy - Forest - Pixelart Tileset] (https://aamatniekss.itch.io/topdown-fantasy-forest)
* 그외: AI
