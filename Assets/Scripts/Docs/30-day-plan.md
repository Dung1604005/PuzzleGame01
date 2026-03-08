# KẾ HOẠCH XÂY DỰNG GAME 30 NGÀY
## Block Blast Clone — Unity 2D

> **Phương châm:** Xây từ lõi ra ngoài — **Data → Model → Controller → View → Services → Polish**
> Mỗi ngày kết thúc phải có **1 thứ chạy được**, không để ngày nào "chỉ căn ke cấu trúc".

---

## TỔNG QUAN TIẾN ĐỘ

| Giai đoạn | Ngày | Mục tiêu |
|---|---|---|
| **Phase 1:** Nền tảng | 1 – 3 | Cấu trúc project, Data Layer |
| **Phase 2:** Lõi logic | 4 – 10 | Model Layer hoàn chỉnh, test thuần C# |
| **Phase 3:** Điều phối | 11 – 17 | Controller Layer, luồng game cơ bản |
| **Phase 4:** Hiển thị | 18 – 22 | View Layer, kéo thả, animation |
| **Phase 5:** Dịch vụ | 23 – 26 | Save, Sound, Pool, DDA |
| **Phase 6:** Polish & Build | 27 – 30 | Tối ưu, UI hoàn chỉnh, build APK |

---

## PHASE 1 — NỀN TẢNG (Ngày 1–3)

### Ngày 1 — Thiết lập Project
**Mục tiêu:** Project sạch, chạy được trên máy và trên thiết bị thật.

- [ ] Tạo cấu trúc thư mục chuẩn:
  ```
  Assets/
  ├── Art/          (Sprites, Tilemaps)
  ├── Audio/        (BGM, SFX)
  ├── Prefabs/      (Tile, Piece, VFX)
  ├── Scenes/       (Menu, Game, Test)
  ├── Scripts/
  │   ├── Data/     (ScriptableObjects)
  │   ├── Models/   (Pure C#)
  │   ├── Controllers/
  │   ├── Views/
  │   ├── Services/
  │   └── Docs/
  └── ScriptableObjects/
  ```
- [ ] Cài đặt **DOTween** (Package Manager hoặc Asset Store)
- [ ] Cấu hình **Input System** cho Mobile Touch
- [ ] Tạo Scene `Game` cơ bản, camera orthographic 9:16
- [ ] Build thử lên Android/iOS — đảm bảo không lỗi môi trường

**Định nghĩa hoàn thành:** App khởi động được trên thiết bị, hiện nền đen, không crash.

---

### Ngày 2 — Data Layer: `PieceData` ScriptableObject
**Mục tiêu:** Tạo được các khối trong Unity Inspector mà không cần code thêm.

- [ ] Tạo `PieceData.cs` (kế thừa `ScriptableObject`):
  ```csharp
  string pieceID
  Vector2Int[] cellOffsets
  Sprite tileSprite
  Color pieceColor
  int spawnWeight
  ```
- [ ] Thêm `[CreateAssetMenu]` attribute để tạo từ menu chuột phải
- [ ] Tạo **tất cả 7 hình dạng cơ bản** dưới dạng asset:
  - `I_1x1`, `I_1x2`, `I_1x3`, `I_2x2`
  - `L_Shape`, `T_Shape`, `Z_Shape` (hoặc biến thể phù hợp Block Blast)
- [ ] Viết `PieceDataValidator` editor script để cảnh báo nếu `cellOffsets` trống

**Định nghĩa hoàn thành:** Mở Project window thấy 7+ asset PieceData, inspector hiển thị hình dạng đúng.

---

### Ngày 3 — Sprites & Grid Placeholder
**Mục tiêu:** Có hình ảnh placeholder để làm việc, không block các ngày sau.

- [ ] Tạo sprite pixel art đơn giản cho ô vuông (16x16 hoặc 32x32), 5–6 màu
- [ ] Gán sprite vào từng `PieceData`
- [ ] Kẻ lưới 8x8 bằng **Tilemap** hoặc Debug.DrawLine để visualize
- [ ] Tạo `GridConfig.cs` (ScriptableObject):
  ```csharp
  int gridWidth = 8
  int gridHeight = 8
  float cellSize = 1.0f
  Vector3 gridOrigin
  ```

**Định nghĩa hoàn thành:** Nhìn vào Scene view thấy lưới 8x8 rõ ràng.

---

