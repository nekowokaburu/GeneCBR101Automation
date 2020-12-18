import socket, time

TCP_IP = '192.168.178.182'
TCP_PORT = 5333
BUFFER_SIZE = 20  # Normally 1024, but we want fast response

import socket
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((TCP_IP, TCP_PORT))
s.listen(1)
conn, addr = s.accept()
while 1:
    data = conn.recv(1024)
    if not data:
        break
    conn.sendall(data)
conn.close()