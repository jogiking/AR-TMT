# TMT-2
튜토리얼, 본게임 수정

Screenshots
-----------

활동범위 측정방식 변경
<div>
<img width = "200" src="https://user-images.githubusercontent.com/38206212/87374598-cf937c80-c5c5-11ea-8018-2f2b168831e5.jpg">
<img width = "200" src="https://user-images.githubusercontent.com/38206212/87374600-d0c4a980-c5c5-11ea-8fea-ad41e8df9ba6.jpg">
<img width = "200" src="https://user-images.githubusercontent.com/38206212/87374601-d15d4000-c5c5-11ea-8dfc-d0f494c874b6.jpg">
<img width = "200" src="https://user-images.githubusercontent.com/38206212/87374605-d1f5d680-c5c5-11ea-84c0-8b2ef57f21d0.jpg">
</div>

### Environment
1. Unity 2019.2.0f1 →  Unity 2019.3.15f1
2. AR Foundation 2.0.2 →  AR Foundation 3.0.1
3. AR Subsystems 3.0.0
4. ARCore XR Plugin 2.0.2 →  ARCore XR Plugin 3.0.1

### Build Setting
1. Build Setting > Android > Switch Platform
2. Build Setting > Player Setting > Player > Other Setting > Identification > Package name : com.svq.tmtgame
3. Build Setting > Player Setting > Player > Other Setting > Identification > Mininum API Level : Android 7.0(API 24) or later
4. Build Setting > Player Setting > Player > Other Setting > Rendering > Multithreaded Rendering : off
5. Build Setting > Player Setting > Player > Other Setting > Configuration > Scripting Backend : IL2CPP
6. Build Setting > Player Setting > Player > Other Setting > Configuration > Target Architecture : ARM64
7. Build Setting > Player Setting > Graphics > Built-in Shader Settings > Always Included Shaders > OutlineShader 추가(Custom Shader)

### Export Project Build 및 메인 Build
1. Build Setting > Check Export Project > Export > Create Android Project
2. 생성된 폴더 내 다음 파일 복사
* root > libs
* root > src > main > asset > bin
* root > src > main > jniLib
3. Build
