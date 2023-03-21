#include "config.h"

int main(int argc , char *argv[])
{
    int socket_desc, client_sock, c, read_size;
    struct sockaddr_in server, client;
    char client_message[DEFAULT_BUFLEN];

    fd_set readfds; 
    int max_fd;
    int activity;

    double arrayReceived[NUM_OF_PAR]; // data transfer from camera to robot

    //Create socket
    socket_desc = socket(AF_INET, SOCK_STREAM, 0);
    if (socket_desc == -1)
    {
            printf("Could not create socket");
    }
    puts("Socket created");

    //Prepare the sockaddr_in structure
    memset(&server, 0, sizeof(server));
    server.sin_family = AF_INET;
    server.sin_addr.s_addr = inet_addr("127.0.1.1");
    server.sin_port = htons(DEFAULT_SERVER_PORT);

    //Bind
    if(bind(socket_desc, (struct sockaddr *)&server, sizeof(server)) < 0)
    {
        //print the error message
        perror("bind failed. Error");
        return 1;
    }
    puts("bind done");

    //Listen
    listen(socket_desc, 1);

    //Accept and incoming connection
    puts("Waiting for incoming connections...");
    c = sizeof(struct sockaddr_in);

    int socket_desc2, c2;
    struct sockaddr_in server2, client2;

    //Create socket
    socket_desc2 = socket(AF_INET, SOCK_STREAM, 0);
    if (socket_desc2 == -1)
    {
            printf("Could not create socket2");
    }
    puts("Socket2 created");

    //Prepare the sockaddr_in structure
    memset(&server2, 0, sizeof(server2));
    server2.sin_family = AF_INET;
    server2.sin_addr.s_addr = inet_addr("127.0.1.1");
    server2.sin_port = htons(DEFAULT_SERVER_PORT + 1);

    //Bind
    if(bind(socket_desc2, (struct sockaddr *)&server2, sizeof(server2)) < 0)
    {
        //print the error message
        perror("bind failed. Error");
        return 1;
    }
    puts("bind done");

    listen(socket_desc2, 1);

    puts("Waiting for incoming connections...");
    c2 = sizeof(struct sockaddr_in);

    while (1) {
        FD_SET(socket_desc , &readfds);
        FD_SET(socket_desc2 , &readfds);

        max_fd = (socket_desc > socket_desc2) ? socket_desc : socket_desc2;

        activity = select(max_fd + 1 , &readfds , NULL , NULL , NULL);

        // Check if select() returned an error
        if ((activity < 0) && (errno!=EINTR)) 
        {
            printf("select error");
            return 1;
        }

        if (FD_ISSET(socket_desc , &readfds)) {
            client_sock = accept(socket_desc, (struct sockaddr *)&client, (socklen_t*)&c);
            printf("test\n");
            if(client_sock < 0)
            {
                perror("accept failed");
                return 1;
            }
            puts("Connection accepted");

            for (int i=0; i<NUM_OF_PAR; i++){
                read_size = recv(client_sock, &arrayReceived[i], sizeof(double), 0);

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
                printf("%f  ", arrayReceived[i]);
            }

            printf("\n");

            close(client_sock);
        }

        if (FD_ISSET(socket_desc2 , &readfds)) {
            client_sock = accept(socket_desc2, (struct sockaddr *)&client2, (socklen_t*)&c2);
            printf("test\n");
            if (client_sock < 0)
            {
                perror("accept failed");
                return 1;
            }
            puts("Connection accepted");

            for (int i = 0; i < NUM_OF_PAR; i++) {
            if(send(client_sock, &arrayReceived[i], sizeof(double), 0) < 0)    
            {
                printf("\nPosition is not successfully sent\n");
            }
            else
            {
                printf("\nPosition is sent\n");
            }
            }
            close(client_sock);
        }
    }

    close(socket_desc);
    close(socket_desc2);

    return 0;
}