## PHASE 2 — LÕI LOGIC THUẦN C# (Ngày 4–10)

> Các class trong giai đoạn này **không kế thừa MonoBehaviour**, có thể test bằng NUnit.

### Ngày 4 — `GridModel`
**Mục tiêu:** Bộ não lưu trữ trạng thái lưới.

- [ ] Tạo `GridModel.cs`:
  ```csharp
  int[,] grid  // 0 = trống, >0 = pieceColorID
  event Action<Vector2Int, int> OnCellChanged
  event Action<List<Vector2Int>> OnLinesCleared
  
  void SetCell(Vector2Int pos, int value)
  int GetCell(Vector2Int pos)
  bool IsEmpty(Vector2Int pos)
  bool IsInBounds(Vector2Int pos)
  void Clear()
  ```
- [ ] Viết **Unit Test** (EditMode):
  - SetCell → GetCell trả về đúng giá trị
  - IsInBounds trả false với (-1, 0) và (8, 8)
  - OnCellChanged bắn ra khi SetCell

**Định nghĩa hoàn thành:** Tất cả unit test xanh.

---

### Ngày 5 — `ScoreModel` & `GameStateModel`
**Mục tiêu:** Quản lý điểm số và trạng thái game.

- [ ] Tạo `ScoreModel.cs`:
  ```csharp
  int CurrentScore  { get; private set; }
  int HighScore     { get; private set; }
  int ComboCount    { get; private set; }
  
  event Action<int, int> OnScoreUpdated  // (current, high)
  
  void AddScore(int lines, int tilesCleared)  // tính điểm theo combo
  void ResetCombo()
  void LoadHighScore(int saved)
  ```
- [ ] Tạo `GameStateModel.cs`:
  ```csharp
  enum GameState { Menu, Playing, Paused, GameOver }
  GameState CurrentState { get; }
  List<PieceData> AvailablePieces  // 3 khối trong khay
  
  event Action<GameState> OnStateChanged
  
  void TransitionTo(GameState newState)
  ```
- [ ] Viết Unit Test cho công thức điểm (combo x2, x3...)

**Định nghĩa hoàn thành:** Unit test điểm combo pass. Log state transition rõ ràng.

---

### Ngày 6 — Thuật toán `CheckAndClearMatches`
**Mục tiêu:** Logic xóa hàng/cột — phần phức tạp nhất của game.

- [ ] Tạo class `LineClearCalculator.cs` (static hoặc service):
  ```csharp
  // Quét ngang: tìm hàng nào đầy đủ 8 ô
  List<int> FindFullRows(int[,] grid)
  
  // Quét dọc: tìm cột nào đầy đủ 8 ô
  List<int> FindFullColumns(int[,] grid)
  
  // Gộp kết quả, trả về danh sách Vector2Int cần xóa
  List<Vector2Int> GetCellsToRemove(int[,] grid)
  ```
- [ ] Viết Unit Test:
  - Lưới có 1 hàng đầy → trả đúng 8 ô
  - Lưới có 1 cột đầy → trả đúng 8 ô
  - Hàng và cột giao nhau → không đếm trùng
  - Lưới trống → trả list rỗng

**Định nghĩa hoàn thành:** 8/8 test case xanh.

---

### Ngày 7 — `GridController` (Phần 1: Đặt khối)
**Mục tiêu:** Logic kiểm tra và đặt khối vào lưới.

- [ ] Tạo `GridController.cs` (MonoBehaviour, inject `GridModel`):
  ```csharp
  bool CanPlacePiece(PieceData piece, Vector2Int origin)
  // Kiểm tra: tất cả cellOffsets + origin có IsInBounds và IsEmpty không?
  
  bool TryPlacePiece(PieceData piece, Vector2Int origin)
  // Nếu CanPlace → SetCell cho từng offset → return true
  // Ngược lại → return false
  ```
- [ ] Viết Unit Test PlayMode đơn giản (dùng `GridModel` giả):
  - Đặt khối 2x2 vào góc (0,0) → thành công
  - Đặt chồng lên ô đã có → thất bại
  - Đặt lệch ra ngoài biên → thất bại

**Định nghĩa hoàn thành:** TryPlacePiece hoạt động đúng với 5 test case.

---

### Ngày 8 — `GridController` (Phần 2: Xóa hàng + Combo)
**Mục tiêu:** Sau khi đặt khối, tự động xóa hàng đầy và tính điểm.

