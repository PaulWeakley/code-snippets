using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Interop.Core.Tests;
public class InteropSerializerTest
{
    private class TestObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<TestObject?>? Children { get; set; }
    }

    [Fact]
    public void Serialize_SingleObject_ReturnsCorrectSerialization()
    {
        var obj = new TestObject { Id = 1, Name = "Test" };

        var result = InteropSerializer.Serialize(obj);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        // Check Entities
        var entities = (List<List<List<object?>>>)result[0];
        Assert.Single(entities);

        // Check Entity
        var entity = entities[0];
        Assert.Equal(2, entity.Count);

        // Check Fields
        // Field Names
        var fieldNames = entity[0];
        Assert.Equal(3, fieldNames.Count);
        Assert.Null(fieldNames[0]);
        Assert.Equal("Id", fieldNames[1]);
        Assert.Equal("Name", fieldNames[2]);

        // Field Values
        var fieldValues = entity[1];
        Assert.Equal(3, fieldValues.Count);
        Assert.Equal(1, fieldValues[0]);
        Assert.Equal(1, fieldValues[1]);
        Assert.Equal("Test", fieldValues[2]);

        // Empty relationships
        var relationships = (List<List<List<object?>>>)result[1];
        Assert.Empty(relationships);
    }

    [Fact(Skip = "Not implemented")]
    public void Serialize_ListOfPrimitives_ReturnsCorrectSerialization()
    {
        var list = new List<int> { 4, 5, 6 };

        var result = InteropSerializer.Serialize(list);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        // Check Entities
        var entities = (List<List<List<object?>>>)result[0];
        Assert.Single(entities);

        // Check Entity
        var entity = entities[0];
        Assert.Equal(4, entity.Count);

        // Check Fields
        // Field Names
        var fieldNames = entity[0];
        Assert.Equal(2, fieldNames.Count);
        Assert.Null(fieldNames[0]);
        Assert.Null(fieldNames[1]);

        // Field Values
        Assert.Equal(1, entity[1][0]);
        Assert.Equal(4, entity[1][1]);
        Assert.Equal(2, entity[2][0]);
        Assert.Equal(5, entity[2][1]);
        Assert.Equal(3, entity[3][0]);
        Assert.Equal(6, entity[3][1]);

        File.WriteAllText("./interop.txt", JsonSerializer.Serialize(result));

        // Empty relationships
        var relationships = (List<List<List<object?>>>)result[1];
        Assert.Empty(relationships);
    }

    [Fact]
    public void Serialize_ComplexObject_ReturnsCorrectSerialization()
    {
        var obj = new TestObject
        {
            Id = 1,
            Name = "Parent",
            Children =
            [
                new() { Id = 2, Name = "Child1" },
                new() { Id = 3, Name = "Child2" }
            ]
        };

        var result = InteropSerializer.Serialize(obj);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        // Check Entities
        var entities = (List<List<List<object?>>>)result[0];
        Assert.Single(entities);

        // Check Entity
        var entity = entities[0];
        Assert.Equal(4, entity.Count);

        // Check Fields
        // Field Names
        var fieldNames = entity[0];
        Assert.Equal(3, fieldNames.Count);
        Assert.Null(fieldNames[0]);
        Assert.Equal("Id", fieldNames[1]);
        Assert.Equal("Name", fieldNames[2]);

        // Field Values
        var fieldValues = entity[1];
        Assert.Equal(3, fieldValues.Count);
        Assert.Equal(1, fieldValues[0]);
        Assert.Equal(1, fieldValues[1]);
        Assert.Equal("Parent", fieldValues[2]);
        fieldValues = entity[2];
        Assert.Equal(3, fieldValues.Count);
        Assert.Equal(2, fieldValues[0]);
        Assert.Equal(2, fieldValues[1]);
        Assert.Equal("Child1", fieldValues[2]);
        fieldValues = entity[3];
        Assert.Equal(3, fieldValues.Count);
        Assert.Equal(3, fieldValues[0]);
        Assert.Equal(3, fieldValues[1]);
        Assert.Equal("Child2", fieldValues[2]);

        // Check Relationships
        var relationships = (List<List<List<object?>>>)result[1];
        Assert.Single(relationships);

        // Check Relationship
        var relationship = relationships[0];
        Assert.Equal(2, relationship.Count);

        // Check Relationship Names
        var relationshipNames = relationship[0];
        Assert.Equal(2, relationshipNames.Count);
        Assert.Null(relationshipNames[0]);
        Assert.Equal("Children", relationshipNames[1]);

        // Check Relationship Values
        var relationshipValues = relationship[1];
        Assert.Equal(2, relationshipValues.Count);
        Assert.Equal(1, relationshipValues[0]);
        Assert.Equal([2, 3], (object[]?)relationshipValues[1]);
    }

    [Fact]
    public void Serialize_ListOfComplexObjects_ReturnsCorrectSerialization()
    {
        var list = new List<TestObject>
        {
            new TestObject { Id = 1, Name = "Object1" },
            new TestObject { Id = 2, Name = "Object2" }
        };

        var result = InteropSerializer.Serialize(list);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        // Check Entities
        var entities = (List<List<List<object?>>>)result[0];
        Assert.Single(entities);

        // Check Entity
        var entity = entities[0];
        Assert.Equal(3, entity.Count);

        // Check Fields
        // Field Names
        var fieldNames = entity[0];
        Assert.Equal(3, fieldNames.Count);
        Assert.Null(fieldNames[0]);
        Assert.Equal("Id", fieldNames[1]);
        Assert.Equal("Name", fieldNames[2]);

        // Field Values
        var fieldValues = entity[1];
        Assert.Equal(3, fieldValues.Count);
        Assert.Equal(1, fieldValues[0]);
        Assert.Equal(1, fieldValues[1]);
        Assert.Equal("Object1", fieldValues[2]);
        fieldValues = entity[2];
        Assert.Equal(3, fieldValues.Count);
        Assert.Equal(2, fieldValues[0]);
        Assert.Equal(2, fieldValues[1]);
        Assert.Equal("Object2", fieldValues[2]);

        // Check Relationships
        var relationships = (List<List<List<object?>>>)result[1];
        Assert.Empty(relationships);
    }
}
