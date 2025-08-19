using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Trivial.Security;

/// <summary>
/// The model of OAuth 2.0 Protected Resource Metadata.
/// </summary>
[DataContract]
public class ProtectedResourceMetadata
{
    /// <summary>
    /// Gets or sets the identifier of the protected resource, which is a URL that uses the https scheme and has no fragment component.
    /// </summary>
    [DataMember(Name = "resource", EmitDefaultValue = false)]
    [JsonPropertyName("resource")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The identifier of the protected resource, which is a URL that uses the https scheme and has no fragment component.")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets a list of OAuth authorization server issuer identifiers, for authorization servers that can be used with this protected resource.
    /// </summary>
    [DataMember(Name = "authorization_servers", EmitDefaultValue = false)]
    [JsonPropertyName("authorization_servers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of OAuth authorization server issuer identifiers, for authorization servers that can be used with this protected resource.")]
    public List<string> AuthorizationServers { get; set; }

    /// <summary>
    /// Gets or sets the JSON Web Key Set document URL.
    /// This contains public keys belonging to the protected resource, such as signing key(s) that the resource server uses to sign resource responses.
    /// </summary>
    [DataMember(Name = "jwks_uri", EmitDefaultValue = false)]
    [JsonPropertyName("jwks_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The JSON Web Key Set document URL.")]
    public string JsonWebKeysUrl { get; set; }

    /// <summary>
    /// Gets or sets a list of scope values, as defined in OAuth 2.0, that are used in authorization requests to request access to this protected resource.
    /// </summary>
    [DataMember(Name = "scopes_supported", EmitDefaultValue = false)]
    [JsonPropertyName("scopes_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of scope values used in authorization requests to request access to this protected resource.")]
    public List<string> Scopes { get; set; }

    /// <summary>
    /// Gets or sets a list of the supported methods of sending an OAuth 2.0 bearer token to the protected resource.
    /// Defined values are "header", "body" and "query".
    /// </summary>
    [DataMember(Name = "bearer_methods_supported", EmitDefaultValue = false)]
    [JsonPropertyName("bearer_methods_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of the supported methods of sending an OAuth 2.0 bearer token to the protected resource. Defined values are header, body and query.")]
    public List<string> BearerMethods { get; set; }

    /// <summary>
    /// Gets or sets a list of the JSON Web Signature and Encryption Algorithms supported by the protected resource for signing resource responses.
    /// </summary>
    [DataMember(Name = "resource_signing_alg_values_supported", EmitDefaultValue = false)]
    [JsonPropertyName("resource_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of the JSON Web Signature and Encryption Algorithms supported by the protected resource for signing resource responses.")]
    public List<string> SigningAlgorithmsSupported { get; set; }

    /// <summary>
    /// Gets or sets human-readable name of the protected resource intended for display to the end user.
    /// </summary>
    [DataMember(Name = "resource_name", EmitDefaultValue = false)]
    [JsonPropertyName("resource_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("Human-readable name of the protected resource intended for display to the end user.")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the URL of a page containing human-readable information that developers might want or need to know when using the protected resource.
    /// </summary>
    [DataMember(Name = "resource_documentation", EmitDefaultValue = false)]
    [JsonPropertyName("resource_documentation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The URL of a page containing human-readable information that developers might want or need to know when using the protected resource.")]
    public string DocumentationUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of a page containing human-readable information about the protected resource's requirements on how the client can use the data provided by the protected resource.
    /// </summary>
    [DataMember(Name = "resource_policy_uri", EmitDefaultValue = false)]
    [JsonPropertyName("resource_policy_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The URL of a page containing human-readable information about the protected resource's requirements on how the client can use the data provided by the protected resource.")]
    public string PolicyUrL { get; set; }

    /// <summary>
    /// Gets or sets the URL of a page containing human-readable information about the protected resource's terms of service.
    /// </summary>
    [DataMember(Name = "resource_tos_uri", EmitDefaultValue = false)]
    [JsonPropertyName("resource_tos_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("The URL of a page containing human-readable information about the protected resource's terms of service.")]
    public string TermsOfServiceUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating protected resource support for mutual-TLS client certificate-bound access tokens.
    /// If omitted, the default value is false.
    /// </summary>
    [DataMember(Name = "tls_client_certificate_bound_access_tokens", EmitDefaultValue = false)]
    [JsonPropertyName("tls_client_certificate_bound_access_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A value indicating protected resource support for mutual-TLS client certificate-bound access tokens.")]
    public bool TlsClientCertificateBoundAccessTokens { get; set; }

    /// <summary>
    /// Gets or sets a list of the authorization details type values supported by the resource server when the authorization_details request parameter is used.
    /// </summary>
    [DataMember(Name = "authorization_details_types_supported", EmitDefaultValue = false)]
    [JsonPropertyName("authorization_details_types_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A list of the authorization details type values supported by the resource server when the authorization_details request parameter is used.")]
    public List<string> AuthorizationDetailsTypesSupported { get; set; }

    /// <summary>
    /// Gets or sets a list of the JSON Web Signature and Encryption Algorithms supported by the resource server for validating Demonstrating Proof of Possession proof JSON Web Tokens.
    /// </summary>
    [DataMember(Name = "dpop_signing_alg_values_supported", EmitDefaultValue = false)]
    [JsonPropertyName("dpop_signing_alg_values_supported")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description(".")]
    public List<string> DPoPSigningAlgorithmsSupported { get; set; }

    /// <summary>
    /// Gets or sets a value specifying whether the protected resource always requires the use of DPoP-bound access tokens.
    /// If omitted, the default value is false.
    /// </summary>
    [DataMember(Name = "dpop_bound_access_tokens_required", EmitDefaultValue = false)]
    [JsonPropertyName("dpop_bound_access_tokens_required")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Description("A value specifying whether the protected resource always requires the use of DPoP-bound access tokens.")]
    public bool DPoPBoundAccessTokensRequired { get; set; }

    /// <summary>
    /// Adds a bearer method "header".
    /// </summary>
    public void AddBearerMethodHeader()
        => AddBearerMethod("header");

    /// <summary>
    /// Adds a bearer method "body".
    /// </summary>
    public void AddBearerMethodBody()
        => AddBearerMethod("body");

    /// <summary>
    /// Adds a bearer method "query".
    /// </summary>
    public void AddBearerMethodQuery()
        => AddBearerMethod("query");

    /// <summary>
    /// Adds a bearer method supported.
    /// </summary>
    /// <param name="value">The method to add.</param>
    public void AddBearerMethod(string value)
    {
        var col = BearerMethods;
        if (col is null)
        {
            col = new();
            if (BearerMethods is null)
            {
                BearerMethods = col;
            }
            else
            {
                var col2 = BearerMethods;
                if (col2 is null) BearerMethods = col;
            }
        }

        if (!string.IsNullOrWhiteSpace(value) && !col.Contains(value)) col.Add(value);
    }
}