- [ ] Tích hợp `LineClearCalculator` vào `GridController`:
  ```csharp
  void CheckAndClearMatches()
  // 1. Gọi GetCellsToRemove
  // 2. Đếm số hàng/cột đầy (để tính combo)
  // 3. Xóa các ô trong GridModel
  // 4. Gọi ScoreModel.AddScore(lineCount, cellCount)
  // 5. Bắn OnLinesCleared
  ```
- [ ] Test tích hợp: đặt đủ 8 khối vào 1 hàng → hàng bị xóa → điểm tăng
- [ ] Log điểm combo khi xóa 2 hàng cùng lúc

**Định nghĩa hoàn thành:** Console log đúng điểm, đúng số hàng xóa.

---

### Ngày 9 — `PieceSpawner` & Weighted Random
**Mục tiêu:** Sinh 3 khối mới theo thuật toán có trọng số.

- [ ] Tạo `PieceSpawner.cs`:
  ```csharp
  // Weighted Random: tổng weight, random [0, total), duyệt cộng dồn
  PieceData PickRandomPiece(List<PieceData> allPieces)
  
  // Fill khay: gọi PickRandomPiece x3, đưa vào GameStateModel
  void SpawnNewBatch(GameStateModel state, List<PieceData> pool)
  ```
- [ ] **DDA (Dynamic Difficulty Adjustment) cơ bản:** nếu điểm > 500, tăng weight khối phức tạp; điểm < 200, ưu tiên khối đơn giản
- [ ] Viết test: gọi 1000 lần, kiểm tra phân phối xấp xỉ đúng tỉ lệ weight

**Định nghĩa hoàn thành:** Log xác suất xuất hiện từng loại khối qua 1000 lần spawn.

---

### Ngày 10 — `IsGameOver` & `GameManager` lõi
**Mục tiêu:** Phát hiện game over, vòng lặp game hoạt động hoàn toàn ở lớp logic.

- [ ] Thêm vào `GridController`:
  ```csharp
  bool IsGameOver(List<PieceData> availablePieces)
  // Duyệt từng piece trong khay
  // Thử đặt vào TẤT CẢ 64 ô
  // Nếu không có ô nào đặt được → true
  ```
- [ ] Tạo `GameManager.cs` (MonoBehaviour, Singleton lazy):
  ```csharp
  void StartGame()   // Khởi tạo Model, spawn batch đầu
  void OnPiecePlaced()  // Sau khi đặt: CheckClear → check GameOver → spawn mới
  void GameOver()    // Chuyển state, lưu high score
  void RestartGame() // Reset tất cả model
  ```
- [ ] Chạy simulation trong console: tự động đặt khối ngẫu nhiên, game chạy đến game over

**Định nghĩa hoàn thành:** Console chạy 1 ván game hoàn chỉnh, không crash, log "GAME OVER" khi hết chỗ.

---

## PHASE 3 — CONTROLLER & INPUT (Ngày 11–17)

### Ngày 11 — `InputHandler` Touch/Drag
**Mục tiêu:** Chuyển đổi tọa độ màn hình → tọa độ lưới.

- [ ] Tạo `InputHandler.cs` dùng New Input System:
  ```csharp
  event Action<Vector2> OnDragStart    // world pos khi chạm
  event Action<Vector2> OnDragUpdate   // world pos khi kéo
  event Action<Vector2> OnDragEnd      // world pos khi nhả
  ```
- [ ] Tạo `GridCoordinateConverter.cs`:
  ```csharp
  Vector2Int WorldToGrid(Vector3 worldPos, GridConfig config)
  Vector3 GridToWorld(Vector2Int gridPos, GridConfig config)
  ```
- [ ] Test trên Editor (chuột) và thiết bị (ngón tay)
- [ ] Thêm **finger offset** (+1.5 unit lên trên) để ngón tay không che khối

**Định nghĩa hoàn thành:** Click/chạm vào ô bất kỳ, log ra tọa độ lưới đúng.

---

### Ngày 12 — Liên kết Input → GridController
**Mục tiêu:** Nhấn → đặt khối → lưới cập nhật.

- [ ] `GameManager` lắng nghe `InputHandler.OnDragEnd`
- [ ] Xác định khối nào đang được chọn (dựa vào vị trí drag start)
- [ ] Gọi `TryPlacePiece` với tọa độ lưới tính được
- [ ] Nếu đặt thành công: xóa khối khỏi khay, kiểm tra spawn batch mới
- [ ] Nếu thất bại: khối bay về vị trí cũ (chỉ log, animation làm sau)

