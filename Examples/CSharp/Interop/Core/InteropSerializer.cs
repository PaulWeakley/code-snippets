using System.Collections;
using System.Reflection;

namespace Interop.Core;

public class InteropSerializer
{
    public static List<List<List<object?>>>[] Serialize(object? root)
    {
        var entities = new List<List<List<object?>>>();
        var relations = new List<List<List<object?>>>();
        if (null != root) {
            if (root is IEnumerable enumerable)
            {
                var counter = 0;
                var hashTable = new Dictionary<int, object>();
                foreach (var element in enumerable)
                    counter = SerializeEntity(element, counter, entities, relations, hashTable);
            }
            else
                SerializeEntity(root, 0, entities, relations, []);
        }
        return [entities, relations];
    }

    private static int SerializeEntity(object root, int counter, List<List<List<object?>>> entities, List<List<List<object?>>> relations, 
                                       Dictionary<int, object> hashTable)
    {
        if (root == null)
            return counter;

        foreach (var kvp in hashTable)
        {
            if (kvp.Value == root)
                return kvp.Key;
        }

        counter++;
        hashTable[counter] = root;

        var fields = new List<string?>();
        var children = new List<string?>();

        var properties = root.GetType().GetProperties();
        var propertyInfo = new Dictionary<string, PropertyInfo>();
        foreach (var property in properties)
        {
            propertyInfo.Add(property.Name, property);
            var value = property.GetValue(root);
            if (value == null)
                continue;
            
            if (value.GetType().IsPrimitive || value is string || value is decimal)
                fields.Add(property.Name);
            else
                children.Add(property.Name);
        }

        if (fields.Count > 0)
            fields.Sort();
        fields.Insert(0, null);

        if (children.Count > 0)
        {
            children.Sort();
            children.Insert(0, null);
        }

        // Process field values
        var fieldsAsObject = fields.Cast<object?>().ToList();
        var entityIndex = -1;
        for (var index = 0; index < entities.Count; index++)
        {
            if (DoArraysMatch(entities[index][0], fieldsAsObject))
            {
                entityIndex = index;
                break;
            }
        }

        if (entityIndex == -1)
        {
            entityIndex = entities.Count;
            entities.Add([fieldsAsObject]);
        }

        var fieldValues = new List<object?> { counter };
        for (var index = 1; index < fields.Count; index++)
        {
            if (index == 0)
                continue;
            
            fieldValues.Add(propertyInfo[fields[index]!]!.GetValue(root));
        }

        entities[entityIndex].Add(fieldValues);

        // Process children
        if (children.Count > 1)
        {
            var childrenAsObject = children.Cast<object?>().ToList();
            var relationships = new List<object?> { counter };
            for (var childIndex = 1; childIndex < children.Count; childIndex++)
            {
                var childProperty = children[childIndex];
                var child = propertyInfo[childProperty!]!.GetValue(root);
                if (child == null)
                    continue;
                if (child is IEnumerable enumerable)
                {
                    var childArray = new List<object>();
                    foreach (var childElement in enumerable)
                    {
                        counter = SerializeEntity(childElement, counter, entities, relations, hashTable);
                        childArray.Add(counter);
                    }
                    childArray.Sort();
                    relationships.Add(childArray.ToArray());
                }
                else
                {
                    counter = SerializeEntity(child, counter, entities, relations, hashTable);
                    relationships.Add(counter);
                }
            }

            var relationshipIndex = -1;
            for (var index = 0; index < relations.Count; index++)
            {
                if (DoArraysMatch(relations[index][0], childrenAsObject))
                {
                    relationshipIndex = index;
                    break;
                }
            }

            if (relationshipIndex == -1)
            {
                relationshipIndex = relations.Count;
                relations.Add([childrenAsObject]);
            }

            relations[relationshipIndex].Add(relationships);
        }

        return counter;
        /*
        # Process children
        if len(children) > 1:
            relationships = [counter]
            for child_index in range(1, len(children)):
                child = children[child_index]
                if root[child] is None:
                    continue
                if isinstance(root[child], list):
                    child_array = []
                    for child_element in root[child]:
                        counter = Interop.serialize_entity(child_element, counter, entities, relations, hash_table)
                        child_array.append(counter)
                    child_array.sort()
                    relationships.append(child_array)
                else:
                    counter = Interop.serialize_entity(root[child], counter, entities, relations, hash_table)
                    relationships.append(counter)

            relationship_index = -1
            for index in range(len(relations)):
                if Interop.__arrays_match(relations[index][0], fields):
                    relationship_index = index
                    break

            if relationship_index == -1:
                relationship_index = len(relations)
                relations.append([children])

            relations[relationship_index].append(relationships)

        return counter
        */
    }

