# Black War — C# Gameplay Prototype

هذا المستودع يحتوي على نسخة أولية كاملة وقابلة للتوسعة بلغة C# للعبة **Black War** حسب وثيقة التصميم: القلعة، المركبات، القتال الأرضي، العقود، الاقتصاد، السوق السوداء، السمعة، التحالفات، وتقلص الخريطة.

> ملاحظة: لعبة بحجم Battle Royale حقيقية تحتاج Unity/Godot، خوادم، أصول 3D، شبكات، صوتيات وواجهات. هذا المشروع يقدم نواة Gameplay كاملة ومحاكاة قابلة للنقل لاحقًا إلى Unity.

## التشغيل

يتطلب .NET SDK 8 أو أحدث:

```bash
dotnet run --project src/BlackWar/BlackWar.csproj
```

## ماذا يوجد؟

- حلقة لعب كاملة: Citadel → تجهيز → عقد → مباراة → مكافآت → عودة.
- مركبات بأنواع Scout/Assault/Tank/Interceptor/Support مع نيترو ودروع وأسلحة.
- انتقال تلقائي إلى القتال الأرضي عند تدمير المركبة.
- عقود اغتيال/تدمير/حماية/سيطرة/عقود خاصة.
- اقتصاد Gold Coins و Reputation Points.
- سوق سوداء P2P بعمولة 5% وحظر بيع المسروقات 24 ساعة.
- نظام رتب متعدد: Assassin/Driver/Reputation/Trader.
- تحالفات وخيانة وعقوبة Traitor لمدة 48 ساعة.
- خريطة 8x8 كم ومناطق وأحداث عشوائية وتقلص Safe Zone.

## Unity Project Structure

تمت إضافة هيكل Unity جاهز داخل `Assets/` مع `Packages/manifest.json` و `ProjectSettings/ProjectVersion.txt`، ويحتوي على الأقسام المطلوبة: Scenes, Prefabs, Scripts, Animations, Networking, Firebase, UI, Audio, Vehicles, Characters.

المشاهد الأولية:

- `Assets/Scenes/Bootstrap/Bootstrap.unity`
- `Assets/Scenes/Citadel/Citadel.unity`
- `Assets/Scenes/Matches/BlackWarArena.unity`

افتح المجلد في Unity 2022.3 LTS، ثم أضف GameObjects للمشاهد واربط scripts من `Assets/Scripts` حسب الحاجة.
