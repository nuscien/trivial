// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValue.cs" company="Nanchang Jinchen Software Co., Ltd.">
//   Copyright (c) 2010 Nanchang Jinchen Software Co., Ltd. All rights reserved.
// </copyright>
// <summary>
//   The data value for definition for a list row or a table column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Security;
using Trivial.Tasks;
using Trivial.Text;

namespace Trivial.Data;

/// <summary>
/// The collection result.
/// </summary>
public interface ICollectionResult<T>
{
    /// <summary>
    /// Gets the message.
    /// It could be an error message, a status description or a notice text. This can be null if no such information.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the result collection.
    /// </summary>
    public IEnumerable<T> Data { get; }

    /// <summary>
    /// Gets the offset of the result; or null, if no such information.
    /// </summary>
    public int? Offset { get; }

    /// <summary>
    /// Gets the total count; or null, if no such information.
    /// </summary>
    public int? TotalCount { get; }
}

/// <summary>
/// The collection data result.
/// </summary>
/// <typeparam name="T">The type of item.</typeparam>
[DataContract]
[Guid("382B9940-024E-4493-B166-B49B6045AA38")]
public class CollectionResult<T> : MessageResult, ICollectionResult<T>
{
    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    public CollectionResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    /// <param name="message">The message.</param>
    public CollectionResult(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    /// <param name="col">The result collection.</param>
    /// <param name="offset">The optional offset of the result.</param>
    /// <param name="count">The optional total count.</param>
    public CollectionResult(IEnumerable<T> col, int? offset = null, int? count = null)
    {
        SetData(col);
        Offset = offset;
        TotalCount = count;
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    /// <param name="col">The result collection.</param>
    /// <param name="all">true if the items in the collection are all about the result, so that means the total count is the count of the collection plus the offset; otherwise, false.</param>
    /// <param name="offset">The optional offset of the result.</param>
    public CollectionResult(IEnumerable<T> col, bool all, int offset = 0)
    {
        SetData(col);
        Offset = offset;
        if (all) TotalCount = offset + Data.Count;
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    /// <param name="col">The result collection.</param>
    /// <param name="message">The message.</param>
    public CollectionResult(IEnumerable<T> col, string message)
        : base(message)
    {
        SetData(col);
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    /// <param name="col">The result collection.</param>
    /// <param name="offset">The optional offset of the result.</param>
    /// <param name="message">The message.</param>
    /// <param name="count">The optional total count.</param>
    public CollectionResult(IEnumerable<T> col, int offset, string message, int? count = null)
        : this(col, message)
    {
        Offset = offset;
        TotalCount = count;
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    /// <param name="col">The result collection.</param>
    /// <param name="all">true if the items in the collection are all about the result, so that means the total count is the count of the collection plus the offset; otherwise, false.</param>
    /// <param name="offset">The optional offset of the result.</param>
    /// <param name="message">The message.</param>
    public CollectionResult(IEnumerable<T> col, bool all, int offset, string message)
        : this(col, message)
    {
        Offset = offset;
        if (all) TotalCount = offset + Data.Count;
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResult class.
    /// </summary>
    /// <param name="trackingId">The tracking identifier.</param>
    /// <param name="col">The result collection.</param>
    /// <param name="offset">The optional offset of the result.</param>
    /// <param name="count">The optional total count.</param>
    /// <param name="message">The message.</param>
    public CollectionResult(string trackingId, IEnumerable<T> col, int? offset, int? count, string message)
        : this(col, message)
    {
        TrackingId = trackingId;
        Offset = offset;
        TotalCount = count;
    }

    /// <summary>
    /// Gets or sets the offset of the result; or null, if no such information.
    /// </summary>
    [DataMember(Name = "offset", EmitDefaultValue = false)]
    [JsonPropertyName("offset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The record offset of the result collection.")]
    public int? Offset
    {
        get => GetCurrentProperty<int?>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets or sets the total count; or null, if no such information.
    /// </summary>
    [DataMember(Name = "count", EmitDefaultValue = false)]
    [JsonPropertyName("count")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The count of the result collection. It can be null if no such information.")]
    public int? TotalCount
    {
        get => GetCurrentProperty<int?>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the count of the data of current result.
    /// </summary>
    [JsonIgnore]
    public virtual int CurrentCount => Data?.Count ?? 0;

    /// <summary>
    /// Gets or sets the result collection.
    /// </summary>
    [DataMember(Name = "col")]
    [JsonPropertyName("col")]
    [Description("The data collection of the result.")]
    public ObservableCollection<T> Data
    {
        get => GetCurrentProperty<ObservableCollection<T>>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets the result collection.
    /// </summary>
    [JsonIgnore]
    IEnumerable<T> ICollectionResult<T>.Data => Data;

    /// <summary>
    /// Gets or sets the additional info.
    /// </summary>
    [DataMember(Name = "info")]
    [JsonPropertyName("info")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Description("The additional information of the result.")]
    public JsonObjectNode AdditionalInfo
    {
        get => GetCurrentProperty<JsonObjectNode>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Adds an item.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(T item)
    {
        var data = InitData();
        data.Add(item);
    }

    /// <summary>
    /// Adds a collection of item in data.
    /// </summary>
    /// <param name="collection">The collection to add.</param>
    public void AddRange(IEnumerable<T> collection)
    {
        var data = InitData();
        foreach (var item in collection)
        {
            data.Add(item);
        }
    }

    /// <summary>
    /// Tries to get the specific one.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="startFromOffset">true if the index is based on offset; otherwise, false.</param>
    /// <returns>The item.</returns>
    public T TryGetValue(int index, bool startFromOffset = false)
        => TryGetValue(index, startFromOffset, out var result) ? result : default;

    /// <summary>
    /// Tries to get the specific one.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="startFromOffset">true if the index is based on offset; otherwise, false.</param>
    /// <param name="result">The item to get.</param>
    /// <returns>true if has; otherwise, false.</returns>
    public bool TryGetValue(int index, bool startFromOffset, out T result)
    {
        if (startFromOffset)
        {
            try
            {
                if (Offset.HasValue) index -= Offset.Value;
            }
            catch (NullReferenceException)
            {
            }
        }

        var col = Data;
        if (index < 0 || col == null)
        {
            result = default;
            return false;
        }

        try
        {
            if (index < col.Count)
            {
                result = col[index];
                return true;
            }
        }
        catch (ArgumentException)
        {
        }
        catch (NullReferenceException)
        {
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Tries to get the specific one.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="result">The item to get.</param>
    /// <returns>true if has; otherwise, false.</returns>
    public bool TryGetValue(int index, out T result)
        => TryGetValue(index, false, out result);

    /// <summary>
    /// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>default(T) if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
    public T FirstOrDefault(Func<T, bool> predicate)
    {
        var col = Data;
        if (col == null) return default;
        return predicate != null ? col.FirstOrDefault(predicate) : col.FirstOrDefault();
    }

    /// <summary>
    /// Returns the last element of the sequence that satisfies a condition or a default value if no such element is found.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>default(T) if source is empty or if no element passes the test specified by predicate; otherwise, the last element in source that passes the test specified by predicate.</returns>
    public T LastOrDefault(Func<T, bool> predicate)
    {
        var col = Data;
        if (col == null) return default;
        return predicate != null ? col.LastOrDefault(predicate) : col.LastOrDefault();
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An collection that contains elements from the input sequence that satisfy the condition.</returns>
    public IEnumerable<T> Where(Func<T, bool> predicate)
    {
        var col = Data;
        if (col == null) return new List<T>();
        return col.Where(predicate);
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An collection that contains elements from the input sequence that satisfy the condition.</returns>
    public IEnumerable<T> Where(Func<T, int, bool> predicate)
    {
        var col = Data;
        if (col == null) return new List<T>();
        return col.Where(predicate);
    }

    /// <summary>
    /// Initializes the data to ensure it is not null.
    /// </summary>
    /// <returns>The data collection.</returns>
    public ObservableCollection<T> InitData()
    {
        var data = Data;
        if (data is not null) return data;
        data = new();
        if (Data == null)
        {
            SetData(data);
        }
        else
        {
            var data2 = Data;
            if (data2 == null)
            {
                data = new();
                SetData(data);
            }
            else
            {
                data = data2;
            }
        }

        return data;
    }

    private void SetData(IEnumerable<T> data)
    {
        if (data is not ObservableCollection<T> list) list = data is null ? new() : new(data);
        SetData(list);
    }

    private void SetData(ObservableCollection<T> data)
        => SetProperty(nameof(Data), data);
}

/// <summary>
/// The JSON collection data result.
/// </summary>
[DataContract]
[Guid("B1B2F7C1-F8D1-4A69-9E5A-543C2FCB75A5")]
public class JsonCollectionResult : CollectionResult<JsonObjectNode>
{
    /// <summary>
    /// Initializes a new instance of the JsonCollectionResult class.
    /// </summary>
    public JsonCollectionResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonCollectionResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    public JsonCollectionResult(IEnumerable<JsonObjectNode> data)
        : base(data)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonCollectionResult class.
    /// </summary>
    /// <param name="col">The result collection.</param>
    /// <param name="offset">The optional offset of the result.</param>
    /// <param name="count">The optional total count.</param>
    public JsonCollectionResult(IEnumerable<JsonObjectNode> col, int? offset = null, int? count = null)
        : base(col, offset, count)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonCollectionResult class.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    public JsonCollectionResult(IEnumerable<JsonObjectNode> data, string message)
        : base(data, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonCollectionResult class.
    /// </summary>
    /// <param name="col">The result collection.</param>
    /// <param name="offset">The optional offset of the result.</param>
    /// <param name="message">The message.</param>
    /// <param name="count">The optional total count.</param>
    public JsonCollectionResult(IEnumerable<JsonObjectNode> col, int offset, string message, int? count = null)
        : base(col, offset, message, count)
    {
    }

    /// <summary>
    /// Gets or sets the components.
    /// </summary>
    [DataMember(Name = "components")]
    [JsonPropertyName("components")]
    [Description("The components for reference.")]
    public JsonObjectNode Components
    {
        get => GetCurrentProperty<JsonObjectNode>();
        set => SetCurrentProperty(value);
    }

    /// <summary>
    /// Gets a value indicating whether the data is null.
    /// </summary>
    [JsonIgnore]
    public bool IsNull => Data is null;

    /// <summary>
    /// Tries to get the value of the specific index and sub-property.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="subKey">The sub-key of the previous property.</param>
    /// <param name="keyPath">The additional property key path.</param>
    /// <returns>The value.</returns>
    public BaseJsonValueNode TryGetValue(int index, string subKey, params string[] keyPath)
    {
        var v = TryGetValue(index);
        if (v is null) return null;
        var path = new List<string>
        {
            subKey
        };
        path.AddRange(keyPath);
        return v.TryGetValue(path);
    }

    /// <summary>
    /// Gets schema.
    /// </summary>
    /// <param name="key">The schema key.</param>
    /// <returns>The schema information.</returns>
    public JsonObjectNode GetSchema(string key)
        => string.IsNullOrWhiteSpace(key) ? null : GetComponent("schemas")?.TryGetObjectValue(key);

    /// <summary>
    /// Gets reference object.
    /// </summary>
    /// <param name="type">The type or group key of the resource.</param>
    /// <param name="id">The identifier of the object.</param>
    /// <returns>The reference object.</returns>
    public JsonObjectNode GetReferenceObject(string type, string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        var components = GetComponents();
        if (components == null) return null;
        var valueKind = components.GetValueKind(type);
        var json = valueKind switch
        {
            JsonValueKind.Object => components.TryGetObjectValue(type)?.TryGetObjectValue(id),
            JsonValueKind.Array => components.TryGetArrayValue(type)?.TryGetObjectValueById(id),
            _ => null,
        };
        if (json == null) return null;
        var refPath = json.TryGetStringValue("$ref");
        if (refPath == null) return json;
        return JsonObjectNode.TryGetRefObjectValue(null, json, ToJson());
    }

    internal JsonObjectNode GetComponents()
        => Components;

    internal JsonObjectNode GetComponent(string key)
    {
        var dict = GetComponents();
        return string.IsNullOrWhiteSpace(key) ? dict : dict?.TryGetObjectValue(key);
    }

    internal JsonObjectNode ToJson()
        => new()
        {
            { "track", TrackingId },
            { "message", Message },
            { "data", Data },
            { "info", AdditionalInfo },
            { "components", Components }
        };
}

/// <summary>
/// The Server-Sent Events collection data result.
/// </summary>
/// <typeparam name="T">The type of item.</typeparam>
public class StreamingCollectionResult<T> : CollectionResult<T>
{
    private Task task;
    private Action<StreamingCollectionResult<T>, ServerSentEventInfo> callback;

    /// <summary>
    /// Initializes a new instance of the CollectionResultSource class.
    /// </summary>
    /// <param name="collection">The input collection.</param>
    /// <param name="callback">The callback handler on Server-Sent Event info is received and processed.</param>
    /// <param name="options">The JSON serializer options.</param>
    /// <param name="message">The message.</param>
    public StreamingCollectionResult(IAsyncEnumerable<ServerSentEventInfo> collection, Action<StreamingCollectionResult<T>, ServerSentEventInfo> callback, JsonSerializerOptions options = null, string message = null)
        : base(message)
    {
        this.callback = callback;
        SerializerOptions = options;
        if (collection is null)
        {
            State = TaskStates.Faulted;
            return;
        }

        State = TaskStates.Working;
        task = LoadDataAsync(collection, this);
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResultSource class.
    /// </summary>
    /// <param name="collection">The input collection.</param>
    /// <param name="callback">The callback handler on Server-Sent Event info is received and processed.</param>
    /// <param name="options">The JSON serializer options.</param>
    public StreamingCollectionResult(IAsyncEnumerable<ServerSentEventInfo> collection, Action<StreamingCollectionResult<T>> callback, JsonSerializerOptions options = null)
        : this(collection, callback == null ? null : (obj, e) => callback.Invoke(obj), options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResultSource class.
    /// </summary>
    /// <param name="collection">The input collection.</param>
    /// <param name="options">The JSON serializer options.</param>
    public StreamingCollectionResult(IAsyncEnumerable<ServerSentEventInfo> collection, JsonSerializerOptions options = null)
        : this(collection, null as Action<StreamingCollectionResult<T>, ServerSentEventInfo>, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResultSource class.
    /// </summary>
    /// <param name="sse">The Server-Sent Events stream.</param>
    /// <param name="options">The JSON serializer options.</param>
    public StreamingCollectionResult(Stream sse, JsonSerializerOptions options = null)
        : this(ServerSentEventInfo.ParseAsync(sse), null as Action<StreamingCollectionResult<T>, ServerSentEventInfo>, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResultSource class.
    /// </summary>
    /// <param name="sse">The Server-Sent Events stream.</param>
    /// <param name="callback">The callback handler on Server-Sent Event info is received and processed.</param>
    /// <param name="options">The JSON serializer options.</param>
    public StreamingCollectionResult(Stream sse, Action<StreamingCollectionResult<T>, ServerSentEventInfo> callback, JsonSerializerOptions options = null)
        : this(ServerSentEventInfo.ParseAsync(sse), callback, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the CollectionResultSource class.
    /// </summary>
    /// <param name="sse">The Server-Sent Events stream.</param>
    /// <param name="callback">The callback handler on Server-Sent Event info is received and processed.</param>
    /// <param name="options">The JSON serializer options.</param>
    public StreamingCollectionResult(Stream sse, Action<StreamingCollectionResult<T>> callback, JsonSerializerOptions options = null)
        : this(ServerSentEventInfo.ParseAsync(sse), callback == null ? null : (obj, e) => callback.Invoke(obj), options)
    {
    }

    /// <summary>
    /// Adds or removes an event hanlder on state changed.
    /// </summary>
    public event DataEventHandler<TaskStates> StateChanged;

    /// <summary>
    /// Gets the current state about data setting.
    /// </summary>
    public TaskStates State
    {
        get
        {
            return GetProperty<TaskStates>(nameof(State));
        }

        internal set
        {
            if (SetProperty(nameof(State), value)) StateChanged?.Invoke(this, value);
        }
    }

    /// <summary>
    /// Gets the count of Server-Sent Event item received.
    /// </summary>
    public int EventItemCount { get; private set; }

    /// <summary>
    /// Gets the JSON serializer options.
    /// </summary>
    protected internal JsonSerializerOptions SerializerOptions { get; }

    /// <summary>
    /// Waits to end.
    /// </summary>
    /// <returns>A task that represents the asynchronous processing operation.</returns>
    public Task WaitAsync()
    {
        var t = task;
        return t ?? Task.CompletedTask;
    }

    /// <summary>
    /// Waits to end.
    /// </summary>
    /// <returns>A task that represents the asynchronous processing operation.</returns>
    public Task WaitAsync(CancellationToken cancellationToken)
    {
        var t = task;
        if (t is null) return Task.CompletedTask;
        return Task.Run(async () => await t, cancellationToken);
    }

    /// <summary>
    /// Occurs on the Server-Sent Event item is received and processed.
    /// </summary>
    /// <param name="info">The Server-Sent Event info.</param>
    protected virtual void OnReceived(ServerSentEventInfo info)
    {
    }

    /// <summary>
    /// Occurs on the Server-Sent Event item is received and processed.
    /// </summary>
    /// <param name="info">The Server-Sent Event info.</param>
    internal void OnReceivedInternal(ServerSentEventInfo info)
    {
        EventItemCount++;
        OnReceived(info);
        callback?.Invoke(this, info);
    }

    private async Task LoadDataAsync(IAsyncEnumerable<ServerSentEventInfo> collection, StreamingCollectionResult<T> source)
    {
        try
        {
            await foreach (var item in collection)
            {
                if (item == null) continue;
                Patch(item.EventName.ToLowerInvariant(), item.DataString, source);
                source.OnReceivedInternal(item);
            }

            if (source.State == TaskStates.Working) source.State = TaskStates.Done;
        }
        catch (Exception)
        {
            source.State = TaskStates.Faulted;
            throw;
        }
        finally
        {
            task = null;
            callback = null;
        }
    }

    private void Patch(string name, string s, StreamingCollectionResult<T> source)
    {
        if (string.IsNullOrEmpty(s)) return;
        switch (name)
        {
            case "message":
                {
                    var obj = JsonObjectNode.TryParse(s);
                    if (obj == null) break;
                    Patch(obj, source);
                }
                break;
            case "info":
                if (AdditionalInfo == null) AdditionalInfo = new();
                if (s == "null" || s == "clear")
                {
                    AdditionalInfo.Clear();
                }
                else
                {
                    var info = JsonObjectNode.TryParse(s);
                    if (info == null) break;
                    AdditionalInfo.SetRange(info);
                }

                break;
            case "add":
                AddItem(s, source, false);
                break;
            case "update":
                AddItem(s, source, true);
                break;
            case "set":
                {
                    var meta = JsonObjectNode.TryParse(s);
                    if (meta == null) break;
                    PatchMeta(meta, source);
                }

                break;
        }
    }

    private void Patch(JsonObjectNode json, StreamingCollectionResult<T> source)
    {
        var clear = json.TryGetStringListValue("clear");
        var data2 = InitData();
        if (clear.Contains("data") || clear.Contains("Data")) data2.Clear();
        if (AdditionalInfo == null) AdditionalInfo = new();
        if (clear.Contains("info") || clear.Contains("AdditionalInfo")) AdditionalInfo.Clear();
        var data = json.TryGetArrayValue("add");
        if (data is not null)
        {
            foreach (var item in data)
            {
                var add = JsonSerializer.Deserialize<T>(item.ToString(), source.SerializerOptions);
                data2.Add(add);
            }
        }

        var info = json.TryGetValue("info");
        if (info is null)
        {
        }
        else if (info is JsonObjectNode infoJson)
        {
            AdditionalInfo.SetRange(infoJson);
        }
        else if (info.ValueKind == JsonValueKind.Null)
        {
            AdditionalInfo.Clear();
        }

        PatchMeta(json, source);
    }

    private void PatchMeta(JsonObjectNode meta, StreamingCollectionResult<T> source)
    {
        if (meta.TryGetBooleanValue("error") == true) source.State = TaskStates.Faulted;
        if (meta.TryGetInt32Value("offset", out var i, out var kind)) Offset = i;
        else if (kind == JsonValueKind.Null) Offset = null;
        if (meta.TryGetInt32Value("count", out i, out kind)) TotalCount = i;
        else if (kind == JsonValueKind.Null) TotalCount = null;
        if (meta.TryGetStringValue("message", out var s, out kind)) Message = s;
        else if (kind == JsonValueKind.Null) Message = null;
        if (TrackingId == null && meta.TryGetStringTrimmedValue("track", out s) && !string.IsNullOrEmpty(s)) TrackingId = s;
    }

    private void AddItem(string s, StreamingCollectionResult<T> source, bool clear)
    {
        var data = InitData();
        if (clear) data.Clear();
        try
        {
            if (s == "null" || s == "default")
            {
                data.Add(default);
            }
            else if (s == "empty")
            {
            }
            else if (s.StartsWith('['))
            {
                var arr = JsonSerializer.Deserialize<List<T>>(s, source.SerializerOptions);
                foreach (var item2 in arr)
                {
                    data.Add(item2);
                }
            }
            else
            {
                var obj = JsonSerializer.Deserialize<T>(s);
                data.Add(obj);
            }
        }
        catch (JsonException)
        {
            source.State = TaskStates.Faulted;
        }
        catch (NotSupportedException)
        {
            source.State = TaskStates.Faulted;
            throw;
        }
    }
}

/// <summary>
/// The collection result builder on Server-Sent Events.
/// </summary>
/// <typeparam name="T">The type of item.</typeparam>
public class CollectionResultBuilder<T>
{
    /// <summary>
    /// Adds or removes the event handler of the creation of Server-Sent Event info.
    /// </summary>
    public event DataEventHandler<ServerSentEventInfo> Created;

    /// <summary>
    /// Adds or removes the event handler of the ending of Server-Sent Event streaming.
    /// </summary>
    public event DataEventHandler<bool> Ended;

    /// <summary>
    /// Gets or sets the JSON serializer options.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; }

    /// <summary>
    /// Gets the count of Server-Sent Event item is created.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the streaming is end.
    /// </summary>
    public bool IsEnd { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether need throw an invalid operation exception on creating after streaming is end.
    /// </summary>
    public bool ThrowExceptionOnEnd { get; set; }

    /// <summary>
    /// Gets the collection result.
    /// </summary>
    public CollectionResult<T> Result { get; } = new(new ObservableCollection<T>())
    {
        AdditionalInfo = new()
    };

    /// <summary>
    /// Sets tracking identifier.
    /// </summary>
    /// <param name="id">The tracking identifier.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetTrackingId(string id)
    {
        if (!string.IsNullOrEmpty(Result.TrackingId)) return null;
        var r = Create("set", new JsonObjectNode()
        {
            { "track", id }
        });
        if (r is null) return null;
        Result.TrackingId = id;
        return r;
    }

    /// <summary>
    /// Adds an item into data collection.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="clearBefore">true if clear current data collection before adding.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo Add(T item, bool clearBefore = false)
    {
        var s = JsonSerializer.Serialize(item, SerializerOptions);
        var r = Create(clearBefore ? "update" : "add", s);
        if (r is null) return null;
        var data = Result.InitData();
        if (clearBefore) data.Clear();
        data.Add(item);
        return r;
    }

    /// <summary>
    /// Adds a set of item into data collection.
    /// </summary>
    /// <param name="collection">The collection to add.</param>
    /// <param name="clearBefore">true if clear current data collection before adding.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo AddRange(IEnumerable<T> collection, bool clearBefore = false)
    {
        var s = collection is null ? "empty" : JsonSerializer.Serialize(collection, SerializerOptions);
        var r = Create(clearBefore ? "update" : "add", s);
        if (r is null) return null;
        var data = Result.InitData();
        if (clearBefore) data.Clear();
        if (collection is not null)
        {
            foreach (var item in collection)
            {
                data.Add(item);
            }
        }

        return r;
    }

    /// <summary>
    /// Clears all items in the data collection.
    /// </summary>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo ClearItems()
    {
        var r = Create("update", "empty");
        if (r is null) return null;
        Result.InitData().Clear();
        return r;
    }

    /// <summary>
    /// Sets the offset.
    /// </summary>
    /// <param name="offset">The offset value; or null, to clear current value.</param>
    /// <param name="totalCount">The total count value; or null, to clear current value.</param>
    /// <param name="message">The message to set to override the current.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetOffset(int? offset, int? totalCount, string message)
    {
        var obj = new JsonObjectNode
        {
            { "message", message },
        };
        obj.SetValue("offset", offset, false);
        obj.SetValue("count", totalCount, false);
        var r = Create("set", obj);
        if (r is null) return null;
        Result.Offset = offset;
        Result.TotalCount = totalCount;
        Result.Message = message;
        return r;
    }

    /// <summary>
    /// Sets the offset.
    /// </summary>
    /// <param name="offset">The offset value; or null, to clear current value.</param>
    /// <param name="totalCount">The total count value; or null, to clear current value.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetOffset(int? offset, int? totalCount)
    {
        var obj = new JsonObjectNode();
        obj.SetValue("offset", offset, false);
        obj.SetValue("count", totalCount, false);
        var r = Create("set", obj);
        if (r is null) return null;
        Result.Offset = offset;
        Result.TotalCount = totalCount;
        return r;
    }

    /// <summary>
    /// Sets the offset.
    /// </summary>
    /// <param name="offset">The offset value; or null, to clear current value.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetOffset(int? offset)
    {
        var obj = new JsonObjectNode();
        obj.SetValue("offset", offset, false);
        var r = Create("set", obj);
        if (r is null) return null;
        Result.Offset = offset;
        return r;
    }

    /// <summary>
    /// Sets the total count.
    /// </summary>
    /// <param name="totalCount">The total count value; or null, to clear current value.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetTotalCount(int? totalCount)
    {
        var obj = new JsonObjectNode();
        obj.SetValue("count", totalCount, false);
        var r = Create("set", obj);
        if (r is null) return null;
        Result.TotalCount = totalCount;
        return r;
    }

    /// <summary>
    /// Sets the message.
    /// </summary>
    /// <param name="message">The message to set to override the current.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetMessage(string message)
    {
        var r = Create("set", new JsonObjectNode
        {
            { "message", message }
        });
        if (r is null) return null;
        Result.Message = message;
        return r;
    }

    /// <summary>
    /// Sets the state to error.
    /// </summary>
    /// <param name="message">The error message to set to override the current.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetError(string message)
    {
        var r = Create("set", new JsonObjectNode
        {
            { "message", message },
            { "error", true }
        });
        if (r is null) return null;
        Result.Message = message;
        return r;
    }

    /// <summary>
    /// Sets the state to error.
    /// </summary>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo SetError()
    {
        var r = Create("set", new JsonObjectNode
        {
            { "error", true }
        });
        return r;
    }

    /// <summary>
    /// Patches the additional info.
    /// </summary>
    /// <param name="info">The info to patch.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo PatchAdditionalInfo(JsonObjectNode info)
    {
        var r = Create("info", info);
        if (r is null) return null;
        if (Result.AdditionalInfo is null) Result.AdditionalInfo = new();
        Result.AdditionalInfo.SetRange(info);
        return r;
    }

    /// <summary>
    /// Clears the additional info.
    /// </summary>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    public ServerSentEventInfo ClearAdditionalInfo()
    {
        var r = Create("info", "null");
        if (r is null) return null;
        if (Result.AdditionalInfo is null) Result.AdditionalInfo = new();
        Result.AdditionalInfo.Clear();
        return r;
    }

    /// <summary>
    /// Ends the streaming.
    /// </summary>
    public void End()
    {
        if (IsEnd) return;
        IsEnd = true;
        Count++;
        OnEnd();
        Ended?.Invoke(this, true);
    }

    /// <summary>
    /// Occurs on the streaming is end.
    /// </summary>
    protected virtual void OnEnd()
    {
    }

    /// <summary>
    /// Occurs on the Server-Sent Event info instance is created.
    /// </summary>
    /// <param name="value">The instance.</param>
    protected virtual void OnCreate(ServerSentEventInfo value)
    {
    }

    /// <summary>
    /// Creates a Server-Sent Event info instance.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <param name="value">The data.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    private ServerSentEventInfo Create(string name, string value)
    {
        if (CheckEnd()) return null;
        var item = new ServerSentEventInfo(null, name, value);
        Count++;
        OnCreate(item);
        Created?.Invoke(this, item);
        return item;
    }

    /// <summary>
    /// Creates a Server-Sent Event info instance.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <param name="value">The data.</param>
    /// <returns>The Server-Sent Event info instance for this operation; or null, if streaming is end.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    private ServerSentEventInfo Create(string name, JsonObjectNode value)
    {
        if (CheckEnd()) return null;
        var item = new ServerSentEventInfo(null, name, value);
        Count++;
        OnCreate(item);
        Created?.Invoke(this, item);
        return item;
    }

    /// <summary>
    /// Checks if the streaming is end.
    /// </summary>
    /// <returns>true if it is end; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">The streaming is end and the options to throw exception for this case is on.</exception>
    private bool CheckEnd()
    {
        if (!IsEnd) return false;
        if (ThrowExceptionOnEnd) throw new InvalidOperationException("The Server-Sent Event streaming is end.");
        return true;
    }
}

/// <summary>
/// The collection result stream writer on Server-Sent Events.
/// </summary>
/// <typeparam name="T">The type of item.</typeparam>
/// <param name="writer">The stream writer to write the Server-Sent Events.</param>
public sealed class CollectionResultWriter<T>(StreamWriter writer) : CollectionResultBuilder<T>, IDisposable
{
    StreamWriter writer = writer;
    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the CollectionResultWriter class.
    /// </summary>
    /// <param name="stream">The stream to write.</param>
    public CollectionResultWriter(Stream stream)
        : this(stream is null || !stream.CanWrite ? null : new StreamWriter(stream, Encoding.UTF8))
    {
    }

    /// <summary>
    /// Gets a value indicating whether it binds a stream to write.
    /// </summary>
    public bool HasStream => writer is not null;

    /// <inheritdoc />
    protected override void OnCreate(ServerSentEventInfo value)
    {
        if (writer == null) return;
        writer.Write(value.ToResponseString(true));
        writer.Write('\n');
        writer.Flush();
    }

    /// <summary>
    /// Disposes this generator, stream writer and stream instances.
    /// </summary>
    public void Dispose()
    {
        if (disposedValue) return;
        disposedValue = true;
        try
        {
            End();
        }
        catch (Exception)
        {
        }

        try
        {
            var w = writer;
            writer = null;
            w?.Dispose();
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }

        GC.SuppressFinalize(this);
    }
}
