# BÀN GIAO LỘ TRÌNH HỌC .NET — CINEMA BOOKING

**Cập nhật:** 26/11/2026  
**Mục đích:** Dán/tải file này vào một cuộc trò chuyện ChatGPT mới, kể cả trên tài khoản khác, để tiếp tục học mà không phải bắt đầu lại.

> **Chỉ dẫn cho trợ lý ChatGPT mới:** Hãy coi tài liệu này là bản bàn giao tiến độ. Giữ nguyên các bài đã hoàn thành, không bắt học lại. Bám sát repository Spring Boot gốc khi chuyển sang ASP.NET Core. Nếu cần kết luận chính xác về một lớp/bảng chưa được cung cấp, yêu cầu người học gửi file hoặc kết quả SQL tương ứng; không tự đoán cấu trúc. Trả lời bằng **tiếng Việt**, lý thuyết **ngắn gọn, có ví dụ dễ hiểu**, dành **~80% cho thực hành**, cho người học tự code trước khi chấm, review trực tiếp file, chấm thang **10**, chỉ rõ lỗi và cách sửa, và hiển thị **thanh/nhãn tiến độ** từng buổi. Không tự nhảy sang bài tiếp theo khi chưa đạt điều kiện, trừ khi người học yêu cầu. Tránh giao Console App, endpoint giả hoặc bài tập không liên quan Cinema Booking.

---

## 1. Mục tiêu, môi trường và nguyên tắc học

- Người học chuẩn bị **thực tập .NET**, đã biết OOP và từng làm **Java/Spring Boot**; mục tiêu chuyển chức năng thực tế từ dự án Spring Boot Cinema Booking sang ASP.NET Core.
- Repository Spring Boot gốc: **https://github.com/HoangDung05/cinema-booking**. Đây là nguồn tham chiếu để so sánh, không giả định những chức năng chưa được kiểm tra.
- Công nghệ đang sử dụng: **C# / .NET 10 / ASP.NET Core 10 / EF Core 10 / MySQL 8 / Docker / Visual Studio / DBeaver / Postman**.
- EF provider từng dùng: **Oracle `MySql.EntityFrameworkCore` 10.0.12**, EF Core 10.0.15. Với provider này cấu hình `options.UseMySQL(connectionString)` (**viết hoa SQL**); đừng nhầm với Pomelo `UseMySql`.
- Project API: **`CinemaBooking.Api`**; `CinemaDbContext` tại namespace `CinemaBooking.Api.Data`; model tại `CinemaBooking.Models`; service tại `CinemaBooking.Services`; controller tại `CinemaBooking.Api.Controllers`.
- Ưu tiên code thực tế có trong Spring Boot; hạn chế lý thuyết lan man, bài console hoặc tự phát minh chức năng ngoài kế hoạch.
- Với bài thực hành: giao nhiệm vụ cụ thể → người học gửi code hoặc kết quả Postman → review theo thang 10 → sửa nếu cần → xác nhận hoàn thành → cập nhật tiến độ.

## 2. TIẾN ĐỘ CHÍNH XÁC

| Buổi | Nội dung | Trạng thái |
|---|---|---|
| **1** | C# / ASP.NET Core nền tảng, API Movie từ dữ liệu trong RAM | **100% (7/7), tổng kết ~9.5/10** |
| **2** | EF Core + MySQL, DbContext, Entity Mapping, LINQ, async/await | **100% (7/7)**; lý thuyết Bài 2.7 **8.5/10**, thực hành **10/10** |
| **3** | CRUD Movie, DTO, validation, xử lý lỗi HTTP | **100% (6/6)**; bài kiểm thử End-to-End **10/10** |
| **4** | Showtime Management: GET, POST, PUT, DELETE, kiểm thử | **Đang thực hiện**; GET và POST hoàn thành, PUT đã code Controller/Service và đang kiểm thử Postman |

**Điểm dừng hiện tại:** Buổi 4 — **Showtime Management, kiểm thử PUT trong Postman**. Không học lại Buổi 1–3.

### Buổi 4 – các mốc

