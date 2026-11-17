# API results

In `Trivial.Data` [namespace](../).

<!-- Start -->

## Base result

`MessageResult` can be used on its own for a response containing status information without a data payload. It also provides the common properties inherited by `DataResult<T>` and `CollectionResult<T>`.

```csharp
using System.Text.Json;
using Trivial.Data;

var result = new MessageResult("Operation completed") { TrackingId = "trace-1" };
string json = JsonSerializer.Serialize(result);
// JSON: {"message":"Operation completed","track":"trace-1"}
var response = JsonSerializer.Deserialize<MessageResult>(json);
```

`Message`, `ErrorCode`, and `TrackingId` map to `message`, `error`, and `track` in JSON. Null values are omitted. `Message` may describe a status, notice, or error; `IsError` instead checks whether `ErrorCode` is nonblank and is not serialized. `CreateNewTrackingId()` generates a GUID without hyphens; an overload accepts a GUID. `Tag` is local state and is not serialized. `AdditionalProperties` holds unrecognized JSON fields, which are written back at the top level.

## Single-object result

```csharp
using System;
using System.Text.Json;
using Trivial.Data;

// On the server, Model is the data type shared by the API and its client.
var result = new DataResult<Model>(model, "OK") { TrackingId = "trace-1" };
string json = JsonSerializer.Serialize(result);
// The JSON contains data, message, and track.

// On the client, json is the response body.
var response = JsonSerializer.Deserialize<DataResult<Model>>(json);
if (response.IsError)
    Console.WriteLine(response.ErrorCode);
else
    Console.WriteLine(response.Data);
```

`Data` maps to `data` and can be null; `IsNull` reports whether it is null and is not serialized. With default serializer options, null `data` is still emitted. `SerializeData()` serializes only `Data`, without the envelope, and accepts `JsonSerializerOptions`. `GetDataType()` returns the runtime data type, or `typeof(T)` when the data is null.

## Collection result

A collection result can represent a complete array or one page. Pagination metadata is optional.

```csharp
using System.Text.Json;
using Trivial.Data;

// On the server: all sets count to offset plus the number of items.
var result = new CollectionResult<string>(new[] { "alpha", "beta" }, all: true);
string json = JsonSerializer.Serialize(result);
// Includes items: ["alpha", "beta"], offset: 0, and count: 2.

// A page starting at index 2 out of 5 total items.
var page = new CollectionResult<string>(new[] { "gamma", "delta" }, offset: 2, count: 5);
```

`Data` maps to `items`, `Offset` to the zero-based `offset`, and `TotalCount` to `count`, the total across all pages. `CurrentCount` counts the items currently held and is not serialized. Null `offset` and `count` are omitted; leave the total null when unknown. `AdditionalInfo` maps to `info`.

A parameterless instance may have null `Data`; `AddItem`, `AddRange`, and `InitData` initialize it when needed. Collection constructors copy a general `IEnumerable<T>` into an `ObservableCollection<T>`, but reuse an `ObservableCollection<T>` passed in directly.

### Reading and merging pages

```csharp
var first = JsonSerializer.Deserialize<CollectionResult<string>>(
    "{\"items\":[\"alpha\",\"beta\"],\"offset\":0,\"count\":4}");
var next = JsonSerializer.Deserialize<CollectionResult<string>>(
    "{\"items\":[\"gamma\",\"delta\"],\"offset\":2,\"count\":4}");
first.AddRange(next);
// Data contains alpha, beta, gamma, delta; Offset is 0 and TotalCount is 4.
```

`AddRange(CollectionResult<T>)` modifies the original `Data`: contiguous pages are appended, overlaps are replaced, and earlier pages are prepended with `Offset` updated. Gaps are filled with `default(T)` (`0` for `int`, `null` for reference types), and can later be replaced by merging the missing page. A placeholder cannot be distinguished from an actual default value. For example, `[1, 2]` at offset 0 merged with `[5, 6]` at offset 4 yields `[1, 2, 0, 0, 5, 6]`.

If the next page has no offset, it is appended at `Offset + Data.Count`, treating a null current offset as zero. Merging does not validate totals or page ranges, or copy `TotalCount`, `Message`, `TrackingId`, or `AdditionalInfo` from the next page. A null page or a page with null `Data` adds no items but initializes the destination `Data`.

`TryGetValue(index, startFromOffset: true, out value)` uses an index in the complete result; without that flag the index is relative to `Data`. A placeholder is present and returns true with `default(T)`; an index outside the result returns false and `default(T)`.
