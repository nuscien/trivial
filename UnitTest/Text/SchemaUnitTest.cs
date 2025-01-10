using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Tasks;

namespace Trivial.Text;

/// <summary>
/// The unit test suite for JSON schema description.
/// </summary>
[TestClass]
public class SchemaUnitTest
{
    /// <summary>
    /// Tests JSON schema.
    /// </summary>
    [TestMethod]
    public void TestJsonSchema()
    {
        var schema = JsonValues.CreateSchema<JsonModel>("Unit test.") as JsonObjectSchemaDescription;
        Assert.IsNotNull(schema);
        Assert.AreEqual(Guid.Parse("DD4F9F4E-D127-424A-B04A-696C5071EC7D"), schema.Tag);
        Assert.AreEqual("Unit test.", schema.Description);
        Assert.AreEqual(5, schema.Properties.Count);
        Assert.AreEqual("Property A.", schema.Properties["str-a"].Description);
        Assert.AreEqual(typeof(JsonStringSchemaDescription), schema.Properties["str-a"].GetType());
        Assert.IsNull(schema.Properties["str-c"].Description);
        var arrSchema = schema.Properties["arr"] as JsonArraySchemaDescription;
        Assert.IsNotNull(arrSchema?.DefaultItems);
        Assert.AreEqual(typeof(JsonIntegerSchemaDescription), arrSchema.DefaultItems.GetType());
        var desc = JsonOperationDescription.Create(typeof(JsonModel), nameof(JsonModel.Create), new[] { typeof(JsonAttributeTestModel) });
        Assert.IsNotNull(desc?.ResultSchema);
        Assert.AreEqual("Create a new instance.", desc.Description);
        Assert.AreEqual("A test model.", desc.ArgumentSchema.Description);
        Assert.IsNotNull(desc.ToTypeScriptDefinitionString("testFunc"));
        desc = JsonOperationDescription.Create(typeof(JsonModel), nameof(JsonModel.Create), new[] { typeof(string), typeof(string) });
        Assert.IsNotNull(desc);
        Assert.AreEqual("Create a new instance.", desc.Description);
        Assert.AreEqual("test", desc.Tag);
        var json = schema.ToJson();
        schema = (JsonNodeSchemaDescription)json as JsonObjectSchemaDescription;
        Assert.IsNotNull(schema);
        Assert.AreEqual("Unit test.", schema.Description);
        Assert.AreEqual(5, schema.Properties.Count);
        schema = new JsonObjectSchemaDescription(new Dictionary<string, JsonNodeSchemaDescription>
        {
            {
                "str", new JsonStringSchemaDescription
                {
                    Description = "Gets or sets a string property.",
                    DefaultValue = "Hey!",
                }
            },
            {
                "num", new JsonNumberSchemaDescription()
            },
            {
                "b", new JsonBooleanSchemaDescription()
            },
            {
                "arr", new JsonArraySchemaDescription
                {
                    DefaultItems = schema
                }
            },
        })
        {
            Description = "This is a test model.",
            DisableAdditionalProperties = true,
        };
        schema.MarkPropertyRequired("str");
        schema.GetProperty("str").EnumItems.AddRange(new[] { "a", "c", "defg" });
        schema.SetProperty("b", new JsonBooleanSchemaDescription
        {
            DefaultValue = false
        });
        Assert.IsNotNull(schema.ToTypeScriptDefinitionString("testContract"));
    }
}