**Định nghĩa hoàn thành:** Kéo khối lên lưới → lưới cập nhật trong `GridModel` (chưa cần visual).

---

### Ngày 13 — Màn hình Preview khi kéo
**Mục tiêu:** Khi kéo, highlight ô sẽ đặt vào (xanh = hợp lệ, đỏ = không hợp lệ).

- [ ] Tạo `PlacementPreview.cs`:
  - Trong `OnDragUpdate`: tính snap position gần nhất
  - Gọi `CanPlacePiece` → tô màu tương ứng
- [ ] Dùng **Tilemap** riêng tầng Preview hoặc `SpriteRenderer` tạm thời
- [ ] Ẩn preview khi `OnDragEnd`

**Định nghĩa hoàn thành:** Kéo khối trên lưới thấy highlight màu xanh/đỏ theo thời gian thực.

---

### Ngày 14 — Hệ thống khay chứa 3 khối
**Mục tiêu:** 3 khối hiển thị ở đáy màn hình, có thể chọn để kéo.

- [ ] Tạo 3 slot UI cố định ở đáy màn hình (World Space hoặc Screen Space)
- [ ] `PieceSpawner` render khối placeholder vào đúng slot
- [ ] Khi `OnDragStart`, xác định ngón tay nhấn vào slot nào → set selectedPiece
- [ ] Slot trống sau khi đặt thành công (alpha = 0.3 hoặc ẩn)

**Định nghĩa hoàn thành:** 3 khối hiện ở đáy, chọn được, biến mất sau khi đặt thành công.

---

### Ngày 15 — Luồng game đầy đủ (Game Loop)
**Mục tiêu:** Chơi được 1 ván game hoàn chỉnh từ đầu đến Game Over.

- [ ] Kết nối toàn bộ: Spawn → Chọn → Kéo → Đặt → Xóa → Điểm → Spawn mới → Game Over
- [ ] Xử lý edge case: đặt liên tiếp 3 khối hết khay → spawn batch mới
- [ ] Log mọi sự kiện game để debug
- [ ] Đảm bảo không có NullReferenceException nào

**Định nghĩa hoàn thành:** Chơi được 1 ván 5 phút không crash, đạt được Game Over screen (dù xấu).

---

### Ngày 16 — Tối ưu Game Feel cơ bản
**Mục tiêu:** Cảm giác kéo thả mượt mà, nhận phản hồi tức thì.

- [ ] Khối phóng to 1.2x khi nhấn vào (Scale tween)
- [ ] Khối dịch lên +1.5 unit khi kéo (không bị ngón tay che)
- [ ] Khối snap mượt về vị trí lưới gần nhất khi đang kéo
- [ ] Nếu thả sai vị trí: khối bounce về slot gốc (DOTween ease)

**Định nghĩa hoàn thành:** Trải nghiệm kéo thả giống ứng dụng native, không giật cục.

---

### Ngày 17 — Buffer & stress test
**Mục tiêu:** Không có bug logic nào còn sót sau phase này.

- [ ] Test tất cả edge case:
  - Đặt khối ở góc biên (0,0), (7,7), (0,7), (7,0)
  - Xóa đồng thời hàng + cột giao nhau
  - Điểm combo x3 (3 hàng cùng lúc)
  - Game over ngay từ đầu (lưới gần đầy)
- [ ] Fix tất cả bug tìm được
- [ ] Code review: đảm bảo không có logic trong View

**Định nghĩa hoàn thành:** 0 bug logic đã biết.

---

## PHASE 4 — VIEW LAYER & VISUAL (Ngày 18–22)

### Ngày 18 — `ObjectPoolManager`
**Mục tiêu:** Hệ thống Pool tập trung, tránh GC trên Mobile.

- [ ] Tạo `ObjectPoolManager.cs` (Singleton):
  ```csharp
  // Generic pool
  T Get<T>(T prefab) where T : Component
  void Return<T>(T instance) where T : Component
  
  // Pre-warm pools khi khởi động
  void Prewarm(GameObject prefab, int count, Transform parent)
  ```
- [ ] Tạo các pool:
  - `TilePool` (64+ instances)
  - `ParticlePool` (20 instances)
  - `FloatingTextPool` (10 instances — hiển thị "+100", "+COMBO!")
