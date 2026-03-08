# KIẾN TRÚC HỆ THỐNG: BLOCK BLAST CLONE (UNITY 2D)

## 1. Mục tiêu kiến trúc (Architecture Goals)
- **Decoupling (Giảm thiểu phụ thuộc):** Logic game (Model) không chứa code liên quan đến Unity Engine (Transform, SpriteRenderer, GameObject). Giao diện (View) chỉ lắng nghe sự kiện từ Logic để hiển thị các asset pixel art hoặc chạy hiệu ứng.
- **Performance (Hiệu suất):** Sử dụng Object Pooling cho toàn bộ các khối nhỏ (Tiles) và hiệu ứng (VFX) để tối ưu hóa bộ nhớ trên thiết bị Mobile.
- **Data-Driven (Hướng dữ liệu):** Sử dụng ScriptableObject để cấu hình các loại khối, giúp dễ dàng cân bằng game (balancing) mà không cần chạm vào code.

---

## 2. Lớp Dữ liệu (Data Layer - ScriptableObjects)
*Dùng để lưu trữ cấu hình tĩnh. Tạo menu trong Unity để designer/developer dễ dàng thêm bớt khối.*

- **`PieceData` (Kế thừa ScriptableObject)**
  - `string pieceID`: Tên định danh khối (VD: "Square_2x2", "L_Shape_Right").
  - `Vector2Int[] cellOffsets`: Tọa độ tương đối của các ô vuông con.
  - `Sprite tileSprite`: Hình ảnh pixel art đại diện cho khối này.
  - `int spawnWeight`: Trọng số xuất hiện (Dùng cho hệ thống Random động).
  - `Color pieceColor`: Màu sắc (nếu dùng chung 1 sprite trắng và đổi màu).

---

## 3. Lớp Trạng thái (Model Layer - Pure C#)
*Chứa dữ liệu thuần túy, không kế thừa MonoBehaviour. Là "bộ não" tính toán của game.*

- **`GridModel`**
  - Trách nhiệm: Quản lý mảng 2D lõi.
  - Dữ liệu: `int[,] grid` (Kích thước 8x8. `0` = Trống, `>0` = Đã bị lấp).
  - Sự kiện (Events): 
    - `Action<Vector2Int, int> OnCellChanged` (Bắn ra khi 1 ô thay đổi trạng thái).
    - `Action<List<Vector2Int>> OnLinesCleared` (Bắn ra danh sách các ô vừa bị xóa).

- **`ScoreModel`**
  - Trách nhiệm: Quản lý điểm số.
  - Dữ liệu: `int currentScore`, `int highScore`, `int comboCount`.
  - Sự kiện: `Action<int, int> OnScoreUpdated` (Điểm hiện tại, Kỷ lục).

- **`GameStateModel`**
  - Trách nhiệm: Theo dõi luồng game.
  - Dữ liệu: `GameState currentState` (Enum: Menu, Playing, GameOver).
  - `List<PieceData> currentAvailablePieces` (3 khối đang chờ dưới khay).

---

## 4. Lớp Điều phối (Controller/Presenter Layer - MonoBehaviour)
*Nhận Input, xử lý logic trên Model và yêu cầu View cập nhật.*

- **`GameManager` (Singleton hợp lệ hoặc sử dụng Dependency Injection)**
  - Trách nhiệm: Quản lý vòng lặp game, khởi tạo các Model, gọi `SaveManager` khi cần.
  - Chức năng: `StartGame()`, `GameOver()`, `Restart()`.

- **`GridController`**
  - Trách nhiệm: Xử lý logic đặt khối và quét hàng.
  - Chức năng:
    - `bool TryPlacePiece(PieceData piece, Vector2Int gridPos)`: Kiểm tra đè lấp, ngoài ranh giới.
    - Cập nhật `GridModel` nếu đặt thành công.
    - `CheckAndClearMatches()`: Quét hàng ngang/dọc, gọi hàm xóa và tính điểm combo.
    - `bool IsGameOver()`: Duyệt mảng xem 3 khối dưới khay còn chỗ nào đặt vừa không.

- **`PieceSpawner`**
  - Trách nhiệm: Sinh khối mới.
  - Chức năng: Thuật toán Weighted Random để nạp 3 `PieceData` mới vào `GameStateModel` khi khay trống, dựa trên điểm số hiện tại (DDA).

- **`InputHandler`**
  - Trách nhiệm: Xử lý thao tác chạm trên Mobile.
  - Chức năng: Bắt sự kiện Touch/Drag, gửi tọa độ màn hình cho hệ thống quy đổi sang tọa độ lưới 2D.

---

## 5. Lớp Hiển thị (View Layer - MonoBehaviour)
*Chỉ hiển thị hình ảnh, phát âm thanh, chạy animation (khuyên dùng thư viện DOTween).*

- **`GridView`**
  - Trách nhiệm: Ráp nối logic lưới với hình ảnh.
  - Chức năng: Lắng nghe `GridModel.OnCellChanged`. Xin 1 `TileView` từ `ObjectPool` để đặt vào đúng tọa độ thế giới (World Space). Lắng nghe `OnLinesCleared` để trả `TileView` về Pool và chạy Particle System.

- **`TileView`**
  - Trách nhiệm: Quản lý 1 ô vuông nhỏ duy nhất trên lưới.
  - Component: `SpriteRenderer`.
  - Chức năng: Chạy animation chớp sáng (flash) trước khi biến mất.

- **`DraggablePieceView`**
  - Trách nhiệm: Hiển thị nguyên 1 cụm khối người chơi đang kéo.
  - Chức năng: Đọc `PieceData` để tự động render ra hình dáng. Xử lý logic Game Feel (phóng to khi chạm, dịch lên trên ngón tay (offset), mượt mà bay về vị trí cũ nếu đặt sai).

- **`UIManager`**
  - Trách nhiệm: Cập nhật Text điểm số, hiển thị màn hình Game Over, Menu.

---

## 6. Lớp Dịch vụ (Services Layer)

- **`ObjectPoolManager`**
  - Trách nhiệm: Quản lý các Pool (TilePool, ParticlePool, FloatingTextPool). Tránh Garbage Collection.

- **`AudioManager`**
  - Trách nhiệm: Phát SFX (tiếng thả khối, tiếng vỡ hàng) thay đổi cao độ (pitch) theo combo.

- **`HapticManager`**
  - Trách nhiệm: Gọi API rung của iOS/Android.
  
- **`SaveManager`**
  - Trách nhiệm: Serialize `GridModel` và `ScoreModel` ra file JSON lưu vào máy.