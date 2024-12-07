.PHONY: build run deploy

build:
	docker-compose build

run:
	docker-compose up

deploy:
	docker-compose up -d

stop:
	docker-compose down

logs:
	docker-compose logs -f

restart: stop run