- [ ] Đo bộ nhớ trước/sau khi dùng Pool (Unity Profiler)

**Định nghĩa hoàn thành:** Profiler không có spike GC khi đặt khối và xóa hàng.

---

### Ngày 19 — `TileView` & `GridView`
**Mục tiêu:** Lưới hiển thị đẹp, ô xuất hiện/biến mất có animation.

- [ ] Tạo `TileView.cs` Prefab:
  - `SpriteRenderer` với sprite ô vuông
  - `DOTween`: scale up khi spawn (0 → 1), flash trắng trước khi biến mất
- [ ] Tạo `GridView.cs` (MonoBehaviour):
  - Lắng nghe `GridModel.OnCellChanged` → Get TileView từ Pool, set sprite/color, đặt vào world pos
  - Lắng nghe `GridModel.OnLinesCleared` → Flash tất cả tile trong list → Return về Pool → Spawn Particle
- [ ] Đường kẻ lưới nhẹ (background grid lines) làm guide

**Định nghĩa hoàn thành:** Đặt khối → tile hiện lên. Xóa hàng → flash trắng → biến mất với particle.

---

### Ngày 20 — `DraggablePieceView`
**Mục tiêu:** Khối hiển thị đúng hình dạng, kéo mượt mà.

- [ ] Tạo `DraggablePieceView.cs`:
  - Nhận `PieceData` → tự render ra đúng hình dạng (spawn TileView child cho mỗi offset)
  - Theo dõi input từ `InputHandler`: lerp vị trí khi kéo
  - Shadow effect nhẹ khi đang kéo (outline hoặc drop shadow)
- [ ] 3 instances trong khay, tự setup khi `GameStateModel.AvailablePieces` thay đổi
- [ ] Hiệu ứng "thở" (idle animation: scale 1.0 → 1.05 → 1.0 loop) khi không được chọn

**Định nghĩa hoàn thành:** 3 khối ở đáy màn hình đẹp, kéo lên mượt, hình dạng đúng với PieceData.

---

### Ngày 21 — `UIManager` & HUD
**Mục tiêu:** Điểm số, kỷ lục, combo hiển thị rõ ràng.

- [ ] Tạo `UIManager.cs`:
  - Lắng nghe `ScoreModel.OnScoreUpdated` → cập nhật Text
  - Animation điểm: số tăng dần (counter tween) thay vì nhảy ngay
  - High score label pulse khi bị phá vỡ
- [ ] `FloatingText`: "+100", "+COMBO x3!" xuất hiện ở vị trí xóa hàng, bay lên và mờ dần
- [ ] Màn hình **Game Over**: hiển thị điểm cuối, kỷ lục, nút Restart
- [ ] Màn hình **Main Menu**: nút Play, Best Score

**Định nghĩa hoàn thành:** UI đầy đủ, không có Text nào "stale" (chưa cập nhật).

---

### Ngày 22 — Particle & Camera Shake
**Mục tiêu:** Juice! Phản hồi hình ảnh cho mọi hành động.

- [ ] Particle System cho xóa ô (màu theo màu tile)
- [ ] Camera shake nhẹ khi xóa 2+ hàng cùng lúc (DOTween Punch)
- [ ] Screen flash (white overlay, alpha 0 → 0.3 → 0) khi xóa combo lớn
- [ ] particle burst nhỏ khi đặt khối thành công

**Định nghĩa hoàn thành:** Chơi cảm giác "satisfying", có phản hồi visual cho mọi action.

---

## PHASE 5 — SERVICES (Ngày 23–26)

### Ngày 23 — `SaveManager`
**Mục tiêu:** Lưu/nạp high score, không mất dữ liệu khi tắt app.

- [ ] Tạo `SaveManager.cs`:
  ```csharp
  void SaveHighScore(int score)  // PlayerPrefs
  int LoadHighScore()
  void SaveSettings(GameSettings s)  // JSON + PlayerPrefs
  GameSettings LoadSettings()
  ```
- [ ] Tích hợp vào `GameManager.GameOver()` → gọi SaveHighScore nếu record mới
- [ ] Tích hợp vào khởi động game → load high score vào `ScoreModel`
- [ ] Test: uninstall/reinstall giữ dữ liệu? (PlayerPrefs thì không — chấp nhận được cho v1)

**Định nghĩa hoàn thành:** Tắt app, mở lại → high score vẫn hiển thị đúng.