1. **GET Showtime:** truy vấn danh sách, Include Movie/Room/Cinema và trả DTO — đã hoàn thành.
2. **POST Showtime:** DTO validation, kiểm tra Movie/Room tồn tại, tính EndTime từ Movie.Duration, kiểm tra trùng lịch, transaction và khóa Room — đã code và kiểm thử thành công.
3. **PUT Showtime:** DTO, Interface/Service, Controller — đã code; Build và kiểm thử Postman cần xác nhận tiếp.
4. **DELETE Showtime:** chưa bắt đầu.
5. **Kiểm thử tích hợp:** kiểm thử các trường hợp thành công, 400, 404, 409; tổng kết và commit — chưa hoàn thành.

Tiến độ Buổi 4 không nên tăng/đánh dấu hoàn thành chỉ vì đã viết code; cần dựa trên Build và kết quả kiểm thử thực tế.

## 3. Trạng thái dự án Cinema Booking .NET hiện tại

### 3.1. Database và kết nối

- Database ban đầu từng dùng: **`cinema_booking`** từ hệ thống Spring Boot.
- Database dành riêng thực hành CRUD: **`cinema_booking_practice`**. Người học đã cấu hình kết nối thành công, thực hiện POST/PUT/DELETE và đối chiếu MySQL.
- Từng có nhầm lẫn vì máy có **hai MySQL server**: MySQL Windows trên cổng **3306** và MySQL Docker publish **3307 → 3306** (container từng tên `cinema_mysql`). Người học đã xử lý được; **không được mặc định server/port đang dùng là gì** nếu chưa xem connection string đang hoạt động.
- Connection String lưu trong **User Secrets** (hoặc cấu hình phù hợp); không đưa mật khẩu vào tài liệu này, không yêu cầu người học đăng mật khẩu.
- `Program.cs` đăng ký `CinemaDbContext` và `IMovieService` bằng DI, ví dụ:
  ```csharp
  builder.Services.AddScoped<IMovieService, MovieService>();
  builder.Services.AddDbContext<CinemaDbContext>(options =>
      options.UseMySQL(connectionString));
  ```
- Đây là schema MySQL có sẵn từ Spring Boot: **không tự chạy `Add-Migration`/`Update-Database` hoặc thay đổi bảng khi chưa soát schema, migration history/baseline và backup**.

### 3.2. Model Movie và EF Mapping

