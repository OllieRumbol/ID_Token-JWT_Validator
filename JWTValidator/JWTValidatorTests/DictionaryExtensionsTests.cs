using JWTValidatorService.Extensions;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JWTValidatorTests;

[TestClass]
public class DictionaryExtensionsTests
{
    // The dictionaries in the worked example in spec 6.2.
    public enum ExampleDictionary
    {
        Null,
        NoEntries,
        RoleMember
    }

    // Plan 7.2 test 10, the IsEmpty column in spec 6.2.
    [TestMethod]
    [DataRow(ExampleDictionary.Null, true, DisplayName = "Null dictionary")]
    [DataRow(ExampleDictionary.NoEntries, true, DisplayName = "Dictionary with no entries")]
    [DataRow(ExampleDictionary.RoleMember, false, DisplayName = "Dictionary with role: member")]
    public void IsEmpty_ExampleDictionary_ReturnsTrueOnlyWhenNullOrEmpty(ExampleDictionary exampleDictionary, Boolean expectedResult)
    {
        // Arrange
        Dictionary<String, List<String>>? dictionary = CreateDictionary(exampleDictionary);

        // Act
        Boolean result = dictionary.IsEmpty();

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    // Plan 7.2 test 10, the ContainsValueInList("member") column in spec 6.2.
    [TestMethod]
    [DataRow(ExampleDictionary.Null, false, DisplayName = "Null dictionary")]
    [DataRow(ExampleDictionary.NoEntries, false, DisplayName = "Dictionary with no entries")]
    [DataRow(ExampleDictionary.RoleMember, true, DisplayName = "Dictionary with role: member")]
    public void ContainsValueInList_ExampleDictionary_ReturnsTrueOnlyWhenAListHoldsTheValue(ExampleDictionary exampleDictionary, Boolean expectedResult)
    {
        // Arrange
        Dictionary<String, List<String>>? dictionary = CreateDictionary(exampleDictionary);

        // Act
        Boolean result = dictionary.ContainsValueInList("member");

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    // Plan 7.2 test 10, the Print column in spec 6.2. The format is unchanged from 1.0.3.
    [TestMethod]
    [DataRow(ExampleDictionary.Null, "", DisplayName = "Null dictionary")]
    [DataRow(ExampleDictionary.NoEntries, "", DisplayName = "Dictionary with no entries")]
    [DataRow(ExampleDictionary.RoleMember, "role: member \n", DisplayName = "Dictionary with role: member")]
    public void Print_ExampleDictionary_ReturnsOneLinePerEntry(ExampleDictionary exampleDictionary, String expectedResult)
    {
        // Arrange
        Dictionary<String, List<String>>? dictionary = CreateDictionary(exampleDictionary);

        // Act
        String result = dictionary.Print();

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    // Plan 7.2 test 11.
    [TestMethod]
    public void ContainsKeyAndValue_NullDictionary_ReturnsFalse()
    {
        // Arrange
        Dictionary<String, List<String>>? dictionary = null;

        // Act
        Boolean result = dictionary.ContainsKeyAndValue("role", "member");

        // Assert
        Assert.IsFalse(result);
    }

    // Plan 7.2 test 18. A null value inside a list is compared, not dereferenced.
    [TestMethod]
    public void ContainsKeyAndValue_ListHoldsNullValue_ReturnsFalse()
    {
        // Arrange
        Dictionary<String, List<String?>> dictionary = new Dictionary<String, List<String?>> { { "role", new List<String?> { null } } };

        // Act
        Boolean result = dictionary.ContainsKeyAndValue("role", "member");

        // Assert
        Assert.IsFalse(result);
    }

    // Plan 7.2 test 18. A null value inside a list is compared, not dereferenced.
    [TestMethod]
    public void ContainsValueInList_ListHoldsNullValue_ReturnsFalse()
    {
        // Arrange
        Dictionary<String, List<String?>> dictionary = new Dictionary<String, List<String?>> { { "role", new List<String?> { null } } };

        // Act
        Boolean result = dictionary.ContainsValueInList("member");

        // Assert
        Assert.IsFalse(result);
    }

    private static Dictionary<String, List<String>>? CreateDictionary(ExampleDictionary exampleDictionary)
    {
        return exampleDictionary switch
        {
            ExampleDictionary.Null => null,
            ExampleDictionary.NoEntries => new Dictionary<String, List<String>>(),
            ExampleDictionary.RoleMember => new Dictionary<String, List<String>> { { "role", new List<String> { "member" } } },
            _ => throw new ArgumentOutOfRangeException(nameof(exampleDictionary))
        };
    }

    [TestMethod]
    public void DictionaryContainsKeyAndValueEmptyDictionaryTest()
    {
        // Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        String key = "role";
        String value = "member";

        // Act
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsKeyAndValueDictionaryDoesNotContainKeyTest()
    {
        // Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("hello", new List<String>
        {
            "member"
        });

        String key = "role";
        String value = "member";

        // Act
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsKeyAndValueDictionaryDoesNotContainValueTest()
    {
        // Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "world"
        });

        String key = "role";
        String value = "member";

        // Act
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsKeyAndValueDictionaryContainsBothKeyAndValueTest()
    {
        // Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "member"
        });

        String key = "role";
        String value = "member";

        // Act
        Boolean result = dictionary.ContainsKeyAndValue(key, value);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void DictionaryContainsValueInList_EmptyList_ReturnsFalse()
    {
        // Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();

        // Act
        Boolean result = dictionary.ContainsValueInList("test123");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryContainsValueInList_NotInList_ReturnsFalse()
    {
        // Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "member"
        });
        dictionary.Add("hello", new List<String>
        {
            "world",
            "12345678"
        });
        dictionary.Add("example", new List<String>
        {
            "test1",
            "test2",
            "test3"
        });

        // Act
        Boolean result = dictionary.ContainsValueInList("test123");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void DictionaryIsEmpty_HasValues_ShouldBeFalse()
    {
        // Arrange
        Dictionary<String, List<String>> dictionary = new Dictionary<String, List<String>>();
        dictionary.Add("role", new List<String>
        {
            "member"
        });
        dictionary.Add("hello", new List<String>
        {
            "world",
            "12345678"
        });
        dictionary.Add("example", new List<String>
        {
            "test1",
            "test2",
            "test3"
        });

        // Act
        Boolean result = dictionary.IsEmpty();

        // Assert
        Assert.IsFalse(result);
    }
}