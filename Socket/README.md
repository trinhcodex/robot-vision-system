checkIP.c is file which aims to see which IP address you have to choose to create the server

model_ai.h is file which aims to simulate the AI program, its output is the double array 7 element

config.h is file which has all libraries and defined constant to run server and client

client_camera.c is file which aims to simulate the client of camera, it will get the double array as output of model AI, then send each element to server

server.c is file which is responsible for transfer the data from client_camera to client robot

client.c is file which aims to get the position data from the server and use them to control the robot to specificed position
