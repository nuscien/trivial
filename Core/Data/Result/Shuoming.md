# 接口结果

在 `Trivial.Data` [命名空间](../)中。

<!-- Start -->

## 基础结果

`MessageResult` 可单独用于不含数据主体的状态响应，也为 `DataResult<T>` 和 `CollectionResult<T>` 提供共同属性。

```csharp
using System.Text.Json;
using Trivial.Data;

var result = new MessageResult("Operation completed") { TrackingId = "trace-1" };
string json = JsonSerializer.Serialize(result);
// JSON: {"message":"Operation completed","track":"trace-1"}
var response = JsonSerializer.Deserialize<MessageResult>(json);
```

`Message` 对应 JSON 的 `message`，可包含状态、提示或错误描述；`ErrorCode` 对应 `error`；`TrackingId` 对应 `track`。这些字段为 null 时省略。`IsError` 根据 `ErrorCode` 是否为非空白字符串判断，且不序列化；不要仅凭 `Message` 判断失败。`CreateNewTrackingId()` 生成不含连字符的 GUID，也可传入 GUID 指定值。`Tag` 是不参与 JSON 序列化的本地信息；`AdditionalProperties` 保存未识别的 JSON 字段，序列化时这些字段仍位于顶层。

## 单对象结果

```csharp
using System;
using System.Text.Json;
using Trivial.Data;

// 服务端：Model 是接口双方约定的数据类型。
var result = new DataResult<Model>(model, "OK") { TrackingId = "trace-1" };
string json = JsonSerializer.Serialize(result);
// JSON 包含 data、message 和 track。

// 客户端：json 是接口返回的 JSON 文本。
var response = JsonSerializer.Deserialize<DataResult<Model>>(json);
if (response.IsError)
    Console.WriteLine(response.ErrorCode);
else
    Console.WriteLine(response.Data);
```

`Data` 对应 `data`，可以为 null；`IsNull` 表示 `Data` 是否为 null，不会序列化。默认序列化设置下，null 的 `data` 仍会输出。`SerializeData()` 只序列化 `Data`，不包括 `message` 等外层字段；可传入 `JsonSerializerOptions`。`GetDataType()` 返回数据的运行时类型；数据为 null 时返回 `typeof(T)`。

## 集合结果

`CollectionResult<T>` 既可表示完整数组，也可表示某一页；分页元数据均为可选值。

### 服务端：构造结果

```csharp
using System.Text.Json;
using Trivial.Data;

// 返回完整数组；all: true 将 count 设为 offset 加当前项目数。
var result = new CollectionResult<string>(new[] { "alpha", "beta" }, all: true);
string json = JsonSerializer.Serialize(result);
// 包含 items: ["alpha", "beta"]、offset: 0 和 count: 2。

// 返回从位置 2 开始的一页，整个结果共有 5 项。
var page = new CollectionResult<string>(new[] { "gamma", "delta" }, offset: 2, count: 5);
```

`Data` 对应 JSON 的 `items`；`Offset` 对应 `offset`，是当前数据在完整结果中的零基起始位置；`TotalCount` 对应 `count`，是完整结果的总数，不是本页数量。`CurrentCount` 是当前 `Data` 的数量，不会序列化。`offset` 和 `count` 为 null 时不会输出；未知总数应保持 null，不应假设等于当前项目数。还可使用 `Message`、`TrackingId` 和 `AdditionalInfo` 分别传递 `message`、`track` 和 `info`。

无参构造后的 `Data` 可以为 null；`AddItem`、`AddRange` 和 `InitData` 会在需要时初始化它。集合构造函数会把一般的 `IEnumerable<T>` 复制到 `ObservableCollection<T>`，但如果传入的已经是 `ObservableCollection<T>`，则使用同一个集合。

### 客户端：读取并合并分页

```csharp
using System.Text.Json;
using Trivial.Data;

var first = JsonSerializer.Deserialize<CollectionResult<string>>(
    "{\"items\":[\"alpha\",\"beta\"],\"offset\":0,\"count\":4}");
var next = JsonSerializer.Deserialize<CollectionResult<string>>(
    "{\"items\":[\"gamma\",\"delta\"],\"offset\":2,\"count\":4}");
first.AddRange(next);
// first.Data: alpha, beta, gamma, delta；first.Offset: 0；first.TotalCount: 4。
```

`AddRange(CollectionResult<T>)` 在原结果中修改 `Data`。连续页被追加；重叠位置由传入页的值替换；较早的页可以前插并更新 `Offset`。跳过的中间位置使用 `default(T)` 填充（例如 `int` 为 `0`，引用类型为 `null`）；之后合并缺失页时，这些位置会被新值替换。占位值与真实数据中的默认值无法区分。

例如，起始位置为 0、数据为 `[1, 2]` 的结果合并起始位置为 4、数据为 `[5, 6]` 的页后，`Data` 为 `[1, 2, 0, 0, 5, 6]`。缺少下一页的 `offset` 时，会按当前结果末尾 `Offset + Data.Count` 追加；当前 `Offset` 为 null 时按 0 计算。合并操作不检查总数或页的有效性。

合并不会从传入页更新 `TotalCount`、`Message`、`TrackingId` 或 `AdditionalInfo`，需要时由调用方自行处理。传入 null 或 `Data` 为 null 的页不会添加项目，但会初始化目标 `Data`。

`TryGetValue(index, startFromOffset: true, out value)` 使用完整结果的索引；不指定该参数时使用 `Data` 内的零基索引。对于填充的占位位置返回 true 和 `default(T)`；对于结果范围之外的索引，返回 false 且输出 `default(T)`。
