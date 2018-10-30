# build with: docker build --rm -t simple-rat-server
# run with: docker run -p 3000:3000 -p 8001:8001 --name simple_rat_server simple-rat-server

FROM node:8.12.0-alpine

RUN mkdir -p /var/www/server
WORKDIR /var/www/server

COPY . /var/www/
RUN npm install


EXPOSE 8001/tcp
EXPOSE 3000/tcp

WORKDIR /var/www/server/
CMD ["node", "app.js"]
