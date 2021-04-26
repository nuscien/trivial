using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Tasks
{
    /// <summary>
    /// Equipartition fragment task and its thread-safe resolver.
    /// </summary>
    public partial class EquipartitionTask
    {
        /// <summary>
        /// Task fragment states.
        /// </summary>
        public enum FragmentStates
        {
            /// <summary>
            /// Pending to assign.
            /// </summary>
            Pending = 0,

            /// <summary>
            /// Working on.
            /// </summary>
            Working = 1,

            /// <summary>
            /// Run succeeded (either of all or partial).
            /// </summary>
            Success = 2,

            /// <summary>
            /// Run failed.
            /// </summary>
            Failure = 3,

            /// <summary>
            /// Retrying to process.
            /// </summary>
            Retrying = 4,

            /// <summary>
            /// Fatal.
            /// </summary>
            Fatal = 5,

            /// <summary>
            /// Canceled or aborted.
            /// </summary>
            Ignored = 6
        }

        /// <summary>
        /// Task fragment model for serialization only.
        /// </summary>
        [DataContract]
        internal class FragmentModel
        {
            /// <summary>
            /// Gets or sets the task fragment identifier.
            /// </summary>
            [DataMember(Name = "id")]
            [JsonPropertyName("id")]
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the fragment index in the task.
            /// </summary>
            [DataMember(Name = "index")]
            [JsonPropertyName("index")]
            public int? Index { get; set; }

            /// <summary>
            /// Gets or sets the task fragment state.
            /// </summary>
            [DataMember(Name = "state")]
            [JsonPropertyName("state")]
            public string State { get; set; }

            /// <summary>
            /// Gets or sets the tag.
            /// </summary>
            [DataMember(Name = "tag", EmitDefaultValue = false)]
            [JsonPropertyName("tag")]
            public string Tag { get; set; }

            /// <summary>
            /// Gets or sets the creation date time tick.
            /// </summary>
            [DataMember(Name = "creation")]
            [JsonPropertyName("creation")]
            public long? Creation { get; set; }

            /// <summary>
            /// Gets or sets the latest modification date time tick.
            /// </summary>
            [DataMember(Name = "update")]
            [JsonPropertyName("update")]
            public long? Modification { get; set; }
        }

        /// <summary>
        /// Task fragment.
        /// </summary>
        public class Fragment : ISerializable
        {
            /// <summary>
            /// Initializes a new instance of the EquipartitionTask.Fragment class.
            /// </summary>
            /// <param name="index">The fragment index in the task.</param>
            public Fragment(int index)
            {
                Id = Guid.NewGuid().ToString("n");
                Index = index;
            }

            /// <summary>
            /// Initializes a new instance of the EquipartitionTask.Fragment class.
            /// </summary>
            /// <param name="id">The task fragment identifier.</param>
            /// <param name="index">The task fragment index in the task.</param>
            /// <param name="state">The task fragemnt state.</param>
            /// <param name="creation">The creation date time.</param>
            /// <param name="modification">The latest modification date time.</param>
            /// <param name="tag">The tag.</param>
            public Fragment(string id, int index, FragmentStates state, DateTime? creation, DateTime? modification, string tag = null)
            {
                Id = id ?? Guid.NewGuid().ToString("n");
                Index = index;
                Tag = tag;
                State = state;
                if (creation.HasValue) Creation = creation.Value;
                if (modification.HasValue) Modification = modification.Value;
            }

            /// <summary>
            /// Initializes a new instance of the Fragment class.
            /// </summary>
            /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
            protected Fragment(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    Id = Guid.NewGuid().ToString("n");
                    return;
                }

                try
                {
                    var id = info.GetString(nameof(Id));
                    if (id != null) Id = id;
                }
                catch (SerializationException)
                {
                }

                try
                {
                    Index = info.GetInt32(nameof(Index));
                }
                catch (SerializationException)
                {
                }

                try
                {
                    var stateStr = info.GetString(nameof(State));
                    if (!string.IsNullOrWhiteSpace(stateStr) && Enum.TryParse(stateStr, true, out FragmentStates state)) State = state;
                }
                catch (SerializationException)
                {
                }

                try
                {
                    Tag = info.GetString(nameof(Tag));
                }
                catch (SerializationException)
                {
                }

                try
                {
                    Creation = WebFormat.ParseDate(info.GetInt64(nameof(Creation)));
                }
                catch (SerializationException)
                {
                }

                try
                {
                    Modification = WebFormat.ParseDate(info.GetInt64(nameof(Modification)));
                }
                catch (SerializationException)
                {
                }
            }

            /// <summary>
            /// Gets the task fragment identifier.
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// Gets the fragment index in the task.
            /// </summary>
            public int Index { get; }

            /// <summary>
            /// Gets the task fragment state.
            /// </summary>
            public FragmentStates State { get; internal set; } = FragmentStates.Pending;

            /// <summary>
            /// Gets a value indicating whether this task fragment is working or retrying.
            /// </summary>
            public bool IsProcessing => State == FragmentStates.Working || State == FragmentStates.Retrying;

            /// <summary>
            /// Gets a value indicating whether this task fragment is done (even if it is any of success, fatal or ignored).
            /// </summary>
            public bool IsDone => State == FragmentStates.Success || State == FragmentStates.Fatal || State == FragmentStates.Ignored;

            /// <summary>
            /// Gets a value indicating whether this task fragment is pending or failure.
            /// </summary>
            public bool IsWaiting => State == FragmentStates.Pending || State == FragmentStates.Failure;

            /// <summary>
            /// Gets a value indicating whether this task fragment is fatal or failure (even if it is either waiting or done).
            /// </summary>
            public bool IsError => State == FragmentStates.Failure || State == FragmentStates.Fatal;

            /// <summary>
            /// Gets a value indicating whether this task fragment is done but not successful (even if it is either waiting or done).
            /// </summary>
            public bool IsErrorOrIgnored => State == FragmentStates.Failure || State == FragmentStates.Fatal || State == FragmentStates.Ignored;

            /// <summary>
            /// Gets the tag.
            /// </summary>
            public string Tag { get; internal set; }

            /// <summary>
            /// Gets the creation date time.
            /// </summary>
            public DateTime Creation { get; } = DateTime.Now;

            /// <summary>
            /// Gets the latest modification date time.
            /// </summary>
            public DateTime Modification { get; internal set; } = DateTime.Now;

            /// <summary>
            /// Sets the System.Runtime.Serialization.SerializationInfo object with the parameter name and additional exception information.
            /// </summary>
            /// <param name="info">The object that holds the serialized object data.</param>
            /// <param name="context">The contextual information about the source or destination.</param>
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info == null) return;
                info.AddValue(nameof(Id), Id, typeof(string));
                info.AddValue(nameof(Index), Index, typeof(int));
                info.AddValue(nameof(State), State.ToString().ToLowerInvariant(), typeof(string));
                if (Tag != null) info.AddValue(nameof(Tag), Tag, typeof(string));
                info.AddValue(nameof(Creation), WebFormat.ParseDate(Creation), typeof(long));
                info.AddValue(nameof(Modification), WebFormat.ParseDate(Modification), typeof(long));
            }

            /// <summary>
            /// Converts to JSON format string.
            /// </summary>
            /// <returns>A JSON format string.</returns>
            public virtual string ToJsonString()
            {
                return ToJsonString(null as JsonSerializerOptions);
            }

            /// <summary>
            /// Converts to query data.
            /// </summary>
            /// <returns>A query data instance.</returns>
            public virtual QueryData ToQueryData()
            {
                var q = new QueryData
                {
                    [nameof(Id)] = Id,
                    [nameof(Index)] = Index.ToString(CultureInfo.InvariantCulture),
                    [nameof(State)] = State.ToString().ToLowerInvariant()
                };
                if (Tag != null) q[nameof(Tag)] = Tag;
                q[nameof(Creation)] = WebFormat.ParseDate(Creation).ToString(CultureInfo.InvariantCulture);
                q[nameof(Modification)] = WebFormat.ParseDate(Modification).ToString(CultureInfo.InvariantCulture);
                return q;
            }

            /// <summary>
            /// Converts to JSON format string.
            /// </summary>
            /// <param name="options">The data contract serializer settings.</param>
            /// <returns>A JSON format string.</returns>
            public virtual string ToJsonString(JsonSerializerOptions options)
            {
                var m = new FragmentModel
                {
                    Id = Id,
                    Index = Index,
                    State = State.ToString().ToLowerInvariant(),
                    Tag = Tag,
                    Creation = WebFormat.ParseDate(Creation),
                    Modification = WebFormat.ParseDate(Modification)
                };
                return StringExtensions.ToJson(m, options);
            }

            /// <summary>
            /// Converts to JSON format string.
            /// </summary>
            /// <param name="options">The data contract serializer settings.</param>
            /// <returns>A JSON format string.</returns>
            public virtual string ToJsonString(DataContractJsonSerializerSettings options)
            {
                var m = new FragmentModel
                {
                    Id = Id,
                    Index = Index,
                    State = State.ToString().ToLowerInvariant(),
                    Tag = Tag,
                    Creation = WebFormat.ParseDate(Creation),
                    Modification = WebFormat.ParseDate(Modification)
                };
                return StringExtensions.ToJson(m, options);
            }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString()
            {
                return string.Format("#{0} [{1}] {2}", Index.ToString(), State.ToString(), Id);
            }

            /// <summary>
            /// Parses from a JSON string.
            /// </summary>
            /// <param name="s">The string to parse.</param>
            public static Fragment Parse(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(nameof(s), "str should not be null or empty.");
                s = s.Trim();
                if (s.IndexOf("<") == 0)
                {
                    var xml = XElement.Parse(s);
                    string id = null;
                    var index = 0;
                    string stateStr = null;
                    string tag = null;
                    long? creation = null;
                    long? modification = null;
                    foreach (var ele in xml.Elements())
                    {
                        if (string.IsNullOrWhiteSpace(ele?.Value)) continue;
                        switch (ele.Name?.LocalName?.ToLowerInvariant())
                        {
                            case "id":
                                id = ele.Value;
                                break;
                            case "index":
                                if (!int.TryParse(ele.Value, out index)) index = 0;
                                break;
                            case "state":
                                stateStr = ele.Value;
                                break;
                            case "tag":
                                tag = ele.Value;
                                break;
                            case "creation":
                                if (long.TryParse(ele.Value, out var creationV)) creation = creationV;
                                break;
                            case "update":
                            case "modification":
                                if (long.TryParse(ele.Value, out var modificationV)) modification = modificationV;
                                break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(stateStr) || !Enum.TryParse(stateStr, true, out FragmentStates state2)) state2 = FragmentStates.Pending;
                    return new Fragment(id, index, state2, WebFormat.ParseDate(creation), WebFormat.ParseDate(modification), tag);
                }

                if (s.IndexOfAny(new[] { '\"', '{' }) < 0)
                {
                    var q = QueryData.Parse(s);
                    var stateStr = q["State"] ?? q["state"];
                    if (!int.TryParse(q["Index"] ?? q["index"], out var index)) index = 0;
                    if (string.IsNullOrWhiteSpace(stateStr) || !Enum.TryParse(stateStr, true, out FragmentStates state2)) state2 = FragmentStates.Pending;
                    long? creation = null;
                    long? modification = null;
                    if (long.TryParse(q["Creation"] ?? q["creation"], out var creationV)) creation = creationV;
                    if (long.TryParse(q["Modification"] ?? q["modification"] ?? q["Update"] ?? q["update"], out var modificationV)) modification = modificationV;
                    return new Fragment(
                        q["Id"] ?? q["ID"] ?? q["id"],
                        index,
                        state2,
                        WebFormat.ParseDate(creation),
                        WebFormat.ParseDate(modification),
                        q["Tag"] ?? q["tag"]
                    );
                }

                var m = JsonSerializer.Deserialize<FragmentModel>(s);
                if (string.IsNullOrWhiteSpace(m.State) || !Enum.TryParse(m.State, true, out FragmentStates state)) state = FragmentStates.Pending;
                return new Fragment(m.Id, m.Index ?? 0, state, WebFormat.ParseDate(m.Creation), WebFormat.ParseDate(m.Modification), m.Tag);
            }
        }

        /// <summary>
        /// The fragment state changed event arguments.
        /// </summary>
        public class FragmentStateEventArgs : ChangeEventArgs<FragmentStates>
        {
            /// <summary>
            /// Initializes a new instance of the EquipartitionTask.FragmentStateEventArgs class.
            /// </summary>
            /// <param name="instance">The task fragment instance.</param>
            /// <param name="oldState">The old task fragment state.</param>
            public FragmentStateEventArgs(Fragment instance, FragmentStates oldState)
                : base(oldState, instance?.State ?? oldState, instance == null || oldState == instance.State ? ChangeMethods.Same : ChangeMethods.Update, "State")
            {
                Source = instance;
            }

            /// <summary>
            /// Gets the fragment identifier.
            /// </summary>
            public string Id => Source?.Id;

            /// <summary>
            /// Gets the task fragment instance.
            /// </summary>
            public Fragment Source { get; }
        }
    }
}
