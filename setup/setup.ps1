# docker pull mongo:latest

docker run -d --name mongodb -p 27017:27017 -v mongodb-data:/data/db mongo:latest

# docker exec -it mongodb mongosh