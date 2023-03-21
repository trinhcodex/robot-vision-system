import socket
import array
import struct
import numpy as np

position = np.array([1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0])

a = array.array('f', position)

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server_address = ('127.0.1.1', 5000)
client.connect(server_address)

for element in a:
    b = struct.pack('d', element)
    client.sendall(b)
    print(f'Sent: {element}')

client.close()