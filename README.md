# NotificationDatabase

Notification Database is a microservice written in .net core 2.2 and it is part of the Notification Service constellation. It is the database wrapper for the other three microservices, which is implemented as a database first project using Entity Framework.




## Technologies Used
- Entity Framework Core
- Concurrent dictionary for caching
- Generic EF core base repository
- Startup task to populate the cache - concurrent dictionary
- Safety statements for all cache transactions.