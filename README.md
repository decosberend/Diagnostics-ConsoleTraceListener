## Run
To run the stack as-is, run the following commands from the repository root:

	docker swarm init
	docker stack deploy -c .\docker-compose.yml testsendcore

This will run 3 instances that log some information to the dev Elastic stack before exiting.

## Build
To build the stack, run the following commands from the repository root:

	docker build -f TestSendCore\Dockerfile -t testsendcore:dev2 --target final .
	docker tag testsendcore:dev2 [username]/testsendcore:dev2

Do not forget to edit the `docker-compose.yml` file with the updated username before running the commands mentioned in **Run**.