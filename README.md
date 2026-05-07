# Noko Game Developer Case - Idle RPG / Arcade Idle

Unity ile hazirlanmis oynanabilir Idle RPG / Arcade Idle prototipi.

## Proje Bilgileri

- Unity Version: 6000.3.11f1
- Dil: C#
- Hedef Platform: Mobile
- Ana Sahne: `Assets/_CaseAssets/CaseScene.unity`
- Secilen Silah: Bow
- Onerilen Game View: 1920x1080 Landscape

## Oynanis Ozeti

Oyuncu harita uzerinde hareket eder, yakindaki dusmanlara otomatik saldirir ve dusman oldurdukce gold/experience kazanir. Dusmanlar wave sistemiyle spawn olur. Kazanilan ilerleme ile karakter statleri gelistirilebilir ve Blacksmith uzerinden yeni bow skillleri acilabilir.

## Ana Sistemler

### Player ve Combat

- `PlayerController`: CharacterController ve Input System ile oyuncu hareketini, kamera yonune gore inputu, rotasyonu ve movement animasyonunu yonetir.
- `PlayerCombat`: Otomatik hedef secme, menzil kontrolu, saldiri cooldownlari, bow attack akisi ve skill tetiklemelerini yonetir.
- Combat sirasinda her frame sahne taramasi yapilmaz. Aktif dusmanlar `Health.Enemies` registry listesi uzerinden okunur.

### Enemy AI ve Wave Sistemi

- `EnemyAI`: NavMeshAgent tabanli dusman davranisini yonetir.
- Dusman oyuncuyu belirli bir detection radius icinde algilar.
- Oyuncu menzil disindaysa kosarak takip eder, attack range icindeyse durup oyuncuya doner ve cooldown ile hasar verir.
- Oyuncu algi menzilinden cikarsa dusman bir sure bekler, baslangic pozisyonuna geri doner ve ilk rotasyonuna toparlanir.
- `EnemySpawner`: Wave tabanli spawn akisini yonetir. Her wave'de dusman sayisi artar, tum dusmanlar oldugunde kisa cooldown sonrasi yeni wave baslar.
- `UIManager` wave bilgisini HUD uzerinde gunceller.

### Health, Reward ve Registry

- `Health`: Player ve Enemy can sistemini ortak yonetir.
- Damage alma, death animasyonu, hit flash, poison damage over time, player respawn ve enemy reward akisi bu script uzerinden ilerler.
- Enemy oldugunde gold ve experience verilir, `EnemySpawner` alive enemy sayisi guncellenir ve aktif quest progress'i ilerletilir.
- Enemy Health componentleri `OnEnable/OnDisable` ile registry listesine girip cikar. Bu sayede target bulma daha efficient calisir.

### Upgrade Sistemi

Karakter gelisimi stat point tabanlidir.

- Damage: Vurus hasarini artirir.
- HP: Maksimum cani artirir.
- Attack Speed: Saldiri sikligini artirir.
- Speed: Hareket hizini artirir.

Temel stat degerleri `PlayerStatsDataSO1` ScriptableObject uzerinden tutulur. Bu yapi designer friendly olacak sekilde ayarlanmistir; denge degerleri kod degistirmeden Inspector uzerinden duzenlenebilir.

### Bow Skill Sistemi

Blacksmith uzerinden acilabilen 3 skill vardir:

- Multishot: Bow saldirisinin ek hedeflere gitmesini saglar.
- Poison Arrow: Belirli sure boyunca oklarin poison damage uygulamasini saglar.
- Explosive Arrow: Bir sonraki oka alan hasari ekler.

Poison ve Explosive baslangicta locked durumdadir. Oyuncu Blacksmith shop panelinden gold harcayarak bu yetenekleri acar.

### Arrow ve VFX Akisi

- `ArrowSystem`: Normal ve poison arrow poollarini yonetir.
- Aktif poison durumuna gore dogru arrow pool secilir.
- Oklar hedefe kisa bir arc hareketiyle gider ve sonra tekrar pool'a doner.
- Explosive Arrow, hedef pozisyonunda alan hasari ve particle VFX olusturur.

### Blacksmith Sistemi

Blacksmith NPC alanina girildiginde shop paneli acilir. Bu panelden:

- Multishot seviyesi artirilabilir.
- Poison Arrow unlock edilebilir.
- Explosive Arrow unlock edilebilir.

Satin alma kontrolleri gold durumuna ve unlock state'lerine gore yapilir. Skill butonlari satin alinmadan aktif hale gelmez.

### Quest Sistemi

Oyuna ekstra olarak basit bir quest sistemi eklendi.

- `Quest` ScriptableObject ile title, description, hedef miktari, hedef enemy id'leri ve oduller tanimlanir.
- Quest NPC'ye yaklasinca dialog UI acilir.
- Dialog ekrani Quest ScriptableObject icindeki title ve description bilgisini gosterir.
- Kill quest ilerlemesi enemy oldukce guncellenir.
- Quest tamamlaninca dialog ekraninda `Well done!` mesaji gosterilir ve odul claim edilebilir.

Bu sistem de designer friendly olacak sekilde ScriptableObject tabanlidir. Yeni gorev eklemek icin yeni bir `Quest` asset'i olusturulup ilgili NPC veya quest database uzerinden baglanabilir.

### UI ve Manager Yapisi

- `GameManager`: Oyun baslatma, enemy kill reward akisi ve player respawn surecini yonetir.
- `CurrencyManager`: Gold ekleme/harcama ve currency changed event akisini yonetir.
- `UIManager`: HP bar, gold text ve wave text guncellemelerini event tabanli yapar.
- `DialogManager`: Quest dialog UI, accept quest ve claim reward akisini yonetir.
- `QuestManager`: Quest database ve quest giver atamalarini yonetir.

## ScriptableObject Kullanimi

Projede denge ve icerik verilerini koddan ayirmak icin ScriptableObject kullanildi:

- Player stat/balance degerleri
- Combat balance degerleri
- Quest icerikleri
- Quest database

Bu sayede designer tarafinda hasar, can, cooldown, quest description, reward ve hedef sayisi gibi degerler Inspector uzerinden hizlica degistirilebilir.

## Teknik Notlar

- FindObjectOfType / FindObjectsOfType kullanimlari runtime akistan temizlendi.
- Enemy targeting icin `Health.Enemies` registry listesi kullaniliyor.
- UI referanslari inspector injection ve event subscription ile yonetiliyor.
- Play Mode'da hareket, combat, HP bar, wave, upgrade, Blacksmith unlock ve quest akisi test edildi.
- Son build kontrolu 0 warning / 0 error ile basarili tamamlandi.

## Calistirma

1. Projeyi Unity 6000.3.11f1 ile acin.
2. `Assets/_CaseAssets/CaseScene.unity` sahnesini acin.
3. Game View'u 1920x1080 Landscape olarak ayarlayin.
4. Play Mode'a girin.

## Notlar

- UI icin placeholder gorseller kullanilmistir.
- Odak, case gereksinimindeki mekaniklerin calisir ve oynanabilir sekilde sunulmasidir.
- Ek olarak Blacksmith ve Quest sistemleri ile prototipin RPG/idle progression hissi guclendirildi.