---

### Ngày 24 — `SoundManager`
**Mục tiêu:** Âm thanh game đầy đủ, điều chỉnh được.

- [ ] Tạo `SoundManager.cs` (Singleton với AudioSource pool):
  ```csharp
  void PlaySFX(AudioClip clip, float volume = 1f)
  void PlayBGM(AudioClip clip, bool loop = true)
  void SetMasterVolume(float v)
  void SetSFXVolume(float v)
  void ToggleMute()
  ```
- [ ] Chuẩn bị / download âm thanh:
  - `place_piece.wav` — tiếng "click" nhẹ
  - `line_clear.wav` — tiếng "pop" thỏa mãn
  - `combo.wav` — tiếng tăng cao cho combo
  - `game_over.wav` — tiếng kết thúc
  - `bgm.mp3` — nhạc nền chilled/lofi
- [ ] Nút Mute trong Settings
- [ ] Không dùng `AudioSource` thẳng trong View — luôn qua `SoundManager`

**Định nghĩa hoàn thành:** Âm thanh phát đúng lúc, không nghe tiếng "tuý" bị cắt.

---

### Ngày 25 — DDA & Settings
**Mục tiêu:** Game tự cân bằng độ khó, player có thể tùy chỉnh.

- [ ] `PieceSpawner` nâng cấp DDA:
  - Score < 200: chỉ spawn khối 1x1, 1x2, 2x2
  - Score 200–800: mix đều 7 loại
  - Score > 800: tăng tỉ lệ khối phức tạp (L, T, Z)
- [ ] Settings ScriptableObject: `SoundVolume`, `MusicVolume`, `VibrateEnabled`
- [ ] Haptic feedback khi đặt khối (Android: `Handheld.Vibrate()`)
- [ ] Settings Panel UI: thanh trượt âm lượng, toggle rung

**Định nghĩa hoàn thành:** Chơi từ đầu → nhận thấy độ khó tăng dần tự nhiên.

---

### Ngày 26 — Màn hình Menu & Scene Transition
**Mục tiêu:** Game có đầu có đuôi, trải nghiệm hoàn chỉnh.

- [ ] Scene `MainMenu`:
  - Logo game, nền động (khối rơi nhẹ ở background)
  - Nút Play, Settings, Quit
  - Hiển thị Best Score
- [ ] Transition mượt giữa Menu ↔ Game (DOTween fade black)
- [ ] Pause menu trong game: Resume, Restart, Main Menu
- [ ] Xử lý nút Back (Android) đúng cách

**Định nghĩa hoàn thành:** Từ Menu → chơi → game over → về Menu, không có gì bị stuck.

---

## PHASE 6 — POLISH & BUILD (Ngày 27–30)

### Ngày 27 — Tối ưu Performance
**Mục tiêu:** Đạt 60 FPS trên thiết bị tầm trung.

- [ ] Chạy **Unity Profiler** trên thiết bị thật:
  - CPU spike ở đâu? (Thường là New Object hoặc Physics)
  - Memory có tăng liên tục không? (GC leak)
- [ ] Kiểm tra Object Pool hoạt động đúng (không có new GameObject ngoài pool)
- [ ] Giảm **Draw Call**: Atlas sprite cho tất cả tile (Sprite Atlas)
- [ ] Tắt tính năng nặng không cần: Anti-aliasing, Real-time shadows
- [ ] Target FPS: `Application.targetFrameRate = 60`

**Định nghĩa hoàn thành:** Profiler < 16ms per frame trên thiết bị Android tầm trung.

---

### Ngày 28 — QA (Quality Assurance)
**Mục tiêu:** Tìm và fix tất cả bug trước khi build.

- [ ] Test matrix:
  | Scenario | Expected | Pass? |
  |---|---|---|
  | Đặt khối ở (0,0) | Thành công | |
  | Đặt chồng lên ô có sẵn | Thất bại, bay về | |
  | Xóa hàng + cột cùng lúc | +Điểm combo | |
  | Tất cả slot hết chỗ | Game Over | |
  | Tắt app giữa ván | High score lưu | |
  | Pause → Resume | Tiếp tục đúng | |
  | Âm lượng về 0 | Không nghe gì | |
- [ ] Test trên ít nhất 2 thiết bị (màn hình nhỏ + lớn)
- [ ] Test portrait và landscape (nếu hỗ trợ)
- [ ] Fix tất cả bug Priority 1 & 2

