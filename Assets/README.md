# Black War — Unity Project Structure

هذه هي بنية مشروع Unity الرسمية للعبة **Black War**. تم تنظيمها حسب طلبك:

```text
Assets/
├── Scenes/       # Bootstrap, Citadel, Match scenes
├── Prefabs/      # Vehicles, Characters, Weapons, UI, Networking
├── Scripts/      # Gameplay code grouped by feature
├── Animations/   # Character, vehicle, and UI clips/controllers
├── Networking/   # Netcode config, network prefabs, multiplayer scripts
├── Firebase/     # Auth, player profile, cloud save, leaderboard adapters
├── UI/           # HUDs, menus, market, contract board, fonts
├── Audio/        # Music, SFX, mixers
├── Vehicles/     # Vehicle art assets and ScriptableObject configs
└── Characters/   # Character art assets and ScriptableObject configs
```

## أول مشاهد مطلوبة

1. `Scenes/Bootstrap/Bootstrap.unity`: تحميل الخدمات، الشبكات، Firebase، ثم فتح القلعة.
2. `Scenes/Citadel/Citadel.unity`: الفندق/اللوبي والمتاجر والسوق والتحالفات.
3. `Scenes/Matches/BlackWarArena.unity`: خريطة القتال 8x8 كم.

> تم وضع ملفات `.gitkeep` لأن Git لا يحفظ المجلدات الفارغة. استبدلها لاحقًا بالأصول الحقيقية داخل Unity.
