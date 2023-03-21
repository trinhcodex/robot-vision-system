#include "config.h"
#include "model_ai.h"

int main(int argc, char *argv[])
{
    int sock;
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
        server.sin_port = htons(DEFAULT_SERVER_PORT);

        //Connect to remote server
        if(connect(sock, (struct sockaddr *)&server, sizeof(server)) < 0)
        {
            perror("connect failed. Error");
            return 1;
        }

        puts("Connected\n");

    // mô phỏng giá trị lấy được từ camera
    double x = 1.0, y = 2.0;

    double *position;
    position = compute(x, y);

    printf("Position input for robot\n");
    for (int i = 0; i < NUM_OF_PAR; i++) {
        printf("%f ", position[i]);
    }

    for (int i = 0; i < NUM_OF_PAR; i++) {
        if(send(sock, &position[i], sizeof(double), 0) < 0)    
        {
            printf("\nPosition is not successfully sent\n");
        }
        else
        {
            printf("\nPosition is sent\n");
        }

        // printf("\n");
    }

    close(sock);

    return 0;
}