import json
from typing import Iterator
import unittest
from interop_serializer import InteropSerializer
from dataclasses import dataclass, field

@dataclass
class TestObject:
    id: int
    name: str
    children: list['TestObject'] = None

    def __iter__(self) -> Iterator['TestObject']:
        yield self.id, self.name, self.children

    def __len__(self):
        return 3
    
    def __getitem__(self, index):
        if index == 0:
            return self.id
        elif index == 1:
            return self.name
        elif index == 2:
            return self.children
        elif index == "id":
            return self.id
        elif index == "name":
            return self.name
        elif index == "children":
            return self.children
        else:
            raise IndexError("Index out of range: " + str(index))

    def __dict__(self):
        return {
            "id": self.id,
            "name": self.name,
            "children": self.children
        }



class InteropSerializerTest(unittest.TestCase):
    def test_serialize_single_object_returns_correct_serialization(self):
        obj = {"Id": 1, "Name": "Test", "Children": None}
        result = InteropSerializer.serialize(obj)

        self.assertIsNotNone(result)
        self.assertEqual(2, len(result), "Entities and Relationships should be returned")

        entities = result[0]
        self.assertEqual(1, len(entities), "One Entity Type should be returned")

        entity = entities[0]
        self.assertEqual(2, len(entity), "Entity should have 2 records, fields and field values")

        # Check Field Names
        field_names = entity[0]
        self.assertEqual(3, len(field_names))
        self.assertIsNone(field_names[0])
        self.assertEqual("Id", field_names[1])
        self.assertEqual("Name", field_names[2])

        field_values = entity[1]
        self.assertEqual(3, len(field_values))
        self.assertEqual(1, field_values[0])
        self.assertEqual(1, field_values[1])
        self.assertEqual("Test", field_values[2])

        relationships = result[1]
        self.assertEqual(0, len(relationships))

    @unittest.skip("Not implemented")
    def test_serialize_list_of_primitives_returns_correct_serialization(self):
        list_of_primitives = [4, 5, 6]
        result = InteropSerializer.serialize(list_of_primitives)

        self.assertIsNotNone(result)
        self.assertEqual(2, len(result))

        entities = result[0]
        self.assertEqual(4, len(entities))

        entity = entities[0]
        self.assertEqual(2, len(entity))
        self.assertIsNone(entity[0])
        self.assertIsNone(entity[1])

        self.assertEqual(1, entities[1][0])
        self.assertEqual(4, entities[1][1])
        self.assertEqual(2, entities[2][0])
        self.assertEqual(5, entities[2][1])
        self.assertEqual(3, entities[3][0])
        self.assertEqual(6, entities[3][1])

        with open("./interop.txt", "w") as f:
            json.dump(result, f)

        relationships = result[1]
        self.assertEqual(0, len(relationships))

    def test_serialize_complex_object_returns_correct_serialization(self):
        obj = {
            "Id": 4,
            "Name": "Parent",
            "Children": [
                {"Id": 5, "Name": "Child1", "Children": None},
                {"Id": 6, "Name": "Child2", "Children": None}
            ]
        }
        result = InteropSerializer.serialize(obj)

        self.assertIsNotNone(result)
        self.assertEqual(2, len(result), "Entities and Relationships should be returned")

        entities = result[0]
        self.assertEqual(1, len(entities), "One Entity Type should be returned")

        entity = entities[0]
        self.assertEqual(4, len(entity), "Entity should have 4 records, fields and field values")

        # Check Field Names
        field_names = entity[0]
        self.assertEqual(3, len(field_names))
        self.assertIsNone(field_names[0])
        self.assertEqual("Id", field_names[1])
        self.assertEqual("Name", field_names[2])

        # Check Field Values
        field_values = entity[1]
        self.assertEqual(3, len(field_values))
        self.assertEqual(1, field_values[0], "Serial Id should be 1")
        self.assertEqual(4, field_values[1], "Id should be 4")
        self.assertEqual("Parent", field_values[2])

        field_values = entity[2]
        self.assertEqual(3, len(field_values))
        self.assertEqual(2, field_values[0], "Serial Id should be 2")
        self.assertEqual(5, field_values[1], "Id should be 5")
        self.assertEqual("Child1", field_values[2])

        field_values = entity[3]
        self.assertEqual(3, len(field_values))
        self.assertEqual(3, field_values[0], "Serial Id should be 3")
        self.assertEqual(6, field_values[1], "Id should be 6")
        self.assertEqual("Child2", field_values[2])

        relationships = result[1]
        self.assertEqual(1, len(relationships), "Single relationship should be returned")

        relationship = relationships[0]
        self.assertEqual(2, len(relationship), "Relationship should have 2 records, relationship names and values")

        relationship_names = relationship[0]
        self.assertEqual(2, len(relationship_names))
        self.assertIsNone(relationship_names[0])
        self.assertEqual("Children", relationship_names[1], "Relationship name should be Children")

        relationship_values = relationship[1]
        self.assertEqual(2, len(relationship_values))
        self.assertEqual(1, relationship_values[0], "Serial Id should be 1 to identify Parent")
        self.assertEqual([2, 3], relationship_values[1], "Children should be Child1 and Child2")

    def test_serialize_list_of_complex_objects_returns_correct_serialization(self):
        list_of_objects = [
            {"Id": 3, "Name": "Object1", "Children": None},
            {"Id": 4, "Name": "Object2", "Children": None}
        ]
        result = InteropSerializer.serialize(list_of_objects)

        self.assertIsNotNone(result)
        self.assertEqual(2, len(result), "Entities and Relationships should be returned")

        entities = result[0]
        self.assertEqual(1, len(entities), "One Entity Type should be returned")

        entity = entities[0]
        self.assertEqual(3, len(entity), "Entity should have 3 records, fields and 2 sets of field values")

        # Check Field Names
        field_names = entity[0]
        self.assertEqual(3, len(field_names))
        self.assertIsNone(field_names[0])
        self.assertEqual("Id", field_names[1])
        self.assertEqual("Name", field_names[2])

        # Check Field Values
        field_values = entity[1]
        self.assertEqual(3, len(field_values))
        self.assertEqual(1, field_values[0], "Serial Id should be 1")
        self.assertEqual(3, field_values[1], "Id should be 3")
        self.assertEqual("Object1", field_values[2])

        field_values = entity[2]
        self.assertEqual(3, len(field_values))
        self.assertEqual(2, field_values[0], "Serial Id should be 2")
        self.assertEqual(4, field_values[1], "Id should be 4")
        self.assertEqual("Object2", field_values[2])

        relationships = result[1]
        self.assertEqual(0, len(relationships))

if __name__ == '__main__':
    unittest.main()