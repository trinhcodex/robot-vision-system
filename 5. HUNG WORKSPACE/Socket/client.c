#include "config.h"

int main(int argc, char *argv[])
{
    int sock, read_size;
    struct sockaddr_in server, client;
    char message[DEFAULT_BUFLEN];

        //Create socket
        sock = socket(AF_INET, SOCK_STREAM, 0);
        if (sock == -1)
        {
            printf("Could not create socket");
        }
        puts("Socket created");

        // server.sin_addr.s_addr = inet_addr("127.0.0.1");
        server.sin_addr.s_addr = inet_addr("127.0.1.1");
        server.sin_family = AF_INET;
        server.sin_port = htons(DEFAULT_SERVER_PORT+1);

        //Connect to remote server
        if(connect(sock, (struct sockaddr *)&server, sizeof(server)) < 0)
        {
            perror("connect failed. Error");
            return 1;
        }

        puts("Connected\n");

    double position[NUM_OF_PAR];

    for (int i=0; i<NUM_OF_PAR; i++){
        read_size = recv(sock, &position[i], sizeof(double), 0);

        if(read_size == 0)
        {
            puts("Client disconnected");
        }
        else if(read_size == -1)
        {
            perror("recv failed");
        }
    }

    printf("Received parameters:    \n");
    for(int i = 0; i < NUM_OF_PAR; i++)
    {
        printf("%f  ", position[i]);
    }

    printf("\n");

    close(sock);

    return 0;
}