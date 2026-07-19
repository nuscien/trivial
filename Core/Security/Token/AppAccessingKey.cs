using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Data;
using Trivial.Net;
using Trivial.Reflection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Security;

/// <summary>
/// The app secret key for accessing api.
/// </summary>
public class AppAccessingKey
{
    /// <summary>
    /// Initializes a new instance of the AppAccessingKey class.
    /// </summary>
    public AppAccessingKey()
    {
    }

    /// <summary>
    /// Initializes a new instance of the AppAccessingKey class.
    /// </summary>
    /// <param name="id">The app identifier or app access key.</param>
    /// <param name="secret">The app secret key.</param>
    public AppAccessingKey(string id, string secret = null)
    {
        Id = id;
        if (secret != null) Secret = secret.ToSecure();
    }

    /// <summary>
    /// Initializes a new instance of the AppAccessingKey class.
    /// </summary>
    /// <param name="id">The app identifier or app access key.</param>
    /// <param name="secret">The app secret key.</param>
    public AppAccessingKey(string id, ReadOnlySpan<char> secret)
    {
        Id = id;
        Secret = secret.ToSecure();
    }

    /// <summary>
    /// Initializes a new instance of the AppAccessingKey class.
    /// </summary>
    /// <param name="id">The app identifier or app access key.</param>
    /// <param name="secret">The app secret key.</param>
    public AppAccessingKey(string id, SecureString secret)
    {
        Id = id;
        Secret = secret?.Copy();
    }

    /// <summary>
    /// Deconstructor.
    /// </summary>
    ~AppAccessingKey()
    {
        if (Secret == null) return;
        Secret.Dispose();
        Secret = null;
    }

    /// <summary>
    /// The app identifier or app access key.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The app secret key.
    /// </summary>
    public SecureString Secret { get; set; }

    /// <summary>
    /// Sets the secret key.
    /// </summary>
    /// <param name="secret">The secret key to set.</param>
    public void SetSecret(string secret)
        => Secret = secret.ToSecure();

    /// <summary>
    /// Sets the secret key.
    /// </summary>
    /// <param name="secret">The secret key to set.</param>
    public void SetSecret(ReadOnlySpan<char> secret)
        => Secret = secret.ToSecure();

    /// <summary>
    /// Gets additional string bag.
    /// </summary>
    public IDictionary<string, string> Bag { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Loads from environment variables to set.
    /// </summary>
    /// <param name="id">The key of environment variable of app identifier or app access key.</param>
    /// <param name="secret">The key of environment variable of app secret key.</param>
    /// <returns>true if gets all these information; otherwise, false.</returns>
    public bool LoadFromEnvironment(string id, string secret)
        => LoadFromEnvironment(id, secret, false);

    /// <summary>
    /// Loads from environment variables to set.
    /// </summary>
    /// <param name="id">The key of environment variable of app identifier or app access key.</param>
    /// <param name="secret">The key of environment variable of app secret key.</param>
    /// <param name="skipIfFails">true if skip to set if fails to load; otherwise, false.</param>
    /// <returns>true if gets all these information; otherwise, false.</returns>
    public bool LoadFromEnvironment(string id, string secret, bool skipIfFails)
    {
        id = GetFromEnvironment(id);
        secret = GetFromEnvironment(secret);
        var succ = id != null && secret != null;
        if (!succ && skipIfFails) return false;
        Id = id;
        Secret = secret?.ToSecure();
        return succ;
    }

    /// <summary>
    /// Loads from environment variables to set.
    /// </summary>
    /// <param name="id">The key of environment variable of app identifier or app access key.</param>
    /// <param name="secret">The key of environment variable of app secret key.</param>
    /// <param name="target">The target used of environment variable.</param>
    /// <param name="skipIfFails">true if skip to set if fails to load; otherwise, false.</param>
    /// <returns>true if gets all these information; otherwise, false.</returns>
    public bool LoadFromEnvironment(string id, string secret, EnvironmentVariableTarget target, bool skipIfFails = false)
        => LoadFromEnvironment(id, target, secret, target, skipIfFails);

    /// <summary>
    /// Loads from environment variables to set.
    /// </summary>
    /// <param name="id">The key of environment variable of app identifier or app access key.</param>
    /// <param name="idTarget">The target used of environment variable of app identifier or app access key.</param>
    /// <param name="secret">The key of environment variable of app secret key.</param>
    /// <param name="secretTarget">The target used of environment variable of app secret key.</param>
    /// <param name="skipIfFails">true if skip to set if fails to load; otherwise, false.</param>
    /// <returns>true if gets all these information; otherwise, false.</returns>
    public bool LoadFromEnvironment(string id, EnvironmentVariableTarget idTarget, string secret, EnvironmentVariableTarget secretTarget, bool skipIfFails = false)
    {
        id = GetFromEnvironment(id, idTarget);
        secret = GetFromEnvironment(secret, secretTarget);
        var succ = id != null && secret != null;
        if (!succ && skipIfFails) return false;
        Id = id;
        Secret = secret?.ToSecure();
        return succ;
    }

    /// <summary>
    /// Returns a System.String that represents the current AppAccessingKey.
    /// </summary>
    /// <returns>A System.String that represents the current AppAccessingKey.</returns>
    public override string ToString()
        => Id ?? string.Empty;

    /// <summary>
    /// Tests if the app accessing key is null or empty.
    /// </summary>
    /// <param name="appKey">The app accessing key instance.</param>
    /// <returns>true if it is null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty(AppAccessingKey appKey)
    {
        try
        {
            return appKey == null || string.IsNullOrWhiteSpace(appKey.Id) || appKey.Secret == null || appKey.Secret.Length == 0;
        }
        catch (ObjectDisposedException)
        {
        }

        return true;
    }

    private static string GetFromEnvironment(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        var v = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrEmpty(v)) return null;
        return v;
    }

    private static string GetFromEnvironment(string key, EnvironmentVariableTarget target)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        var v = Environment.GetEnvironmentVariable(key, target);
        if (string.IsNullOrEmpty(v)) return null;
        return v;
    }
}