    static private bool DoArraysMatch(List<object?>? arr1, List<object?>? arr2)
    {
        if (null == arr1 || null == arr2)
            return false;
        if (arr1.Count != arr2.Count)
            return false;

        for (var i = 0; i < arr1.Count; i++)
        {
            if (arr1[i] != arr2[i])
                return false;
        }

        return true;
    }


}

/*

"""
Module: interop
"""
class Interop:
    @staticmethod
    def serialize(root):
        entities = []
        relations = []
        if isinstance(root, list):
            counter = 0
            hash_table = {}
            for element in root:
                counter = Interop.__serialize_entity(element, counter, entities, relations, hash_table)
        else:
            Interop.__serialize_entity(root, 0, entities, relations, {})

        return [entities, relations]
    
    @staticmethod
    def __serialize_entity(root, counter, entities, relations, hash_table):
        if root is None:
            return counter

        for c, v in hash_table.items():
            if v == root:
                return c

        counter += 1
        hash_table[counter] = root

        fields = []
        children = []

        for key in root:
            if root[key] is None:
                continue
            elif isinstance(root[key], list) or isinstance(root[key], dict):
                children.append(key)
            else:
                fields.append(key)

        if len(fields) > 0:
            fields.sort()
        fields.insert(0, None)
        if len(children) > 0:
            children.sort()
            children.insert(0, None)

        # Process field values
        entity_index = -1
        for index in range(len(entities)):
            if Interop.__arrays_match(entities[index][0], fields):
                entity_index = index
                break

        if entity_index == -1:
            entity_index = len(entities)
            entities.append([fields])

        field_values = [counter]
        for index in range(1, len(fields)):
            if index == 0:
                continue
            field_values.append(root[fields[index]])
        entities[entity_index].append(field_values)

        # Process children
        if len(children) > 1:
            relationships = [counter]
            for child_index in range(1, len(children)):
                child = children[child_index]
                if root[child] is None:
                    continue
                if isinstance(root[child], list):
                    child_array = []
                    for child_element in root[child]:
                        counter = Interop.serialize_entity(child_element, counter, entities, relations, hash_table)
                        child_array.append(counter)
                    child_array.sort()
                    relationships.append(child_array)
                else:
                    counter = Interop.serialize_entity(root[child], counter, entities, relations, hash_table)
                    relationships.append(counter)

            relationship_index = -1
            for index in range(len(relations)):
                if Interop.__arrays_match(relations[index][0], fields):
                    relationship_index = index
                    break

            if relationship_index == -1:
                relationship_index = len(relations)
                relations.append([children])

            relations[relationship_index].append(relationships)

        return counter

    @staticmethod
    def __arrays_match(arr1, arr2):
        if len(arr1) != len(arr2):
            return False

        for i in range(len(arr1)):
            if arr1[i] != arr2[i]:
                return False

        return True

    @staticmethod
    def deserialize(data_object):
        # Check if the JSON data is null or empty
        if data_object is None or len(data_object) == 0:
            return None
        if not isinstance(data_object, list):
            print("data_object is not an array")

        entities = {}

        for entity_group in data_object[0]:
            for index in range(1, len(entity_group)):
                newObject = {}
                for field_index in range(1, len(entity_group[index])):
                    newObject[entity_group[0][field_index]] = entity_group[index][field_index]
                entities[entity_group[index][0]] = newObject

        if len(data_object) > 1:
            for relation_group in data_object[1]:
                for index in range(1, len(relation_group)):
                    for relation_index in range(1, len(relation_group[index])):
                        if isinstance(relation_group[index][relation_index], list):
                            newArray = []
                            for arrayIndex in relation_group[index][relation_index]:
                                newArray.append(entities[arrayIndex])
                            entities[relation_group[index][0]][relation_group[0][relation_index]] = newArray
                        else:
                            entities[relation_group[index][0]][relation_group[0][relation_index]] = entities[relation_group[index][relation_index]]

        roots = Interop.__find_root_entities(entities, data_object[1])
        if len(roots) == 1:
            return entities[1]

        root_array = []
        for root in roots:
            root_array.append(entities[root])
        return root_array

    @staticmethod
    def __find_root_entities(entities, relations):
        root_entities = set(entities.keys())
        for relation_group in relations:
            for index in range(1, len(relation_group)):
                for relation_index in range(1, len(relation_group[index])):
                    if isinstance(relation_group[index][relation_index], list):
                        for arrayIndex in relation_group[index][relation_index]:
                            if arrayIndex in root_entities:
                                root_entities.remove(arrayIndex)
                    else:
                        if relation_group[index][relation_index] in root_entities:
                            root_entities.remove(relation_group[index][relation_index])
        return root_entities
        */