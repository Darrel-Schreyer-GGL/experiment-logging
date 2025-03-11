use('LogsDb');

const useSimpleIndex = false;

// List current indexes
print("Current indexes:");
db.getCollection('DomainEvents').getIndexes().forEach(index => printjson(index));

// Drop all indexes starting with "Properties.Category"
const indexes = db.getCollection('DomainEvents').getIndexes();
indexes.forEach(index => {
    const indexName = index.name;
    if (indexName.startsWith("Properties.Category")) {
        print(`Dropping index: ${indexName}`);
        db.getCollection('DomainEvents').dropIndex(indexName);
    }
});

// Create the new index based on useSimpleIndex
if (useSimpleIndex) {
    print("Creating simple index on Properties.Category");
    db.getCollection('DomainEvents').createIndex({ "Properties.Category": 1 });
} else {
    print("Creating compound index on Properties.Category and _id");
    db.getCollection('DomainEvents').createIndex({ "Properties.Category": 1, "_id": -1 });
}

// List indexes after modification
print("Indexes after modification:");
db.getCollection('DomainEvents').getIndexes().forEach(index => printjson(index));