Model `Movie` hiện có các trường (tên trong C#):

```csharp
int Id;
string Title;
string? Description;
int Duration;
DateOnly? ReleaseDate;
string? PosterUrl;
string? TrailerUrl;
MovieStatus Status;
```

`MovieStatus`:

```csharp
[JsonConverter(typeof(JsonStringEnumConverter<MovieStatus>))]
public enum MovieStatus { COMING_SOON, NOW_SHOWING, ENDED }
```

`CinemaDbContext` có `DbSet<Movie> Movies`, ánh xạ bảng **`movies`** với cột `id` tự tăng, `title` varchar(255), `description` TEXT, `duration` int, `release_date` date, `poster_url`, `trailer_url` varchar(255), `status` enum chuỗi. Trường `ReleaseDate` dùng chuyển đổi `DateOnly ↔ DateTime` do Oracle MySQL EF Core Provider đã từng gây lỗi khi đọc `DATE`:

```csharp
entity.Property(s => s.ReleaseDate)
    .HasColumnType("date")
    .HasColumnName("release_date")
    .IsRequired(false)
    .HasConversion(
        date => date.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime));
```

`Status` map `.HasConversion<string>().HasDefaultValue(MovieStatus.COMING_SOON)`.

### 3.3. Các API Movie đã triển khai, kiểm thử

| Method | Route | Công dụng / Status thành công |
|---|---|---|
| GET | `/movies` | Danh sách phim, lọc `status` và `date` — 200 |
| GET | `/movies/{id}` | Chi tiết — 200 hoặc 404 |
| GET | `/movies/now-showing` | Phim `NOW_SHOWING` — 200 |
| GET | `/movies/coming-soon` | Phim `COMING_SOON` — 200 |
| GET | `/movies/search?keyword=...` | Tìm theo tên, trim từ khóa, từ khóa trống → 400 |
| POST | `/movies` | Tạo phim — **201 Created**, `CreatedAtAction` |
| PUT | `/movies/{id}` | Sửa phim — **200 OK**, không có → 404 |
| DELETE | `/movies/{id}` | Xóa phim — **204 No Content**, không có → 404 |

- Các truy vấn GET chủ yếu dùng `AsNoTracking()` và `ToListAsync()`/`FirstOrDefaultAsync()`.
- `FilterMoviesAsync(MovieStatus? status, DateOnly? date)` xây `IQueryable` và các `Where()` nối tiếp có nghĩa **AND**.
- C# `async/await` xuyên suốt Controller → Service → EF Core; không chạy nhiều thao tác đồng thời trên **cùng một `DbContext`**.
- `GetMovieByIdAsync()` đọc không tracking; `UpdateMovieAsync()` và `DeleteMovieAsync()` dùng `FindAsync(id)` để nhận entity được tracking.
- `CreateMovieAsync(Movie)` dùng `_context.Movies.Add(movie)` rồi `await _context.SaveChangesAsync()`.
- `UpdateMovieAsync(int, Movie)` cập nhật **Title, Description, Duration, ReleaseDate, PosterUrl, TrailerUrl, Status**, không đổi ID, rồi SaveChangesAsync, trả `Movie?` hoặc null.
- `DeleteMovieAsync(int)` dùng `FindAsync`, `Remove`, `SaveChangesAsync`; trả `bool`.

### 3.4. DTO và Validation đã làm

- Tạo `CreateMovieRequest` trong namespace **`CinemaBooking.Api.DTOs.Movies`** (thư mục `DTOs/Movies`).
- DTO không có `Id`, có `Title`, `Description`, `Duration`, `ReleaseDate`, `PosterUrl`, `TrailerUrl`, `Status`.
- `Title`: `[Required]`, `[StringLength(255)]`.
- `Duration`: `[Range(1, int.MaxValue)]`, kiểu `int` (giá trị 0 bị từ chối, kể cả trường hợp client không gửi).
- `PosterUrl`, `TrailerUrl`: `[StringLength(255)]`.
- `Status`: `[Required] public MovieStatus? Status { get; set; }` để phân biệt client **không gửi** với enum mặc định `COMING_SOON`.
- Controller chuyển DTO thành Entity `Movie`, dùng `request.Status!.Value` sau model validation thành công.
- Bài tập hiện tái sử dụng DTO `CreateMovieRequest` cho cả POST và PUT; về sau có thể tách `UpdateMovieRequest` theo nhu cầu.
- `[ApiController]` tự động trả 400 khi validation không đạt.
- POST hợp lệ → 201; Title rỗng → 400; Duration = 0 → 400. PUT Duration = 0 → 400; PUT hợp lệ → 200. **Tất cả đã được người học kiểm thử thành công.**

### 3.5. Xử lý lỗi 409 và ngoại lệ

- Có class `CinemaBooking.Api.Exceptions.ResourceConflictException : Exception` với constructor `(string message) : base(message)`.
- `MovieService.DeleteMovieAsync` bắt **`DbUpdateException` khi `InnerException` là `MySql.Data.MySqlClient.MySqlException` mã **1451** (foreign key RESTRICT/NO ACTION), ném `ResourceConflictException`.
- `MovieController.DeleteMovie` bắt exception này và trả `Conflict(new { message = ex.Message })` (mẫu được đề xuất); các lỗi khác **không được ép** thành 409.
- Đã sửa thiếu dấu `;` tại `using MySql.Data.MySqlClient;` và **Build thành công**.
- Đã kiểm thử DELETE ID không tồn tại → 404; DELETE hợp lệ → 204; GET lại → 404.
- **HTTP 409 chưa được kiểm thử tích hợp**, vì chưa có một trường hợp khóa ngoại thực tế phù hợp; không tuyên bố đã test 409.

### 3.6. Showtime Management – tiến độ và code đã làm

**Lưu ý:** Đây là cập nhật mới nhất về tiến độ thực tế; thay cho kế hoạch cũ ghi Buổi 4 đang ở 4.1 khảo sát DB. Người học đã tiếp tục triển khai chức năng Showtime. Không bắt đầu lại 4.1 hoặc map entity từ đầu.

#### GET Showtime
- Service lấy danh sách Showtime có `Include(s => s.Movie)`, `Include(s => s.Room)`, `ThenInclude(r => r.Cinema)`, `AsNoTracking()` và `ToListAsync()`.
- Controller map sang `ShowtimeResponse` với các thông tin: Id, MovieTitle, RoomName, CinemaName, StartTime, EndTime, Price.

#### POST Showtime – đã code và kiểm thử
- Request DTO `CreateShowtimeRequest`: MovieId, RoomId, StartTime, Price; validation ID và giá.
- Service xác thực Movie và Room tồn tại; tự tính EndTime dựa trên thời lượng phim, không lấy EndTime từ client.
- Dùng transaction `ReadCommitted`, khóa Room bằng `SELECT ... FOR UPDATE`, kiểm tra lịch chồng lấn trong cùng phòng.
- Kiểm tra thời gian kết thúc của lịch cũ: dùng EndTime nếu có, hoặc `StartTime + Movie.Duration` nếu EndTime null (hỗ trợ dữ liệu legacy).
- Nếu trùng lịch, ném `ShowtimeConflictException`; Controller trả **409 Conflict**. Movie/Room không hợp lệ trả **400 Bad Request**.
- Tạo Showtime, `SaveChangesAsync()`, commit; sau commit truy vấn lại entity kèm Movie/Room/Cinema để trả response đầy đủ.
- POST đã kiểm thử: ID âm bị validation trả 400; Movie không tồn tại trả 400; tạo hợp lệ lưu DB và response có tên Movie/Room/Cinema.

#### PUT Showtime – code hiện tại
- Route Controller: `PUT /showtimes/{id:int}`.
- Service `UpdateShowtimeAsync(id, request)`:
  1. Xác thực Movie và Room.
  2. Tính EndTime từ Movie.Duration.
  3. Mở transaction `ReadCommitted`.
  4. Khóa Showtime cần sửa bằng `LockShowtimeAsync(id)` (`SELECT ... FOR UPDATE`); không có thì rollback và trả null.
  5. Khóa Room đích bằng `LockRoomAsync`.
  6. Gọi `HasScheduleConflictAsync(..., excludeShowtimeId: id)` để không tự coi lịch đang sửa là lịch xung đột.
  7. Cập nhật MovieId, RoomId, StartTime, EndTime, Price; SaveChanges và Commit.
  8. Query lại sau Commit (ngoài try/catch transaction) kèm Movie/Room/Cinema và trả Showtime.
- Controller PUT: null → 404; success → map `ShowtimeResponse` và 200; `ShowtimeConflictException` → 409; `ArgumentException` → 400.
- Đã review cấu trúc Controller và Service; đánh giá logic Controller phù hợp. Cần Build/test Postman xác nhận kết quả.
- Test PUT đầu tiên nên giữ MovieId, RoomId, StartTime của Showtime hiện có và chỉ thay Price, để tránh tạo xung đột lịch. PUT là cập nhật đầy đủ nên vẫn gửi đủ các field DTO.
- Không mặc định ID 55 hoặc MovieId/RoomId/time đã nhớ là còn đúng; hãy lấy dữ liệu thực tế hiện tại.

#### Conflict checker và locking
- `HasScheduleConflictAsync(roomId, newStart, newEnd, excludeShowtimeId)` lấy các lịch trong cùng Room có StartTime trước newEnd, loại lịch đang sửa khi có exclude ID, rồi tính existingEnd và kiểm tra overlap:
  `newStart < existingEnd && newEnd > existingStart`.
- `LockShowtimeAsync(id)` dùng `SELECT * FROM showtimes WHERE id = {id} FOR UPDATE` và `FirstOrDefaultAsync()` để có thể trả null.
- `LockRoomAsync(roomId)` khóa Room đích để các thao tác ghi lịch dùng cùng cơ chế có thể tuần tự hóa việc kiểm tra lịch theo phòng.
- Đây là cơ chế bảo vệ cạnh tranh ở mức transaction/row-lock; không tuyên bố bảo đảm tuyệt đối cho mọi writer nếu các thao tác khác không dùng cùng locking path. PUT hiện chưa có optimistic concurrency token, nên các cập nhật tuần tự vẫn có thể là last-write-wins.

#### Bài học Controller: UpdateMovie ngắn hơn UpdateShowtime
- `UpdateMovie()` đơn giản hơn: nhận ID/DTO, tạo Movie, gọi Service, null → 404, success → 200.
- `UpdateShowtime()` dài hơn vì cần xử lý exception nghiệp vụ (400/409), và map `ShowtimeResponse` gồm MovieTitle, RoomName, CinemaName cùng thông tin giờ/giá.
- Độ dài code khác nhau phản ánh độ phức tạp nghiệp vụ. Không cần cố làm Controller ngắn bằng cách giấu logic; có thể refactor mapping sang helper riêng khi cần, nhưng hiện ưu tiên hiểu rõ flow.


## 4. Những lỗi thực tế đã sửa — tránh lặp lại

1. `DateOnly` từ MySQL `DATE`: Oracle provider gây lỗi đọc `DateTime`/`DateOnly`; đã dùng `.HasConversion(...)`.
2. MySQL Docker và MySQL Windows khác host-port/database: đã từng gặp `Unknown database 'cinema_booking_practice'`; luôn kiểm tra instance thật và Connection String.
3. EF Core `FindAsync()` phải `await` trước khi truy cập thuộc tính, vì trả `ValueTask<Movie?>`.
4. Phương thức UPDATE cần `Task<Movie?>` cho trường hợp không tồn tại; cần trả **existingMovie**, không phải input movie.
5. Khi tạo thì `Add()` chỉ thay đổi trạng thái tracking, `SaveChangesAsync()` mới INSERT.
6. `AsNoTracking()` dùng cho truy vấn chỉ đọc, không dùng cho flow UPDATE theo dõi thay đổi thông thường.
7. `var updatedMovie = await ...` vẫn được C# suy luận kiểu `Movie?`, không phải `dynamic`.
8. `DELETE` có thể trả 200 nhưng đã cải thiện thành **204 No Content**; không trả `bool` trong body.
9. `DbUpdateException` không đồng nghĩa 409; chỉ xử lý khi có lỗi khóa ngoại cụ thể, các lỗi khác cần xử lý khác.
10. Không tự bổ sung endpoint `/movies/statistics`: trước đó từng được đề xuất như bài tập nhưng người học muốn **ưu tiên chức năng thật của dự án gốc**.

## 5. Các kết quả học và kiểm thử đã xác nhận

### Buổi 1
- Từ Movie Java sang C# Model; cấu trúc Controller/Service/Interface, DI trong .NET.
- Kết thúc 7/7 bài, đánh giá khoảng **9.5/10**.

### Buổi 2
- **2.1** EF Core vs JPA/Hibernate.
- **2.2** DbContext, DbSet, Entity Mapping.
- **2.3** Kết nối MySQL.
- **2.4** Migration và database có sẵn.
- **2.5** LINQ, truy vấn phim và refactor Controller/Service sang async.
- **2.6** Async/Await; lý thuyết đạt **9/10**; thực hành đã có trong chính Cinema Booking API. **Không yêu cầu Console App `Task.Delay` nữa**.
- **2.7** Regression tests: Build OK; GET list, GET 404, GET now showing, search có khoảng trắng, filter kết hợp đều đạt. Tổng kết lý thuyết **8.5/10**, thực hành **10/10**. Xác nhận buổi 2 100%.

### Buổi 3
- **3.1 Create:** `IMovieService`, `MovieService`, Controller được review 10/10; POST 201 và tạo phim trong MySQL.
- **3.2 Update:** service/interface sau sửa đạt 10/10; Controller dùng `var updatedMovie`, PUT thành công Postman.
- **3.3 Delete:** interface/service/controller đạt, đổi sang HTTP 204, kiểm thử thành công.
- **3.4 DTO & Validation:** 3 test POST và 2 test PUT đều đạt.
- **3.5 HTTP Error:** `ResourceConflictException`, xử lý FK error 1451, Build OK, 404/204/GET lại 404 đạt. 409 chưa test tích hợp.
- **3.6 End-to-End:** Build Succeeded; **POST 201, GET 200, PUT 200, POST invalid 400, DELETE 204, GET after delete 404**, tất cả đạt; **10/10**. Xác nhận buổi 3 100%.

### Buổi 4 — Showtime Management (đang học)
- GET Showtime đã hoàn thành.
- POST Showtime đã code, Build/test thành công các trường hợp validation và tạo lịch hợp lệ.
- PUT Showtime: DTO/Interface/Service/Controller đã viết và được review; đang ở bước Build + test Postman.
- DELETE Showtime, kiểm thử tích hợp đầy đủ, tổng kết và commit: chưa làm.
- Bài học mới: Transaction, `ReadCommitted`, `SELECT ... FOR UPDATE`, kiểm tra overlap, tính EndTime dựa trên Movie.Duration, xử lý lịch legacy có EndTime null.
- **Điểm dừng chính xác khi chuyển chat: test PUT Showtime trên Postman.**


## 6. VIỆC PHẢI LÀM TIẾP NGAY — TEST PUT SHOWTIME

Trợ lý mới **không được bắt người học quay lại khảo sát DB hoặc học lại Buổi 1–3**. Tiếp tục từ việc kiểm thử PUT đang dở.

### Bước tiếp theo
1. Build Solution trong Visual Studio; nếu lỗi, gửi Error List và review trước.
2. Chạy `CinemaBooking.Api`.
3. Trong Postman, gửi PUT tới `{{baseUrl}}/showtimes/{id}` với ID Showtime thực có.
4. Lấy MovieId, RoomId, StartTime hiện tại từ DB/API/SQL đáng tin; test lần đầu chỉ đổi Price, giữ nguyên các trường lịch để tránh conflict. Request vẫn gửi đủ DTO fields.
5. Kiểm tra response 200 và dữ liệu cập nhật; kiểm tra lại DB.
6. Sau test thành công, test ID không tồn tại → 404, lịch trùng → 409, request validation/ID Movie hoặc Room sai → 400 (nếu logic Controller/Service hiện tại trả như vậy).
7. Khi PUT đã đạt, chuyển DELETE Showtime; sau đó integration test và tổng kết Buổi 4.
8. Chỉ commit khi chức năng hoàn thành và tests đã xác nhận.

### Request mẫu (phải thay bằng dữ liệu thật)

```http
PUT {{baseUrl}}/showtimes/{showtimeId}
Content-Type: application/json
```

```json
{
  "movieId": 1,
  "roomId": 2,
  "startTime": "2026-12-01T18:00:00",
  "price": 90000
}
```

Các giá trị trên chỉ là ví dụ; không sử dụng trực tiếp nếu không khớp DB.

### Tiến độ Buổi 4
- GET + POST: hoàn thành.
- PUT code: hoàn thành, kiểm thử đang chờ.
- DELETE: chưa bắt đầu.
- Integration tests + tổng kết + commit: chưa bắt đầu.


## 7. Các file nên đính kèm khi chuyển tài khoản

File `.md` này giúp nhớ tiến độ, nhưng **không thay thế source code hiện tại**. Nếu có thể hãy tải/đính kèm thêm mã nguồn mới nhất hoặc link repository .NET (nếu đã đẩy GitHub):

- `Movie.cs`, `MovieStatus.cs`
- `CinemaDbContext.cs`, `Program.cs`
- `IMovieService.cs`, `MovieService.cs`, `MovieController.cs`
- `DTOs/Movies/CreateMovieRequest.cs`
- `Exceptions/ResourceConflictException.cs`
- `CinemaBooking.Api.csproj`
- `compose.yaml` **đã xóa secrets** nếu cần kiểm tra Docker
- Java Entity/Repository/Controller/Service và schema SQL của chức năng suất chiếu từ Spring Boot.

**Tuyệt đối không đính kèm mật khẩu, connection string có password, API keys, User Secrets hoặc file `.env` chứa bí mật.**

## 8. Prompt ngắn để dán vào ChatGPT tài khoản mới

> Tôi đang học C#/.NET để thực tập bằng cách chuyển dự án Spring Boot Cinema Booking sang ASP.NET Core + EF Core + MySQL. Hãy đọc kỹ file `CINEMA_BOOKING_HANDOFF_UPDATED.md` đính kèm và dùng làm tiến độ học chính thức. Buổi 1–3 đã hoàn thành 100%, không học lại. Buổi 4 đang làm Showtime Management: GET và POST đã hoàn thành; PUT DTO/Interface/Service/Controller đã code và được review, **đang cần Build và test Postman**. Tiếp tục đúng bước này, rồi mới làm DELETE, integration tests, tổng kết và commit. Hướng dẫn bằng tiếng Việt, ngắn gọn, khoảng 80% thực hành; giao từng nhiệm vụ, để tôi tự làm, review file/code hoặc ảnh Postman và chấm thang 10. Bám sát repository gốc https://github.com/HoangDung05/cinema-booking; không tự sáng tác chức năng, không chạy migration hoặc sửa schema DB tùy tiện, không bắt tôi học lại những phần đã hoàn thành. Khi cần thông tin chưa có, hỏi file/schema/kết quả SQL thay vì đoán.

**Ghi chú về độ chính xác:** Tài liệu này tổng hợp từ nội dung cuộc trò chuyện và những file code đã được gửi/đọc. Không phải bản xuất nguyên văn toàn bộ lịch sử. Những điểm như cổng MySQL đang dùng, mã nguồn mới nhất, trạng thái Build/test PUT, và dữ liệu DB thực tế cần xác nhận lại khi tiếp tục.
