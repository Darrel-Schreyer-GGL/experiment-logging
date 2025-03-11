use('LogsDb');

db.getCollection('DomainEvents')
  .find(
    {
      "Properties.Category": "WeatherEvent",
    },
    {
      
    }
  )
  .sort({
    "_id": -1
  })
  .limit(3);
 // .explain("executionStats");