**Định nghĩa hoàn thành:** Không có bug P1. Danh sách P2 đã có kế hoạch xử lý.

---

### Ngày 29 — UI Polish & Accessibility
**Mục tiêu:** Game trông đẹp, dễ đọc trên mọi màn hình.

- [ ] Kiểm tra UI trên nhiều tỉ lệ màn hình (16:9, 18:9, 19.5:9)
- [ ] Font đủ lớn (tối thiểu 24sp cho text phụ, 36sp cho điểm chính)
- [ ] Màu sắc đủ tương phản (WCAG AA: contrast ratio ≥ 4.5:1)
- [ ] Icon thay thế cho text quan trọng (accessibility)
- [ ] Loading screen / splash screen khi khởi động
- [ ] App icon và tên game

**Định nghĩa hoàn thành:** Screenshot game trông ổn để đăng lên store.

---

### Ngày 30 — Build & Release
**Mục tiêu:** APK/IPA sạch, sẵn sàng distribute.

- [ ] Cập nhật `ProjectSettings`:
  - `Bundle ID`: `com.yourname.puzzlegame01`
  - `Version`: `1.0.0`, `Build`: `1`
  - `Product Name`, `Company Name`
- [ ] Cấu hình **Android Build**:
  - Min SDK: 21 (Android 5.0)
  - Target SDK: 34
  - IL2CPP backend
  - ARM64 + ARMv7
- [ ] Build APK Release (Signed)
- [ ] Test APK trên thiết bị thật — không cài từ Unity
- [ ] (Optional) Upload lên **Google Play Internal Testing**
- [ ] Viết `RELEASE_NOTES.md` ngắn gọn

**Định nghĩa hoàn thành:** APK cài được trên thiết bị không cần developer mode, chơi được hoàn chỉnh.

---

## CHECKLIST KỸ THUẬT TỔNG HỢP

### Architecture Compliance
- [ ] Không có logic game trong bất kỳ `View` class nào
- [ ] Không có `GameObject.Find()` hoặc `FindObjectOfType()` trong production code
- [ ] Tất cả dependency được inject qua Inspector hoặc constructor, không dùng global static (ngoại trừ Singleton có kiểm soát)
- [ ] Mọi event (`Action`) đều được unsubscribe trong `OnDestroy`

### Performance
- [ ] 0 `new` allocation trong Update loop
- [ ] Không dùng `Camera.main` trong Update (cache trước)
- [ ] Tất cả string concatenation trong UI dùng `StringBuilder` hoặc `string.Format`

### Code Quality
- [ ] Không có `Debug.Log` trong production code (dùng conditional compile `#if UNITY_EDITOR`)
- [ ] Không có magic number — tất cả hằng số trong Config ScriptableObject
- [ ] Naming convention nhất quán: `PascalCase` cho class/method, `camelCase` cho field, `_camelCase` cho private field

---

## MILESTONE CHECKPOINT

| Ngày | Milestone | Thành công khi... |
|---|---|---|
| **3** | Foundation Done | Project build được, có 7 PieceData assets |
| **10** | Core Logic Done | Simulation console chạy 1 ván hoàn chỉnh |
| **17** | Gameplay Done | Chơi được 5 phút không crash, không bug logic |
| **22** | Visual Done | Game trông đẹp, có juice, satisfying để chơi |
| **26** | Feature Complete | Đầy đủ tính năng: save, sound, menu, DDA |
| **30** | Ship It! | APK cài được, chơi được, sẵn sàng release |

---

## RỦI RO & DỰ PHÒNG

| Rủi ro | Khả năng | Kế hoạch dự phòng |
|---|---|---|
| DOTween khó tích hợp | Thấp | Dùng `Coroutine` + `Lerp` thủ công |
| Input touch trên Android không mượt | Trung bình | Test sớm (Ngày 11), có fallback bằng chuột |
| Performance <30 FPS | Trung bình | Cut bớt particle, giảm pool size |
| Art asset không kịp | Cao | Dùng màu đơn sắc + shape đơn giản, art sau |
| Scope creep (thêm tính năng) | Cao | **Freeze scope sau Ngày 15**, mọi ý tưởng mới → backlog |

---

*Ngày bắt đầu: 07/03/2026 | Ngày kết thúc dự kiến: 05/04/2026*
*Kiến trúc tham khảo: [`architecture.md`](./architecture.md)*
