"""
Module: interop
"""
class InteropSerializer:
    @staticmethod
    def serialize(root):
        entities = []
        relations = []
        if isinstance(root, list):
            counter = 0
            hash_table = {}
            for element in root:
                counter = InteropSerializer.__serialize_entity(element, counter, entities, relations, hash_table)
        else:
            InteropSerializer.__serialize_entity(root, 0, entities, relations, {})

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
            if InteropSerializer.__arrays_match(entities[index][0], fields):
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
                        counter = InteropSerializer.__serialize_entity(child_element, counter, entities, relations, hash_table)
                        child_array.append(counter)
                    child_array.sort()
                    relationships.append(child_array)
                else:
                    counter = InteropSerializer.__serialize_entity(root[child], counter, entities, relations, hash_table)
                    relationships.append(counter)

            relationship_index = -1
            for index in range(len(relations)):
                if InteropSerializer.__arrays_match(relations[index][0], fields):
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

        roots = InteropSerializer.__find_root_entities(entities, data_object[1])
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