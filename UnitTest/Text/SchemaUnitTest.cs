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
        Assert.AreEqual("Unit test.", schema.Description);
        Assert.AreEqual(5, schema.Properties.Count);
        Assert.AreEqual("Property A.", schema.Properties["str-a"].Description);
        Assert.AreEqual(typeof(JsonStringSchemaDescription), schema.Properties["str-a"].GetType());
        Assert.IsNull(schema.Properties["str-c"].Description);
        var arrSchema = schema.Properties["arr"] as JsonArraySchemaDescription;
        Assert.IsNotNull(arrSchema?.DefaultItems);
        Assert.AreEqual(typeof(JsonIntegerSchemaDescription), arrSchema.DefaultItems.GetType());
        var desc = JsonOperationDescription.Create(typeof(JsonModel), "Create", new[] { typeof(JsonAttributeTestModel) });
        Assert.IsNotNull(desc);
        Assert.AreEqual("Create a new instance.", desc.Description);
        Assert.AreEqual("A test model.", desc.ArgumentSchema.Description);
        desc = JsonOperationDescription.Create(typeof(JsonModel), "Create", new[] { typeof(string), typeof(string) });
        Assert.IsNotNull(desc);
        Assert.AreEqual("Create a new instance.", desc.Description);
        Assert.AreEqual("test", desc.Tag);
    }
}
