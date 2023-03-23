#include<stdio.h>   //printf    
#include<string.h>  //strlen
#include<sys/socket.h>  //socket
#include<arpa/inet.h>   //inet_addr
#include<unistd.h>      //write

#include<stdlib.h>

#include <sys/types.h>  //recvfrom
#include <sys/socket.h> //recvfrom

#include <sys/time.h> //FD_SET, FD_ISSET, FD_ZERO macros 
#include <signal.h>
#include <errno.h>

#define DEFAULT_BUFLEN         512
#define DEFAULT_CLIENT_PORT    5000
#define DEFAULT_SERVER_PORT    5000
#define NUM_OF_PAR             7